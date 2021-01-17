#pragma once 
#include "UsbComms.h"

Result UsbSerialInitializeForStreaming(UsbInterface* video, UsbInterface* audio);
void UsbSerialExit();