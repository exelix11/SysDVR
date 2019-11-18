#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>
#include <pthread.h>

#include "grcd.h"
#include "UsbSerial.h"

#if defined(RELEASE)
#pragma message "Building release"
#else
//#define USB_ONLY
#endif

/*
	Build with USB_ONLY to have a smaller impact on memory,
	it will only stream via USB and won't support the config app.
	Socketing requires a lot more memory
*/
#if defined(USB_ONLY)
#define INNER_HEAP_SIZE 500 * 1024
#pragma message "Building USB-only mode"
#else
#define INNER_HEAP_SIZE 3 * 1024 * 1024
#if defined(__SWITCH__)
#include <stdatomic.h>
#endif
#endif

//Silence visual studio errors
#if !defined(__SWITCH__)
#define __attribute__(x) 
typedef bool atomic_bool;
#endif

extern u32 __start__;
u32 __nx_applet_type = AppletType_None;

size_t nx_inner_heap_size = INNER_HEAP_SIZE;
char nx_inner_heap[INNER_HEAP_SIZE];

void __libnx_initheap(void)
{
	void* addr = nx_inner_heap;
	size_t size = nx_inner_heap_size;

	// Newlib
	extern char* fake_heap_start;
	extern char* fake_heap_end;

	fake_heap_start = (char*)addr;
	fake_heap_end = (char*)addr + size;
}

void __attribute__((weak)) __appInit(void)
{
	svcSleepThread(2E+10); // 20 seconds

	Result rc;

	rc = smInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

#if !defined(USB_ONLY)
	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	rc = socketInitializeDefault();
	if (R_FAILED(rc)) fatalSimple(rc);
#endif

	rc = setsysInitialize();
	if (R_SUCCEEDED(rc)) {
		SetSysFirmwareVersion fw;
		rc = setsysGetFirmwareVersion(&fw);
		if (R_SUCCEEDED(rc))
			hosversionSet(MAKEHOSVERSION(fw.major, fw.minor, fw.micro));
		setsysExit();
	}

	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(1, 10));

	fsdevMountSdmc();
}

void __attribute__((weak)) __appExit(void)
{
	fsdevUnmountAll();
#if !defined(USB_ONLY)
	socketExit();
	fsExit();
#endif
	smExit();
}

const int VbufSz = 0x32000;
const int AbufSz = 0x1000;
const int AudioBatchSz = 12;

u8* Vbuf = NULL;
u8* Abuf = NULL;
u32 VOutSz = 0;
u32 AOutSz = 0;

Service grcdVideo;
Service grcdAudio;

#if defined(USB_ONLY)
const bool IsThreadRunning = true;
#else
/*
	Accessing this is rather slow, avoid using it in the main flow of execution.
	When stopping the main thread will close the sockets or dispose the usb interfaces, causing the others to fail
	Only in that case this variable should be checked
*/
atomic_bool IsThreadRunning = false;
#endif

static void AllocateRecordingBuf()
{
	Vbuf = aligned_alloc(0x1000, VbufSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 11));

	Abuf = aligned_alloc(0x1000, AbufSz * AudioBatchSz);
	if (!Abuf)
		fatalSimple(MAKERESULT(1, 12));
}

static void FreeRecordingBuf()
{
	free(Vbuf);
	free(Abuf);
}

static Result OpenGrcdForThread(GrcStream stream)
{
	Result rc;
	if (stream == GrcStream_Audio)
		rc = grcdServiceOpen(&grcdAudio);
	else
	{
		rc = grcdServiceOpen(&grcdVideo);
		if (R_FAILED(rc)) return rc;
		rc = grcdServiceBegin(&grcdVideo);
	}
	return rc;
}

//Batch sending audio samples to improve speed
static void ReadAudioStream()
{
	u32 unk = 0;
	u64 timestamp = 0;
	u32 TmpAudioSz = 0;
	AOutSz = 0;
	for (int i = 0, fails = 0; i < AudioBatchSz; i++)
	{
		Result res = grcdServiceRead(&grcdAudio, GrcStream_Audio, Abuf + AOutSz, AbufSz, &unk, &TmpAudioSz, &timestamp);
		if (R_FAILED(res) || TmpAudioSz <= 0)
		{
			if (fails++ > 9 && !IsThreadRunning)
			{
				AOutSz = 0;
				break;
			}
			--i;
			continue;
		}
		fails = 0;
		AOutSz += TmpAudioSz;
	}
}

static void ReadVideoStream()
{
	u32 unk = 0;
	u64 timestamp = 0;
	int fails = 0;

	while (true) {
		Result res = grcdServiceRead(&grcdVideo, GrcStream_Video, Vbuf, VbufSz, &unk, &VOutSz, &timestamp);
		if (R_SUCCEEDED(res) && VOutSz > 0) break;
		VOutSz = 0;
		if (fails++ > 8 && !IsThreadRunning) break;
	}
}

UsbInterface VideoStream;
UsbInterface AudioStream;
const u32 REQMAGIC_VID = 0xAAAAAAAA;
const u32 REQMAGIC_AUD = 0xBBBBBBBB;

