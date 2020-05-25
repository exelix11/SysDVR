#pragma once 
#include "UsbComms.h"

Result UsbSerialInitializeForStreaming(UsbPipe* video, UsbPipe* audio);
void UsbSerialExit();