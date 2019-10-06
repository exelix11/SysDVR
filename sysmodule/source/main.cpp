#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <switch.h>

#include "usb.h"
#include "USBSerialInterface.h"

#if !__SWITCH__
#define __attribute__(x) 
#endif

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

// Init/exit services, update as needed.
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
const u32 REQMAGIC = 0xDEADCAFE;
const u32 REQMAGIC_AUD = 0xBA5EBA11;
u8* Vbuf = nullptr;

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

	g_recInited = true;
}

void StopRecording()
{
	if (!g_recInited) return;
	grcdExit();
	delete[] Vbuf;
	g_recInited = false;
}

u32 WaitForInputReq()
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

int GetAndSendStream(GrcStream stream)
{
	u32 unk = 0;
	u32 size = 0;
	u64 timestamp = 0;

	auto res = grcdRead(stream, Vbuf, VbufSz, &unk, &size, &timestamp);
	if (R_FAILED(res) || size <= 0)
	{
		size = 0;
		serial_interface->write((char*)&size, sizeof(size), (u64)1e+9);
		return 1;
	}

	if (serial_interface->write((char*)&size, sizeof(size), (u64)1e+8) != sizeof(size)) return 1;
	if (serial_interface->write((char*)Vbuf, size, (u64)2E+9) != size) return 1;
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
			if (GetAndSendStream(GrcStream_Video))
				svcSleepThread(10000000); // 10ms
			continue;
		}
		else if (cmd == REQMAGIC_AUD) 
		{
			GetAndSendStream(GrcStream_Video);
			GetAndSendStream(GrcStream_Audio);
		}
	}
	
    return 0;
}
