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

#include <string.h>
#include <malloc.h>

#include "usb.h"

#define TOTAL_INTERFACES 4
#define TOTAL_ENDPOINTS 4

typedef struct {
    UsbDsEndpoint *endpoint;
    u8 *buffer;
    RwLock lock;
} usbCommsEndpoint;

typedef struct {
    RwLock lock;
    bool initialized;
    UsbDsInterface* interface;
    u32 endpoint_number;
    usbCommsEndpoint endpoint[TOTAL_ENDPOINTS];
} usbCommsInterface;

static bool g_usbCommsInitialized = false;
static usbCommsInterface g_usbCommsInterfaces[TOTAL_INTERFACES];
static bool g_usbCommsErrorHandling = 0;
static RwLock g_usbCommsLock;
static int ep_in = 1;
static int ep_out = 1;

static Result _usbCommsInterfaceInit5x(u32 intf_ind, const UsbInterfaceDesc *info);
static Result _usbCommsInterfaceInit(u32 intf_ind, const UsbInterfaceDesc *info);

Result usbInitialize(struct usb_device_descriptor *device_descriptor, u32 num_interfaces, const UsbInterfaceDesc *infos)
{
    Result rc = 0;
    rwlockWriteLock(&g_usbCommsLock);
    
    if (g_usbCommsInitialized) {
        rc = MAKERESULT(Module_Libnx, LibnxError_AlreadyInitialized);
    } else if (num_interfaces > TOTAL_INTERFACES) {
        rc = MAKERESULT(Module_Libnx, LibnxError_OutOfMemory);
    } else {
        rc = usbDsInitialize();
        
        if (R_SUCCEEDED(rc)) {
            if (hosversionAtLeast(5,0,0)) {
                u8 iManufacturer, iProduct, iSerialNumber;
                static const u16 supported_langs[1] = {0x0409};
                // Send language descriptor
                rc = usbDsAddUsbLanguageStringDescriptor(NULL, supported_langs, sizeof(supported_langs)/sizeof(u16));
                // Send manufacturer
                if (R_SUCCEEDED(rc)) rc = usbDsAddUsbStringDescriptor(&iManufacturer, "Nintendo");
                // Send product
                if (R_SUCCEEDED(rc)) rc = usbDsAddUsbStringDescriptor(&iProduct, "Nintendo Switch");
                // Send serial number
                if (R_SUCCEEDED(rc)) rc = usbDsAddUsbStringDescriptor(&iSerialNumber, "SerialNumber");
                
                // Send device descriptors
                device_descriptor->iManufacturer = iManufacturer;
                device_descriptor->iProduct = iProduct;
                device_descriptor->iSerialNumber = iSerialNumber;

                // Full Speed is USB 1.1
                if (R_SUCCEEDED(rc)) rc = usbDsSetUsbDeviceDescriptor(UsbDeviceSpeed_Full, device_descriptor);
                
                // High Speed is USB 2.0
                device_descriptor->bcdUSB = 0x0200;
                if (R_SUCCEEDED(rc)) rc = usbDsSetUsbDeviceDescriptor(UsbDeviceSpeed_High, device_descriptor);
                
                // Super Speed is USB 3.0
                device_descriptor->bcdUSB = 0x0300;
                // Upgrade packet size to 512
                device_descriptor->bMaxPacketSize0 = 0x09;
                if (R_SUCCEEDED(rc)) rc = usbDsSetUsbDeviceDescriptor(UsbDeviceSpeed_Super, device_descriptor);
                
                // Define Binary Object Store
                u8 bos[0x16] = {
                    0x05, // .bLength
                    USB_DT_BOS, // .bDescriptorType
                    0x16, 0x00, // .wTotalLength
                    0x02, // .bNumDeviceCaps
                    
                    // USB 2.0
                    0x07, // .bLength
                    USB_DT_DEVICE_CAPABILITY, // .bDescriptorType
                    0x02, // .bDevCapabilityType
                    0x02, 0x00, 0x00, 0x00, // dev_capability_data
                    
                    // USB 3.0
                    0x0A, // .bLength
                    USB_DT_DEVICE_CAPABILITY, // .bDescriptorType
                    0x03, // .bDevCapabilityType
                    0x00, 0x0E, 0x00, 0x03, 0x00, 0x00, 0x00
                };
                if (R_SUCCEEDED(rc)) rc = usbDsSetBinaryObjectStore(bos, sizeof(bos));
            }
            
            if (R_SUCCEEDED(rc)) {
                for (u32 i = 0; i < num_interfaces; i++) {
                    usbCommsInterface *intf = &g_usbCommsInterfaces[i];
                    const UsbInterfaceDesc *info = &infos[i];
                    intf->endpoint_number = info->interface_desc->bNumEndpoints;
                    rwlockWriteLock(&intf->lock);
                    for (u32 i = 0; i < intf->endpoint_number; i++)
                    {
                        rwlockWriteLock(&intf->endpoint[i].lock);
                    }
                    rc = _usbCommsInterfaceInit(i, info);
                    for (u32 i = 0; i < intf->endpoint_number; i++)
                    {
                        rwlockWriteUnlock(&intf->endpoint[i].lock);
                    }
                    rwlockWriteUnlock(&intf->lock);
                    if (R_FAILED(rc)) {
                        break;
                    }
                }
            }
        }
        
        if (R_SUCCEEDED(rc) && hosversionAtLeast(5,0,0)) {
            rc = usbDsEnable();
        }
        
        if (R_FAILED(rc)) {
            usbExit();
        }
    }
    
    if (R_SUCCEEDED(rc)) {
        g_usbCommsInitialized = true;
        g_usbCommsErrorHandling = false;
    }

    rwlockWriteUnlock(&g_usbCommsLock);
    return rc;
}

