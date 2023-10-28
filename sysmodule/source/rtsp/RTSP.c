#if !defined(USB_ONLY)
#include <switch.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <stdarg.h>
#include <assert.h>

#include "../third_party/nanoprintf.h"
#include "../net/sockets.h"
#include "RTSP.h"

//For RTP.h
uint16_t SequenceNumbers[2];

#define RTSP_PORT 6666

#define INTERLEAVED_SUPPORT
#define UDP_SUPPORT

#if !(defined(UDP_SUPPORT) || defined(INTERLEAVED_SUPPORT))
#pragma error no streaming mode is supported
#endif

static const char SDP[] = "s=SysDVR - https://github.com/exelix11/sysdvr \r\n"
"m=video 0 RTP/AVP 96\r\n"
"a=rtpmap:96 H264/90000\r\n"
"a=fmtp:96 profile-level-id=42A01E; sprop-parameter-sets=Z2QMIKwrQCgC3TUBDQHggA==,aO48sA==;\r\n" /* Hardcoded SPS and PPS*/
"a=StreamName:string;\"Video\"\r\n"
"a=control:video\r\n"
"m=audio 0 RTP/AVP 97\r\n"
"a=rtpmap:97 L16/48000/2\r\n"
"a=StreamName:string;\"Audio\"\r\n"
"a=control:audio\r\n";

static int client = SOCKET_INVALID;

struct sockaddr_in clientAddress;
#ifdef UDP_SUPPORT
struct sockaddr_in clientVAddr;
struct sockaddr_in clientAAddr;
static int clientAudio = SOCKET_INVALID;
static int clientVideo = SOCKET_INVALID;
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

#define printl(...) LOG(__VA_ARGS__)

static atomic_bool RTSP_Running = false;
atomic_bool RTSP_ClientStreaming = false;

static inline void RTSP_MainLoop();

void RTSP_ServerThread(void* _)
{
#ifdef INTERLEAVED_SUPPORT
	mutexInit(&RTSP_operation_lock);
#endif
	RTSP_Running = true;

reconnect:
	int listener = SocketTcpListen(RTSP_PORT);
	while (RTSP_Running)
	{
		unsigned int clientAddRlen = sizeof(clientAddress);
		client = SocketTcpAccept(listener, (struct sockaddr*)&clientAddress, &clientAddRlen);
		if (client == SOCKET_INVALID)
		{
			if (!RTSP_Running)
				break;

			svcSleepThread(1E+9);
			if (SocketIsListenNetDown())
			{
				SocketClose(&listener);
				listener = SocketTcpListen(RTSP_PORT);
				SocketMakeNonBlocking(listener);
			}

			continue;
		}

		SocketClose(&listener);
		SocketMakeNonBlocking(client);
		RTSP_MainLoop();

		//MainLoop returns on TEARDOWN or error, wait for all the socketing operation to finish
		LOCK_SOCKETING;
		RTSP_ClientStreaming = false;
		SocketClose(&client);
#ifdef UDP_SUPPORT
		SocketClose(&clientAudio);
		SocketClose(&clientVideo);
#endif
		UNLOCK_SOCKETING;

		svcSleepThread(1E+9);

		goto reconnect;
	}

	SocketClose(&listener);
}

void RTSP_StopServer()
{
	RTSP_Running = false;
	RTSP_ClientStreaming = false;

	SocketClose(&client);
#ifdef UDP_SUPPORT
	SocketClose(&clientAudio);
	SocketClose(&clientVideo);
#endif
}

typedef struct
{
	unsigned short data, control;
} RtspPorts;

static RtspPorts ports[2];

static inline int RTSP_SendInternal(const char* data, size_t len)
{
	if (client < 0)
		return 1;

	int error = 0;

	LOCK_SOCKETING;
	if (!SocketSendAll(client, data, len))
	{
		printl("RTSP_Send error %d \n", g_bsdErrno);
		SocketClose(&client);
		RTSP_ClientStreaming = false;
		error = 1;
	}
	UNLOCK_SOCKETING;

	return error;
}

//Send data over the data channel, RTCP is not used currently
static inline void RTSP_PrepareBinaryheader(char header[4], const size_t totalLen, unsigned int stream)
{
	header[0] = '$';
	header[1] = ports[stream].data;
	header[2] = (totalLen & 0xFF00) >> 8;
	header[3] = totalLen & 0x00FF;
}

static inline int RTSP_SendRawData(const char* const data, const size_t len)
{
	return RTSP_SendInternal(data, len);
}

int RTSP_H264SendPacket(const void* header, const size_t headerLen, const void* data, const size_t len)
{
	int res = 0;

	char* vBuffer = Buffers.RTSPMode.VideoSendBuffer;
	if (len > sizeof(Buffers.RTSPMode.VideoSendBuffer) - RTSPBinHeaderSize)
		return -1;

	if (RTSP_Transfer_interleaved) {
#ifdef INTERLEAVED_SUPPORT
		RTSP_PrepareBinaryheader(vBuffer, headerLen + len, STREAM_VIDEO);
		memcpy(vBuffer + RTSPBinHeaderSize, header, headerLen);
		memcpy(vBuffer + RTSPBinHeaderSize + headerLen, data, len);
		res = RTSP_SendRawData(vBuffer, RTSPBinHeaderSize + headerLen + len);
#endif
	}
	else
	{
#ifdef UDP_SUPPORT
		memcpy(vBuffer, header, headerLen);
		memcpy(vBuffer + headerLen, data, len);
		res = SocketUDPSendTo(clientVideo, vBuffer, headerLen + len, (struct sockaddr*)&clientVAddr, sizeof(clientVAddr)) == false;
#endif
	}

	return res;
}

