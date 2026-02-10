#include <stdio.h>
#include <string.h>
#include <malloc.h>
#include "ipc.h"

SYSDVR_VARIANT SysDvrDetectVariant(const char* exefsFilePath)
{
	FILE* file = fopen(exefsFilePath, "rb");
	if (!file)
		return SYSDVR_VARIANT_UNKNOWN;

	fseek(file, 0, SEEK_END);
	size_t size = ftell(file);
	fseek(file, 0, SEEK_SET);

	// Max 2MB, and even that's huge since the current builds are less than 100k
	if (size >= 2 * 1024 * 1024)
	{
		fclose(file);
		return SYSDVR_VARIANT_UNKNOWN;
	}

	char* buf = malloc(size);
	if (!buf)
	{
		fclose(file);
		return SYSDVR_VARIANT_UNKNOWN;
	}

	size_t read = fread(buf, 1, size, file);
	fclose(file);

	const char* fullBuildTag = "SYSDVR_BUILD_FULL";
	const char* usbOnlyBuildTag = "SYSDVR_BUILD_USB_ONLY";

	if (memmem(buf, read, fullBuildTag, strlen(fullBuildTag)))
	{
		free(buf);
		return SYSDVR_VARIANT_FULL;
	}
	else if (memmem(buf, read, usbOnlyBuildTag, strlen(usbOnlyBuildTag)))
	{
		free(buf);
		return SYSDVR_VARIANT_USB_ONLY;
	}

	free(buf);
	return SYSDVR_VARIANT_UNKNOWN;
}

SYSDVR_VARIANT SysDvrDetectVariantCached(const char* path)
{
	static bool variantKnown;
	static SYSDVR_VARIANT variant;

	if (variantKnown)
		return variant;

	variant = SysDvrDetectVariant(path);
	variantKnown = true;

	return variant;
}

#ifdef __SWITCH__
static bool hasPmShell;
static bool hasPmDmnt;

Result SysDvrProcManagerInit()
{
	Result rc = pmshellInitialize();
	if (R_FAILED(rc))
		return rc;

	hasPmShell = true;

	rc = pmdmntInitialize();
	if (R_FAILED(rc))
		return rc;

	hasPmDmnt = true;
	return rc;
}

bool SysDvrProcManagerIsInitialized()
{
	return hasPmShell && hasPmDmnt;
}

void SysDvrProcManagerExit()
{
	if (hasPmShell)
		pmshellExit();

	if (hasPmDmnt)
		pmdmntExit();

	hasPmShell = false;
	hasPmDmnt = false;
}

bool SysDvrProcIsRunning()
{
	u64 pid;
	if (R_SUCCEEDED(pmdmntGetProcessId(&pid, SYSDVR_CONTENT_ID)))
	{
		return true;
	}

	return false;
}

Result SysDvrProcTerminate()
{
	u64 pid;
	Result rc = pmdmntGetProcessId(&pid, SYSDVR_CONTENT_ID);
	if (R_SUCCEEDED(rc))
		rc = pmshellTerminateProcess(pid);

	return rc;
}

Result SysDvrProcLaunch()
{
	NcmProgramLocation programLocation = { 0 };
	programLocation.program_id = SYSDVR_CONTENT_ID;
	programLocation.storageID = NcmStorageId_None;

	u64 pid;
	return pmshellLaunchProgram(0, &programLocation, &pid);
}

static Service dvr;

// Ams extension, see https://github.com/Atmosphere-NX/Atmosphere/blob/master/docs/components/modules/sm.md#ipc-commands
static Result smAtmosphereHasService(SmServiceName name, bool *available)
{
    // _smShouldUseTipc from libnx
    // Not sure if this check is actually needed since this is supposed to work only on ams anyway
	if (hosversionIsAtmosphere() || hosversionAtLeast(12,0,0))
	    return tipcDispatchInOut(smGetServiceSessionTipc(), 65100, name, *available);
    else
        return serviceDispatchInOut(smGetServiceSession(), 65100, name, *available);
}

static bool smLegacyServiceCheck(SmServiceName name)
{
	Handle h;
	Result rc = smRegisterService(&h, name, false, 1);	
	
	if (R_SUCCEEDED(rc))
	{
		smUnregisterService(name);
		return false;
	}
	
	return true;
}

bool SysDvrIsRunning()
{
	SmServiceName name = {0};
	memcpy(name.name, "sysdvr", sizeof("sysdvr"));
	
	bool available = false;
    Result rc = smAtmosphereHasService(name, &available);
	if (R_SUCCEEDED(rc))
	{
        printf("Using modern service check\n");
        return available;
    }
	
	// Hopefully, never used ever again.
    printf("Modern check failed with %x, using legacy check...\n", rc);
	return smLegacyServiceCheck(name);
}

Result SysDvrConnect()
{	
	// If we're running the USB version the process will be counted as running but there will be no IPC service
	// This is an issue since smGetService block forever if the service name is not found. We must always ensure it is available first.
	if (!SysDvrIsRunning())
		return ERR_MAIN_NOTRUNNING;
	
	Result rc = smGetService(&dvr, "sysdvr");
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

Result SysDvrProcManagerInit()
{
	return 0;
}

void SysDvrProcManagerExit()
{
	return 0;
}

bool SysDvrProcManagerIsInitialized()
{
	return true;
}

bool SysDvrProcIsRunning()
{
	return true;
}

Result SysDvrProcTerminate()
{
	return 0;
}

Result SysDvrProcLaunch()
{
	return 0;
}

Result SysDvrConnect()
{
	return 0;
}

void SysDvrClose()
{
	
}

Result SysDvrGetVersion(u32* out_ver)
{
	*out_ver = SYSDVR_IPC_VERSION;
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
#endif