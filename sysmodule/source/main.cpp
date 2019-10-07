#include <stdio.h>
#include <tuple>
#include <thread>   
#include <switch.h>

#include "usb.h"
#include "USBSerialInterface.h"

#if !__SWITCH__
//Silence visual studio errors
#define __attribute__(x) 
#endif
#include <mutex>

void StopRecording();

extern "C" {
	extern u32 __start__;
    u32 __nx_applet_type = AppletType_None;
    #define INNER_HEAP_SIZE 0x80000
    size_t nx_inner_heap_size = INNER_HEAP_SIZE;
    char nx_inner_heap[INNER_HEAP_SIZE];
    void __libnx_initheap(void);
    void __appInit(void);
    void __appExit(void);
}

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
		fatalSimple(MAKERESULT(1, 20));
	
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

USBSerialInterface* serial_interface = NULL;
void UsbInit() 
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

	serial_interface = new USBSerialInterface(0, &info);
	usbInitialize(&device_descriptor, 1, &info);
}

const int VbufSz = 0x32000;
const int AbufSz = 0x1000;
const u32 REQMAGIC = 0xDEADCAFE;
const u32 REQMAGIC_AUD = 0xBA5EBA11;
u8* Vbuf = nullptr;
u8* Abuf = nullptr;
u32 VOutSz = 0;
u32 AOutSz = 0;

bool g_recInited = false;
void InitRecording() 
{
	if (g_recInited) return;

	auto res = grcdInitialize();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 10));

	res = grcdBegin();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 30));

	Vbuf = new (std::align_val_t(0x1000))  u8[VbufSz];
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 40));

	Abuf = new (std::align_val_t(0x1000))  u8[AbufSz];
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 50));

	g_recInited = true;
}

void StopRecording()
{
	if (!g_recInited) return;
	grcdExit();
	delete[] Vbuf;
	delete[] Abuf;
	g_recInited = false;
}

static u32 WaitForInputReq()
{
	while (true)
	{
		u32 initSeq = 0;
		auto rc = serial_interface->read((char*)&initSeq, sizeof(initSeq), 16666666);

		if (rc) {
			if (initSeq == REQMAGIC || initSeq == REQMAGIC_AUD) return initSeq;
		}

		svcSleepThread(3E+9);
	}
	return 0;
}

static void ReadStream(GrcStream stream)
{
	u32 unk = 0;
	u64 timestamp = 0;

	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;
	u32 BufSz = stream == GrcStream_Video ? VbufSz : AbufSz;
	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;

	auto res = grcdRead(stream, TargetBuf, BufSz, &unk, size, &timestamp);
	if (R_FAILED(res) || *size <= 0)
		*size = 0;
}

static int SendStream(GrcStream stream)
{
	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;
	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;

	if (*size <= 0)
	{
		*size = 0;
		serial_interface->write((char*)size, sizeof(*size), (u64)1e+8);
		return 1;
	}

	if (serial_interface->write((char*)size, sizeof(*size), (u64)1e+8) != sizeof(*size)) return 1;
	if (serial_interface->write((char*)TargetBuf, *size, (u64)2E+9) != *size) return 1;
	return 0;
}

int main(int argc, char* argv[])
{		
	const u32 ZeroValue = 0;

	UsbInit();

	WaitForInputReq();
	InitRecording();
	serial_interface->write((const char*)&ZeroValue, sizeof(ZeroValue));

	while (true)
	{
		auto cmd = WaitForInputReq();

		if (cmd == REQMAGIC)
		{
			ReadStream(GrcStream_Video);
			if (SendStream(GrcStream_Video))
				svcSleepThread(10000000); // 10ms
			continue;
		}
		else if (cmd == REQMAGIC_AUD) 
		{
			ReadStream(GrcStream_Video);
			ReadStream(GrcStream_Audio);
			SendStream(GrcStream_Video);
			SendStream(GrcStream_Audio);
		}
		else fatalSimple(MAKERESULT(1, 80));
	}
	
    return 0;
}
