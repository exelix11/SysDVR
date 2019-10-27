#include <string.h>
#include <stdio.h>
#include <switch.h>

#ifdef __SWITCH__
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <fcntl.h>
#include <errno.h>
#else
//not actually used, just to stop visual studio from complaining.
#define F_SETFL 1
#define O_NONBLOCK 1
#define F_GETFL 1
#include <WinSock2.h>
#endif

#define SYSDVR_VERSION 1
#define TYPE_MODE_USB 1
#define TYPE_MODE_TCP 2
#define TYPE_MODE_NULL 3
#define TYPE_MODE_ERROR 999999

static bool FileExists(const char* fname)
{
	FILE* f = fopen(fname, "rb");
	if (f)
	{
		fclose(f);
		return true;
	}
	return false;
}

void PrintDefaultBootMode() 
{
	if (FileExists("/config/sysdvr/usb"))
		printf("On boot SysDVR will stream over USB\n");
	else if (FileExists("/config/sysdvr/tcp"))
		printf("On boot SysDVR will stream over NETWORK\n");
	else
		printf("On boot SysDVR will not stream\n");
}

static bool CreateDummyFile(const char* fname)
{
	FILE* f = fopen(fname, "w");
	if (f)
	{
		fwrite(".", 1, 1, f);
		fclose(f);
		return true;
	}
	return false;
}

bool SetDefaultBootMode(u32 mode)
{
	unlink("/config/sysdvr/usb");
	unlink("/config/sysdvr/tcp");
	if (mode == TYPE_MODE_USB)
		return CreateDummyFile("/config/sysdvr/usb");
	else if (mode == TYPE_MODE_TCP)
		return CreateDummyFile("/config/sysdvr/tcp");
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

#define ERR_CONNECT -1
int ConnectToSysmodule()
{
	int sockfd;
	struct sockaddr_in servaddr = {0};

	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if (sockfd == -1)
	{
		FatalError("Failed to create a socket");
		return ERR_CONNECT;
	}
	
	servaddr.sin_family = AF_INET;
	servaddr.sin_addr.s_addr = inet_addr("127.0.0.1");
	servaddr.sin_port = htons(6668);

	struct timeval tv;
	tv.tv_sec = 8;
	tv.tv_usec = 0;
	setsockopt(sockfd, SOL_SOCKET, SO_RCVTIMEO, (const char*)&tv, sizeof tv);

	if (connect(sockfd, (struct sockaddr*)&servaddr, sizeof(servaddr)) != 0) {
		FatalError("Failed to connect to SysDVR, is the module set up properly ?");
		return ERR_CONNECT;
	}
	
	return sockfd;
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
		printf("SysDVR is streaming over NETWORK.\n");
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
		"Stream over NETWORK",
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
			if (!MenuSetMode(sock, TYPE_MODE_NULL))
				goto FAIL;
			break;
		case 3:
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
		FatalError("Couldn't communicate with SysDVR, are you sure it's running ? Also make sure you're not using the USB-only version.");
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
	if (sock >= 0)
		MainLoop(sock);
	else
		FatalErrorLoop();

	consoleExit(NULL);
	socketExit();
	return 0;
}