int RTSP_LE16SendPacket(const void* header, const void* data, const size_t len)
{
	int res = 0;

	char* aBuffer = Buffers.RTSPMode.AudioSendBuffer;
	if (len > sizeof(Buffers.RTSPMode.AudioSendBuffer) - RTSPBinHeaderSize)
		return -1;

	if (RTSP_Transfer_interleaved)
	{
#ifdef INTERLEAVED_SUPPORT
		RTSP_PrepareBinaryheader(aBuffer, RTPHeaderSz + len, STREAM_AUDIO);
		memcpy(aBuffer + RTSPBinHeaderSize, header, RTPHeaderSz);
		memcpy(aBuffer + RTSPBinHeaderSize + RTPHeaderSz, data, len);
		res = RTSP_SendRawData(aBuffer, RTSPBinHeaderSize + RTPHeaderSz + len);
#endif
	}
	else
	{
#ifdef UDP_SUPPORT
		memcpy(aBuffer, header, RTPHeaderSz);
		memcpy(aBuffer + RTPHeaderSz, data, len);
		res = SocketUDPSendTo(clientVideo, aBuffer, RTPHeaderSz + len, (struct sockaddr*)&clientAAddr, sizeof(clientAAddr)) == false;
#endif
	}

	return res;
}

static inline void RTSP_StartPlayback()
{
	RTSP_ClientStreaming = true;
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
	npf_vsnprintf(toSend, buf, format, args);
	va_end(args);

	RTSP_SendText(toSend);

#ifndef  __SWITCH__
	free(toSend);
#endif
}

#define FMT_HEADER_OK "RTSP/1.0 200 OK\r\n"
#define FMT_HEADER_S "RTSP/1.0 %s\r\n"
#define FMT_CSEQ_D "CSeq: %d\r\n"
#define FMT_CNTSDP "Content-Type: application/sdp\r\n"
#define FMT_CNTLEN_D "Content-Length: %d\r\n"
#define FMT_TERMINATOR "\r\n"

static inline int RTSP_ParseCseq(const char* source)
{
	const char* cseq = strstr(source, "CSeq:");
	if (!cseq)
		return 0;

	return atoi(cseq + sizeof("CSeq:"));
}

static inline void RTSP_AnswerError(const char* error)
{
	RTSP_SendFormat(85,
		FMT_HEADER_S
		FMT_TERMINATOR, error);
}

static inline void RTSP_AnswerEmpty(char* source)
{
	int Cseq = RTSP_ParseCseq(source);
	RTSP_SendFormat(200,
		FMT_HEADER_OK
		FMT_CSEQ_D
		FMT_TERMINATOR, Cseq);
}

static inline void RTSP_AnswerText(char* source, const char* text)
{
	int Cseq = RTSP_ParseCseq(source);

	RTSP_SendFormat(200,
		FMT_HEADER_OK
		FMT_CSEQ_D
		"%s"
		FMT_TERMINATOR, Cseq, text);
}

static inline void RTSP_AnswerTextSDPContent(char* source, char* text, const char* content)
{
	int Cseq = RTSP_ParseCseq(source);

	RTSP_SendFormat(800,
		FMT_HEADER_OK
		FMT_CSEQ_D
		FMT_CNTSDP
		FMT_CNTLEN_D
		"%s"
		FMT_TERMINATOR
		"%s", Cseq, strlen(content), text, content);
}


//recev in non blocking mode
static inline int recvAll(int sock, char* data, int size)
{
	s32 read = 0;
	while (read < size)
	{
		s32 res = SocketRecv(sock, data + read, size - read);
		if (res == 0) // EAGAIN 
		{
			// On avalid RTSP terminator we can stop reading
			if (read > 10 && !strncmp(data + read - 4, "\r\n\r\n", 4))
				return read;

			if (read == 0)
				// If there is no pending data we can sleep more to avoid using BSD sessions needed by other threads
				svcSleepThread(1E+8);
			else
				// Othwerise just wait a bit for sockets to be ready
				svcSleepThread(1);

			continue;
		}
		else if (res < 0)
			return -1;

		read += res;
	}
	return read;
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
			RTSP_AnswerText(rtspBuf, "Public: DESCRIBE, SETUP, PLAY, TEARDOWN\r\n");
		else if (ISCOMMAND("DESCRIBE"))
		{
			char* arg1 = rtspBuf + sizeof("DESCRIBE");
			char* arg1End = arg1;

			while (*arg1End != 0 && *arg1End != ' ' && *arg1End != '\r') arg1End++;

			char original = *arg1End;
			*arg1End = '\0';

			char contBase[80];
			npf_snprintf(contBase, sizeof(contBase), "Content-Base: %.63s\r\n", arg1);
			*arg1End = original;

			RTSP_AnswerTextSDPContent(rtspBuf, contBase, SDP);
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

					printl("Setting %d ports: %d %d\n", targetStream, ports[targetStream].data, ports[targetStream].control);

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

					*endPort1 = oldChar;

					if (!ports[targetStream].data)
					{
						RTSP_AnswerError("461 Unsupported transport");
						continue;
					}

					struct sockaddr_in* updateAddr;
					if (targetStream == STREAM_VIDEO)
					{
						SocketClose(&clientVideo);
						clientVideo = SocketUdp();
						updateAddr = &clientVAddr;
					}
					else
					{
						SocketClose(&clientAudio);
						clientAudio = SocketUdp();
						updateAddr = &clientAAddr;
					}

					*updateAddr = clientAddress;
					updateAddr->sin_port = htons(ports[targetStream].data);

#ifndef INTERLEAVED_FLAG_CONST
					RTSP_Transfer_interleaved = false;
#endif
#endif
				}

				char transportRespnse[150];
				npf_snprintf(transportRespnse, sizeof(transportRespnse), "Transport: %s\r\nSession: 69\r\n", transport);
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