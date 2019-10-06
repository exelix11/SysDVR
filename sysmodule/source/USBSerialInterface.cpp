/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#include "USBSerialInterface.h"

#define EP_IN 0
#define EP_OUT 1

USBSerialInterface::USBSerialInterface(int index, UsbInterfaceDesc *info)
{
    interface_index = index;
    info->interface_desc = &serial_interface_descriptor;
    info->endpoint_desc[EP_IN] = &serial_endpoint_descriptor_in;
    info->endpoint_desc[EP_OUT] = &serial_endpoint_descriptor_out;
    info->string_descriptor = NULL;
}

USBSerialInterface::~USBSerialInterface() {
}

ssize_t USBSerialInterface::read(char *ptr, size_t len, u64 timeout)
{
    return usbTransfer(interface_index, EP_OUT, UsbDirection_Read, (void*)ptr, len, timeout);
}
ssize_t USBSerialInterface::write(const char *ptr, size_t len, u64 timeout)
{
    return usbTransfer(interface_index, EP_IN, UsbDirection_Write, (void*)ptr, len, timeout);
}
