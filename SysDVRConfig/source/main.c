#include <switch.h>
#include "utils.h"

#define SYSDVR_VERSION 2
#define TYPE_MODE_USB 1
#define TYPE_MODE_TCP 2
#define TYPE_MODE_RTSP 4
#define TYPE_MODE_NULL 3
#define TYPE_MODE_ERROR 999999

void PrintDefaultBootMode() 
{
	if (FileExists("/config/sysdvr/usb"))
		printf("On boot SysDVR will stream over USB\n");
	else if (FileExists("/config/sysdvr/tcp"))
		printf("On boot SysDVR will stream over TCP\n");
	else if (FileExists("/config/sysdvr/rtsp"))
		printf("On boot SysDVR will stream via RTSP\n");
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

bool SendValue(int sock, u32 value)
{
	return write(sock, &value, sizeof(value)) == sizeof(value);
}

bool ReadValue(int sock, u32 *out)
{
	return read(sock, out, sizeof(*out)) == sizeof(*out);
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

void ReadCurrentMode(int sock)
{
	u32 mode = 0;
	CurrentStreamMode = ReadValue(sock, &mode) ? mode : TYPE_MODE_ERROR;
	PrintCurrentMode();
}

bool MenuSetMode(int sock, u32 mode)
{
	printf("\x1b[6;0H\x1b[0J");
	printf("Loading...\n");
	consoleUpdate(NULL);
	printf("\x1b[6;0H\x1b[0J");
	if (!SendValue(sock, mode))
	{
		FatalError("Couldn't communicate with the sysmodule");
		return false;
	}
	PrintDefaultBootMode();
	ReadCurrentMode(sock);
	return true;
}

bool DefaultMenu(int sock) 
{
	static int Selection = 0;
	const char* MenuOptions[] = 
	{
		"Stream over USB",
		"Stream over TCP (direct network mode, requires pc configuration)",
		"Stream over RTSP (simple network mode, should work without setup but may have some lag or glitches)",
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

#if !defined(RELEASE)
	if (KhlpIndex < KhlpMax && kDown) {
		if (kDown == kHelper[KhlpIndex]) {
			if (KhlpIndex++ == KhlpMax - 1)
				return PrintBuffer(H264_testBuf), true;
		}
		else KhlpIndex = 0;
	}
#endif

	printf("Select an option: \n");
	for (int i = 0; MenuOptions[i] != NULL; i++)
	{
		if (Selection == i) printf(" >> "); else printf("    ");
		printf("%s\n", MenuOptions[i]);
	}
	printf("\n");
	printf("If you're not sure which mode should you pick read the readme on GitHub.\n");
	printf(CONSOLE_YELLOW "Warning:" CONSOLE_WHITE " Changing mode while streaming will hang, if you're streaming currently resume the game, close the client and come back here.");

	if (kDown & KEY_DOWN)
		Selection = Selection >= 4 ? 0 : Selection + 1;
	else if (kDown & KEY_UP)
		Selection = Selection <= 0 ? 4 : Selection - 1;
	else if (kDown & KEY_A)
		switch(Selection)
		{
		case 0:
			if (!MenuSetMode(sock, TYPE_MODE_USB))
				goto FAIL;
			break;
		case 1:
			if (!MenuSetMode(sock, TYPE_MODE_TCP))
				goto FAIL;
			break;
		case 2:
			if (!MenuSetMode(sock, TYPE_MODE_RTSP))
				goto FAIL;
			break;
		case 3:
			if (!MenuSetMode(sock, TYPE_MODE_NULL))
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

void MainLoop(int sock)
{
	u32 version = 0;
	if (!ReadValue(sock, &version))
		FatalError("Couldn't communicate with SysDVR, are you sure it's running ?\nAlso make sure you're not using the USB-only version.");
	else if (version > SYSDVR_VERSION)
		FatalError("You're running a newer version of SysDVR that is not supported by this application, download latest SysDVR Settings app from GitHub");
	else if (version < SYSDVR_VERSION)
		FatalError("You're running an outdated version of SysDVR, download latest version from GitHub");

	if (version != SYSDVR_VERSION)
	{
		close(sock);
		FatalErrorLoop();
		return;
	}

	printf(CONSOLE_GREEN "Connected to SysDVR !\n" CONSOLE_WHITE);

	printf("\x1b[6;0H\x1b[0J");
	PrintDefaultBootMode();
	ReadCurrentMode(sock);

	while (DefaultMenu(sock))
	{
		hidScanInput();
		consoleUpdate(NULL);
	}

	close(sock);
}

int main(int argc, char** argv)
{
	socketInitializeDefault();
	consoleInit(NULL);

	printf(
		"SysDVR 2.0 Settings - by exelix\n"
		"https://github.com/exelix11/SysDVR\n\n"
	);
	consoleUpdate(NULL);

	int sock = ConnectToSysmodule();

	if (sock == ERR_CONNECT)
		FatalError("Failed to connect to SysDVR, is the module set up properly ?");
	else if (sock == ERR_SOCK)
		FatalError("Failed to create a socket");

	if (sock >= 0)
		MainLoop(sock);
	else
		FatalErrorLoop();

	consoleExit(NULL);
	socketExit();
	return 0;
}