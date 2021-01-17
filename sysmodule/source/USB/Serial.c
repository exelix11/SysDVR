#include <string.h>
#include "Serial.h"

static struct usb_interface_descriptor Viface;
static struct usb_interface_descriptor Aiface;

static struct usb_endpoint_descriptor VInt;
static struct usb_endpoint_descriptor VBulk;

static struct usb_endpoint_descriptor AInt;
static struct usb_endpoint_descriptor ABulk;

void ClearState()
{
#define clearVal(x) memset(&x, 0, sizeof(x))
	clearVal(Viface);
	clearVal(Aiface);
	clearVal(VInt);
	clearVal(VBulk);
	clearVal(AInt);
	clearVal(ABulk);
#undef clearVal
}

void UsbSerialExit()
{
	UsbCommsExit();
	ClearState();
}

Result UsbSerialInitializeForStreaming(UsbInterface* video, UsbInterface* audio)
{
	ClearState();

	struct usb_device_descriptor device_descriptor = {
		.bLength = USB_DT_DEVICE_SIZE,
		.bDescriptorType = USB_DT_DEVICE,
		.bcdUSB = 0x0200,
		.bDeviceClass = 0,
		.bDeviceSubClass = 0,
		.bDeviceProtocol = 0,
		.bMaxPacketSize0 = 0x40,
		.idVendor = 0x057e,
		.idProduct = 0x3006,
		.bcdDevice = 0x0100,
		.bNumConfigurations = 0x01
	};

	Aiface = Viface = (struct usb_interface_descriptor){
		.bLength = USB_DT_INTERFACE_SIZE,
		.bDescriptorType = USB_DT_INTERFACE,
		.bInterfaceNumber = 0,
		.bNumEndpoints = 2,
		.bInterfaceClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceSubClass = USB_CLASS_VENDOR_SPEC,
		.bInterfaceProtocol = USB_CLASS_VENDOR_SPEC,
		.iInterface = 0,
	};

	Aiface.bInterfaceNumber = 1;

	AInt = VInt = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_OUT,
		.bmAttributes = USB_TRANSFER_TYPE_INTERRUPT,
		.wMaxPacketSize = 4,
		.bInterval = 1
	};

	ABulk = VBulk = (struct usb_endpoint_descriptor){
		.bLength = USB_DT_ENDPOINT_SIZE,
		.bDescriptorType = USB_DT_ENDPOINT,
		.bEndpointAddress = USB_ENDPOINT_IN,
		.bmAttributes = USB_TRANSFER_TYPE_BULK,
		.wMaxPacketSize = 0x200,
		.bInterval = 1 //Max nak rate
	};

	VInt.bEndpointAddress |= 1;
	VBulk.bEndpointAddress |= 1;

	AInt.bEndpointAddress |= 2;
	ABulk.bEndpointAddress |= 2;

	UsbInterfaceDesc iface[2] = {
		{
			.interface_desc = &Viface,
			.endpoint_in = &VBulk,
			.endpoint_out = &VInt,
			.string_descriptor = "SysDVR - Video"
		},
		{
			.interface_desc = &Aiface,
			.endpoint_in = &ABulk,
			.endpoint_out = &AInt,
			.string_descriptor = "SysDVR - Audio"
		}
	};

	Result rc = UsbCommsInitialize(&device_descriptor, 2, iface);
	if (R_FAILED(rc))
		return rc;

	*video = Viface.bInterfaceNumber;
	*audio = Aiface.bInterfaceNumber;

	return 0;
}