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
		VideoSock = CreateTCPListener(6667, false, 2);
	}
	else
	{
		if (AudioSock != -1) close(AudioSock);
		AudioSock = CreateTCPListener(6668, false, 3);
	}
}

static void* TCP_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalThrow(MAKERESULT(SYSDVR_CRASH_MODULEID, 14));

	const GrcStream stream = (GrcStream)_stream;
	void (* const ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	const u32* const size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	const u8* const TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;
	const u64* const ts = stream == GrcStream_Video ? &VTimestamp : &ATimestamp;

	int* const sock = stream == GrcStream_Video ? &VideoSock : &AudioSock;
	int* const OutSock = stream == GrcStream_Video ? &VideoCurSock : &AudioCurSock;

	TCP_InitSockets(stream);

	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 3 seconds
	*/
	int sockFails = 0;
	while (IsThreadRunning) {
		int curSock = accept(*sock, 0, 0);
		if (curSock < 0)
		{
			if (sockFails++ >= 3 && IsThreadRunning)
				TCP_InitSockets(stream);
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

		*OutSock = curSock;

		while (true)
		{
			ReadStreamFn();

			const u32 StreamMagic = 0x11111111;
			if (write(curSock, &StreamMagic, sizeof(StreamMagic)) <= 0)
				break;
			if (write(curSock, ts, sizeof(u64)) <= 0)
				break;
			if (write(curSock, size, sizeof(*size)) <= 0)
				break;
			if (write(curSock, TargetBuf, *size) <= 0)
				break;
		}

		close(curSock);
		*OutSock = -1;
		svcSleepThread(1E+9);
	}
	return NULL;
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

StreamMode TCP_MODE = { NULL, TCP_Exit, TCP_StreamThreadMain };

#endif