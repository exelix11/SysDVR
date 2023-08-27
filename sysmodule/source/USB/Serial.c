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

UsbStreamRequest UsbStreamingWaitConnection()
{
	u32 request = 0;
	size_t read = 0;

	do
		read = usbSerialRead(&request, sizeof(request), 1E+9);
	while (read == 0 && IsThreadRunning);

	if (read != sizeof(request) || !IsThreadRunning)
		return UsbStreamRequestFailed;

	LOG("USB request received: %x\n", request);
	if (request == UsbStreamRequestVideo || request == UsbStreamRequestAudio || request == UsbStreamRequestBoth)
		return (UsbStreamRequest)request;

	return UsbStreamRequestFailed;
}

bool UsbStreamingSend(const void* data, size_t length)
{
	size_t sent = usbSerialWrite(data, length, 1E+9);
	return sent == length;
}