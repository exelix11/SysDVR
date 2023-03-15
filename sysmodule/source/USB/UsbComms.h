#pragma once
#include <switch.h>

// Usb code mostly taken from libnx, can't use their usbcomms because we want custom VID/PID and device strings

typedef struct {
	u8 bInterfaceClass;
	u8 bInterfaceSubClass;
	u8 bInterfaceProtocol;
} UsbSerailInterfaceInfo;

typedef struct {
	u16 VendorId;
	u16 ProductId;

	const char* DeviceName;
	const char* DeviceManufacturer;
	const char* DeviceSerialNumber;

	u8 NumInterfaces;
	UsbSerailInterfaceInfo Interfaces[1];
} UsbSerialInitializationInfo;

/// Initializes usbComms with a specific number of interfaces.
Result usbSerialInitialize(const UsbSerialInitializationInfo* info);

/// Exits usbComms.
void usbSerialExit(void);

/// Read data with the default interface.
size_t usbSerialRead(void* buffer, size_t size);

/// Write data with the default interface.
size_t usbSerialWrite(const void* buffer, size_t size);

/// Same as usbSerialRead except with the specified interface.
size_t usbSerialReadEx(void* buffer, size_t size, u32 interface);

/// Same as usbSerialWrite except with the specified interface.
size_t usbSerialWriteEx(const void* buffer, size_t size, u32 interface);
