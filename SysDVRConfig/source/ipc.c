#include <stdio.h>
#include <string.h>
#include "ipc.h"

#ifdef __SWITCH__
static Service dvr;

bool SysDvrIsRunning()
{
	SmServiceName name = {0};
	memcpy(name.name, "sysdvr", sizeof("sysdvr"));
	
	Handle h;
	Result rc = smRegisterService(&h, name, false, 1);	
	
	if (R_SUCCEEDED(rc))
	{
		smUnregisterService(name);
		return false;
	}
	
	return true;
}

Result SysDvrConnect()
{
	if (!SysDvrIsRunning())
		return ERR_MAIN_NOTRUNNING;
	
	Result rc = smGetService(&dvr, "sysdvr");
	return rc;
}

Result SysDvrSetAudioBatchingOverride(int batching)
{
	if (batching == 1)
		return serviceDispatch(&dvr, CMD_AUDIO_NO_BATCHING);
	else if (batching == 2)
		return serviceDispatch(&dvr, CMD_AUDIO_BATCHING_2);
	else if (batching == 3)
		return serviceDispatch(&dvr, CMD_AUDIO_BATCHING_3);
	else 
		return MAKERESULT(Module_Libnx, LibnxError_BadInput);
}

void SysDVRDebugCrash()
{
	serviceDispatch(&dvr, CMD_DEBUG_CRASH);
}

void SysDvrClose()
{
	serviceClose(&dvr);
}

Result SysDvrGetVersion(u32* out_ver)
{
	u32 val;
	Result rc = serviceDispatchOut(&dvr, CMD_GET_VER, val);
	if (R_SUCCEEDED(rc))
		*out_ver = val;
	else printf("SysDvrGetVersion: %x\n", rc);
	return rc;
}

Result SysDvrGetMode(u32* out_mode)
{
	u32 val;
	Result rc = serviceDispatchOut(&dvr, CMD_GET_MODE, val);
	if (R_SUCCEEDED(rc))
		*out_mode = val;
	return rc;
}

Result SysDvrSetMode(u32 command)
{
	return serviceDispatch(&dvr, MODE_TO_CMD_SET(command));
}

Result SysDvrSetUSB()
{
	return SysDvrSetMode(TYPE_MODE_USB);
}

Result SysDvrSetRTSP()
{
	return SysDvrSetMode(TYPE_MODE_RTSP);
}

Result SysDvrSetTCP()
{
	return SysDvrSetMode(TYPE_MODE_TCP);
}

Result SysDvrSetOFF()
{
	return SysDvrSetMode(TYPE_MODE_NULL);
}
#else
// Stub implementation

Result SysDvrConnect()
{
	return 0;
}

void SysDvrClose()
{
	
}

Result SysDvrGetVersion(u32* out_ver)
{
	*out_ver = SYSDVR_VERSION;
	return 0;
}

Result SysDvrGetMode(u32* out_mode)
{
	*out_mode = 0;
	return 0;
}

Result SysDvrSetMode(u32 command)
{
	return 0;
}

Result SysDvrSetUSB()
{
	return 0;
}

Result SysDvrSetRTSP()
{
	return 0;
}

Result SysDvrSetTCP()
{
	return 0;
}

Result SysDvrSetOFF()
{
	return 0;
}
#endif