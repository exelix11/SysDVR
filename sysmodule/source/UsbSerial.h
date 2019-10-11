#pragma once
#include <switch.h>

//Mostly taken from libnx but edited to allow setting a timeout to read/writes

Result UsbSerialInitialize(u32 num_interfaces, const UsbCommsInterfaceInfo *infos);
void UsbSerialExit(void);
size_t UsbSerialRead(void* buffer, size_t size, u32 interface, u64 timeout);
size_t UsbSerialWrite(const void* buffer, size_t size, u32 interface, u64 timeout);
