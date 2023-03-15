#include "modes.h"
#include "../USB/Serial.h"
#include "../capture.h"

typedef struct {
	UsbStreamChannel Channel;
	u32 CheckCode;
	VLAPacket* Packet;
	ConsumerProducer* Consumer;
} UsbStreamBlock;

// Usb streaming is now single-threaded
static void USB_StreamThread(void*)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_GENERIC);

	LOG("Usb:beginThread %p\n", armGetTls());

	while (IsThreadRunning)
	{
		UsbStreamRequest req = UsbStreamingWaitConnection();
		LOG("UsbStreamRequest req = %x\n", req);
		if (req == UsbStreamRequestFailed)
			continue;

		bool Video = req == UsbStreamRequestVideo || req == UsbStreamRequestBoth;
		bool Audio = req == UsbStreamRequestAudio || req == UsbStreamRequestBoth;

		u64 lastConnection = armGetSystemTick();

		LOG("UsbStreamRequest connected V:%d A:%d\n", Video, Audio);
		
		// Signal the client connected
		if (Video) CaptureOnClientConnected(&VideoProducer);
		if (Audio) CaptureOnClientConnected(&AudioProducer);

		// Once client is connected just continuously send data
		while (true) {
			// Wait for one of the two streams to be available
			ConsumerProducer* target = CaptureWaitBeginConsumeAny(&VideoProducer, &AudioProducer);

			// Should not happen, the current implementation will fatal
			if (!target)
				continue;

			// If we wasted too much time the client will try to reconnect
			bool isTooLate = armGetSystemTick() - lastConnection > armNsToTicks(1.5E+9);
			bool success = !isTooLate && IsThreadRunning;

			// If we can, send the data
			if (success)
			{
				if (target == &VideoProducer)
					success = UsbStreamingSend(&VPkt, VPkt.Header.DataSize + sizeof(PacketHeader), UsbStreamChannelVideo);
				else 
					success = UsbStreamingSend(&APkt, APkt.Header.DataSize + sizeof(PacketHeader), UsbStreamChannelAudio);
			}

			lastConnection = armGetSystemTick();
			CaptureEndConsume(target);

			// If writing fails go back to waiting for client
			if (!success)
			{
				if (target == &VideoProducer)
					LOG("Video failed\n");
				else 
					LOG("Audio failed\n");
				
				break;
			}
		}

		if (Video) CaptureOnClientDisconnected(&VideoProducer);
		if (Audio) CaptureOnClientDisconnected(&AudioProducer);
	}

	LOG("Usb:endThread\n");
}

static void USB_Init()
{
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
	Result rc = UsbStreamingInitialize();
	if (R_FAILED(rc)) 
		fatalThrow(rc);
}

static void USB_Exit()
{
	UsbStreamingExit();
}

const StreamMode USB_MODE = {
	USB_Init, USB_Exit, 
	USB_StreamThread, NULL,
	NULL, NULL
};