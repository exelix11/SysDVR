#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>

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
*/
#if defined(USB_ONLY)
	#define INNER_HEAP_SIZE 100 * 1024
	#pragma message "Building USB-only mode"
#else
	#define INNER_HEAP_SIZE 1024 * 1024
	
	#include "rtsp/RTP.h"
	#include "ipc/ipc.h"
#endif

u32 __nx_applet_type = AppletType_None;
u32 __nx_fs_num_sessions = 1;
u32 __nx_fsdev_direntry_cache_size = 1;

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

	const SocketInitConfig initConfig = {
		.bsdsockets_version = 1,
	
		.tcp_tx_buf_size = MaxRTPPacket,
		.tcp_rx_buf_size = MaxRTPPacket,
		.tcp_tx_buf_max_size = sizeof(VideoPacket) + sizeof(AudioPacket),
		.tcp_rx_buf_max_size = 0,
	
		.udp_tx_buf_size = MaxRTPPacket * 2,
		.udp_rx_buf_size = MaxRTPPacket,
	
		.sb_efficiency = 2,
	
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

#if !defined(USB_ONLY)
	fsdevMountSdmc();
#endif
}

void __attribute__((weak)) __appExit(void)
{
#if !defined(USB_ONLY)
	fsdevUnmountAll();
	socketExit();
	fsExit();
#endif
	smExit();
}

VideoPacket alignas(0x1000) VPkt;
AudioPacket alignas(0x1000) APkt;

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

bool ReadAudioStream()
{
	Result rc = grcdServiceTransfer(&grcdAudio, GrcStream_Audio, APkt.Data, AbufSz, NULL, &APkt.Header.DataSize, &APkt.Header.Timestamp);
	
	for (int i = 1; i < ABatching && R_SUCCEEDED(rc); i++)
	{
		u32 tmpSize = 0;
		rc = grcdServiceTransfer(&grcdAudio, GrcStream_Audio, APkt.Data + APkt.Header.DataSize, AbufSz, NULL, &tmpSize, NULL);
		APkt.Header.DataSize += tmpSize;
	}

	return R_SUCCEEDED(rc);
}

static const uint8_t SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
static const uint8_t PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

bool ReadVideoStream()
{		
	static int SPSCount = 0;
	
	Result res = grcdServiceTransfer(&grcdVideo, GrcStream_Video, VPkt.Data, VbufSz, NULL, &VPkt.Header.DataSize, &VPkt.Header.Timestamp);
	bool result = R_SUCCEEDED(res) && VPkt.Header.DataSize > 0;

	//If there's space append SPS and PPS every once in a while
	if (++SPSCount > 500 && result && (VbufSz - VPkt.Header.DataSize) >= (sizeof(PPS) + sizeof(SPS)))
	{
		SPSCount = 0;
		memcpy(VPkt.Data + VPkt.Header.DataSize, SPS, sizeof(SPS));
		memcpy(VPkt.Data + VPkt.Header.DataSize + sizeof(SPS), PPS, sizeof(PPS));
		VPkt.Header.DataSize += sizeof(SPS) + sizeof(PPS);
	}

	return result;
}

void LaunchThreadEx(Thread* t, ThreadFunc f, void* arg, int prio, int cpuid)
{
	Result rc = threadCreate(t, f, arg, NULL, 0x2000, prio, cpuid);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadStart(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

void LaunchThread(Thread* t, ThreadFunc f, void* arg)
{
	LaunchThreadEx(t, f, arg, 0x3F, 3);
}

void JoinThread(Thread* t)
{
	Result rc = threadWaitForExit(t);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadClose(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

static Thread AudioThread;
#if !defined(USB_ONLY)
static Thread VideoThread;

StreamMode* CurrentMode = NULL;
static atomic_bool IsSwitchingModes = false;

static void SetModeInternal(void* argmode)
{ 
	IsSwitchingModes = true;
	StreamMode* mode = argmode;

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
		if (CurrentMode->VThread)
			JoinThread(&VideoThread);
		if (CurrentMode->AThread)
			JoinThread(&AudioThread);
	}
	CurrentMode = mode;
	if (mode)
	{
		IsThreadRunning = true;
		svcSleepThread(5E+8);
		if (mode->InitFn)
			mode->InitFn();
		if (mode->VThread)
			LaunchThread(&VideoThread, mode->VThread, NULL);
		if (mode->AThread)
			LaunchThread(&AudioThread, mode->AThread, NULL);
	}
	IsSwitchingModes = false;
}

static Thread SwitchingThread;
static void BeginSetMode(StreamMode* mode)
{	
	if (IsSwitchingModes)
		fatalThrow(ERR_MAIN_SWITCHING);

	if (SwitchingThread.handle)
	{
		threadClose(&SwitchingThread);
		SwitchingThread.handle = 0;
	}

	/* 
		Using the default LaunchThread options seems to cause usbdsExit to hang forever, not sure why.
		May want to change proprity to other threads too but would require too much testing to make sure it doesn't affect performances in games, 
		the current settings are already battle-tested.
		This is called by the IPC thread, we use another thread to set modes as it can get stuck.
		By keeping the IPC thread free the settings app can show a proper error message instead of hanging while connecting.
	*/
	LaunchThreadEx(&SwitchingThread, SetModeInternal, mode, 0x2C, -2);
}

u32 GetCurrentMode()
{
	if (IsSwitchingModes)
		return TYPE_MODE_SWITCHING;

	if (CurrentMode == NULL)
		return TYPE_MODE_NULL;
	else if (CurrentMode == &USB_MODE)
		return TYPE_MODE_USB;
	else if (CurrentMode == &TCP_MODE)
		return TYPE_MODE_TCP;
	else if (CurrentMode == &RTSP_MODE)
		return TYPE_MODE_RTSP;
	else fatalThrow(ERR_MAIN_UNKMODE);
}

bool CanChangeMode()
{
	return !IsSwitchingModes;
}

void SetModeID(u32 mode)
{
	switch (mode)
	{
	case TYPE_MODE_USB:
		BeginSetMode(&USB_MODE);
		break;
	case TYPE_MODE_TCP:
		BeginSetMode(&TCP_MODE);
		break;
	case TYPE_MODE_RTSP:
		BeginSetMode(&RTSP_MODE);
		break;
	case TYPE_MODE_NULL:
		BeginSetMode(NULL);
		break;
	default:
		fatalThrow(ERR_MAIN_UNKMODESET);
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
#ifdef USE_LOGGING
	freopen("/sysdvr_log.txt", "w", stdout);
#endif

	Result rc = OpenGrcdForThread(GrcStream_Audio);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = OpenGrcdForThread(GrcStream_Video);
	if (R_FAILED(rc)) fatalThrow(rc);

#if defined(USB_ONLY)
	USB_MODE.InitFn();
	LaunchThread(&AudioThread, USB_MODE.AThread, NULL);
	USB_MODE.VThread(NULL);
	USB_MODE.ExitFn();
#else
	if (FileExists("/config/sysdvr/usb"))
		SetModeInternal(&USB_MODE);
	else if (FileExists("/config/sysdvr/rtsp"))
		SetModeInternal(&RTSP_MODE);
	else if (FileExists("/config/sysdvr/tcp"))
		SetModeInternal(&TCP_MODE);

	IpcThread();
#endif

	grcdServiceClose(&grcdVideo);
	grcdServiceClose(&grcdAudio);

	return 0;
}
