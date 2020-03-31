#include "modes.h"
#include "../UsbSerial.h"
#include "../grcd.h"

static UsbInterface VideoStream;
static UsbInterface AudioStream;
static const u32 REQMAGIC_VID = 0xAAAAAAAA;
static const u32 REQMAGIC_AUD = 0xBBBBBBBB;

static u32 USB_WaitForInputReq(const UsbInterface* dev)
{
	do
	{
		u32 initSeq = 0;
		if (UsbSerialRead(dev, &initSeq, sizeof(initSeq), 1E+9) == sizeof(initSeq))
			return initSeq;
		svcSleepThread(1E+9);
	} while (IsThreadRunning);
	return 0;
}

static void USB_SendStream(GrcStream stream, const UsbInterface* Dev)
{
	u32* const size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	const u32 Magic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	if (*size <= 0)
		*size = 0;

	if (UsbSerialWrite(Dev, &Magic, sizeof(Magic), 1E+8) != sizeof(Magic))
		return;
	if (UsbSerialWrite(Dev, size, sizeof(*size), 1E+8) != sizeof(*size))
		return;

	if (*size)
	{
		u64* const ts = stream == GrcStream_Video ? &VTimestamp : &ATimestamp;
		u8* const TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

		if (UsbSerialWrite(Dev, ts, sizeof(*ts), 1E+8) != sizeof(*ts))
			return;

		if (UsbSerialWrite(Dev, TargetBuf, *size, 2E+8) != *size)
			return;
	}
	return;
}

static void* USB_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 13));

	const GrcStream stream = (GrcStream)_stream;
	const UsbInterface* const Dev = stream == GrcStream_Video ? &VideoStream : &AudioStream;
	const u32 ThreadMagic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	void (* const ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	while (true)
	{
		//TODO: try improving performance by not waiting for requests like the new TCP mode
		u32 cmd = USB_WaitForInputReq(Dev);

		if (cmd == ThreadMagic)
		{
			ReadStreamFn();
			USB_SendStream(stream, Dev);
		}
		else if (!IsThreadRunning) break;
	}
	pthread_exit(NULL);
	return NULL;
}

static void USB_Init()
{
	Result rc = UsbSerialInitializeDefault(&VideoStream, &AudioStream);
	if (R_FAILED(rc)) fatalSimple(rc);
}

static void USB_Exit()
{
	usbSerialExit();
}

StreamMode USB_MODE = { USB_Init, USB_Exit, USB_StreamThreadMain };