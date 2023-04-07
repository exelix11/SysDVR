#if !defined(USB_ONLY)
#include "modes.h"
#include "../capture.h"
#include "../sockUtil.h"

static int VideoSock = -1, AudioSock = -1, VideoCurSock = -1, AudioCurSock = -1;
static inline void TCP_InitSockets(GrcStream stream)
{
	if (stream == GrcStream_Video)
	{
		if (VideoSock != -1) close(VideoSock);
		VideoSock = CreateTCPListener(9911, false, ERR_SOCK_TCP_1);
	}
	else
	{
		if (AudioSock != -1) close(AudioSock);
		AudioSock = CreateTCPListener(9922, false, ERR_SOCK_TCP_2);
	}
}

static inline bool SendData(int sock, PacketHeader header, const char* FullPacket)
{
	u32 toSend = sizeof(PacketHeader) + header.DataSize; 

	while (toSend)
	{
		int res = send(sock, FullPacket, toSend, 0);

		if (res < 0)
		{
			if (errno == EWOULDBLOCK || errno == EAGAIN)
			{
				svcSleepThread(1);
				continue;
			}
			else return false;
		}
		
		toSend -= res;
		FullPacket += res;
	}

	return true;
}

static inline bool GetClient(int *client, int *sock, GrcStream stream)
{
	int fails = 0;
	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 5 seconds
	*/
	while (IsThreadRunning)
	{
		*client = accept(*sock, 0, 0);
		if (*client < 0)
		{
			svcSleepThread(1E+9);
			if (++fails >= 5)
			{
				fails = 0;
				TCP_InitSockets(stream);
			}
			continue;
		}

		return true;
	}
	return false;
}

static void TCP_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_VIDEO);

	while (IsThreadRunning) {
		if (!GetClient(&VideoCurSock, &VideoSock, GrcStream_Video))
			continue;

		CaptureOnClientConnected(&VideoProducer);

		while (true)
		{
			CaptureBeginConsume(&VideoProducer);
			bool success = IsThreadRunning && SendData(VideoCurSock, VPkt.Header, (const char*)&VPkt);
			CaptureEndConsume(&VideoProducer);

			if (!success)
				break;			
		}

		CaptureOnClientDisconnected(&VideoProducer);
		close(VideoCurSock);
		VideoCurSock = -1;
		svcSleepThread(2E+8);
	}
}

static void TCP_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_AUDIO);

	while (IsThreadRunning) 
	{
		if (!GetClient(&AudioCurSock, &AudioSock, GrcStream_Audio))
			continue;

		CaptureOnClientConnected(&AudioProducer);

		while (true)
		{
			CaptureBeginConsume(&AudioProducer);
			bool success = IsThreadRunning && SendData(AudioCurSock, APkt.Header, (const char*)&APkt);
			CaptureEndConsume(&AudioProducer);
			
			if (!success)
				break;
		}

		CaptureOnClientDisconnected(&AudioProducer);
		close(AudioCurSock);
		AudioCurSock = -1;
		svcSleepThread(2E+8);
	}
}

static void TCP_Init() 
{
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
	TCP_InitSockets(GrcStream_Video);
	TCP_InitSockets(GrcStream_Audio);
}

static void TCP_Exit()
{
#define CloseSock(x) do if (x != -1) {close(x); x = -1;} while(0)
	CloseSock(VideoSock);
	CloseSock(AudioSock);
	CloseSock(AudioCurSock);
	CloseSock(VideoCurSock);
#undef CloseSock
}

const StreamMode TCP_MODE = { TCP_Init, TCP_Exit, TCP_StreamVideo, TCP_StreamAudio, NULL, NULL };

#endif