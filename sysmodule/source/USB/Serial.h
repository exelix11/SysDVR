#pragma once 
#include "UsbComms.h"

// SysDVR specific usb code
Result UsbStreamingInitialize();

void UsbStreamingExit();

// This is not protected by a mutex, the caller should ensure thread safety
bool UsbStreamingSend(const void* data, size_t length);

// This is not protected by a mutex, the caller should ensure thread safety
bool UsbStreamingReceive(void* data, size_t length, size_t* out_read);