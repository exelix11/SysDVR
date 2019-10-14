#include <stdlib.h>
#include <switch.h>
#include <string.h>
#include <pthread.h>

#include "grcd.h"
#include "UsbSerial.h"

#if defined(RELEASE)
#pragma message "Building release"
#endif

//Silence visual studio errors
#if defined(__SWITCH__)
#include <stdatomic.h>
#else
#define __attribute__(x) 
typedef bool atomic_bool;
#endif

extern u32 __start__;
u32 __nx_applet_type = AppletType_None;
	
#define INNER_HEAP_SIZE 0x300000
size_t nx_inner_heap_size = INNER_HEAP_SIZE;
char nx_inner_heap[INNER_HEAP_SIZE];

void __libnx_initheap(void)
{
	void*  addr = nx_inner_heap;
	size_t size = nx_inner_heap_size;

	// Newlib
	extern char* fake_heap_start;
	extern char* fake_heap_end;

	fake_heap_start = (char*)addr;
	fake_heap_end   = (char*)addr + size;
}

void __attribute__((weak)) __appInit(void)
{
	svcSleepThread(2E+10); // 20 seconds

	Result rc;

	rc = smInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	rc = hidInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_HID));

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
	fsExit();
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

atomic_bool IsThreadRunning = false;

void AllocateRecordingBuf() 
{
	Vbuf = aligned_alloc(0x1000, VbufSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 40));

	Abuf = aligned_alloc(0x1000, AbufSz * AudioBatchSz);
	if (!Abuf)
		fatalSimple(MAKERESULT(1, 50));
}

void FreeRecordingBuf()
{
	free(Vbuf);
	free(Abuf);
}

Result OpenGrcdForThread(GrcStream stream) 
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
	for (int i = 0; i < AudioBatchSz; i++)
	{
		Result res = grcdServiceRead(&grcdAudio, GrcStream_Audio, Abuf + AOutSz, AbufSz, &unk, &TmpAudioSz, &timestamp);
		if (R_FAILED(res) || TmpAudioSz <= 0)
		{
			--i;
			continue;
		}
		AOutSz += TmpAudioSz;
	}
}

static void ReadVideoStream()
{
	u32 unk = 0;
	u64 timestamp = 0;

	while (true) {
		Result res = grcdServiceRead(&grcdVideo, GrcStream_Video, Vbuf, VbufSz, &unk, &VOutSz, &timestamp);
		if (R_FAILED(res) || VOutSz <= 0)
		{
			VOutSz = 0;
			if (IsThreadRunning)
				continue;
		}
		break;
	}
}

UsbInterface VideoStream;
UsbInterface AudioStream;
const u32 REQMAGIC_VID = 0xAAAAAAAA;
const u32 REQMAGIC_AUD = 0xBBBBBBBB;

static u32 USB_WaitForInputReq(UsbInterface* dev)
{
	while (IsThreadRunning)
	{
		u32 initSeq = 0;
		if (UsbSerialRead(dev, &initSeq, sizeof(initSeq), 1E+9) == sizeof(initSeq))
			return initSeq;
		svcSleepThread(2E+9);
	}
	return 0;
}

static void USB_SendStream(GrcStream stream, UsbInterface *Dev)
{
	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	u32 Magic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	if (*size <= 0)
		*size = 0;

	if (UsbSerialWrite(Dev, &Magic, sizeof(Magic), 1E+8) != sizeof(Magic))
		return;
	if (UsbSerialWrite(Dev, size, sizeof(*size), 1E+8) != sizeof(*size))
		return;

	if (*size)
	{
		u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;
		if (UsbSerialWrite(Dev, TargetBuf, *size, 2E+8) != *size) 
			return; 
	}
	return;
}

void* USB_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 67));

	GrcStream stream = (GrcStream)_stream;
	UsbInterface *Dev = stream == GrcStream_Video ? &VideoStream : &AudioStream;
	u32 ThreadMagic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	void (*ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	while (IsThreadRunning)
	{
		u32 cmd = USB_WaitForInputReq(Dev);

		if (cmd == ThreadMagic)
		{
			ReadStreamFn();
			USB_SendStream(stream, Dev);
		}
	}
	pthread_exit(NULL);
	return NULL;
}

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

Result CreateSocket(int *OutSock, int port, int baseError)
{
	int err = 0, sock = -1;
	struct sockaddr_in temp;

	sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock < 0)
		return MAKERESULT(baseError, 1);
	
	temp.sin_family = AF_INET;
	temp.sin_addr.s_addr = INADDR_ANY;
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

Result SocketingInit(GrcStream stream)
{
	Result rc;
	if (stream == GrcStream_Video)
	{
		if (VideoSock != -1) close(VideoSock);
		rc = CreateSocket(&VideoSock, 6666, 2);
		if (R_FAILED(rc)) return rc;
	}
	else 
	{
		if (AudioSock != -1) close(AudioSock);
		rc = CreateSocket(&AudioSock, 6667, 3);
		if (R_FAILED(rc)) return rc;
	}
	return 0;
}

const u8 SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
const u8 PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

