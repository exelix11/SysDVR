#include <switch.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <pthread.h>
#include <stdarg.h>

#include "../socketing.h"
#include "RTSP.h"
#include "defines.h"
#include "RTP.h"

static const char RTSPHeader[] = "RTSP/1.0 200 OK\r\n";

static const char SDP[] =	"s=SysDVR - https://github.com/exelix11/sysdvr \r\n"
							"m=video 0 RTP/AVP 96\r\n"
							"a=rtpmap:96 H264/90000\r\n"
							"a=fmtp:96 profile-level-id=42A01E; sprop-parameter-sets=Z2QMIKwrQCgC3TUBDQHggA==,aO48sA==;\r\n" /* Hardcoded SPS and PPS*/
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

static inline void CloseSocket(int* sptr)
{
	int sock = *sptr;
	*sptr = -1;
	if (sock != -1)
		close(sock);
}

static inline void RTSP_MainLoop();

void* RTSP_ServerThread(void* arg)
{
	Result rc = CreateSocket(&RTSPSock, 6666, 4, false);
	if (R_FAILED(rc)) fatalSimple(rc);

	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 3 seconds
	*/
	int sockFails = 0;
	RTSP_Running = true;
	while (RTSP_Running)
	{
		client = accept(RTSPSock, 0, 0);
		if (client < 0)
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
		fcntl(client, F_SETFL, fcntl(client, F_GETFL, 0) & ~O_NONBLOCK);
		RTSP_MainLoop();

		RTSP_ClientStreaming = false;
		CloseSocket(&client);
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

static RtspPorts ports[2];

static inline int RTSP_Send(const char* data, size_t len) 
{
	if (client < 0) return 1;
	if (write(client, data, len) <= 0)
	{
		//printl("RTSP_Send error %s %d \n", strerror(errno), errno);
		CloseSocket(&client);
		RTSP_ClientStreaming = false;
		return 1;
	}
	return 0;
}

//Send data over the data channel, RTCP is not used currently
static inline int RTSP_SendBinaryHeader(const size_t totalLen, unsigned int stream)
{
	char header[4];
	header[0] = '$';
	header[1] = ports[stream].data;
	header[2] = (totalLen & 0xFF00) >> 8;
	header[3] = totalLen & 0x00FF;
	return RTSP_Send(header, 4);
}

static inline int RTSP_SendRawData(const void* const data, const size_t len)
{
	return RTSP_Send(data, len);
}

//Not optimal but if the the first send fails the last is going to fail as well
static inline int RTSP_SendData(const void* const data, const size_t len, unsigned char stream)
{
	RTSP_SendBinaryHeader(len, stream);
	return RTSP_SendRawData(data, len);
}

int RTSP_H264SendPacket(const void* header, const void* extHeader, const size_t extLen, const void* data, const size_t len)
{
	RTSP_SendBinaryHeader(RTPHeaderSz + extLen + len, STREAM_VIDEO);
	RTSP_SendRawData(header, RTPHeaderSz);
	RTSP_SendRawData(extHeader, extLen);
	return RTSP_SendRawData(data, len);
}

int RTSP_LE16SendPacket(const void* header, const void* data, const size_t len)
{
	RTSP_SendBinaryHeader(RTPHeaderSz + len, STREAM_AUDIO);
	RTSP_SendRawData(header, RTPHeaderSz);
	return RTSP_SendRawData(data, len);
}

static inline void RTSP_StartPlayback() 
{
	RTSP_ClientStreaming = true;
}

static inline void RTSP_SendHeader()
{
	//printl("> %s", RTSPHeader);
	RTSP_Send(RTSPHeader, sizeof(RTSPHeader) - 1);
}

static inline void RTSP_SendTerminator()
{
	RTSP_Send("\r\n", 2);
}

static inline void RTSP_SendText(const char* source)
{
	//printl("> %s", source);
	RTSP_Send(source, strlen(source));
}

static inline void RTSP_SendFormat(int buf, const char* format, ...)
{
#ifdef __SWITCH__
	char toSend[buf];
#else
	char* toSend = (char*)malloc(buf);
#endif

	va_list args;
	va_start(args, format);
	vsnprintf(toSend, buf, format, args);
	va_end(args);

	RTSP_SendText(toSend);

#ifndef  __SWITCH__
	free(toSend);
#endif
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

//recev in non blocking mode
static inline int recvAll(int sock, char* data, int size)
{
	while (true)
	{
		int res = recv(sock, data, size, MSG_DONTWAIT);
		if (res == -1 && (errno == EWOULDBLOCK || errno == EAGAIN)) 
		{
			svcSleepThread(2E+8); 
			continue; 
		}
		else return res;
	}
}

static inline void RTSP_MainLoop()
{
	char rtspBuf[512];

	while (client > 0 && RTSP_Running)
	{
		int len = recvAll(client, rtspBuf, sizeof(rtspBuf));
		if (len <= 0) break;

		rtspBuf[len] = '\0';

		//printl("%s\n", rtspBuf);

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
					transport[sizeof(transport) - 1] = '\0';
					*transportOptEnd = original;
				}

				char* portSettings = strstr(transport, "interleaved=") + sizeof("interleaved");

				int targetStream = strstr(rtspBuf, "/video") ? STREAM_VIDEO : STREAM_AUDIO;

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
			break;
		}
	}
}