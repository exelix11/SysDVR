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

Result SysDvrSetUserOverrides(const UserOverrides* options)
{
	return serviceDispatchIn(&dvr, CMD_SET_USER_OVERRIDES, *options);
}

Result SysDvrGetUserOverrides(UserOverrides* out_options)
{
	UserOverrides opt;
	Result rc = serviceDispatchOut(&dvr, CMD_GET_USER_OVERRIDES , opt);
	if (R_SUCCEEDED(rc))
		*out_options = opt;
	return rc;
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
	return serviceDispatch(&dvr, command);
}

Result SysDvrSetUSB()
{
	return SysDvrSetMode(CMD_SET_USB);
}

Result SysDvrSetRTSP()
{
	return SysDvrSetMode(CMD_SET_RTSP);
}

Result SysDvrSetTCP()
{
	return SysDvrSetMode(CMD_SET_TCP);
}

Result SysDvrSetOFF()
{
	return SysDvrSetMode(CMD_SET_OFF);
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
	*out_mode = TYPE_MODE_USB;
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

Result SysDvrSetUserOverrides(const UserOverrides* options)
{
	return 0;
}

Result SysDvrGetUserOverrides(UserOverrides* out_options)
{
	*out_options = (UserOverrides){ true, 2, 3 };
	return 0;
}
#endif