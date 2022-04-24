#include "modes.h"
#include "../USB/Serial.h"
#include "../grcd.h"

static inline bool SerialWrite(UsbInterface interface, const VLAPacket* packet)
{
	const u32 size = packet->Header.DataSize + sizeof(PacketHeader);
	return UsbSerialWrite(interface, packet, size, 2E+8) == size;
}

static inline bool WaitRequest(UsbInterface interface)
{
	u32 data = 0;
	return UsbSerialRead(interface, &data, sizeof(data), 3e+9) == sizeof(data) && data == 0xAAAAAAAA;
}

typedef struct {
	u32 CheckCode;
	UsbInterface Pipe;
	VLAPacket* Packet;
	bool (*ReadStreamFunc)();
} UsbStreamBlock;

static void USB_StreamThread(void* threadConf)
{
	const UsbStreamBlock* const info = (UsbStreamBlock*)threadConf;

	if (!IsThreadRunning)
		fatalThrow(info->CheckCode);

	while (IsThreadRunning)
	{
		// Wait for client
		if (!WaitRequest(info->Pipe))
			continue;

		// Once client is connected just continuously send data
		while (true) {
			if (!info->ReadStreamFunc())
			{
				if (IsThreadRunning) continue; else break;
			}

			if (!SerialWrite(info->Pipe, info->Packet))
			{
				// If writing fails go back to waiting for client
				break;
			}
		}
	}
}

UsbStreamBlock VideoConfig = {
	.CheckCode = ERR_USB_VIDEO,
	.Packet = (VLAPacket*)&VPkt,
	.ReadStreamFunc = ReadVideoStream
};

UsbStreamBlock AudioConfig = {
	.CheckCode = ERR_USB_AUDIO,
	.Packet = (VLAPacket*)&APkt,
	.ReadStreamFunc = ReadAudioStream
};

static void USB_Init()
{
	VPkt.Header.Magic = 0xAAAAAAAA;
	APkt.Header.Magic = 0xAAAAAAAA;
	Result rc = UsbSerialInitializeForStreaming(&VideoConfig.Pipe, &AudioConfig.Pipe);
	if (R_FAILED(rc)) 
		fatalThrow(rc);
}

static void USB_Exit()
{
	UsbSerialExit();
}

const StreamMode USB_MODE = {
	USB_Init, USB_Exit, 
	USB_StreamThread, USB_StreamThread,
	&VideoConfig, &AudioConfig 
};