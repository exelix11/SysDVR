#include "modes.h"
#include "../USB/Serial.h"
#include "../grcd.h"

static UsbInterface VFace;
static UsbInterface AFace;

static inline bool SerialWrite(UsbInterface * interface, const void* data, u32 len)
{
	u32 sz = UsbSerialWrite(interface, data, len, 1E+9);
	return sz == len;
}

static void USB_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_VIDEO);

	while (true)
	{
		if (!ReadVideoStream())
			TerminateOrContinue

		const u32 size = sizeof(PacketHeader) + VPkt.Header.DataSize;
		if (!SerialWrite(&VFace, &VPkt, size))
			TerminateOrContinue
	}
}

static void USB_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_AUDIO);

	while (true)
	{
		if (!ReadAudioStream())
			TerminateOrContinue

		const u32 size = sizeof(PacketHeader) + APkt.Header.DataSize;
		if (!SerialWrite(&AFace,&APkt, size))
			TerminateOrContinue
	}
}

static void USB_Init()
{
	SetAudioBatching(4);
	VPkt.Header.Magic = 0xAAAAAAAA;
	APkt.Header.Magic = 0xAAAAAAAA;
	Result rc = UsbSerialInitialize(&VFace, &AFace);
	if (R_FAILED(rc)) 
		fatalThrow(rc);
}

static void USB_Exit()
{
	UsbCommsExit();
}

StreamMode USB_MODE = { USB_Init, USB_Exit, USB_StreamVideo, USB_StreamAudio };