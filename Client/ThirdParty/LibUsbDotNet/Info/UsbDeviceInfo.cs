// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// Copyright © 2011-2023 LibUsbDotNet contributors. All rights reserved.
// 
// website: http://github.com/libusbdotnet/libusbdotnet
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
//

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LibUsbDotNet.Info;

/// <summary> Contains USB device descriptor information.
/// </summary>
public class UsbDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceInfo"/> class.
    /// </summary>
    protected UsbDeviceInfo()
    {
    }

    private readonly Collection<UsbConfigInfo> configurations = new Collection<UsbConfigInfo>();

    /// <summary>
    /// Create a <see cref="UsbDeviceInfo"/> from a <see cref="DeviceDescriptor"/>.
    /// </summary>
    /// <param name="device">Device the descriptor came from.</param>
    /// <param name="descriptor">Descriptor struct.</param>
    /// <returns>Resulting <see cref="UsbDeviceInfo"/>.</returns>
    public static UsbDeviceInfo FromUsbDeviceDescriptor(LibUsb.IUsbDevice device, DeviceDescriptor descriptor)
    {
        Debug.Assert(descriptor.DescriptorType == (int)DescriptorType.Device, "A config descriptor was expected");

        var value = new UsbDeviceInfo
        {
            Device = descriptor.Device,
            DeviceClass = descriptor.DeviceClass,
            DeviceProtocol = descriptor.DeviceProtocol,
            DeviceSubClass = descriptor.DeviceSubClass,
            ProductId = descriptor.IdProduct,
            VendorId = descriptor.IdVendor,
            Manufacturer = device.GetStringDescriptor(descriptor.Manufacturer, failSilently: true),
            MaxPacketSize0 = descriptor.MaxPacketSize0,
            NumConfigurations = descriptor.NumConfigurations,
            Product = device.GetStringDescriptor(descriptor.Product, failSilently: true),
            SerialNumber = device.GetStringDescriptor(descriptor.SerialNumber, failSilently: true),
            Usb = descriptor.USB
        };

        for (byte i = 0; i < descriptor.NumConfigurations; i++)
        {
            if (device.TryGetConfigDescriptor(i, out var configDescriptor))
            {
                value.configurations.Add(configDescriptor);
            }
        }
            
        return value;
    }

    /// <summary>
    /// Device release number in binary-coded decimal.
    /// </summary>
    public virtual ushort Device { get; protected set; }

    /// <summary>
    /// USB-IF <see cref="ClassCode"/> for the device.
    /// </summary>
    public virtual byte DeviceClass { get; protected set; }

    /// <summary>
    /// USB-IF protocol code for the device, qualified by the <see cref="DeviceClass"/> and <see cref="DeviceSubClass"/> values.
    /// </summary>
    public virtual byte DeviceProtocol { get; protected set; }

    /// <summary>
    /// USB-IF subclass code for the device, qualified by the <see cref="DeviceClass"/> value.
    /// </summary>
    public virtual byte DeviceSubClass { get; protected set; }

    /// <summary>
    /// USB-IF product ID.
    /// </summary>
    public virtual ushort ProductId { get; protected set; }

    /// <summary>
    /// USB-IF vendor ID.
    /// </summary>
    public virtual ushort VendorId { get; protected set; }

    /// <summary>
    /// String descriptor describing manufacturer.
    /// </summary>
    /// <remarks>
    /// Device must be opened to retrieve this value.
    /// </remarks>
    public virtual string Manufacturer { get; protected set; }

    /// <summary>
    /// Maximum packet size for endpoint 0.
    /// </summary>
    public virtual byte MaxPacketSize0 { get; protected set; }

    /// <summary>
    /// Number of possible configurations.
    /// </summary>
    public virtual byte NumConfigurations { get; protected set; }

    /// <summary>
    /// String descriptor describing product.
    /// </summary>
    /// <remarks>
    /// Device must be opened to retrieve this value.
    /// </remarks>
    public virtual string Product { get; protected set; }

    /// <summary>
    /// Device serial number.
    /// </summary>
    /// <remarks>
    /// Device must be opened to retrieve this value.
    /// </remarks>
    public virtual string SerialNumber { get; protected set; } = string.Empty;

    /// <summary>
    /// USB specification release number in binary-coded decimal.
    /// </summary>
    /// <remarks>
    /// A value of 0x0200 indicates USB 2.0, 0x0110 indicates USB 1.1, etc.
    /// </remarks>
    public virtual ushort Usb { get; protected set; }

    /// <summary>
    /// Collection of all configurations available on this device.
    /// </summary>
    public virtual ReadOnlyCollection<UsbConfigInfo> Configurations => new(this.configurations);

    /// <inheritdoc />
    public override string ToString() =>
        $"Device: 0x{Device:X4}\n" +
        $"DeviceClass: {DeviceClass}\n" +
        $"DeviceSubClass: 0x{DeviceSubClass:X2}\n" +
        $"VendorId: 0x{VendorId:X4}\n" +
        $"ProductId: 0x{ProductId:X4}\n" +
        $"Manufacturer: {Manufacturer}\n" +
        $"Product: {Product}\n" +
        $"SerialNumber: {SerialNumber}\n" +
        $"USB: 0x{Usb:X4}\n" +
        $"MaxPacketSize: {MaxPacketSize0}\n" +
        $"NumConfigurations: {NumConfigurations}";
}