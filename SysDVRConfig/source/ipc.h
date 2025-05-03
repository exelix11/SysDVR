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

	// These functions use the process management API to start and kill the sysdvr process
	Result SysDvrProcManagerInit();
	void SysDvrProcManagerExit();
	bool SysDvrProcManagerIsInitialized();
	
	bool SysDvrProcIsRunning();
	Result SysDvrProcTerminate();
	Result SysDvrProcLaunch();

	// This function scans the exefs.nsp file of the sysmodule to detect if it's the usb-only variant
	typedef enum {
		SYSDVR_VARIANT_FULL,
		SYSDVR_VARIANT_USB_ONLY,
		SYSDVR_VARIANT_UNKNOWN,
	} SYSDVR_VARIANT;
	
	SYSDVR_VARIANT SysDvrDetectVariant(const char* exefsFilePath);
	SYSDVR_VARIANT SysDvrDetectVariantCached(const char* exefsFilePath);

	// These functions use the sysdvr IPC service to communicate with the sysmodule
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