void* TCP_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 66));

	GrcStream stream = (GrcStream)_stream;
	void (*ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

	int* sock = stream == GrcStream_Video ? &VideoSock : &AudioSock;
	int* OutSock = stream == GrcStream_Video ? &VideoCurSock : &AudioCurSock;

	{
		Result rc = SocketingInit(stream);
		if (R_FAILED(rc)) fatalSimple(rc);
	}

	int fails = 0;
	while (IsThreadRunning) {
		int curSock = accept(*sock, 0, 0);
		if (curSock < 0)
		{
			if (++fails > 2 && IsThreadRunning)
			{
				fails = 0;
				Result rc = SocketInit(stream);
				if (R_FAILED(rc)) fatalSimple(rc);
			}
			svcSleepThread(1E+9);
			continue;
		}
		
		/*
			There appear to be an issue with socketing, even if the video and audio listeners are used on different threads
			for some reason calling accept on one of them blocks the other thread as well, even while a client is connected.
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
			if (write(curSock, TargetBuf, *size) == -1)
				break;
		}

		
		close(curSock);
		*OutSock = -1;
		if (!IsThreadRunning) break;
		svcSleepThread(1E+9);
	}
	pthread_exit(NULL);
	return NULL;
}

typedef struct _streamMode
{
	void (*InitFn)();
	void (*ExitFn)();
	void* (*MainThread)(void*);
} StreamMode ;

static void USB_Init() 
{
	Result rc = UsbSerialInitializeDefault(&VideoStream, &AudioStream);
	if (R_FAILED(rc)) fatalSimple(rc);
}

static void USB_Exit() 
{
	usbSerialExit();
}

static void TCP_Init() 
{
	Result rc = socketInitializeDefault();
		if (R_FAILED(rc)) fatalSimple(rc);
}

static void TCP_Exit() 
{
#define CloseSock(x) do if (x != -1) {close(x); x = -1;} while(0)
	CloseSock(VideoSock);
	CloseSock(AudioSock);
	CloseSock(AudioCurSock);
	CloseSock(VideoCurSock);
#undef CloseSock
	socketExit();
}

StreamMode USB_MODE = { USB_Init, USB_Exit, USB_StreamThreadMain };
StreamMode TCP_MODE = { TCP_Init, TCP_Exit, TCP_StreamThreadMain };
StreamMode* CurrentMode = NULL;

pthread_t VideoThread, AudioThread;

void SetMode(StreamMode* mode)
{
	if (CurrentMode)
	{
		IsThreadRunning = false;

		if (CurrentMode->ExitFn)
			CurrentMode->ExitFn();
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

u32 VibrationDeviceHandles[2][2];
HidVibrationValue VibrationValue;
HidVibrationValue VibrationValue_stop;
HidVibrationValue VibrationValues[2];

void Vibrate() 
{
	int target_device = 0;
	if (!hidGetHandheldMode())
		target_device = 1;

	memcpy(&VibrationValues[0], &VibrationValue, sizeof(HidVibrationValue));
	memcpy(&VibrationValues[1], &VibrationValue, sizeof(HidVibrationValue));
	hidSendVibrationValues(VibrationDeviceHandles[target_device], VibrationValues, 2);
	
	svcSleepThread(5E+8);

	memcpy(&VibrationValues[0], &VibrationValue_stop, sizeof(HidVibrationValue));
	memcpy(&VibrationValues[1], &VibrationValue_stop, sizeof(HidVibrationValue));
	hidSendVibrationValues(VibrationDeviceHandles[target_device], VibrationValues, 2);
	hidSendVibrationValues(VibrationDeviceHandles[1 - target_device], VibrationValues, 2);
}

int main(int argc, char* argv[])
{
	AllocateRecordingBuf();

	Result rc = OpenGrcdForThread(GrcStream_Audio);
	if (R_FAILED(rc)) fatalSimple(rc);
	rc = OpenGrcdForThread(GrcStream_Video);
	if (R_FAILED(rc)) fatalSimple(rc);

	rc = hidInitializeVibrationDevices(VibrationDeviceHandles[0], 2, CONTROLLER_HANDHELD, TYPE_HANDHELD | TYPE_JOYCON_PAIR);
	if (R_SUCCEEDED(rc)) rc = hidInitializeVibrationDevices(VibrationDeviceHandles[1], 2, CONTROLLER_PLAYER_1, TYPE_HANDHELD | TYPE_JOYCON_PAIR);

	VibrationValue.amp_low = 0.2f;
	VibrationValue.freq_low = 10.0f;
	VibrationValue.amp_high = 0.2f;
	VibrationValue.freq_high = 10.0f;

	memset(VibrationValues, 0, sizeof(VibrationValues));

	memset(&VibrationValue_stop, 0, sizeof(HidVibrationValue));
	VibrationValue_stop.freq_low = 160.0f;
	VibrationValue_stop.freq_high = 320.0f;

	const u64 combo = KEY_L | KEY_R;

	while (1)
	{
		hidScanInput();
		u64 kDown = hidKeysHeld(CONTROLLER_P1_AUTO);

		if (kDown == (combo | KEY_MINUS))
		{
			SetMode(&USB_MODE);
			Vibrate();
		}
		else if (kDown == (combo | KEY_PLUS))
		{
			SetMode(&TCP_MODE);
			Vibrate();
		}
		else if (kDown == (combo | KEY_DDOWN))
			Vibrate();

		svcSleepThread(1E+9);
	}	
	
	SetMode(NULL);

	grcdServiceClose(&grcdVideo);
	grcdServiceClose(&grcdAudio);
	FreeRecordingBuf();

    return 0;
}
