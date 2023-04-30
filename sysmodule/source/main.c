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

#if NEEDS_SOCKETS
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

#if NEEDS_FS
	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalThrow(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));
#endif

#if NEEDS_SOCKETS
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

#if NEEDS_FS
	fsdevMountSdmc();
#endif
}

void __attribute__((weak)) __appExit(void)
{
#if NEEDS_SOCKETS
	SocketDeinit();
#endif
#if NEEDS_FS
	fsdevUnmountAll();
	fsExit();
#endif
	smExit();
}

#ifndef USB_ONLY
atomic_bool IsThreadRunning = false;

static u8 alignas(0x1000) VStreamStackArea[0x2000 + LOGGING_HEAP_BOOST];
#endif
static u8 alignas(0x1000) AStreamStackArea[0x2000 + LOGGING_HEAP_BOOST];

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

static Thread AudioThread;
#ifndef USB_ONLY
static Thread VideoThread;

const StreamMode* CurrentMode = NULL;

// Mode switching is complicated because the streaming thread can get stuck indefinitely waiting for grc when a game is not running
// So we fake mode switching: when the user puts in a request a thread starts and waits for the streaming thread to exit
// the request can be changed at any time while this thread is waiting, once the streaming threads actually exit the new request is processed
static u8 alignas(0x1000) ModeSwitchingStackArea[0x2000 + LOGGING_HEAP_BOOST];
static atomic_bool IsModeSwitchPending = false;
static Thread ModeSwitchThread;
static Mutex ModeSwitchingMutex;
static const StreamMode* SwitchModeTarget = NULL;

// Configurable user parameters
static UserOverrides Overrides = { false, 0, 0 };

void ApplyUserOverrides(UserOverrides overrides)
{
	Overrides = overrides;
	if (!overrides.Enabled)
	{
		if (CurrentMode)
			CaptureSetAudioBatching(CurrentMode->AudioBatches);
		CaptureResetStaticDropThreshold();
	}
	else 
	{
		CaptureSetAudioBatching(overrides.AudioBatching);
		CaptureSetStaticDropThreshold(overrides.StaticDropThreshold);
	}
}

UserOverrides GetUserOverrides()
{
	UserOverrides res;
	res.Enabled = Overrides.Enabled;
	res.AudioBatching = CaptureGetAudioBatching();
	res.StaticDropThreshold = CaptureGetStaticDropThreshold();
	return res;
}

const StreamMode* GetUserVisibleMode() {
	mutexLock(&ModeSwitchingMutex);
	
	const StreamMode* ret = CurrentMode;

	if (IsModeSwitchPending)
		ret = SwitchModeTarget;

	mutexUnlock(&ModeSwitchingMutex);
	return ret;
}

void EnterTargetMode()
{
	mutexLock(&ModeSwitchingMutex);

	if (CurrentMode)
		fatalThrow(ERR_MAIN_UNEXPECTED_MODE);

	CurrentMode = SwitchModeTarget;
	SwitchModeTarget = NULL;

	if (CurrentMode)
	{
		LOG("Starting mode\n");
		IsThreadRunning = true;
		memset(&Buffers, 0, sizeof(Buffers));

		// Reset capture options depending on the state of overrides
		ApplyUserOverrides(Overrides);

		LOG("Calling init fn\n");
		if (CurrentMode->InitFn)
			CurrentMode->InitFn();

		LOG("Starting video thread\n");
		if (CurrentMode->VThread) {
			memset(VStreamStackArea, 0, sizeof(VStreamStackArea));
			LaunchThread(&VideoThread, CurrentMode->VThread, CurrentMode->Vargs, VStreamStackArea, sizeof(VStreamStackArea), 0x2C);
		}

		LOG("Starting audio thread\n");
		if (CurrentMode->AThread) {
			memset(AStreamStackArea, 0, sizeof(AStreamStackArea));
			LaunchThread(&AudioThread, CurrentMode->AThread, CurrentMode->Aargs, AStreamStackArea, sizeof(AStreamStackArea), 0x2C);
		}
	}

	IsModeSwitchPending = false;

	LOG("Mode started\n");
	mutexUnlock(&ModeSwitchingMutex);
}

void ExitCurrentMode()
{
	if (CurrentMode)
	{
		LOG("Terminating mode\n");
		IsThreadRunning = false;

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
		CurrentMode = NULL;
	}
}

void SwitchModesThreadMain(void*)
{
	// This will take forever
	ExitCurrentMode();

	// This is fast
	EnterTargetMode();
}

// To be used only for initialization
void SetModeInternal(const StreamMode* mode)
{
	SwitchModeTarget = mode;
	EnterTargetMode();
}

void SwitchModes(const StreamMode* mode)
{
	mutexLock(&ModeSwitchingMutex);

	LOG("Mode switch requested\n");
	SwitchModeTarget = mode;

	if (!IsModeSwitchPending)
	{
		IsModeSwitchPending = true;

		if (ModeSwitchThread.handle) {
			LOG("Closing old mode switch thread\n");
			JoinThread(&ModeSwitchThread);
		}

		LOG("Launching mode switch thread\n");
		LaunchThread(&ModeSwitchThread, SwitchModesThreadMain, NULL, ModeSwitchingStackArea, sizeof(ModeSwitchingStackArea), 0x2C);
	}

	mutexUnlock(&ModeSwitchingMutex);
}

u32 GetCurrentMode()
{
	const StreamMode* mode = GetUserVisibleMode();

	if (mode == NULL)
		return TYPE_MODE_NULL;
	else if (mode == &USB_MODE)
		return TYPE_MODE_USB;
	else if (mode == &TCP_MODE)
		return TYPE_MODE_TCP;
	else if (mode == &RTSP_MODE)
		return TYPE_MODE_RTSP;
	else fatalThrow(ERR_MAIN_UNKMODE);
}

void SetModeID(u32 mode)
{
	switch (mode)
	{
	case TYPE_MODE_USB:
		SwitchModes(&USB_MODE);
		break;
	case TYPE_MODE_TCP:
		SwitchModes(&TCP_MODE);
		break;
	case TYPE_MODE_RTSP:
		SwitchModes(&RTSP_MODE);
		break;
	case TYPE_MODE_NULL:
		SwitchModes(NULL);
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
	Result rc = CaptureInitialize();
	if (R_FAILED(rc)) fatalThrow(rc);

#ifdef USB_ONLY
	USB_MODE.InitFn();
	memset(AStreamStackArea, 0, sizeof(AStreamStackArea));
	LaunchThread(&AudioThread, USB_MODE.AThread, USB_MODE.Aargs, AStreamStackArea, sizeof(AStreamStackArea), 0x2C);
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
	
	return 0;
}
