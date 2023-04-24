#pragma once 
#include "UsbComms.h"

// SysDVR specific usb code

typedef enum {
	UsbStreamChannelVideo,
	UsbStreamChannelAudio,
} UsbStreamChannel;

typedef enum {
	UsbStreamRequestFailed = 0,
	UsbStreamRequestBoth = 0xAAAAAAAA,
	UsbStreamRequestVideo = 0xBBBBBBBB,
	UsbStreamRequestAudio = 0xCCCCCCCC,
} UsbStreamRequest;

Result UsbStreamingInitialize();

void UsbStreamingExit();

UsbStreamRequest UsbStreamingWaitConnection();

// This is not protected by a mutex, the caller should ensure thread safety
bool UsbStreamingSend(const void* data, size_t length);