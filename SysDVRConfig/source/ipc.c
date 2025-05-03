#include <stdio.h>
#include <string.h>
#include <malloc.h>
#include "ipc.h"

#ifndef memmem
static void* memmem(const void* haystack, size_t haystacklen, const void* needle, size_t needlelen) {
	const char* h = (const char*)haystack;
	const char* n = (const char*)needle;
	size_t i;

	if (needlelen > haystacklen) {
		return NULL;
	}

	for (i = 0; i <= haystacklen - needlelen; i++) {
		if (h[i] == n[0] && memcmp(&h[i], n, needlelen) == 0) {
			return (void*)&h[i];
		}
	}

	return NULL;
}
#endif

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