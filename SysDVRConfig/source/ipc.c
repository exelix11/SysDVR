#include "ipc.h"
#include "../../sysmodule/source/modes/defines.h"
#include <stdio.h>

static Service dvr;

Result SysDvrConnect()
{
	Result rc = smGetService(&dvr, "sysdvr");
	return rc;
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

Result SysDvrSetUSB()
{
	return serviceDispatch(&dvr, CMD_SET_USB);
}

Result SysDvrSetRTSP()
{
	return serviceDispatch(&dvr, CMD_SET_RTSP);
}

Result SysDvrSetTCP()
{
	return serviceDispatch(&dvr, CMD_SET_TCP);
}

Result SysDvrSetOFF()
{
	return serviceDispatch(&dvr, CMD_SET_OFF);
}