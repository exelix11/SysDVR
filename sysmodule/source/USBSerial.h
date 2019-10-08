#pragma once
#include "usb.h"

#if !defined(__SWITCH__)
typedef u64 ssize_t;
#endif

typedef struct USBDeviceStream
{
	int interface, WriteEP, ReadEP;
} USBDevStream;

void UsbEndpointPairInit(UsbInterfaceDesc* info, USBDevStream* out1, USBDevStream* out2, int interface);
ssize_t UsbRead(USBDevStream* dev, void* ptr, size_t len, u64 timeout);
ssize_t UsbWrite(USBDevStream* dev, const void* ptr, size_t len, u64 timeout);