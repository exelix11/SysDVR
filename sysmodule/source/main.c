#include <stdlib.h>
#include <switch.h>
#include <pthread.h>

#include "usb.h"
#include "USBSerial.h"

#if !defined(__SWITCH__)
//Silence visual studio errors
#define __attribute__(x) 
#endif

void StopRecording();

extern u32 __start__;
u32 __nx_applet_type = AppletType_None;
#define INNER_HEAP_SIZE 0x80000
size_t nx_inner_heap_size = INNER_HEAP_SIZE;
char nx_inner_heap[INNER_HEAP_SIZE];

void __libnx_initheap(void)
{
	void*  addr = nx_inner_heap;
	size_t size = nx_inner_heap_size;

	// Newlib
	extern char* fake_heap_start;
	extern char* fake_heap_end;

	fake_heap_start = (char*)addr;
	fake_heap_end   = (char*)addr + size;
}

void __attribute__((weak)) __appInit(void)
{
	svcSleepThread(1E+10);

	Result rc;

	rc = smInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	rc = setsysInitialize();
    if (R_SUCCEEDED(rc)) {
        SetSysFirmwareVersion fw;
        rc = setsysGetFirmwareVersion(&fw);
        if (R_SUCCEEDED(rc))
            hosversionSet(MAKEHOSVERSION(fw.major, fw.minor, fw.micro));
        setsysExit();
    }
	
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(1, 10));
	
	fsdevMountSdmc();
}

void __attribute__((weak)) userAppExit(void);

void __attribute__((weak)) __appExit(void)
{
	fsdevUnmountAll();
	fsExit();
	usbExit();
	StopRecording();
	smExit();
}

USBDevStream VideoStream;
USBDevStream AudioStream;
static Result UsbInit() 
{
	struct usb_device_descriptor device_descriptor = {
	  .bLength = USB_DT_DEVICE_SIZE,
	  .bDescriptorType = USB_DT_DEVICE,
	  .bcdUSB = 0x0110,
	  .bDeviceClass = 0x00,
	  .bDeviceSubClass = 0x00,
	  .bDeviceProtocol = 0x00,
	  .bMaxPacketSize0 = 0x40,
	  .idVendor = 0x057e,
	  .idProduct = 0x3000,
	  .bcdDevice = 0x0100,
	  .bNumConfigurations = 0x01
	};

	UsbInterfaceDesc info;
	UsbEndpointPairInit(&info, &VideoStream, &AudioStream, 0);
	return usbInitialize(&device_descriptor, 1, &info);
}

const int VbufSz = 0x32000;
const int AbufSz = 0x1000;
const int AudioBatchSz = 12;
const u32 REQMAGIC_VID = 0xAAAAAAAA;
const u32 REQMAGIC_AUD = 0xBBBBBBBB;
u8* Vbuf = NULL;
u8* Abuf = NULL;
u32 VOutSz = 0;
u32 AOutSz = 0;

bool g_recInited = false;
void InitRecording() 
{
	if (g_recInited) return;

	Result res = grcdInitialize();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 20));

	res = grcdBegin();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 30));

	Vbuf = aligned_alloc(0x1000, VbufSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 40));

	Abuf = aligned_alloc(0x1000, AbufSz * AudioBatchSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 50));

	g_recInited = true;
}

void StopRecording()
{
	if (!g_recInited) return;
	grcdExit();
	free(Vbuf);
	free(Abuf);
	g_recInited = false;
}

static u32 WaitForInputReq(USBDevStream* dev)
{
	while (true)
	{
		u32 initSeq = 0;
		ssize_t rc = UsbRead(dev, &initSeq, sizeof(initSeq), 16666666);

		if (rc == sizeof(initSeq)) return initSeq;

		svcSleepThread(3E+9);
	}
	return 0;
}

//Batch sending audio samples to improve speed
static void ReadAudioStream()
{
	u32 unk = 0;
	u64 timestamp = 0;
	u32 TmpAudioSz = 0;
	AOutSz = 0;
	for (int i = 0; i < AudioBatchSz; i++)
	{
		Result res = grcdRead(GrcStream_Audio, Abuf + AOutSz, AbufSz, &unk, &TmpAudioSz, &timestamp);
		if (R_FAILED(res) || TmpAudioSz <= 0)
		{
			--i;
			continue;
		}
		AOutSz += TmpAudioSz;
	}
}

static void ReadVideoStream()
{
	u32 unk = 0;
	u64 timestamp = 0;

	while (true) {
		Result res = grcdRead(GrcStream_Video, Vbuf, VbufSz, &unk, &VOutSz, &timestamp);
		if (R_FAILED(res) || VOutSz <= 0)
		{
			VOutSz = 0;
			svcSleepThread(5000000);
			continue;
		}
		break;
	}
}

static void SendStream(GrcStream stream, USBDevStream* Dev)
{
	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	 
	if (*size <= 0)
	{
		*size = 0;
		UsbWrite(Dev, size, sizeof(*size), (u64)1e+8);
	}

	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

	if (UsbWrite(Dev, size, sizeof(*size), (u64)1e+8) != sizeof(*size)) return;
	if (UsbWrite(Dev, TargetBuf, *size, (u64)2E+9) != *size) return;
	return;
}

void* StreamThreadMain(void* _stream)
{
	GrcStream stream = (GrcStream)_stream;
	USBDevStream* Dev = stream == GrcStream_Video ? &VideoStream : &AudioStream;
	u8 ErrorCode = stream == GrcStream_Video ? 70 : 80;
	u32 ThreadMagic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	void (*ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	while (true)
	{
		u32 cmd = WaitForInputReq(Dev);

		if (cmd == ThreadMagic)
		{
			ReadStreamFn();
			SendStream(stream, Dev);
		}
		else fatalSimple(MAKERESULT(1, ErrorCode));
	}
	return NULL;
}

int main(int argc, char* argv[])
{
	if (R_FAILED(UsbInit()))
		fatalSimple(MAKERESULT(1, 60));

	InitRecording();

	pthread_t videoThread;
	if (pthread_create(&videoThread, NULL, StreamThreadMain, (void*)GrcStream_Video))
		fatalSimple(MAKERESULT(1, 90));

	StreamThreadMain((void*)GrcStream_Audio);

    return 0;
}