static void _usbCommsInterfaceFree(usbCommsInterface *interface)
{
    rwlockWriteLock(&interface->lock);
    if (!interface->initialized) {
        rwlockWriteUnlock(&interface->lock);
        return;
    }

    interface->initialized = 0;
    interface->interface = NULL;

    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        rwlockWriteLock(&interface->endpoint[i].lock);
        interface->endpoint[i].endpoint = NULL;
        free(interface->endpoint[i].buffer);
        interface->endpoint[i].buffer = NULL;
        rwlockWriteUnlock(&interface->endpoint[i].lock);
    }

    rwlockWriteUnlock(&interface->lock);
}

void usbExit(void)
{
    u32 i;

    rwlockWriteLock(&g_usbCommsLock);

    usbDsExit();

    g_usbCommsInitialized = false;

    rwlockWriteUnlock(&g_usbCommsLock);

    for (i=0; i<TOTAL_INTERFACES; i++)
    {
        _usbCommsInterfaceFree(&g_usbCommsInterfaces[i]);
    }
}

static Result _usbCommsInterfaceInit(u32 intf_ind, const UsbInterfaceDesc *info)
{
    /*if (hosversionAtLeast(5,0,0)) {*/
        return _usbCommsInterfaceInit5x(intf_ind, info);
    /*} else {
        return _usbCommsInterfaceInit1x(intf_ind, info);
    }*/
}

