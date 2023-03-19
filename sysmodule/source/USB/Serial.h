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

// Since switching to a single interface there's no need for two different send functions
bool UsbStreamingSend(const void* data, size_t length);