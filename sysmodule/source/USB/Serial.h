#pragma once 
#include "UsbComms.h"

Result UsbSerialInitialize(UsbInterface* VideoStream, UsbInterface* AudioStream);

Result UsbSerialInitializeSingle(UsbInterface* VideoStream);