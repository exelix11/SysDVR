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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LibUsbDotNet.Info;

/// <summary> Describes a USB device interface.
/// </summary>
public class UsbInterfaceInfo : UsbBaseInfo
{
    private List<UsbEndpointInfo> endpoints = new List<UsbEndpointInfo>();

    /// <summary>
    /// Get a collection of <see cref="UsbInterfaceInfo"/> from a <see cref="Interface"/> struct.
    /// </summary>
    /// <param name="device">The device the interface came from.</param>
    /// <param name="interface">The <see cref="Interface"/> struct.</param>
    /// <returns>The <see cref="UsbInterfaceInfo"/> collection.</returns>
    public static unsafe Collection<UsbInterfaceInfo> FromUsbInterface(LibUsb.UsbDevice device, Interface @interface)
    {
        var interfaces = (InterfaceDescriptor*)@interface.Altsetting;
        Collection<UsbInterfaceInfo> value = new Collection<UsbInterfaceInfo>();

        for (int i = 0; i < @interface.NumAltsetting; i++)
        {
            value.Add(FromUsbInterfaceDescriptor(device, interfaces[i]));
        }

        return value;
    }

    /// <summary>
    /// Get a <see cref="UsbInterfaceInfo"/> from a <see cref="InterfaceDescriptor"/> struct.
    /// </summary>
    /// <param name="device">The device the interface came from.</param>
    /// <param name="descriptor">The <see cref="InterfaceDescriptor"/> struct.</param>
    /// <returns>The <see cref="UsbInterfaceInfo"/>.</returns>
    public static unsafe UsbInterfaceInfo FromUsbInterfaceDescriptor(LibUsb.UsbDevice device, InterfaceDescriptor descriptor)
    {
        Debug.Assert(descriptor.DescriptorType == (int)DescriptorType.Interface, "A config descriptor was expected");

        var value = new UsbInterfaceInfo
        {
            AlternateSetting = descriptor.AlternateSetting,
            Interface = device.GetStringDescriptor(descriptor.Interface, failSilently: true),
            Class = (ClassCode)descriptor.InterfaceClass,
            Number = descriptor.InterfaceNumber,
            Protocol = descriptor.InterfaceProtocol,
            SubClass = descriptor.InterfaceSubClass,
            RawDescriptors = new byte[descriptor.ExtraLength]
        };
            
        var endpoints = descriptor.Endpoint;

        for (int i = 0; i < descriptor.NumEndpoints; i++)
        {
            if (endpoints[i].DescriptorType != 0)
            {
                value.endpoints.Add(UsbEndpointInfo.FromUsbEndpointDescriptor(endpoints[i]));
            }
        }
            
        if (descriptor.ExtraLength > 0)
        {
            Span<byte> extra = new Span<byte>(descriptor.Extra, descriptor.ExtraLength);
            extra.CopyTo(value.RawDescriptors);
        }

        return value;
    }

    /// <summary>
    /// Value used to select this alternate setting for this interface.
    /// </summary>
    public virtual byte AlternateSetting { get; private set; }

    /// <summary>
    /// USB-IF <see cref="ClassCode"/> for this interface.
    /// </summary>
    public virtual ClassCode Class { get; private set; }

    /// <summary>
    /// Number of this interface.
    /// </summary>
    public virtual int Number { get; private set; }

    /// <summary>
    /// USB-IF protocol code for this interface, qualified by the <see cref="Class"/> and <see cref="SubClass"/> values.
    /// </summary>
    public virtual byte Protocol { get; private set; }

    /// <summary>
    /// String descriptor describing this interface.
    /// </summary>
    public virtual string Interface { get; private set; }

    /// <summary>
    /// USB-IF subclass code for this interface, qualified by the <see cref="Class"/> value.
    /// </summary>
    public virtual byte SubClass { get; private set; }

    /// <summary>
    /// Gets the collection of endpoint descriptors associated with this interface.
    /// </summary>
    public virtual ReadOnlyCollection<UsbEndpointInfo> Endpoints
    {
        get { return this.endpoints.AsReadOnly(); }
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"Interface: {Interface}\n" +
        $"InterfaceId: {Number}\n" +
        $"AlternateId: {AlternateSetting}\n" +
        $"Class: {Class}\n" +
        $"Protocol: 0x{Protocol:X2}\n" +
        $"SubClass: 0x{SubClass:X2}";
}