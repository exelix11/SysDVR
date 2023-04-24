#include "modes.h"
#include "../USB/Serial.h"
#include "../capture.h"

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

static void USB_VideoStreamThread(void*)
{
	while (IsThreadRunning)
	{
		// Video streaming thread acts as master and receives the initial streaming request
		if (!ClientConnected()) {
			UsbStreamRequest req = UsbStreamingWaitConnection();
			if (req == UsbStreamRequestFailed) {
				svcSleepThread(5E+8L);
				continue;
			}

			VideoConnected = req == UsbStreamRequestVideo || req == UsbStreamRequestBoth;
			AudioConnected = req == UsbStreamRequestAudio || req == UsbStreamRequestBoth;

			LastConnection = armGetSystemTick();
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
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
	
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
	// USB uses an higher audio batching value to avoid stuttering
	3
};