#include "modes.h"
#include "../USB/Serial.h"
#include "../capture.h"
#include "proto.h"

static bool VideoConnected = false;
static bool AudioConnected = false;
static Mutex UsbMutex;
static u64 LastConnection;

static bool ClientConnected() 
{
	return VideoConnected || AudioConnected;
}

static void DisconnectClient() 
{
	VideoConnected = false;
	AudioConnected = false;
	ProtoClientGlobalStateDisconnected();
}

static bool SendData(void* data, size_t length) 
{
	bool success;
	mutexLock(&UsbMutex);

	bool isTooLate = armGetSystemTick() - LastConnection > armNsToTicks(1.5E+9);
	if (isTooLate || !ClientConnected())
		success = false;
	else 
		success = UsbStreamingSend(data, length);

	if (success)
		LastConnection = armGetSystemTick();

	mutexUnlock(&UsbMutex);
	return success;
}

bool USB_ConnectClient()
{
	size_t num = 0;

	// Try this in a loop until a client connects
	do
		num = usbSerialWrite(PROTO_HANDSHAKE_HELLO, sizeof(PROTO_HANDSHAKE_HELLO), 1E+9);
	while (num == 0 && IsThreadRunning);

	if (num != sizeof(PROTO_HANDSHAKE_HELLO) || !IsThreadRunning)
		return false;

	u8 buffer[PROTO_HANDSHAKE_SIZE];
	if (usbSerialRead(buffer, sizeof(buffer), 1E+9) != sizeof(buffer))
		return false;

	LOG("USB request received\n");	
	ProtoParsedHandshake res = ProtoHandshake(ProtoHandshakeAccept_Any, buffer, sizeof(buffer));

	if (!UsbStreamingSend(&res.Result, sizeof(res.Result)))
	{
		ProtoClientGlobalStateDisconnected();
		return false;
	}

	if (res.Result != Handshake_Ok)
	{
		ProtoClientGlobalStateDisconnected();
		return false;
	}

	VideoConnected = res.RequestedVideo;
	AudioConnected = res.RequestedAudio;

	if (VideoConnected)
		CaptureVideoConnected();

	if (AudioConnected)
		CaptureAudioConnected();
	
	LastConnection = armGetSystemTick();
	return true;
}

static void USB_VideoStreamThread(void*)
{
	while (IsThreadRunning)
	{
		// Video streaming thread acts as master and receives the initial streaming request
		if (!ClientConnected()) {
			if (!USB_ConnectClient()) {
				svcSleepThread(5E+8L);
				continue;
			}
		}
		else if (VideoConnected)
		{
			if (!CaptureReadVideo() || !IsThreadRunning)
				continue;

			if (!SendData(&VPkt, VPkt.Header.DataSize + sizeof(PacketHeader)))
			{
				DisconnectClient();
				svcSleepThread(5E+8);
			}
		}
		else if (!VideoConnected)
		{
			svcSleepThread(1E+9);
		}
	}
}

static void USB_AudioStreamThread(void*)
{
	while (IsThreadRunning)
	{
		if (AudioConnected)
		{
			if (!CaptureReadAudio() || !IsThreadRunning)
				continue;

			if (!SendData(&APkt, APkt.Header.DataSize + sizeof(PacketHeader)))
			{
				DisconnectClient();
				svcSleepThread(5E+8);
			}
		}
		else
		{
			svcSleepThread(1E+9);
		}
	}
}

static void USB_Init()
{
	VideoConnected = false;
	AudioConnected = false;
	mutexInit(&UsbMutex);

	Result rc = UsbStreamingInitialize();
	if (R_FAILED(rc)) 
		fatalThrow(rc);
}

static void USB_Exit()
{
	mutexLock(&UsbMutex);
	VideoConnected = false;
	AudioConnected = false;
	UsbStreamingExit();
	mutexUnlock(&UsbMutex);
}

const StreamMode USB_MODE = {
	USB_Init, USB_Exit, 
	USB_VideoStreamThread, USB_AudioStreamThread,
	NULL, NULL,
};