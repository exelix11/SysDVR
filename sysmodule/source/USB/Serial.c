#include "Serial.h"
#include <string.h>

static struct usb_interface_descriptor StreamingInterface;
static struct usb_endpoint_descriptor VideoEndpoints[2];
static struct usb_endpoint_descriptor AudioEndpoints[2];

static void ClearDescriptors()
{
#define clearVal(x) memset(&x, 0, sizeof(x))
	clearVal(StreamingInterface);
	clearVal(VideoEndpoints);
	clearVal(AudioEndpoints);
#undef clearVal
}

Result UsbSerialInitializeSingle(UsbInterface* Interface) 
{
	ClearDescriptors();

	struct usb_device_descriptor device_descriptor = {
		.bLength = USB_DT_DEVICE_SIZE,
		.bDescriptorType = USB_DT_DEVICE,
		.bcdUSB = 0x0110,
		.bDeviceClass = 0x00,
		.bDeviceSubClass = 0x00,
		.bDeviceProtocol = 0x00,
		.bMaxPacketSize0 = 0x40,
		.idVendor = 0x057e,
		.idProduct = 0x3006,
		.bcdDevice = 0x0100,
		.bNumConfigurations = 0x01
	};

	StreamingInterface = (struct usb_interface_descriptor){
		.bLength = USB_DT_INTERFACE_SIZE,
		.bDescriptorType = USB_DT_INTERFACE,
		.bNumEndpoints = 2,
		.bInterfaceClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceProtocol = USB_CLASS_VENDOR_SPEC,
	};

	VideoEndpoints[0] = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_IN | 1,
		.bmAttributes = USB_TRANSFER_TYPE_BULK,
		.wMaxPacketSize = 0x200,
	};

	VideoEndpoints[1] = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_OUT | 1,
		.bmAttributes = USB_TRANSFER_TYPE_BULK,
		.wMaxPacketSize = 0x200,
	};

	UsbInterfaceDesc info = {0};

	Interface->interface = 0;
	Interface->WriteEP = 0;
	Interface->ReadEP = 1;

	info.interface_desc = &StreamingInterface;
	info.endpoint_desc[Interface->WriteEP] = &VideoEndpoints[0];
	info.endpoint_desc[Interface->ReadEP] = &VideoEndpoints[1];
	
	return UsbCommsInitialize(&device_descriptor, 1, &info);
}

Result UsbSerialInitialize(UsbInterface* VideoStream, UsbInterface* AudioStream)
{
	ClearDescriptors();

	struct usb_device_descriptor device_descriptor = {
		.bLength = USB_DT_DEVICE_SIZE,
		.bDescriptorType = USB_DT_DEVICE,
		.bcdUSB = 0x0110,
		.bDeviceClass = 0x00,
		.bDeviceSubClass = 0x00,
		.bDeviceProtocol = 0x00,
		.bMaxPacketSize0 = 0x40,
		.idVendor = 0x057e,
		.idProduct = 0x3006,
		.bcdDevice = 0x0100,
		.bNumConfigurations = 0x01
	};

	StreamingInterface = (struct usb_interface_descriptor) {
		.bLength = USB_DT_INTERFACE_SIZE,
		.bDescriptorType = USB_DT_INTERFACE,
		.bNumEndpoints = 2,
		.bInterfaceClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceProtocol = USB_CLASS_VENDOR_SPEC,
	};

	VideoEndpoints[0] = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_IN | 1,
		.bmAttributes = USB_TRANSFER_TYPE_BULK,
		.wMaxPacketSize = 0x200,
	};

	//VideoEndpoints[1] = (struct usb_endpoint_descriptor){
	//	.bLength = USB_DT_ENDPOINT_SIZE,
	//	.bDescriptorType = USB_DT_ENDPOINT,
	//	.bEndpointAddress = USB_ENDPOINT_OUT | 1,
	//	.bmAttributes = USB_TRANSFER_TYPE_BULK,
	//	.wMaxPacketSize = 0x200,
	//};

	AudioEndpoints[0] = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_IN | 2,
		.bmAttributes = USB_TRANSFER_TYPE_BULK,
		.wMaxPacketSize = 0x200,
	};

	//AudioEndpoints[1] = (struct usb_endpoint_descriptor){
	//	.bLength = USB_DT_ENDPOINT_SIZE,
	//	.bDescriptorType = USB_DT_ENDPOINT,
	//	.bEndpointAddress = USB_ENDPOINT_OUT | 2,
	//	.bmAttributes = USB_TRANSFER_TYPE_BULK,
	//	.wMaxPacketSize = 0x200,
	//};

	UsbInterfaceDesc info = {0};

	VideoStream->interface = 0;
	VideoStream->WriteEP = 0;
	//VideoStream->ReadEP = 1;

	AudioStream->interface = 0;
	AudioStream->WriteEP = 1;
	//AudioStream->ReadEP = 3;

	info.interface_desc = &StreamingInterface;
	info.endpoint_desc[VideoStream->WriteEP] = &VideoEndpoints[0];
	info.endpoint_desc[AudioStream->WriteEP] = &AudioEndpoints[0];
	//info.endpoint_desc[VideoStream->ReadEP] = &VideoEndpoints[1];
	//info.endpoint_desc[AudioStream->ReadEP] = &AudioEndpoints[1];

	return UsbCommsInitialize(&device_descriptor, 1, &info);
}