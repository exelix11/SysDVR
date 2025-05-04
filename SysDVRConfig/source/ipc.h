#pragma once

#ifdef __SWITCH__
#include <switch.h>
#else
#include <stdint.h>
#include <stdbool.h>
typedef uint32_t u32;
typedef uint32_t Result;
typedef uint8_t u8;
#define MAKERESULT(x,y) 0
#define R_FAILED(x) (x != 0)
#define R_SUCCEEDED(x) (x == 0)
#define R_DESCRIPTION(x) x
#define R_MODULE(x) x
#endif

#ifdef __cplusplus
extern "C" {
#endif
	#include "../../sysmodule/source/modes/defines.h"

	#define SYSDVR_EXEFS_PATH "/atmosphere/contents/00FF0000A53BB665/exefs.nsp"

	// These functions use the process management API to start and kill the sysdvr process. These work with both the full and usb-only version as they share the same content id
	Result SysDvrProcManagerInit();
	void SysDvrProcManagerExit();
	bool SysDvrProcManagerIsInitialized();
	
	// This function detects if the SysDVR sysmodule process is running. This detects both the usb-only and full versions.
	// Even when this returns true it does not guarantee that SysDVR is ready to receive commands since it has a 20 sec delay on boot before actually starting.
	bool SysDvrProcIsRunning();
	
	Result SysDvrProcTerminate();
	Result SysDvrProcLaunch();

	typedef enum {
		SYSDVR_VARIANT_FULL,
		SYSDVR_VARIANT_USB_ONLY,
		SYSDVR_VARIANT_UNKNOWN,
	} SYSDVR_VARIANT;

	// This function scans the exefs.nsp file of the sysmodule to detect if it's the usb-only variant	
	SYSDVR_VARIANT SysDvrDetectVariant(const char* exefsFilePath);
	
	// Same as SysDvrDetectVariant but the result is cached in a static variable. The cache ignores the path, once it's set it's permanent.
	SYSDVR_VARIANT SysDvrDetectVariantCached(const char* exefsFilePath);

	// These functions use the sysdvr IPC service to communicate with the sysmodule. These only work with the full version of the sysmodule.
	
	// This function checks that the SysDVR IPC server is available. When this is true SysDVR is the full version and it's ready to receive commands.
	bool SysDvrIsRunning();
	
	Result SysDvrConnect();
	void SysDvrClose();
	
	void SysDVRDebugCrash();
	
	Result SysDvrGetVersion(u32* out_ver);
	Result SysDvrGetMode(u32* out_mode);

	Result SysDvrSetMode(u32 command);
		
	Result SysDvrSetUSB();
	Result SysDvrSetRTSP();
	Result SysDvrSetTCP();
	Result SysDvrSetOFF();
#ifdef __cplusplus
}
#endif