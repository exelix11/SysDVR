#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>
#include <pthread.h>

#include "grcd.h"
#include "modes/modes.h"

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
	#define INNER_HEAP_SIZE 1024
	#pragma message "Building USB-only mode"
#else
	//TODO It's probably possible to reduce memory usage by using a custom initialization for libnx sockets
	#define INNER_HEAP_SIZE 256 * 1024
	
	#include "rtsp/RTP.h"
	#include "sockUtil.h"
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
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

#if !defined(USB_ONLY)
	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	//rc = socketInitializeDefault();
	const SocketInitConfig initConfig = {
		.bsdsockets_version = 1,

		.tcp_tx_buf_size = MaxRTPPacketSize_TCP,
		.tcp_rx_buf_size = MaxRTPPacketSize_TCP,
		.tcp_tx_buf_max_size = MaxRTPPacketSize_TCP * 6,
		.tcp_rx_buf_max_size = MaxRTPPacketSize_TCP * 6,

		.udp_tx_buf_size = MaxRTPPacketSize_UDP * 2,
		.udp_rx_buf_size = MaxRTPPacketSize_UDP,

		.sb_efficiency = 4,

		.num_bsd_sessions = 3,
		.bsd_service_type = BsdServiceType_User,
	};
	rc = socketInitialize(&initConfig);
	if (R_FAILED(rc))
		fatalThrow(rc);
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
		fatalThrow(MAKERESULT(SYSDVR_CRASH_MODULEID, 10));

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

uint8_t alignas(0x1000) Vbuf[VbufSz];
uint8_t alignas(0x1000) Abuf[AbufSz * AudioBatchSz];
uint32_t VOutSz = 0;
uint32_t AOutSz = 0;

//Note: timestamps are in usecs
uint64_t VTimestamp = 0;
uint64_t ATimestamp = 0;

static Service grcdVideo;
static Service grcdAudio;

#if !defined(USB_ONLY)
atomic_bool IsThreadRunning = false;
#endif

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
void ReadAudioStream()
{
	u64 timestamp = 0;
	u32 TmpAudioSz = 0;
	AOutSz = 0;
	for (int i = 0, fails = 0; i < AudioBatchSz; i++)
	{
		Result res = grcdServiceTransfer(&grcdAudio, GrcStream_Audio, Abuf + AOutSz, AbufSz, NULL, &TmpAudioSz, &timestamp);
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
		if (i == 0)
			ATimestamp = timestamp;
	}
}

void ReadVideoStream()
{
	int fails = 0;

	while (true) {
		Result res = grcdServiceTransfer(&grcdVideo, GrcStream_Video, Vbuf, VbufSz, NULL, &VOutSz, &VTimestamp);
		if (R_SUCCEEDED(res) && VOutSz > 0) break;
		VOutSz = 0;
		if (fails++ > 8 && !IsThreadRunning) break;
	}
}

static pthread_t AudioThread;
#if !defined(USB_ONLY)
static pthread_t VideoThread;

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

static void ConfigThread()
{
	//Maybe hosting our own service is better but it looks harder than this
	int ConfigSock = -1, sockFails = 0;
	ConfigSock = CreateTCPListener(6668, true, 4);

	while (1)
	{
		int curSock = accept(ConfigSock, 0, 0);
		if (curSock < 0)
		{
			if (sockFails++ >= 3)
			{
				sockFails = 0;
				close(ConfigSock);
				ConfigSock = CreateTCPListener(6668, true, 4);
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
		else if (CurrentMode == &RTSP_MODE)
			Type = TYPE_MODE_RTSP;
		else fatalThrow(MAKERESULT(SYSDVR_CRASH_MODULEID, 15));

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
			case TYPE_MODE_RTSP:
				SetMode(&RTSP_MODE);
				break;
			case TYPE_MODE_NULL:
				SetMode(NULL);
				break;
			default:
				fatalThrow(MAKERESULT(SYSDVR_CRASH_MODULEID, 16));
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
	Result rc = OpenGrcdForThread(GrcStream_Audio);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = OpenGrcdForThread(GrcStream_Video);
	if (R_FAILED(rc)) fatalThrow(rc);

#if defined(USB_ONLY)
	USB_MODE.InitFn();
	pthread_create(&AudioThread, NULL, USB_MODE.MainThread, (void*)GrcStream_Audio);
	USB_MODE.MainThread((void*)GrcStream_Video);
	USB_MODE.ExitFn();
#else
	if (FileExists("/config/sysdvr/usb"))
		SetMode(&USB_MODE);
	else if (FileExists("/config/sysdvr/rtsp"))
		SetMode(&RTSP_MODE);
	else if (FileExists("/config/sysdvr/tcp"))
		SetMode(&TCP_MODE);

	ConfigThread();
	SetMode(NULL);
#endif

	grcdServiceClose(&grcdVideo);
	grcdServiceClose(&grcdAudio);

	return 0;
}
