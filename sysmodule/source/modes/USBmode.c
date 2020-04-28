#include "modes.h"
#include "../USB/Serial.h"
#include "../grcd.h"

static UsbInterface VFace;
static UsbInterface AFace;

static inline bool SerialWrite(UsbInterface * interface, const PacketHeader* header, const void* data)
{
	//Must do different transfers because libusb breaks otherwise
	u32 req = 0;
	u32 sz = UsbSerialRead(interface, &req, sizeof(req), 1E+9);
	if (sz != sizeof(req) || req != 0xAAAAAAAA)
		return 0;

	sz = UsbSerialWrite(interface, header, sizeof(PacketHeader), 1E+9);
	if (sz != sizeof(PacketHeader))
		return 0;

	sz = UsbSerialWrite(interface, data, header->DataSize, 1E+9);
	return sz == header->DataSize;
}

static void USB_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_VIDEO);

	while (true)
	{
		if (!ReadVideoStream())
			TerminateOrContinue

		if (!SerialWrite(&VFace, &VPkt.Header, VPkt.Data))
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

		if (!SerialWrite(&AFace, &APkt.Header, APkt.Data))
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