#include <stdlib.h>
#include <stdio.h>
#include <switch.h>
#include <string.h>

#include "../core.h"
#include "../modes/modes.h"
#include "../capture.h"

// As of SysDVR 6.0 the sysmodule can run with no dynamic allocations at all
// All big buffers are statically allocated in the compile units where they're needed
// For buffers that are mutually exclusive like ones used by the different streaming modes, we use the StaticBuffers union
// To make this work we override some libnx symbols to ensure the linker won't include malloc and its implementation
#define USE_HEAP 0

#if !USE_HEAP
void* __libnx_aligned_alloc(size_t alignment, size_t size)
{
	fatalThrow(ERR_MAIN_ALLOC_DISABLED);
	return NULL;
}

void __libnx_free(void* p)
{
	fatalThrow(ERR_MAIN_ALLOC_DISABLED);
}

void __libnx_initheap(void)
{
	// Newlib
	extern char* fake_heap_start;
	extern char* fake_heap_end;

	fake_heap_start = NULL;
	fake_heap_end = NULL;
}
#else
#define INNER_HEAP_SIZE (1 * 1024 * 1024)

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
#endif

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

#if NEEDS_FS
	static Result fsInitResult;
	static FsFileSystem sdCard;
#endif

u32 __nx_applet_type = AppletType_None;
u32 __nx_fs_num_sessions = 1;
u32 __nx_fsdev_direntry_cache_size = 1;

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
	// Ignore erorrs, at most we won't be able to read the default mode
	// We use this directly because we don't want to depend on fsdev which requires malloc
	fsInitResult = fsOpenSdCardFileSystem(&sdCard);
#endif
}

void __attribute__((weak)) __appExit(void)
{
#if NEEDS_SOCKETS
	SocketDeinit();
#endif
#if NEEDS_FS
	if (R_SUCCEEDED(fsInitResult))
		fsFsClose(&sdCard);
	fsExit();
#endif
	smExit();
}

#ifndef USB_ONLY
static bool FileExists(const char* fname)
{
	if (R_FAILED(fsInitResult))
		return false;

	FsFile file;
	if (R_SUCCEEDED(fsFsOpenFile(&sdCard, fname, FsOpenMode_Read, &file)))
	{
		fsFileClose(&file);
		return true;
	}
	return false;
}
#endif

// from core.c
void UsbOnlyEntrypoint();

// from TCPMode.c
extern int g_tcpEnableBroadcast;

int main(int argc, char* argv[])
{
#ifdef USB_ONLY
	UsbOnlyEntrypoint();
#else
	if (FileExists("/config/sysdvr/no_adv"))
		g_tcpEnableBroadcast = false;

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
