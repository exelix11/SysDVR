// SDL.h provides the SDL_Main() symbol and must be kept
#include "SDL.h"
#include <android/log.h>
#include <stdbool.h>
#include <jni.h>

struct NativeInitBlock
{
    // Info
	void* Version;
	void* Sizeof;
	void* PrintFunction;

	// Threading
	void* AttachThread;
	void* DetachThread;

    // Usb
	void* UsbAcquireSnapshot;
	void* UsbReleaseSnapshot;
	void* UsbGetSnapshotDeviceSerial;
	void* UsbOpenHandle;
	void* UsbCloseHandle;
	void* UsbGetLastError;

	// Util
	void* SysOpenURL;
	void* SysGetClipboard;
	void* SysGetFileAccessInfo;
	void* SysRequestFileAccess;
};

// Forward declare needed functions

// from thread.c
void InitThreading();
void AttachThread();
void DetachThread();

// from usb.c
void UsbInit();
bool UsbAcquireSnapshot(int vid, int pid, int* deviceCount);
void UsbReleaseSnapshot();
jchar* UsbGetSnapshotDeviceSerial(int idx);
jchar* UsbGetLastError();
bool UsbOpenHandle(jchar* serial, void** handle);
void UsbCloseHandle(void* handle);

// from native.c
void SysInit();
bool SysOpenUrl(const jchar* string);
void SysGetClipboard(char* buffer, int size);
bool SysGetFileAccessInfo(bool* hasPermission, bool* canRequest);
void SysRequestFileAccess();

#define L(...) __android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", __VA_ARGS__)

void LOG(const char* string) 
{
	__android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", "%s", string);	
}

struct NativeInitBlock g_native = 
{
	.Version = (void*)1,
	.Sizeof = (void*)sizeof(struct NativeInitBlock),
	.PrintFunction = LOG,

	.AttachThread = AttachThread,
	.DetachThread = DetachThread,

	.UsbAcquireSnapshot = UsbAcquireSnapshot,
	.UsbReleaseSnapshot = UsbReleaseSnapshot,
	.UsbGetSnapshotDeviceSerial = UsbGetSnapshotDeviceSerial,
	.UsbOpenHandle = UsbOpenHandle,
	.UsbCloseHandle = UsbCloseHandle,
	.UsbGetLastError = UsbGetLastError,

    .SysOpenURL = SysOpenUrl,
    .SysGetClipboard = SysGetClipboard,
	.SysGetFileAccessInfo = SysGetFileAccessInfo,
	.SysRequestFileAccess = SysRequestFileAccess
};

extern int sysdvr_entrypoint(struct NativeInitBlock* init);

int main(int argc, char *argv[])
{
	L("sdl main called()");

    // Initialize JNI components
    InitThreading();
    SysInit();
    UsbInit();

	L("Calling entrypoint");
  	int result = sysdvr_entrypoint(&g_native);
  	L("sysdvr_entrypoint returned %d", result);

  	return 0;
}
