#include <string.h>
#include <stdatomic.h>
#include <stdio.h>

#include "Serial.h"
#include "UsbComms.h"

// We need the thread running flag
#include "../modes/modes.h"

Result UsbStreamingInitialize()
{
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
		.DeviceSerialNumber = SysDVRBeacon,

		.NumInterfaces = 1,
		.Interfaces = { interfaces }
	};

	return usbSerialInitialize(&info);
}

void UsbStreamingExit()
{
	usbSerialExit();
}

bool UsbStreamingSend(const void* data, size_t length)
{
	size_t sent = usbSerialWrite(data, length, 1E+9);
	return sent == length;
}

bool UsbStreamingReceive(void* data, size_t length, size_t* out_read)
{
	size_t read = usbSerialRead(data, length, 1E+9);
	
	if (out_read)
		*out_read = read;
	
	return read != 0;
}