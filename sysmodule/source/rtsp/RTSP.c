#include <switch.h>
#include <stdio.h>
#include <time.h>
#include "../socketing.h"
#include "RTSP.h"

static const char RTSPHeader[] = "RTSP/1.0 200 OK\r\n";

static const char SDP[] =	"s=SysDVR - https://github.com/exelix11/sysdvr \r\n"
							"m=video 0 RTP/AVP 96\r\n"
							"a=rtpmap:96 H264/90000\r\n"
							"a=fmtp:96 profile-level-id=42A01E; sprop-parameter-sets=Z2QMIKwrQCgC3TUBDQHggA==,aO48sA==;\r\n"
							"a=StreamName:string;\"Video\"\r\n"
							"a=control:video\r\n"
							"m=audio 0 RTP/AVP 97\r\n"
							"a=rtpmap:97 L16/48000/2\r\n"
							"a=StreamName:string;\"Audio\"\r\n"
							"a=control:audio\r\n";


int RTSPSock = -1;
int client = -1;

static atomic_bool RTSP_Running = false;
atomic_bool RTSP_ClientStreaming = false;

static inline void CloseSocket(int* soc)
{
	if (*soc != -1)
	{
		close(*soc);
		*soc = -1;
	}
}

static inline void RTSP_MainLoop(int socket);

void* RTSP_ServerThread(void* arg)
{
	Result rc = CreateSocket(&RTSPSock, 6666, 4, false);
	if (R_FAILED(rc)) fatalSimple(rc);
	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 3 seconds
	*/
	int sockFails = 0;
	while (RTSP_Running)
	{
		int curSock = accept(RTSPSock, 0, 0);
		if (curSock < 0)
		{
			if (sockFails++ >= 3 && RTSP_Running)
			{
				CloseSocket(&RTSPSock);
				Result rc = CreateSocket(&RTSPSock, 6666, 4, false);
				if (R_FAILED(rc)) fatalSimple(rc);
			}
			svcSleepThread(1E+9);
			continue;
		}

		/*
			Cooperative multithreading (at least i think that's the issue here) causes some issues with socketing,
			even if the video and audio listeners are used on different threads calling accept on one of them
			blocks the others as well, even while a client is connected.
			The workaround is making the socket non-blocking and then to set the client socket as blocking.
			By default the socket returned from accept inherits this flag.
		*/
		fcntl(curSock, F_SETFL, fcntl(curSock, F_GETFL, 0) & ~O_NONBLOCK);
		RTSP_MainLoop(curSock);

		RTSP_ClientStreaming = false;
		close(curSock);
		svcSleepThread(1E+9);
	}
	pthread_exit(NULL);
	return NULL;
}

void RTSP_StopServer()
{
	RTSP_Running = false;
	RTSP_ClientStreaming = false;
	CloseSocket(&RTSPSock);
	CloseSocket(&client);
}

typedef struct
{
	unsigned char data, control;
} RtspPorts;

#define VIDEO_PORT 0
#define AUDIO_PORT 1

static RtspPorts ports[2];

static inline void RTSP_Send(const void* const data, const size_t len) 
{
	if (send(client, data, len, 0) <= 0)
	{
		CloseSocket(&client); //TODO is this enough go get the main thread out of receive ?
		RTSP_ClientStreaming = false;
	}
}

//Send data over the data channel, RTCP is not used currently
void RTSP_SendBinaryHeader(const size_t totalLen, unsigned int stream)
{
	char header[4];
	header[0] = '$';
	header[1] = ports[stream].data;
	header[2] = (totalLen & 0xFF00) >> 8;
	header[3] = totalLen & 0x00FF;
	RTSP_Send(client, header, 4);
}

void RTSP_SendRawData(const void* const data, const size_t len)
{
	RTSP_Send(client, data, len);
}

void RTSP_SendData(const void* const data, const size_t len, unsigned char stream)
{
	RTSP_BeginPushData(len, stream);
	RTSP_SendRaw(data, len);
}

static inline void RTSP_StartPlayback() 
{
	// TODO
}

static inline void RTSP_SendHeader()
{
	//printf("> %s", RTSPHeader);
	RTSP_Send(client, RTSPHeader, sizeof(RTSPHeader) - 1);
}

static inline void RTSP_SendTerminator()
{
	RTSP_Send(client, "\r\n", 2);
}

