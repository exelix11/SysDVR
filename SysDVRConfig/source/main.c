#include <switch.h>
#include "utils.h"

#include "../../sysmodule/source/modes/defines.h"
#include "ipc.h"

void PrintDefaultBootMode() 
{
	if (FileExists("/config/sysdvr/usb"))
		printf("On boot SysDVR will stream over USB\n");
	else if (FileExists("/config/sysdvr/rtsp"))
		printf("On boot SysDVR will stream via RTSP\n");
	else if (FileExists("/config/sysdvr/tcp"))
		printf("On boot SysDVR will stream over TCP\n");
	else
		printf("On boot SysDVR will not stream\n");
}

bool SetDefaultBootMode(u32 mode)
{
	unlink("/config/sysdvr/usb");
	unlink("/config/sysdvr/tcp");
	unlink("/config/sysdvr/rtsp");
	if (mode == TYPE_MODE_USB)
		return CreateDummyFile("/config/sysdvr/usb");
	else if (mode == TYPE_MODE_TCP)
		return CreateDummyFile("/config/sysdvr/tcp");
	else if (mode == TYPE_MODE_RTSP)
		return CreateDummyFile("/config/sysdvr/rtsp");
	return true;
}

void FatalError(const char* msg)
{
	printf(CONSOLE_RED "Error: " CONSOLE_WHITE "%s\nPress A to quit", msg);
}

void FatalErrorLoop() 
{
	consoleUpdate(NULL);
	while (appletMainLoop())
	{
		hidScanInput();
		u64 kDown = hidKeysDown(CONTROLLER_P1_AUTO);
		if (kDown & KEY_PLUS || kDown & KEY_A) break;
	}
}

u32 CurrentStreamMode;
void PrintCurrentMode()
{
	switch (CurrentStreamMode)
	{
	case TYPE_MODE_NULL:
		printf("SysDVR is not streaming currently.\n");
		break;
	case TYPE_MODE_USB:
		printf("SysDVR is streaming over USB.\n");
		break;
	case TYPE_MODE_TCP:
		printf("SysDVR is streaming over TCP.\n");
		break;
	case TYPE_MODE_RTSP:
		printf("SysDVR is streaming over RTSP.\n");
		break;
	default:
		printf(CONSOLE_RED "Couldn't read SysDVR's status.\n" CONSOLE_WHITE);
	}
	printf("\n");
}

void ReadCurrentMode()
{
	u32 mode = 0;
	CurrentStreamMode = R_SUCCEEDED(SysDvrGetMode(&mode)) ? mode : TYPE_MODE_ERROR;
	PrintCurrentMode();
}

bool DefaultMenu() 
{
	static int Selection = 0;
	const int MenuOptCount = 5;
	const char* MenuOptions[] = 
	{
		"Stream over USB",
		"Stream over TCP Bridge (direct network mode, requires pc configuration)",
		"Stream over RTSP (simple network mode, no setup, may have some glitches)",
		"Stop streaming",
		"Set current mode as default on boot",
		"Quit",
		NULL
	};

	printf("\x1b[9;0H\x1b[0J");
	if (!appletMainLoop()) return false;
	u64 kDown = hidKeysDown(CONTROLLER_P1_AUTO);

	if (CurrentStreamMode == TYPE_MODE_ERROR)
	{
		printf(
			"It seems SysDVR is not responding, this can happen if the mode was changed while streaming\n"
			"The sysmodule is stuck waiting for the next frame, when resuming the game it will start working again and switch modes as requested\n\n"
			"Press A to quit");
		return !(kDown & KEY_A);
	}

	printf("Select an option: \n");
	for (int i = 0; MenuOptions[i] != NULL; i++)
	{
		if (Selection == i) printf(" >> "); else printf("    ");
		printf("%s\n", MenuOptions[i]);
	}
	printf("\n");
	printf("If you're not sure which mode should you pick read the readme on GitHub.\n");
	printf("\n");
	printf(CONSOLE_YELLOW "Warning:" CONSOLE_WHITE " Changing mode while streaming will hang, if you're streaming currently resume the game, close the client and come back here.");

	if (kDown & KEY_DOWN)
		Selection = Selection >= MenuOptCount ? 0 : Selection + 1;
	else if (kDown & KEY_UP)
		Selection = Selection <= 0 ? MenuOptCount : Selection - 1;
	else if (kDown & KEY_A)
		switch(Selection)
		{
		case 0:
			if (R_FAILED(SysDvrSetUSB()))
				goto FAIL;
			break;
		case 1:
			if (R_FAILED(SysDvrSetTCP()))
				goto FAIL;
			break;
		case 2:
			if (R_FAILED(SysDvrSetRTSP()))
				goto FAIL;
			break;
		case 3:
			if (R_FAILED(SysDvrSetOFF()))
				goto FAIL;
			break;
		case 4:
			SetDefaultBootMode(CurrentStreamMode);
			printf("\x1b[6;0H\x1b[0J");
			PrintDefaultBootMode();
			PrintCurrentMode();
			break;
		default:
			return false;
		}

	return true;
FAIL:
	FatalErrorLoop();
	return false;
}

void MainLoop()
{
	u32 version = 0;
	if (R_FAILED(SysDvrGetVersion(&version)))
		FatalError("Couldn't communicate with SysDVR, are you sure it's running ?\nAlso make sure you're not using the USB-only version.");
	else if (version > SYSDVR_VERSION)
		FatalError("You're running a newer version of SysDVR that is not supported by this application, download latest SysDVR Settings app from GitHub");
	else if (version < SYSDVR_VERSION)
		FatalError("You're running an outdated version of SysDVR, download latest version from GitHub");

	if (version != SYSDVR_VERSION)
	{
		FatalErrorLoop();
		return;
	}

	printf(CONSOLE_GREEN "Connected to SysDVR !\n" CONSOLE_WHITE);

	printf("\x1b[6;0H\x1b[0J");
	PrintDefaultBootMode();
	ReadCurrentMode();

	while (DefaultMenu())
	{
		hidScanInput();
		consoleUpdate(NULL);
	}
}

int main(int argc, char** argv)
{
	consoleInit(NULL);

	printf(
		"SysDVR 4.0.1 Settings - by exelix\n"
		"https://github.com/exelix11/SysDVR\n\n"
	);
	consoleUpdate(NULL);

	Result rc = SysDvrConnect();

	if (R_SUCCEEDED(rc))
		MainLoop();
	else 
	{
		printf("err %u\n", rc);
		FatalErrorLoop();
	}

	SysDvrClose();
	consoleExit(NULL);
	return 0;
}