static u32 USB_WaitForInputReq(const UsbInterface* dev)
{
	do
	{
		u32 initSeq = 0;
		if (UsbSerialRead(dev, &initSeq, sizeof(initSeq), 1E+9) == sizeof(initSeq))
			return initSeq;
		svcSleepThread(1E+9);
	} while (IsThreadRunning);
	return 0;
}

static void USB_SendStream(GrcStream stream, const UsbInterface* Dev)
{
	u32 *const size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	u32 Magic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	if (*size <= 0)
		*size = 0;

	if (UsbSerialWrite(Dev, &Magic, sizeof(Magic), 1E+8) != sizeof(Magic))
		return;
	if (UsbSerialWrite(Dev, size, sizeof(*size), 1E+8) != sizeof(*size))
		return;

	if (*size)
	{
		u8 *const TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;
		if (UsbSerialWrite(Dev, TargetBuf, *size, 2E+8) != *size)
			return;
	}
	return;
}

static void* USB_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 13));

	const GrcStream stream = (GrcStream)_stream;
	const UsbInterface *const Dev = stream == GrcStream_Video ? &VideoStream : &AudioStream;
	const u32 ThreadMagic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	void (*const ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	while (true)
	{
		u32 cmd = USB_WaitForInputReq(Dev);

		if (cmd == ThreadMagic)
		{
			ReadStreamFn();
			USB_SendStream(stream, Dev);
		}
		else if (!IsThreadRunning) break;
	}
	pthread_exit(NULL);
	return NULL;
}

#if !defined(USB_ONLY)
#ifdef __SWITCH__
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <fcntl.h>
#include <errno.h>
#else
//not actually used, just to stop visual studio from complaining.
//~~i regret nothing~~
#define F_SETFL 1
#define O_NONBLOCK 1
#define F_GETFL 1
#include <WinSock2.h>
#endif

static Result CreateSocket(int* OutSock, int port, int baseError, bool LocalOnly)
{
	int err = 0, sock = -1;
	struct sockaddr_in temp;

	sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock < 0)
		return MAKERESULT(baseError, 1);

	temp.sin_family = AF_INET;
	temp.sin_addr.s_addr = LocalOnly ? htonl(INADDR_LOOPBACK) : INADDR_ANY;
	temp.sin_port = htons(port);

	//We don't actually want a non-blocking socket but this is a workaround for the issue described in StreamThreadMain
	fcntl(sock, F_SETFL, O_NONBLOCK);

	const int optVal = 1;
	err = setsockopt(sock, SOL_SOCKET, SO_REUSEADDR, (void*)&optVal, sizeof(optVal));
	if (err)
		return MAKERESULT(baseError, 2);

	err = bind(sock, (struct sockaddr*) & temp, sizeof(temp));
	if (err)
		return MAKERESULT(baseError, 3);

	err = listen(sock, 1);
	if (err)
		return MAKERESULT(baseError, 4);

	*OutSock = sock;
	return 0;
}

int VideoSock = -1;
int AudioSock = -1;
int AudioCurSock = -1;
int VideoCurSock = -1;

static Result SocketingInit(GrcStream stream)
{
	Result rc;
	if (stream == GrcStream_Video)
	{
		if (VideoSock != -1) close(VideoSock);
		rc = CreateSocket(&VideoSock, 6666, 2, false);
		if (R_FAILED(rc)) return rc;
	}
	else
	{
		if (AudioSock != -1) close(AudioSock);
		rc = CreateSocket(&AudioSock, 6667, 3, false);
		if (R_FAILED(rc)) return rc;
	}
	return 0;
}

const u8 SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
const u8 PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

static void* TCP_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 14));

	const GrcStream stream = (GrcStream)_stream;
	void (*const ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	u32 *const size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	u8 *const TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

	int *const sock = stream == GrcStream_Video ? &VideoSock : &AudioSock;
	int *const OutSock = stream == GrcStream_Video ? &VideoCurSock : &AudioCurSock;

	{
		Result rc = SocketingInit(stream);
		if (R_FAILED(rc)) fatalSimple(rc);
	}

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
			{
				Result rc = SocketingInit(stream);
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

		*OutSock = curSock;

		if (stream == GrcStream_Video) {
			write(curSock, SPS, sizeof(SPS));
			write(curSock, PPS, sizeof(PPS));
		}

		while (true)
		{
			ReadStreamFn();
			if (write(curSock, TargetBuf, *size) <= 0)
				break;
		}

		close(curSock);
		*OutSock = -1;
		svcSleepThread(1E+9);
	}
	pthread_exit(NULL);
	return NULL;
}
#endif

static void USB_Init()
{
	Result rc = UsbSerialInitializeDefault(&VideoStream, &AudioStream);
	if (R_FAILED(rc)) fatalSimple(rc);
}

static void USB_Exit()
{
	usbSerialExit();
}

pthread_t AudioThread;
#if !defined(USB_ONLY)
pthread_t VideoThread;

