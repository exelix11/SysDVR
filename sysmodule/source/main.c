#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>

#include "modes/modes.h"
#include "capture.h"

#ifdef RELEASE
#pragma message "Building release"
#else
#pragma message "Building debug"
//#define USB_ONLY
#endif

#ifdef USE_LOGGING
#pragma message "Logging is enabled"
#endif

/*
	Build with USB_ONLY to have a smaller impact on memory,
	it will only stream via USB and won't support the config app.
*/
#ifdef USB_ONLY
	#define INNER_HEAP_SIZE 100 * 1024
	#pragma message "Building USB-only version"
#else
	// Memory is carefully calculated, for development and logging it should be increased
	#define INNER_HEAP_SIZE 1024 * 1024
	#pragma message "Building full version"
	
	#include "rtsp/RTP.h"
	#include "ipc/ipc.h"
#endif

u32 __nx_applet_type = AppletType_None;
#ifndef USE_LOGGING
	u32 __nx_fs_num_sessions = 1;
	u32 __nx_fsdev_direntry_cache_size = 1;
#endif

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
	svcSleepThread(20E+9); // 20 seconds

	Result rc;

	rc = smInitialize();
	if (R_FAILED(rc))
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

#ifndef USB_ONLY
	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	const SocketInitConfig initConfig = {
		.bsdsockets_version = 1,
	
		.tcp_tx_buf_size = MaxRTPPacket,
		.tcp_rx_buf_size = MaxRTPPacket,
		.tcp_tx_buf_max_size = sizeof(VideoPacket) + sizeof(AudioPacket),
		.tcp_rx_buf_max_size = 0,
	
		.udp_tx_buf_size = MaxRTPPacket,
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

#ifndef USB_ONLY
	fsdevMountSdmc();
#endif
}

void __attribute__((weak)) __appExit(void)
{
#ifndef USB_ONLY
	fsdevUnmountAll();
	socketExit();
	fsExit();
#endif
	smExit();
}

#ifndef USB_ONLY
atomic_bool IsThreadRunning = false;
#endif

static inline void LaunchThreadEx(Thread* t, ThreadFunc f, void* arg, u32 stack, u32 prio)
{
	Result rc = threadCreate(t, f, arg, NULL, stack, prio, 3);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadStart(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

static inline void LaunchInternalThread(Thread* t, ThreadFunc f, void* arg)
{
	LaunchThreadEx(t, f, arg, 0x1000, 0x26);
}

// Only used by rtsp
void LaunchExtraThread(Thread* t, ThreadFunc f, void* arg)
{
	LaunchThreadEx(t, f, arg, 0x1000, 0x2D);
}

void JoinThread(Thread* t)
{
	Result rc = threadWaitForExit(t);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadClose(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

static Thread AudioThread;
#ifndef USB_ONLY
static Thread VideoThread;

const StreamMode* CurrentMode = NULL;
static atomic_bool IsSwitchingModes = false;

static void SetModeInternal(const void* argmode)
{ 
	if (IsSwitchingModes)
		fatalThrow(ERR_MAIN_SWITCHING);

	IsSwitchingModes = true;
	StreamMode* mode = (StreamMode*)argmode;

	if (CurrentMode)
	{
		LOG("Terminating mode\n");
		IsThreadRunning = false;
		svcSleepThread(5E+8);

		LOG("Unlocking consumers\n");
		CaptureForceUnlockConsumers();
	
		svcSleepThread(5E+8);
		LOG("Calling exit fn\n");
		if (CurrentMode->ExitFn)
			CurrentMode->ExitFn();

		LOG("Waiting video thread\n");
		if (CurrentMode->VThread)
			JoinThread(&VideoThread);
		
		LOG("Waiting audio thread\n");
		if (CurrentMode->AThread)
			JoinThread(&AudioThread);

		LOG("Terminated\n");
	}
	CurrentMode = mode;
	if (mode)
	{
		LOG("Starting mode\n");
		IsThreadRunning = true;
		svcSleepThread(5E+8);
		
		LOG("Calling init fn\n");
		if (mode->InitFn)
			mode->InitFn();
		
		LOG("Starting video thread\n");
		if (mode->VThread)
			LaunchInternalThread(&VideoThread, mode->VThread, mode->Vargs);
		
		LOG("Starting audio thread\n");
		if (mode->AThread)
			LaunchInternalThread(&AudioThread, mode->AThread, mode->Aargs);
	}
	IsSwitchingModes = false;
	LOG("Done\n");
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
		SetModeInternal(&USB_MODE);
		break;
	case TYPE_MODE_TCP:
		SetModeInternal(&TCP_MODE);
		break;
	case TYPE_MODE_RTSP:
		SetModeInternal(&RTSP_MODE);
		break;
	case TYPE_MODE_NULL:
		SetModeInternal(NULL);
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
	Result rc = CaptureStartThreads();
	if (R_FAILED(rc)) fatalThrow(rc);

#ifdef USB_ONLY
	USB_MODE.InitFn();
	// There is no USB thread dedicated to audio anymore
	USB_MODE.VThread(USB_MODE.Vargs);
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
	
	// Should dispose capture resources here...

	return 0;
}
