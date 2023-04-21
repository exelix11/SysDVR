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

#if FILE_LOGGING
#pragma message "File logging is enabled"
#endif

#if UDP_LOGGING
#pragma message "UDP logging is enabled"
#endif

#if LOGGING_ENABLED
#pragma message "You're building with logging enabled, this increases the heap size, remember to test without logging."
#endif

// Memory is carefully calculated, for development and logging it is increased
// Note that sysdvr makes an effort to not use any dynamic allocation, this memory is only needed by libnx itself
// All big buffers needed by sysdvr are statically allocated in the compile units where they're needed
// For buffers that are mutually exclusive like ones used by the different streaming modes, we use the StaticBuffers union
#define INNER_HEAP_SIZE (10 * 1024 + LOGGING_HEAP_BOOST)

/*
	Build with USB_ONLY to have a smaller impact on memory,
	it will only stream via USB and won't support the config app.
	Note that memory savings don't come from the INNER_HEAP_SIZE variable
	but from the statically allocated stacks and buffers needed for network modes
*/
#ifdef USB_ONLY
	#pragma message "Building USB-only version"
#else
	#pragma message "Building full version"
	
	#include "ipc/ipc.h"
#endif

#if !defined(USB_ONLY) || UDP_LOGGING
	#include "net/sockets.h"
#endif

u32 __nx_applet_type = AppletType_None;
#ifndef FILE_LOGGING
	u32 __nx_fs_num_sessions = 1;
	u32 __nx_fsdev_direntry_cache_size = 1;
#endif

size_t nx_inner_heap_size = INNER_HEAP_SIZE;
char nx_inner_heap[INNER_HEAP_SIZE];

// Statically allocate all needed buffers
StaticBuffers Buffers;

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

#if !defined(USB_ONLY) || FILE_LOGGING
	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));
#endif

#if !defined(USB_ONLY)
	SocketInit();
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
		fatalThrow(ERR_INIT_FAILED);

#if !defined(USB_ONLY) || FILE_LOGGING
	fsdevMountSdmc();
#endif
}

void __attribute__((weak)) __appExit(void)
{
#if !defined(USB_ONLY) || UDP_LOGGING
	SocketDeinit();
#endif
#if !defined(USB_ONLY) || FILE_LOGGING
	fsdevUnmountAll();
	fsExit();
#endif
	smExit();
}

#ifndef USB_ONLY
atomic_bool IsThreadRunning = false;

static u8 alignas(0x1000) VStreamStackArea[0x2000 + LOGGING_HEAP_BOOST];
static u8 alignas(0x1000) AStreamStackArea[0x2000 + LOGGING_HEAP_BOOST];
#endif

void LaunchThread(Thread* t, ThreadFunc f, void* arg, void* stackLocation, u32 stackSize, u32 prio)
{
	Result rc = threadCreate(t, f, arg, stackLocation, stackSize, prio, 3);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadStart(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

void JoinThread(Thread* t)
{
	Result rc = threadWaitForExit(t);
	if (R_FAILED(rc)) fatalThrow(rc);
	rc = threadClose(t);
	if (R_FAILED(rc)) fatalThrow(rc);
}

#ifndef USB_ONLY
static Thread AudioThread;
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

		LOG("Unlocking consumers\n");
		CaptureForceUnlockConsumers();

		LOG("Waiting video thread\n");
		if (CurrentMode->VThread)
			JoinThread(&VideoThread);
		
		LOG("Waiting audio thread\n");
		if (CurrentMode->AThread)
			JoinThread(&AudioThread);

		LOG("Calling exit fn\n");
		if (CurrentMode->ExitFn)
			CurrentMode->ExitFn();

		LOG("Terminated\n");
	}
	CurrentMode = mode;
	if (mode)
	{
		LOG("Starting mode\n");
		IsThreadRunning = true;
		memset(&Buffers, 0, sizeof(Buffers));
		
		LOG("Calling init fn\n");
		if (mode->InitFn)
			mode->InitFn();
		
		LOG("Starting video thread\n");
		if (mode->VThread) {
			memset(VStreamStackArea, 0, sizeof(VStreamStackArea));
			LaunchThread(&VideoThread, mode->VThread, mode->Vargs, VStreamStackArea, sizeof(VStreamStackArea), 0x26);
		}

		LOG("Starting audio thread\n");
		if (mode->AThread) {
			memset(AStreamStackArea, 0, sizeof(AStreamStackArea));
			LaunchThread(&AudioThread, mode->AThread, mode->Aargs, AStreamStackArea, sizeof(AStreamStackArea), 0x26);
		}
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