static Result _usbCommsInterfaceInit5x(u32 intf_ind, const UsbInterfaceDesc *info)
{
    Result rc = 0;
    usbCommsInterface *interface = &g_usbCommsInterfaces[intf_ind];

    u8 index = 0;
    if(info->string_descriptor != NULL)
    {
        usbDsAddUsbStringDescriptor(&index, info->string_descriptor);
    }
    info->interface_desc->iInterface = index;
    
    struct usb_ss_endpoint_companion_descriptor endpoint_companion = {
        .bLength = sizeof(struct usb_ss_endpoint_companion_descriptor),
        .bDescriptorType = USB_DT_SS_ENDPOINT_COMPANION,
        .bMaxBurst = 0x0F,
        .bmAttributes = 0x00,
        .wBytesPerInterval = 0x00,
    };

    interface->initialized = 1;

    //The buffer for PostBufferAsync commands must be 0x1000-byte aligned.
    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        interface->endpoint[i].buffer = (u8*)memalign(0x1000, 0x1000);
        if (interface->endpoint[i].buffer == NULL)
        {
            rc = MAKERESULT(Module_Libnx, LibnxError_OutOfMemory);
            break;
        }
        memset(interface->endpoint[i].buffer, 0, 0x1000);
    }
    if (R_FAILED(rc)) return rc;
    
    rc = usbDsRegisterInterface(&interface->interface);
    if (R_FAILED(rc)) return rc;
    
    info->interface_desc->bInterfaceNumber = interface->interface->interface_index;
    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        if((info->endpoint_desc[i]->bEndpointAddress & USB_ENDPOINT_IN) != 0)
        {
            info->endpoint_desc[i]->bEndpointAddress |= ep_in;
            ep_in++;
        }
        else
        {
            info->endpoint_desc[i]->bEndpointAddress |= ep_out;
            ep_out++;
        } 
    }
    
    // Full Speed Config
    rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_Full, info->interface_desc, USB_DT_INTERFACE_SIZE);
    if (R_FAILED(rc)) return rc;

    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        if(info->endpoint_desc[i]->bmAttributes == USB_TRANSFER_TYPE_BULK)
            info->endpoint_desc[i]->wMaxPacketSize = 0x40;
        rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_Full, info->endpoint_desc[i], USB_DT_ENDPOINT_SIZE);
        if (R_FAILED(rc)) return rc;
    }
    
    // High Speed Config
    rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_High, info->interface_desc, USB_DT_INTERFACE_SIZE);
    if (R_FAILED(rc)) return rc;

    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        if(info->endpoint_desc[i]->bmAttributes == USB_TRANSFER_TYPE_BULK)
            info->endpoint_desc[i]->wMaxPacketSize = 0x200;
        rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_High, info->endpoint_desc[i], USB_DT_ENDPOINT_SIZE);
        if (R_FAILED(rc)) return rc;
    }
    
    // Super Speed Config
    rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_Super, info->interface_desc, USB_DT_INTERFACE_SIZE);
    if (R_FAILED(rc)) return rc;

    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        if(info->endpoint_desc[i]->bmAttributes == USB_TRANSFER_TYPE_BULK)
            info->endpoint_desc[i]->wMaxPacketSize = 0x400;
        rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_Super, info->endpoint_desc[i], USB_DT_ENDPOINT_SIZE);
        if (R_FAILED(rc)) return rc;
        rc = usbDsInterface_AppendConfigurationData(interface->interface, UsbDeviceSpeed_Super, &endpoint_companion, USB_DT_SS_ENDPOINT_COMPANION_SIZE);
        if (R_FAILED(rc)) return rc;
    }
    
    //Setup endpoints.    
    for (u32 i = 0; i < interface->endpoint_number; i++)
    {
        rc = usbDsInterface_RegisterEndpoint(interface->interface, &interface->endpoint[i].endpoint, info->endpoint_desc[i]->bEndpointAddress);
        if (R_FAILED(rc)) return rc;
    }

    rc = usbDsInterface_EnableInterface(interface->interface);
    if (R_FAILED(rc)) return rc;
    
    return rc;
}

