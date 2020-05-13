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

size_t UsbCommsTransfer(u32 interface, u32 endpoint, UsbDirection dir, const void* buffer, size_t size, u64 timeout);

static inline size_t UsbSerialRead(const UsbPipe* stream, const void* buf, u32 bufSize, u64 timeout)
{
	return UsbCommsTransfer(stream->interface, stream->ReadEP, UsbDirection_Read, buf, bufSize, timeout);
}

static inline size_t UsbSerialWrite(const UsbPipe* stream, const void* buf, u32 bufSize, u64 timeout)
{
	return UsbCommsTransfer(stream->interface, stream->WriteEP, UsbDirection_Write, buf, bufSize, timeout);
}