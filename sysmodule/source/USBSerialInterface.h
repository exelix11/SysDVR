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

#ifndef __USB_SERIAL_INTERFACE_H
#define __USB_SERIAL_INTERFACE_H

#include "usb.h"

class USBSerialInterface {
private:

    int interface_index;

    struct usb_interface_descriptor serial_interface_descriptor = {
        .bLength = USB_DT_INTERFACE_SIZE,
        .bDescriptorType = USB_DT_INTERFACE,
        .bNumEndpoints = 2,
        .bInterfaceClass = USB_CLASS_VENDOR_SPEC,
        .bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
        .bInterfaceProtocol = USB_CLASS_VENDOR_SPEC,
    };

    struct usb_endpoint_descriptor serial_endpoint_descriptor_in = {
       .bLength = USB_DT_ENDPOINT_SIZE,
       .bDescriptorType = USB_DT_ENDPOINT,
       .bEndpointAddress = USB_ENDPOINT_IN,
       .bmAttributes = USB_TRANSFER_TYPE_BULK,
       .wMaxPacketSize = 0x200,
    };

    struct usb_endpoint_descriptor serial_endpoint_descriptor_out = {
       .bLength = USB_DT_ENDPOINT_SIZE,
       .bDescriptorType = USB_DT_ENDPOINT,
       .bEndpointAddress = USB_ENDPOINT_OUT,
       .bmAttributes = USB_TRANSFER_TYPE_BULK,
       .wMaxPacketSize = 0x200,
    };

public:

            USBSerialInterface(int index, UsbInterfaceDesc *info);
    virtual ~USBSerialInterface();

	ssize_t read(char* ptr, size_t len, u64 timepout = U64_MAX);
    ssize_t write(const char *ptr, size_t len, u64 timepout = U64_MAX);
};

#endif /* __USB_SERIAL_INTERFACE_H */
