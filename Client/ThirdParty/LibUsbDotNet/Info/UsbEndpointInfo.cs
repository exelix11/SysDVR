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
using System.Diagnostics;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.Info;

/// <summary> Contains Endpoint information for the current <see cref="T:LibUsbDotNet.Info.UsbConfigInfo"/>.
/// </summary>
public class UsbEndpointInfo : UsbBaseInfo
{
    /// <summary>
    /// Get a <see cref="UsbEndpointInfo"/> from a <see cref="EndpointDescriptor"/> struct.
    /// </summary>
    /// <param name="descriptor"><see cref="EndpointDescriptor"/> struct.</param>
    /// <returns><see cref="UsbEndpointInfo"/> result.</returns>
    public static unsafe UsbEndpointInfo FromUsbEndpointDescriptor(EndpointDescriptor descriptor)
    {
        Debug.Assert(descriptor.DescriptorType == (int)DescriptorType.Endpoint, "An endpoint descriptor was expected");

        var value = new UsbEndpointInfo
        {
            Attributes = descriptor.Attributes,
            EndpointAddress = descriptor.EndpointAddress,
            RawDescriptors = new byte[descriptor.ExtraLength],
            Interval = descriptor.Interval,
            MaxPacketSize = descriptor.MaxPacketSize,
            Refresh = descriptor.Refresh,
            SyncAddress = descriptor.SynchAddress
        };

        if (descriptor.ExtraLength > 0)
        {
            Span<byte> extra = new Span<byte>(descriptor.Extra, descriptor.ExtraLength);
            extra.CopyTo(value.RawDescriptors);
        }

        return value;
    }

    /// <summary>
    /// Attributes which apply to the endpoint when it is configured using the bConfigurationValue.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Bits 0:1 determine the transfer type and correspond to <see cref="EndpointType"/>.</item>
    /// <item>Bits 2:3 are only used for isochronous endpoints and correspond to <see cref="IsoSyncType"/>.</item>
    /// <item>Bits 4:5 are also only used for isochronous endpoints and correspond to <see cref="IsoUsageType"/>.</item>
    /// <item>Bits 6:7 are reserved.</item>
    /// </list>
    /// </remarks>
    public virtual byte Attributes { get; private set; }

    /// <summary>
    /// The address of the endpoint described by this descriptor.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Bits 0:3 are the endpoint number.</item>
    /// <item>Bits 4:6 are reserved.</item>
    /// <item>Bit 7 indicates direction, see <see cref="EndpointDirection"/>.</item>
    /// </list>
    /// </remarks>
    public virtual byte EndpointAddress { get; private set; }

    /// <summary>
    /// Interval for polling endpoint for data transfers.
    /// </summary>
    public virtual byte Interval { get; private set; }

    /// <summary>
    /// Maximum packet size this endpoint is capable of sending/receiving.
    /// </summary>
    public virtual ushort MaxPacketSize { get; private set; }

    /// <summary>
    /// For audio devices only: the rate at which synchronization feedback is provided.
    /// </summary>
    public virtual byte Refresh { get; private set; }

    /// <summary>
    /// For audio devices only: the address if the synch endpoint.
    /// </summary>
    public virtual byte SyncAddress { get; private set; }

    /// <inheritdoc />
    public override string ToString() =>
        $"Address: 0x{EndpointAddress:X2}\n" +
        $"Interval: {Interval}\n" +
        $"MaxPacketSize: {MaxPacketSize}\n" +
        $"Refresh: {Refresh}\n" +
        $"SyncAddress: 0x{SyncAddress:X2}";
}