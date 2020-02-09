#if !defined(USB_ONLY)
#include <switch.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <pthread.h>
#include <stdarg.h>
#include <assert.h>

#include "../sockUtil.h"
#include "RTSP.h"
#include "defines.h"

int MaxRTPPacket;
int MaxRTPPayload;

#define INTERLEAVED_SUPPORT
#define UDP_SUPPORT

#if !(defined(UDP_SUPPORT) || defined(INTERLEAVED_SUPPORT))
#pragma error no streaming mode is supported
#endif

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

static int RTSPSock = -1;
static int client = -1;

struct sockaddr_in clientAddress;
#ifdef UDP_SUPPORT
struct sockaddr_in clientVAddr;
struct sockaddr_in clientAAddr;
static int clientAudio = -1;
static int clientVideo = -1;
#endif

#ifdef INTERLEAVED_SUPPORT
/*
	When using interleaved mode directly sending data has to be locked between operations
	because to save memory most replies are composed by sending multiple buffers and we
	must prevent other threads from breaking the order.
	RTSP_Answer* and the encoder callback functions will call SOCKETING_LOCK,
	RTSP_Send* functions won't but should be used only in a locked context.
*/
static Mutex RTSP_operation_lock;
#define LOCK_SOCKETING mutexLock(&RTSP_operation_lock)
#define UNLOCK_SOCKETING mutexUnlock(&RTSP_operation_lock)
#else
#define LOCK_SOCKETING 
#define UNLOCK_SOCKETING 
#endif

#if (defined(INTERLEAVED_SUPPORT) && !defined(UDP_SUPPORT))
#define INTERLEAVED_FLAG_CONST true
#elif (!defined(INTERLEAVED_SUPPORT) && defined(UDP_SUPPORT))
#define INTERLEAVED_FLAG_CONST false
#endif

#ifdef INTERLEAVED_FLAG_CONST
static const bool RTSP_Transfer_interleaved = INTERLEAVED_FLAG_CONST;
#else 
//Are we streaming via TCP or UDP ?
static bool RTSP_Transfer_interleaved = false;
#endif

#define printl(...) 

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
	Result rc = CreateTCPListener(&RTSPSock, 6666, 4, false);
	if (R_FAILED(rc)) fatalSimple(rc);

#ifdef INTERLEAVED_SUPPORT
	mutexInit(&RTSP_operation_lock);
#endif
	int sockFails = 0;
	RTSP_Running = true;
	while (RTSP_Running)
	{
		unsigned int clientAddRlen = sizeof(clientAddress);
		client = accept(RTSPSock, (struct sockaddr*)&clientAddress, &clientAddRlen);
		if (client < 0)
		{
			if (sockFails++ >= 3 && RTSP_Running)
			{
				CloseSocket(&RTSPSock);
				Result rc = CreateTCPListener(&RTSPSock, 6666, 4, false);
				if (R_FAILED(rc)) fatalSimple(rc);
			}
			svcSleepThread(1E+9);
			continue;
		}
		
		fcntl(client, F_SETFL, fcntl(client, F_GETFL, 0) & ~O_NONBLOCK);
		RTSP_MainLoop();

		//MainLoop returns on TEARDOWN or error, wait for all the socketing operation to finish
		LOCK_SOCKETING;
		RTSP_ClientStreaming = false;
		CloseSocket(&client);
#ifdef UDP_SUPPORT
		CloseSocket(&clientAudio);
		CloseSocket(&clientVideo);
#endif
		UNLOCK_SOCKETING;
		
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
#ifdef UDP_SUPPORT
	CloseSocket(&clientAudio);
	CloseSocket(&clientVideo);
#endif
}

typedef struct
{
	unsigned short data, control;
} RtspPorts;

static RtspPorts ports[2];

