#pragma once

#include <switch.h>

typedef enum {
	UsbDirection_Read = 0,
	UsbDirection_Write = 1,
} UsbDirection;

typedef struct {
	u32 interface, WriteEP, ReadEP;
} UsbPipe;

typedef struct {
	struct usb_interface_descriptor* interface_desc;
	struct usb_endpoint_descriptor* endpoint_desc[4];
	const char* string_descriptor;
} UsbInterfaceDesc;

Result UsbCommsInitialize(struct usb_device_descriptor* device_descriptor, u32 num_interfaces, const UsbInterfaceDesc* infos);
void UsbCommsExit(void);

size_t UsbCommsReadEx(void* buffer, size_t size, u32 interface, u64 timeout);
size_t UsbCommsWriteEx(const void* buffer, size_t size, u32 interface, u64 timeout);

static inline size_t UsbSerialRead(const UsbPipe* stream, void* buf, u32 bufSize, u64 timeout)
{
	return UsbCommsReadEx(buf, bufSize, stream->interface, timeout);
}

static inline size_t UsbSerialWrite(const UsbPipe* stream, const void* buf, u32 bufSize, u64 timeout)
{
	return UsbCommsWriteEx(buf, bufSize, stream->interface, timeout);
}