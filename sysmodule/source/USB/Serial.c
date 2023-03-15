#include <string.h>
#include <stdatomic.h>
#include <stdio.h>

#include "Serial.h"
#include "UsbComms.h"

static Mutex UsbStreamingMutex;
static atomic_bool CancelOperation;

static const char* GetDeviceSerial() 
{
	static char serialStr[50] = "SysDVR:Unknown serial";
	static bool initialized = false;

	if (!initialized) {
		initialized = true;

		Result rc = setsysInitialize();
		if (R_SUCCEEDED(rc))
		{
			SetSysSerialNumber serial;
			rc = setsysGetSerialNumber(&serial);
			
			if (R_SUCCEEDED(rc))
				snprintf(serialStr, sizeof(serialStr), "SysDVR:%s", serial.number);
			
			setsysExit();
		}		
	}

	return serialStr;
}

Result UsbStreamingInitialize()
{
	mutexInit(&UsbStreamingMutex);
	CancelOperation = false;

	UsbSerailInterfaceInfo interfaces = {
		.bInterfaceClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceProtocol = USB_CLASS_VENDOR_SPEC
	};

	UsbSerialInitializationInfo info = {
		// Google Nexus (generic) so we can use Google's signed WinUSB drivers instead of needing zadig
		.VendorId = 0x18D1,
		.ProductId = 0x4EE0,

		.DeviceName = "SysDVR",
		.DeviceManufacturer = "https://github.com/exelix11/SysDVR",
		.DeviceSerialNumber = GetDeviceSerial(),

		.NumInterfaces = 1,
		.Interfaces = { interfaces }
	};

	return usbSerialInitialize(&info);
}

void UsbStreamingExit()
{
	CancelOperation = true;
	
	usbSerialExit();

	// Wait for all other threads to finish
	// TODO: Maybe fatal after a timeout ?
	mutexLock(&UsbStreamingMutex);
	mutexUnlock(&UsbStreamingMutex);
}

UsbStreamRequest UsbStreamingWaitConnection()
{
	// In theory nothing else should be going on over usb at this point
	mutexLock(&UsbStreamingMutex);

	u32 request;
	size_t read = usbSerialRead(&request, sizeof(request), UINT64_MAX);

	mutexUnlock(&UsbStreamingMutex);

	if (read != sizeof(request))
		return UsbStreamRequestFailed;

	if (request == UsbStreamRequestVideo || request == UsbStreamRequestAudio || request == UsbStreamRequestBoth)
		return (UsbStreamRequest)request;

	return UsbStreamRequestFailed;
}

bool UsbStreamingSend(const void* data, size_t length, UsbStreamChannel channel)
{
	// Since switching to a single interface this does the same as UsbStreamingSendVideo now
	(void)channel;

	mutexLock(&UsbStreamingMutex);

	size_t sent = usbSerialWrite(data, length, 1E+9);

	mutexUnlock(&UsbStreamingMutex);

	return sent == length;
}