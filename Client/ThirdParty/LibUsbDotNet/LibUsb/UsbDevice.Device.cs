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
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LibUsbDotNet.LibUsb;

// Contains the functionality which only requires a device handle.
public partial class UsbDevice
{
    private readonly Device device;

    private UsbDeviceInfo descriptor;

    /// <summary>
    /// Gets the device descriptor for this device.
    /// </summary>
    public unsafe UsbDeviceInfo Descriptor
    {
        get
        {
            this.EnsureNotDisposed();

            if (this.descriptor == null)
            {
                DeviceDescriptor descriptor;
                NativeMethods.GetDeviceDescriptor(this.device, &descriptor).ThrowOnError();
                this.descriptor = UsbDeviceInfo.FromUsbDeviceDescriptor(this, descriptor);
            }

            return this.descriptor;
        }
    }

    /// <inheritdoc/>
    public UsbDeviceInfo Info => this.Descriptor;

    /// <inheritdoc/>
    public ushort VendorId => this.Descriptor.VendorId;

    /// <inheritdoc/>
    public ushort ProductId => this.Descriptor.ProductId;

    /// <summary>
    /// Configurations of the device.
    /// </summary>
    public ReadOnlyCollection<UsbConfigInfo> Configs
    {
        get
        {
            return this.Descriptor.Configurations;
        }
    }

    /// <summary>
    /// Gets the USB configuration descriptor for the currently active configuration.
    /// </summary>
    public unsafe UsbConfigInfo ActiveConfigDescriptor
    {
        get
        {
            this.EnsureNotDisposed();

            ConfigDescriptor* list = null;
            UsbConfigInfo value = null;

            try
            {
                NativeMethods.GetActiveConfigDescriptor(this.device, &list).ThrowOnError();
                value = UsbConfigInfo.FromUsbConfigDescriptor(this, list[0]);
                return value;
            }
            finally
            {
                if (list != null)
                {
                    NativeMethods.FreeConfigDescriptor(list);
                }
            }
        }
    }

    /// <summary>
    /// Get the number of the bus that a device is connected to.
    /// </summary>
    public byte BusNumber => NativeMethods.GetBusNumber(this.device);

    /// <summary>
    /// Gets the number of the port that a device is connected to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unless the OS does something funky, or you are hot-plugging USB extension cards, the port number returned by this
    /// call is usually guaranteed to be uniquely tied to a physical port, meaning that different devices
    /// plugged on the same physical port should return the same port number.
    /// </para>
    /// <para>
    /// But outside of this, there is no guarantee that the port number returned by this call will remain the same,
    /// or even match the order in which ports have been numbered by the HUB/HCD manufacturer.
    /// </para>
    /// </remarks>
    public byte PortNumber => NativeMethods.GetPortNumber(this.device);

    /// <summary>
    /// Gets the list of all port numbers from root for the specified device.
    /// </summary>
    public unsafe ReadOnlyCollection<byte> PortNumbers
    {
        get
        {
            Span<byte> portNumbers = stackalloc byte[8];
            int numPorts;
                
            fixed (byte* ptr = &MemoryMarshal.GetReference(portNumbers))
                numPorts = NativeMethods.GetPortNumbers(this.device, ptr, portNumbers.Length).GetValueOrThrow();

            return new ReadOnlyCollection<byte>(portNumbers[..numPorts].ToArray());
        }
    }

    /// <summary>
    /// Get the the parent from the specified device.
    /// </summary>
    /// <returns>
    /// The device parent or <see langword="null"/> if not available
    /// </returns>
    public UsbDevice GetParent()
    {
        var parent = NativeMethods.GetParent(this.device);

        if (parent == Device.Zero)
        {
            return null;
        }
        else
        {
            return new UsbDevice(parent, originatingContext);
        }
    }

    /// <summary>
    /// Gets the address of the device on the bus it is connected to.
    /// </summary>
    public byte Address
    {
        get
        {
            return NativeMethods.GetDeviceAddress(this.device);
        }
    }

    /// <summary>
    /// Get the negotiated connection speed for a device.
    /// </summary>
    public int Speed
    {
        get
        {
            return NativeMethods.GetDeviceSpeed(this.device);
        }
    }

    /// <summary>
    /// Gets the <c>wMaxPacketSize</c> value for a particular endpoint in the active device configuration.
    /// </summary>
    /// <param name="endPoint">
    /// The address of the endpoint in question
    /// </param>
    /// <returns>
    /// The <c>wMaxPacketSize</c> value
    /// </returns>
    /// <remarks>
    /// This function was originally intended to be of assistance when setting up isochronous transfers,
    /// but a design mistake resulted in this function instead. It simply returns the <c>wMaxPacketSize</c>
    /// value without considering its contents. If you're dealing with isochronous transfers, you
    /// probably want libusb_get_max_iso_packet_size() instead.
    /// </remarks>
    public int GetMaxPacketSize(byte endPoint)
    {
        return NativeMethods.GetMaxPacketSize(this.device, endPoint);
    }

    /// <summary>
    /// Calculate the maximum packet size which a specific endpoint is capable is sending or receiving
    /// in the duration of 1 microframe.
    /// </summary>
    /// <param name="endPoint">
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Only the active configuration is examined. The calculation is based on the <c>wMaxPacketSize</c> field in
    /// the endpoint descriptor as described in section 9.6.6 in the USB 2.0 specifications.
    /// </para>
    /// <para>
    /// If acting on an isochronous or interrupt endpoint, this function will multiply the value found in bits
    /// 0:10 by the number of transactions per microframe (determined by bits 11:12). Otherwise, this function
    /// just returns the numeric value found in bits 0:10.
    /// </para>
    /// <para>
    /// This function is useful for setting up isochronous transfers, for example you might pass the return value from
    /// this function to libusb_set_iso_packet_lengths() in order to set the length field of every isochronous
    /// packet in a transfer.
    /// </para>
    /// </remarks>
    public int GetMaxIsoPacketSize(byte endPoint)
    {
        return NativeMethods.GetMaxIsoPacketSize(this.device, endPoint);
    }

    /// <summary>
    /// Get a USB configuration descriptor based on its index.
    /// </summary>
    /// <param name="configIndex">
    /// The index of the configuration you wish to retrieve
    /// </param>
    /// <returns>
    /// The requested descriptor.
    /// </returns>
    public UsbConfigInfo GetConfigDescriptor(byte configIndex)
    {
        if (this.TryGetConfigDescriptor(configIndex, out UsbConfigInfo descriptor))
        {
            return descriptor;
        }
        else
        {
            throw new UsbException(Error.NotFound);
        }
    }

    /// <summary>
    /// Attempts to get a USB configuration descriptor based on its index.
    /// </summary>
    /// <param name="configIndex">
    /// The index of the configuration you wish to retrieve
    /// </param>
    /// <param name="descriptor">
    /// The requested descriptor.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the descriptor could be loaded correctly; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public unsafe bool TryGetConfigDescriptor(byte configIndex, out UsbConfigInfo descriptor)
    {
        this.EnsureNotDisposed();

        ConfigDescriptor* list = null;
        UsbConfigInfo value = null;

        try
        {
            var ret = NativeMethods.GetConfigDescriptor(this.device, configIndex, &list);

            if (ret == Error.NotFound)
            {
                descriptor = null;
                return false;
            }

            ret.ThrowOnError();

            value = UsbConfigInfo.FromUsbConfigDescriptor(this, list[0]);
            descriptor = value;
            return true;
        }
        finally
        {
            if (list != null)
            {
                NativeMethods.FreeConfigDescriptor(list);
            }
        }
    }
}