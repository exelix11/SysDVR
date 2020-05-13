#include "modes.h"
#include "../USB/Serial.h"
#include "../grcd.h"

static UsbPipe VPipe, APipe;

static inline bool SerialWrite(UsbPipe* interface, const VLAPacket* packet)
{
	const u32 size = packet->Header.DataSize + sizeof(PacketHeader);
	return UsbSerialWrite(interface, packet, size, 1E+9) == size;
}

static inline bool WaitRequest(UsbPipe* interface) 
{
	u32 data = 0;
	return UsbSerialRead(interface, &data, sizeof(data), 3e+9) == sizeof(data) && data == 0xAAAAAAAA;
}

static void USB_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_VIDEO);

	while (true)
	{
		if (!WaitRequest(&VPipe))
			TerminateOrContinue

		if (!ReadVideoStream())
			TerminateOrContinue

		if (!SerialWrite(&VPipe, (VLAPacket*)&VPkt))
			TerminateOrContinue
	}
}

static void USB_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_AUDIO);

	while (true)
	{
		if (!WaitRequest(&APipe))
			TerminateOrContinue

		if (!ReadAudioStream())
			TerminateOrContinue

		if (!SerialWrite(&APipe, (VLAPacket*)&APkt))
			TerminateOrContinue
	}
}

static void USB_Init()
{
	VPkt.Header.Magic = 0xAAAAAAAA;
	APkt.Header.Magic = 0xAAAAAAAA;
	Result rc = UsbSerialInitializeForStreaming(&VPipe, &APipe);
	if (R_FAILED(rc)) 
		fatalThrow(rc);
}

static void USB_Exit()
{
	UsbSerialExit();
}

StreamMode USB_MODE = { USB_Init, USB_Exit, USB_StreamVideo, USB_StreamAudio };