#include "modes.h"
#include "../USB/Serial.h"
#include "../capture.h"

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
	ConsumerProducer* Consumer;
} UsbStreamBlock;

static void USB_StreamThread(void* threadConf)
{
	const UsbStreamBlock* const info = (UsbStreamBlock*)threadConf;

	if (!IsThreadRunning)
		fatalThrow(info->CheckCode);

	LOG("Usb:beginThread %x %p\n", info->CheckCode, armGetTls());

	/* 
		Since latest refactor there's a weird condition that now causes a crash in armDCacheFlush at the strb w1, [x0, #0x104] instruction
		x0 points to TLS and this function is called inside SerialWrite by libnx itself, no clue what could be the issue or how to debug it.
		
		It seems to happen whenever the client sends multiple stream begin requests while sysdvr is already sending data.
		With the previous implementations those transfers would just timeout and fail but now they don't.
		The fix is to receive the stream begin request whenever we know it's coming so we don't run in this condition.
		
		There may be adeeper issue here but understanding it is probably going to require too much effort.
	*/

	while (IsThreadRunning)
	{
		// Wait for client
		if (!WaitRequest(info->Pipe))
			continue;

		u64 lastConnection = armGetSystemTick();
		CaptureOnClientConnected(info->Consumer);

		// Once client is connected just continuously send data
		while (true) {
			CaptureBeginConsume(info->Consumer);
			
			// If we wasted too much time the client will try to reconnect
			bool isTooLate = armGetSystemTick() - lastConnection > armNsToTicks(1.5E+9);
			
			bool success = 
				!isTooLate && 
				IsThreadRunning && 
				SerialWrite(info->Pipe, info->Packet);

			lastConnection = armGetSystemTick();
			CaptureEndConsume(info->Consumer);

			// If writing fails go back to waiting for client
			if (!success)
				break;
		}

		CaptureOnClientDisconnected(info->Consumer);
	}

	LOG("Usb:endThread\n");
}

UsbStreamBlock VideoConfig = {
	.CheckCode = ERR_USB_VIDEO,
	.Packet = (VLAPacket*)&VPkt,
	.Consumer = &VideoProducer
};

UsbStreamBlock AudioConfig = {
	.CheckCode = ERR_USB_AUDIO,
	.Packet = (VLAPacket*)&APkt,
	.Consumer = &AudioProducer
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