static inline void RTSP_SendText(const char* source)
{
	//printf("> %s", source)
	RTSP_Send(client, source, strlen(source));
}

static inline void RTSP_SendFormat(int buf, const char* format, ...)
{
	char* toSend = (char*)malloc(buf);

	va_list args;
	va_start(args, format);
	vsnprintf(toSend, buf, format, args);
	va_end(args);

	RTSP_SendText(toSend);

	free(toSend);
}

static inline void RTSP_SendError(const char* error)
{
	RTSP_SendFormat(80, "RTSP/1.0 %s\r\n", error);
	RTSP_SendTerminator();
}

static inline void RTSP_SendCseq(const char* source)
{
	const char* cseq = strstr(source, "CSeq:");
	if (!cseq) return;

	RTSP_SendFormat(30, "CSeq: %d\r\n", atoi(cseq + sizeof("CSeq:")));
}

static inline void RTSP_AnswerEmpty(char* source)
{
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendTerminator();
}

static inline void RTSP_AnswerText(char* source, const char* text)
{
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendText(text);
	RTSP_SendTerminator();
}

static inline void RTSP_AnswerTextContent(char* source, char* text, const char* content)
{
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendText("Content-Type: application/sdp\r\n");
	RTSP_SendFormat(30, "Content-Length: %d\r\n", strlen(content));
	RTSP_SendText(text);
	RTSP_SendTerminator();
	RTSP_SendText(content);
}

static inline void RTSP_MainLoop(int socket)
{
	char rtspBuf[512];

	while (client > 0 && RTSP_Running)
	{
		int len = recv(client, rtspBuf, sizeof(rtspBuf), 0);
		if (len <= 0) return;
		rtspBuf[len] = '\0';

		//printf("%s\n", rtspBuf);

#define ISCOMMAND(command) (!strncmp(rtspBuf, command, sizeof(command) - 1))

		if (ISCOMMAND("OPTIONS"))
			RTSP_AnswerText(rtspBuf, "Public: DESCRIBE, SETUP, PLAY\r\n");
		else if (ISCOMMAND("DESCRIBE"))
		{
			char* arg1 = rtspBuf + sizeof("DESCRIBE");
			char* arg1End = arg1;

			while (*arg1End != 0 && *arg1End != ' ' && *arg1End != '\r') arg1End++;

			char original = *arg1End;
			*arg1End = '\0';

			char contBase[60];
			snprintf(contBase, sizeof(contBase), "Content-Base: %s\r\n", arg1);
			*arg1End = original;

			RTSP_AnswerTextContent(rtspBuf, contBase, SDP);
		}
		else if (ISCOMMAND("SETUP"))
		{
			char* transportOpt = strstr(rtspBuf, "Transport:");

			if (!strstr(rtspBuf, "RTP/AVP/TCP") || !transportOpt || !strstr(rtspBuf, "interleaved"))
				RTSP_SendError("461 Unsupported transport");
			else
			{
				char transport[100];
				{
					transportOpt += sizeof("Transport:");
					char* transportOpt = strstr(rtspBuf, "Transport:") + sizeof("Transport:");
					char* transportOptEnd = transportOpt;
					while (*transportOptEnd != 0 && *transportOptEnd != ' ' && *transportOptEnd != '\r') transportOptEnd++;

					char original = *transportOptEnd;
					*transportOptEnd = '\0';

					strncpy(transport, transportOpt, sizeof(transport));
					*transportOptEnd = original;
				}

				char* portSettings = strstr(transport, "interleaved=") + sizeof("interleaved");

				int targetStream = strstr(rtspBuf, "/video") ? VIDEO_PORT : AUDIO_PORT;

				ports[targetStream].data = portSettings[0] - '0';
				ports[targetStream].control = portSettings[2] - '0';

				char transportRespnse[130];
				snprintf(transportRespnse, sizeof(transportRespnse), "Transport: %s\r\n", transport);
				RTSP_AnswerText(rtspBuf, transportRespnse);
			}
		}
		else if (ISCOMMAND("PLAY"))
		{
			RTSP_AnswerEmpty(rtspBuf);
			RTSP_StartPlayback();
		}
		else if (ISCOMMAND("TEARDOWN"))
		{
			return;
		}

	}
}