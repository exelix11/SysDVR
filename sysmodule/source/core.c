#include <string.h>

#include "core.h"
#include "modes/defines.h"
#include "modes/modes.h"
#include "capture.h"

#if NEEDS_SOCKETS
#include "net/sockets.h"
#endif

//#define USB_ONLY

#if FILE_LOGGING
#pragma message "File logging is enabled"
#endif

#if LOGGING_ENABLED
#pragma message "You're building with logging enabled, this increases the heap size, remember to test without logging."
#endif

#define PLACEHOLDER_SERIAL "Unknown serial\0\0\0\0\0\0\0\0\0"
_Static_assert(sizeof(PLACEHOLDER_SERIAL) == sizeof(SetSysSerialNumber));

char SysDVRBeacon[] = "SysDVR" "|" SYSDVR_VERSION_STRING "|" SYSDVR_PROTOCOL_VERSION "|" PLACEHOLDER_SERIAL;
int SysDVRBeaconLen = sizeof(SysDVRBeacon);

// Statically allocate all needed buffers
StaticBuffers Buffers;

Result CoreInit()
{
	Result rc = setsysInitialize();

	// Ignore error, we'll just use a placeholder serial
	if (R_SUCCEEDED(rc)) {
		SetSysSerialNumber serial;
		Result rc2 = setsysGetSerialNumber(&serial);

		if (R_SUCCEEDED(rc2)) {
			memcpy(SysDVRBeacon + SysDVRBeaconLen - sizeof(SetSysSerialNumber), serial.number, sizeof(SetSysSerialNumber));
			SysDVRBeacon[SysDVRBeaconLen - 1] = '\0';
		}

		setsysExit();
	}

	rc = CaptureInitialize();
	if (R_FAILED(rc))
		return rc;

#ifdef FILE_LOGGING
	freopen("/sysdvr_log.txt", "w", stdout);
#endif

#if NEEDS_SOCKETS
	SocketInit();
#endif

	return 0;
}

#ifndef USB_ONLY
atomic_bool IsThreadRunning = false;

static u8 alignas(0x1000) VStreamStackArea[0x2000 + LOGGING_STACK_BOOST];
#endif
static u8 alignas(0x1000) AStreamStackArea[0x2000 + LOGGING_STACK_BOOST];

void LaunchThread(Thread* t, ThreadFunc f, void* arg, void* stackLocation, u32 stackSize, u32 prio)
{
#if FAKEDVR
	Result rc = threadCreate(t, f, arg, stackLocation, stackSize, prio, -2);
#else
	Result rc = threadCreate(t, f, arg, stackLocation, stackSize, prio, 3);
#endif
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
static u8 alignas(0x1000) ModeSwitchingStackArea[0x2000 + LOGGING_STACK_BOOST];
static atomic_bool IsModeSwitchPending = false;
static Thread ModeSwitchThread;
static Mutex ModeSwitchingMutex;
static const StreamMode* SwitchModeTarget = NULL;

static const StreamMode* GetUserVisibleMode() {
	mutexLock(&ModeSwitchingMutex);

	const StreamMode* ret = CurrentMode;

	if (IsModeSwitchPending)
		ret = SwitchModeTarget;

	mutexUnlock(&ModeSwitchingMutex);
	return ret;
}

static void EnterTargetMode()
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

static void ExitCurrentMode()
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

static void SwitchModesThreadMain(void*)
{
	// This will take forever
	ExitCurrentMode();

	// This is fast
	EnterTargetMode();
}

static void SwitchModes(const StreamMode* mode)
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

		// If there is a mode active, we use a thread to wait for it to exit
		if (CurrentMode)
		{
			LOG("Launching mode switch thread\n");
			LaunchThread(&ModeSwitchThread, SwitchModesThreadMain, NULL, ModeSwitchingStackArea, sizeof(ModeSwitchingStackArea), 0x2C);
		}
		else // Otherwise we just do it directly
		{
			LOG("Perform inline mode switching\n");
			mutexUnlock(&ModeSwitchingMutex);
			SwitchModesThreadMain(NULL);
			return;
		}
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
#else
void UsbOnlyEntrypoint() 
{
	USB_MODE.InitFn();
	memset(AStreamStackArea, 0, sizeof(AStreamStackArea));
	LaunchThread(&AudioThread, USB_MODE.AThread, USB_MODE.Aargs, AStreamStackArea, sizeof(AStreamStackArea), 0x2C);
	USB_MODE.VThread(USB_MODE.Vargs);
	USB_MODE.ExitFn();
}
#endif