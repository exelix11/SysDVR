#pragma once

#ifdef __SWITCH__
#include <switch.h>
#else
#include <stdint.h>
#include <stdbool.h>
typedef uint32_t u32;
typedef uint32_t Result;
#define MAKERESULT(x,y) 0
#define R_FAILED(x) (x != 0)
#define R_DESCRIPTION(x) x
#define R_MODULE(x) x
#endif

#include "../../sysmodule/source/modes/defines.h"

#ifdef __cplusplus
extern "C" {
#endif
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