static Result _usbCommsTransfer(usbCommsEndpoint *ep, UsbDirection dir, const void* buffer, size_t size, u64 timeout, size_t *transferredSize)
{
    Result rc=0;
    u32 urbId=0;
    u32 chunksize=0;
    u8 transfer_type=0;
    u8 *bufptr = (u8*)buffer;
    u8 *transfer_buffer = NULL;
    u32 tmp_transferredSize = 0;
    size_t total_transferredSize=0;
    UsbDsReportData reportdata;

    //Makes sure endpoints are ready for data-transfer / wait for init if needed.
    rc = usbDsWaitReady(U64_MAX);
    if (R_FAILED(rc)) return rc;

    while(size)
    {
        if(((u64)bufptr) & 0xfff)//When bufptr isn't page-aligned copy the data into g_usbComms_endpoint_in_buffer and transfer that, otherwise use the bufptr directly.
        {
            transfer_buffer = ep->buffer;
            memset(ep->buffer, 0, 0x1000);

            chunksize = 0x1000;
            chunksize-= ((u64)bufptr) & 0xfff;//After this transfer, bufptr will be page-aligned(if size is large enough for another transfer).
            if (size<chunksize) chunksize = size;

            if(dir == UsbDirection_Write)
                memcpy(ep->buffer, bufptr, chunksize);

            transfer_type = 0;
        }
        else
        {
            transfer_buffer = bufptr;
            chunksize = size;
            transfer_type = 1;
        }

        //Start transfer.
        rc = usbDsEndpoint_PostBufferAsync(ep->endpoint, transfer_buffer, chunksize, &urbId);
        if(R_FAILED(rc))return rc;

        //Wait for the transfer to finish.
        rc = eventWait(&ep->endpoint->CompletionEvent, timeout);

        if (R_FAILED(rc))
        {
            usbDsEndpoint_Cancel(ep->endpoint);
            eventWait(&ep->endpoint->CompletionEvent, U64_MAX);
            eventClear(&ep->endpoint->CompletionEvent);
            return rc;
        }
        eventClear(&ep->endpoint->CompletionEvent);

        rc = usbDsEndpoint_GetReportData(ep->endpoint, &reportdata);
        if (R_FAILED(rc)) return rc;

        rc = usbDsParseReportData(&reportdata, urbId, NULL, &tmp_transferredSize);
        if (R_FAILED(rc)) return rc;

        if (tmp_transferredSize > chunksize) tmp_transferredSize = chunksize;

        total_transferredSize+= (size_t)tmp_transferredSize;

        if ((transfer_type==0) && (dir == UsbDirection_Read))
            memcpy(bufptr, transfer_buffer, tmp_transferredSize);

        bufptr+= tmp_transferredSize;
        size-= tmp_transferredSize;

        if (tmp_transferredSize < chunksize) break;
    }

    if (transferredSize) *transferredSize = total_transferredSize;

    return rc;
}

size_t usbTransfer(u32 interface, u32 endpoint, UsbDirection dir, void* buffer, size_t size, u64 timeout)
{
    size_t transferredSize=-1;
    u32 state=0;
    Result rc, rc2;
    bool initialized;

    usbCommsInterface *inter = &g_usbCommsInterfaces[interface];
    usbCommsEndpoint *ep = &inter->endpoint[endpoint];
    rwlockReadLock(&inter->lock);
    initialized = inter->initialized;
    rwlockReadUnlock(&inter->lock);
    if (!initialized) return 0;

    rwlockWriteLock(&ep->lock);
    rc = _usbCommsTransfer(ep, dir, buffer, size, timeout, &transferredSize);
    rwlockWriteUnlock(&ep->lock);
    if (R_FAILED(rc)) {
        rc2 = usbDsGetState(&state);
        if (R_SUCCEEDED(rc2)) {
            if (state!=5) {
                rwlockWriteLock(&ep->lock);
                rc = _usbCommsTransfer(ep, dir, buffer, size, timeout, &transferredSize); //If state changed during transfer, try again. usbDsWaitReady() will be called from this.
                rwlockWriteUnlock(&ep->lock);
            }
        }
        if (R_FAILED(rc) && g_usbCommsErrorHandling)
        {
            if(dir == UsbDirection_Write)
                fatalSimple(MAKERESULT(Module_Libnx, LibnxError_BadUsbCommsWrite));
            else
                fatalSimple(MAKERESULT(Module_Libnx, LibnxError_BadUsbCommsRead));
        }
    }
    return transferredSize;
}
