#include "modes.h"
#include "../USB/Serial.h"
#include "../capture.h"

// Usb streaming is now single-threaded
static void USB_StreamThread(void*)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_USB_THREAD_NOT_RUNNING);

	LOG("Usb:beginThread %p\n", armGetTls());

	while (IsThreadRunning)
	{
		UsbStreamRequest req = UsbStreamingWaitConnection();
		if (req == UsbStreamRequestFailed) {
			svcSleepThread(5E+8L);
			continue;
		}

		bool Video = req == UsbStreamRequestVideo || req == UsbStreamRequestBoth;
		bool Audio = req == UsbStreamRequestAudio || req == UsbStreamRequestBoth;

		if (!Audio && !Video)
			fatalThrow(ERR_USB_UNKMODE);

		u64 lastConnection = armGetSystemTick();

		LOG("UsbStreamRequest connected V:%d A:%d\n", Video, Audio);
		
		// Clear any pending data from previous connections,
		// If we don't do this CaptureWaitBeginConsumeAny may return video data
		// from a previous connection to an only audio stream (or vice-versa)
		CaptureClearPendingData();
		
		// Signal the releveant producers that the client connected
		if (Video) CaptureOnClientConnected(&VideoProducer);
		if (Audio) CaptureOnClientConnected(&AudioProducer);

		// Once client is connected just continuously send data
		while (true) {
			// Wait for one of the two streams to be available
			// Target will never be NULL, CaptureWaitBeginConsumeAny will fatal
			// in case something interrupts the wait (exiting mode cleanly unlocks simulating a produce)
			ConsumerProducer* target = CaptureWaitBeginConsumeAny(&AudioProducer, &VideoProducer);

			// If we wasted too much time the client will try to reconnect
			bool isTooLate = armGetSystemTick() - lastConnection > armNsToTicks(1.5E+9);
			bool success = !isTooLate && IsThreadRunning;

			// If we can, send the data
			if (success)
			{
				const PacketHeader* send = 
					target == &VideoProducer ? &VPkt.Header : &APkt.Header;
				
				success = UsbStreamingSend(send, send->DataSize + sizeof(PacketHeader));
			}

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

			// Update lastConnection only if succeeded
			lastConnection = armGetSystemTick();
		}

		if (Video) CaptureOnClientDisconnected(&VideoProducer);
		if (Audio) CaptureOnClientDisconnected(&AudioProducer);

		// We're here because of a client error (disconnection ?)
		// Wait a bit before accepting commands again
		if (IsThreadRunning) 
			svcSleepThread(5E+8L);
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