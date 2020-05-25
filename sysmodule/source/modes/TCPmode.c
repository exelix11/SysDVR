#if !defined(USB_ONLY)
#include "modes.h"
#include "../grcd.h"
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

static inline bool SendData(int sock, PacketHeader header, const void* FullPacket)
{
	const u32 size = sizeof(PacketHeader) + header.DataSize; 

	if (write(sock, FullPacket, size) <= 0)
		return false;

	return true;
}

static inline bool GetClient(int *client, int *sock, GrcStream stream)
{
	int fails = 0;
	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 3 seconds
	*/
	while (IsThreadRunning)
	{
		*client = accept(*sock, 0, 0);
		if (*client < 0)
		{
			svcSleepThread(1E+9);
			if (fails++ >= 3)
			{
				fails = 0;
				TCP_InitSockets(stream);
			}
			continue;
		}
		/*
			Even if the video and audio listeners are used on different threads calling accept on one of them
			blocks the others as well, even while a client is connected.
			The workaround is making the socket non-blocking and then to set the client socket as blocking.
			By default the socket returned from accept inherits this flag.
		*/
		fcntl(*client, F_SETFL, fcntl(*client, F_GETFL, 0) & ~O_NONBLOCK);
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

		while (true)
		{
			if (!ReadVideoStream())
				TerminateOrContinue

			if (!SendData(VideoCurSock, VPkt.Header, &VPkt)) 
				break;			
		}

		close(VideoCurSock);
		VideoCurSock = -1;
		svcSleepThread(1E+9);
	}
}

static void TCP_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_AUDIO);

	while (IsThreadRunning) {
		if (!GetClient(&AudioCurSock, &AudioSock, GrcStream_Audio))
			continue;

		while (true)
		{
			if (!ReadAudioStream())
				TerminateOrContinue

			if (!SendData(AudioCurSock, APkt.Header, &APkt))
				break;
		}

		close(AudioCurSock);
		AudioCurSock = -1;
		svcSleepThread(1E+9);
	}
}

static void TCP_Init() 
{
	VPkt.Header.Magic = 0xAAAAAAAA;
	APkt.Header.Magic = 0xAAAAAAAA;
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

StreamMode TCP_MODE = { TCP_Init, TCP_Exit, TCP_StreamVideo, TCP_StreamAudio };

#endif