//Don't need a TCP_Init funciton as each thread will initialize its own socket
static void TCP_Exit()
{
#define CloseSock(x) do if (x != -1) {close(x); x = -1;} while(0)
	CloseSock(VideoSock);
	CloseSock(AudioSock);
	CloseSock(AudioCurSock);
	CloseSock(VideoCurSock);
#undef CloseSock
}

typedef struct _streamMode
{
	void (*InitFn)();
	void (*ExitFn)();
	void* (*MainThread)(void*);
} StreamMode;

StreamMode USB_MODE = { USB_Init, USB_Exit, USB_StreamThreadMain };
StreamMode TCP_MODE = { NULL, TCP_Exit, TCP_StreamThreadMain };
StreamMode* CurrentMode = NULL;

static void SetMode(StreamMode* mode)
{
	if (CurrentMode)
	{
		IsThreadRunning = false;
		svcSleepThread(5E+8);
		if (CurrentMode->ExitFn)
			CurrentMode->ExitFn();
		/*
			If a client is connected this will hang as GrcdServiceRead will block till it acquires a new buffer,
			to resume you need to go back in the game and disconnect the client
		*/
		pthread_join(VideoThread, NULL);
		pthread_join(AudioThread, NULL);
	}
	CurrentMode = mode;
	if (mode)
	{
		IsThreadRunning = true;
		svcSleepThread(5E+8);
		if (mode->InitFn)
			mode->InitFn();
		pthread_create(&VideoThread, NULL, mode->MainThread, (void*)GrcStream_Video);
		pthread_create(&AudioThread, NULL, mode->MainThread, (void*)GrcStream_Audio);
	}
}

#define SYSDVR_VERSION 1
#define TYPE_MODE_USB 1
#define TYPE_MODE_TCP 2
#define TYPE_MODE_NULL 3

static void ConfigThread()
{
	//Maybe hosting our own service is better but it looks harder than this
	int ConfigSock = -1, sockFails = 0;
	{
		Result rc = CreateSocket(&ConfigSock, 6668, 3, true);
		if (R_FAILED(rc)) fatalSimple(rc);
	}

	while (1)
	{
		int curSock = accept(ConfigSock, 0, 0);
		if (curSock < 0)
		{
			if (sockFails++ >= 3)
			{
				sockFails = 0;
				close(ConfigSock);
				Result rc = CreateSocket(&ConfigSock, 6668, 3, true);
				if (R_FAILED(rc)) fatalSimple(rc);
			}
			svcSleepThread(1E+9);
			continue;
		}
		sockFails = 0;
		fcntl(curSock, F_SETFL, fcntl(curSock, F_GETFL, 0) & ~O_NONBLOCK);

		/*
			Very simple protocol, only consists of u32 excanges :
			Sysmodule sends version and current mode
			While client is connected:
				Client sends mode to set
				Sysmodule confirms
		*/
		u32 ver = SYSDVR_VERSION;
		write(curSock, &ver, sizeof(u32));

		u32 Type = 0;
		if (CurrentMode == NULL)
			Type = TYPE_MODE_NULL;
		else if (CurrentMode == &USB_MODE)
			Type = TYPE_MODE_USB;
		else if (CurrentMode == &TCP_MODE)
			Type = TYPE_MODE_TCP;
		else fatalSimple(MAKERESULT(1, 15));

		write(curSock, &Type, sizeof(u32));

		while (1)
		{
			u32 cmd = 0;
			if (read(curSock, &cmd, sizeof(cmd)) != sizeof(cmd))
				break;

			switch (cmd)
			{
			case TYPE_MODE_USB:
				SetMode(&USB_MODE);
				break;
			case TYPE_MODE_TCP:
				SetMode(&TCP_MODE);
				break;
			case TYPE_MODE_NULL:
				SetMode(NULL);
				break;
			default:
				fatalSimple(MAKERESULT(1, 16));
			}

			//Due to syncronization terminating the threads may require a few seconds, answer with the mode as confirmation
			write(curSock, &cmd, sizeof(u32));
		}

		svcSleepThread(1E+9);
		close(curSock);
	}
}

static bool FileExists(const char* fname)
{
	FILE* f = fopen(fname, "rb");
	if (f)
		return fclose(f), true;
	return false;
}
#endif

int main(int argc, char* argv[])
{
	AllocateRecordingBuf();

	Result rc = OpenGrcdForThread(GrcStream_Audio);
	if (R_FAILED(rc)) fatalSimple(rc);
	rc = OpenGrcdForThread(GrcStream_Video);
	if (R_FAILED(rc)) fatalSimple(rc);

#if defined(USB_ONLY)
	USB_Init();
	pthread_create(&AudioThread, NULL, USB_StreamThreadMain, (void*)GrcStream_Audio);
	USB_StreamThreadMain((void*)GrcStream_Video);
	USB_Exit();
#else
	if (FileExists("/config/sysdvr/usb"))
		SetMode(&USB_MODE);
	else if (FileExists("/config/sysdvr/tcp"))
		SetMode(&TCP_MODE);

	ConfigThread();
	SetMode(NULL);
#endif

	grcdServiceClose(&grcdVideo);
	grcdServiceClose(&grcdAudio);
	FreeRecordingBuf();

	return 0;
}