static inline int RTSP_SendInternal(const char* data, size_t len)
{
	if (client < 0) return 1;
	if (send(client, data, len, 0) <= 0)
	{
		printl("RTSP_Send error %s %d \n", strerror(errno), errno);
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
	return RTSP_SendInternal(header, 4);
}

static inline int RTSP_SendRawData(const void* const data, const size_t len)
{
	return RTSP_SendInternal(data, len);
}

//Not optimal but if the the first send fails the last is going to fail as well
static inline int RTSP_SendData(const void* const data, const size_t len, unsigned char stream)
{
	RTSP_SendBinaryHeader(len, stream);
	return RTSP_SendRawData(data, len);
}

#ifdef UDP_SUPPORT
//For streaming via TCP it would be better to increase the max packet
char VideoSendBuffer[MaxRTPPacketSize_UDP];
#endif
int RTSP_H264SendPacket(const void* header, const size_t headerLen, const void* data, const size_t len)
{
	int res = 0;

	if (RTSP_Transfer_interleaved) {
#ifdef INTERLEAVED_SUPPORT
		LOCK_SOCKETING;
		RTSP_SendBinaryHeader(headerLen + len, STREAM_VIDEO);
		RTSP_SendRawData(header, headerLen);
		res = RTSP_SendRawData(data, len);
		UNLOCK_SOCKETING;
#endif
	}
	else
	{
#ifdef UDP_SUPPORT
		memcpy(VideoSendBuffer, header, headerLen);
		memcpy(VideoSendBuffer + headerLen, data, len);
		res = sendto(clientVideo, VideoSendBuffer, headerLen + len, 0, (struct sockaddr*) & clientVAddr, sizeof(clientVAddr)) < 0;
#endif
	}

	return res;
}

#ifdef UDP_SUPPORT
char AudioSendBuffer[MaxRTPPacketSize_UDP];
#endif
int RTSP_LE16SendPacket(const void* header, const void* data, const size_t len)
{
	int res = 0;

	if (RTSP_Transfer_interleaved)
	{
#ifdef INTERLEAVED_SUPPORT
		LOCK_SOCKETING;
		RTSP_SendBinaryHeader(RTPHeaderSz + len, STREAM_AUDIO);
		RTSP_SendRawData(header, RTPHeaderSz);
		res = RTSP_SendRawData(data, len);
		UNLOCK_SOCKETING;
#endif
	}
	else
	{
#ifdef UDP_SUPPORT
		memcpy(AudioSendBuffer, header, RTPHeaderSz);
		memcpy(AudioSendBuffer + RTPHeaderSz, data, len);
		res = sendto(clientVideo, AudioSendBuffer, RTPHeaderSz + len, 0, (struct sockaddr*) & clientAAddr, sizeof(clientAAddr)) < 0;
#endif
	}

	return res;
}

static inline void RTSP_StartPlayback()
{
	MaxRTPPacket = RTSP_Transfer_interleaved ? MaxRTPPacketSize_TCP : MaxRTPPacketSize_UDP;
	MaxRTPPayload = MaxRTPPacket - RTPHeaderSz;
	RTSP_ClientStreaming = true;
}

static inline void RTSP_SendHeader()
{
	printl("> %s", RTSPHeader);
	RTSP_SendInternal(RTSPHeader, sizeof(RTSPHeader) - 1);
}

static inline void RTSP_SendTerminator()
{
	RTSP_SendInternal("\r\n", 2);
}

static inline void RTSP_SendText(const char* source)
{
	printl("> %s", source);
	RTSP_SendInternal(source, strlen(source));
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

static inline void RTSP_SendCseq(const char* source)
{
	const char* cseq = strstr(source, "CSeq:");
	if (!cseq) return;

	RTSP_SendFormat(30, "CSeq: %d\r\n", atoi(cseq + sizeof("CSeq:")));
}

static inline void RTSP_AnswerError(const char* error)
{
	LOCK_SOCKETING;
	RTSP_SendFormat(80, "RTSP/1.0 %s\r\n", error);
	RTSP_SendTerminator();
	UNLOCK_SOCKETING;
}

static inline void RTSP_AnswerEmpty(char* source)
{
	LOCK_SOCKETING;
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendTerminator();
	UNLOCK_SOCKETING;
}

static inline void RTSP_AnswerText(char* source, const char* text)
{
	LOCK_SOCKETING;
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendText(text);
	RTSP_SendTerminator();
	UNLOCK_SOCKETING;
}

static inline void RTSP_AnswerTextContent(char* source, char* text, const char* content)
{
	LOCK_SOCKETING;
	RTSP_SendHeader();
	RTSP_SendCseq(source);
	RTSP_SendText("Content-Type: application/sdp\r\n");
	RTSP_SendFormat(30, "Content-Length: %d\r\n", strlen(content));
	RTSP_SendText(text);
	RTSP_SendTerminator();
	RTSP_SendText(content);
	UNLOCK_SOCKETING;
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

//Don't use RTSP_Send* functions here !
static inline void RTSP_MainLoop()
{
	char rtspBuf[512];

	while (client > 0 && RTSP_Running)
	{
		int len = recvAll(client, rtspBuf, sizeof(rtspBuf));
		if (len <= 0) break;

		rtspBuf[len] = '\0';

		printl("%s\n", rtspBuf);

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

			char contBase[80];
			snprintf(contBase, sizeof(contBase), "Content-Base: %s\r\n", arg1);
			*arg1End = original;

			RTSP_AnswerTextContent(rtspBuf, contBase, SDP);
		}
		else if (ISCOMMAND("SETUP"))
		{
			char* transportOpt = strstr(rtspBuf, "Transport:");

			if (!transportOpt)
			{
				RTSP_AnswerError("461 Unsupported transport");
				continue;
			}

			bool interleaved = false, udp = false;
#ifdef INTERLEAVED_SUPPORT
			interleaved = strstr(rtspBuf, "RTP/AVP/TCP") && strstr(rtspBuf, "interleaved=");
#endif		
#ifdef UDP_SUPPORT
			udp = strstr(rtspBuf, "RTP/AVP") && !strstr(rtspBuf, "RTP/AVP/TCP") && strstr(rtspBuf, "client_port=");
#endif
			if (interleaved || udp)
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

				const int targetStream = strstr(rtspBuf, "/video") ? STREAM_VIDEO : STREAM_AUDIO;

				if (interleaved)
				{
#ifdef INTERLEAVED_SUPPORT
					char* portSettings = strstr(transport, "interleaved=") + sizeof("interleaved");

					if (!portSettings || strlen(portSettings) < 3)
					{
						RTSP_AnswerError("461 Unsupported transport");
						continue;
					}

					ports[targetStream].data = portSettings[0] - '0';
					ports[targetStream].control = portSettings[2] - '0';

#ifndef INTERLEAVED_FLAG_CONST
					RTSP_Transfer_interleaved = true;
#endif					
#endif
				}
				else
				{
#ifdef UDP_SUPPORT
					char* portSettings = strstr(transport, "client_port=") + sizeof("client_port");
					char* endPort1 = portSettings;

					while (*endPort1 && *endPort1 != ' ' && *endPort1 != ';' && *endPort1 != '-' && *endPort1 != '\r') endPort1++;

					//TODO: parse RTCP port as well

					char oldChar = *endPort1;
					*endPort1 = '\0';

					ports[targetStream].data = atoi(portSettings);
					ports[targetStream].control = 0;

					assert(ports[targetStream].data);
					*endPort1 = oldChar;

					if (!ports[targetStream].data)
					{
						RTSP_AnswerError("461 Unsupported transport");
						continue;
					}

					struct sockaddr_in* updateAddr;
					if (targetStream == STREAM_VIDEO)
					{
						CloseSocket(&clientVideo);
						clientVideo = socket(AF_INET, SOCK_DGRAM, 0);
						updateAddr = &clientVAddr;
						assert(clientVideo >= 0);
					}
					else
					{
						CloseSocket(&clientAudio);
						clientAudio = socket(AF_INET, SOCK_DGRAM, 0);
						updateAddr = &clientAAddr;
						assert(clientAudio >= 0);
					}

					*updateAddr = clientAddress;
					updateAddr->sin_port = htons(ports[targetStream].data);

#ifndef INTERLEAVED_FLAG_CONST
					RTSP_Transfer_interleaved = false;
#endif
#endif
				}

				char transportRespnse[130];
				snprintf(transportRespnse, sizeof(transportRespnse), "Transport: %s\r\n", transport);
				RTSP_AnswerText(rtspBuf, transportRespnse);
			}
			else RTSP_AnswerError("461 Unsupported transport");
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
#endif