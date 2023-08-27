#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>

#include "../core.h"
#include "../modes/modes.h"
#include "../capture.h"

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
	
	#include "../ipc/ipc.h"
#endif

#if NEEDS_SOCKETS
	#include "../net/sockets.h"
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

	rc = CoreInit();
	if (R_FAILED(rc))
		fatalThrow(rc);

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
#ifdef USB_ONLY
	CaptureSetAudioBatching(USB_MODE.AudioBatches);
	USB_MODE.InitFn();
	memset(AStreamStackArea, 0, sizeof(AStreamStackArea));
	LaunchThread(&AudioThread, USB_MODE.AThread, USB_MODE.Aargs, AStreamStackArea, sizeof(AStreamStackArea), 0x2C);
	USB_MODE.VThread(USB_MODE.Vargs);
	USB_MODE.ExitFn();
#else
	if (FileExists("/config/sysdvr/usb"))
		SetModeID(TYPE_MODE_USB);
	else if (FileExists("/config/sysdvr/rtsp"))
		SetModeID(TYPE_MODE_RTSP);
	else if (FileExists("/config/sysdvr/tcp"))
		SetModeID(TYPE_MODE_TCP);

	IpcThread();
#endif
	
	return 0;
}
