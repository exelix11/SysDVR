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

#ifndef __USB_H
#define __USB_H

#ifdef __cplusplus
extern "C" {
#endif

#include <switch.h>

typedef struct {
    struct usb_interface_descriptor *interface_desc;
    struct usb_endpoint_descriptor *endpoint_desc[4];
    const char *string_descriptor;
} UsbInterfaceDesc;

typedef enum {
    UsbDirection_Read  = 0,
    UsbDirection_Write = 1,
} UsbDirection;

Result usbInitialize(struct usb_device_descriptor *device_descriptor, u32 num_interfaces, const UsbInterfaceDesc *infos);
void usbExit(void);
size_t usbTransfer(u32 interface, u32 endpoint, UsbDirection dir, void* buffer, size_t size, u64 timeout);

#ifdef __cplusplus
} // extern "C"
#endif

#endif /* __USB_H */
