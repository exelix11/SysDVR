#include "USBSerial.h"

struct usb_interface_descriptor serial_interface_descriptor = {
	.bLength = USB_DT_INTERFACE_SIZE,
	.bDescriptorType = USB_DT_INTERFACE,
	.bNumEndpoints = 4,
	.bInterfaceClass = USB_CLASS_VENDOR_SPEC,
	.bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
	.bInterfaceProtocol = USB_CLASS_VENDOR_SPEC,
};

struct usb_endpoint_descriptor serial_endpoint_descriptor_in_1 = {
   .bLength = USB_DT_ENDPOINT_SIZE,
   .bDescriptorType = USB_DT_ENDPOINT,
   .bEndpointAddress = USB_ENDPOINT_IN | 1,
   .bmAttributes = USB_TRANSFER_TYPE_BULK,
   .wMaxPacketSize = 0x10,
};

struct usb_endpoint_descriptor serial_endpoint_descriptor_out_1 = {
   .bLength = USB_DT_ENDPOINT_SIZE,
   .bDescriptorType = USB_DT_ENDPOINT,
   .bEndpointAddress = USB_ENDPOINT_OUT | 1,
   .bmAttributes = USB_TRANSFER_TYPE_ISOCHRONOUS,
   .wMaxPacketSize = 0x200,
};

struct usb_endpoint_descriptor serial_endpoint_descriptor_in_2 = {
   .bLength = USB_DT_ENDPOINT_SIZE,
   .bDescriptorType = USB_DT_ENDPOINT,
   .bEndpointAddress = USB_ENDPOINT_IN | 2,
   .bmAttributes = USB_TRANSFER_TYPE_BULK,
   .wMaxPacketSize = 0x10,
};

struct usb_endpoint_descriptor serial_endpoint_descriptor_out_2 = {
   .bLength = USB_DT_ENDPOINT_SIZE,
   .bDescriptorType = USB_DT_ENDPOINT,
   .bEndpointAddress = USB_ENDPOINT_OUT | 2,
   .bmAttributes = USB_TRANSFER_TYPE_ISOCHRONOUS,
   .wMaxPacketSize = 0x200,
};

void UsbEndpointPairInit(UsbInterfaceDesc* info, USBDevStream* out1, USBDevStream* out2, int interface)
{
	out1->interface = interface;
	out1->WriteEP = 0;
	out1->ReadEP = 1;

	out2->interface = interface;
	out2->WriteEP = 2;
	out2->ReadEP = 3;

	info->interface_desc = &serial_interface_descriptor;
	info->endpoint_desc[out1->WriteEP] = &serial_endpoint_descriptor_in_1;
	info->endpoint_desc[out2->WriteEP] = &serial_endpoint_descriptor_in_2;
	info->endpoint_desc[out1->ReadEP] = &serial_endpoint_descriptor_out_1;
	info->endpoint_desc[out2->ReadEP] = &serial_endpoint_descriptor_out_2;
	info->string_descriptor = NULL;
}

ssize_t UsbRead(USBDevStream* dev, void* ptr, size_t len, u64 timeout)
{
	return usbTransfer(dev->interface, dev->ReadEP, UsbDirection_Read, (void*)ptr, len, timeout);
}

ssize_t UsbWrite(USBDevStream* dev, const void* ptr, size_t len, u64 timeout)
{
	return usbTransfer(dev->interface, dev->WriteEP, UsbDirection_Write, (void*)ptr, len, timeout);
}