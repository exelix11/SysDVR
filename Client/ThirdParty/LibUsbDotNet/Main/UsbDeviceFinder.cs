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

using LibUsbDotNet.LibUsb;
using System;
using System.Runtime.Serialization;

namespace LibUsbDotNet.Main;

/// <summary>
/// Finds and identifies usb devices. Used for easily locating
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>
/// Pass instances of this class into the
/// <see cref="UsbContext.Find(UsbDeviceFinder)"/> or
/// <see cref="UsbContext.FindAll(UsbDeviceFinder)"/>
/// functions of a  <see cref="UsbContext"/>
/// instance to find connected usb devices without opening devices or interrogating the bus.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code source="../../Examples/Show.Info/ShowInfo.cs" lang="cs"/>
/// </example>
public class UsbDeviceFinder : ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class for locating and identifying usb devices.
    /// </summary>
    /// <param name="vid">The vendor id of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
    /// <param name="pid">The product id of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
    /// <param name="revision">The revision number of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
    /// <param name="serialNumber">The serial number of the usb device to find, or null to ignore.</param>
    /// <param name="deviceInterfaceGuid">The unique guid of the usb device to find, or <see cref="Guid.Empty"/> to ignore.</param>
    public UsbDeviceFinder(int vid, int pid, int revision, string serialNumber, Guid deviceInterfaceGuid)
    {
        this.Vid = vid;
        this.Pid = pid;
        this.Revision = revision;
        this.SerialNumber = serialNumber;
        this.DeviceInterfaceGuid = deviceInterfaceGuid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class for locating usb devices by VendorID, ProductID, and Serial number.
    /// </summary>
    /// <param name="vid">The vendor id of the usb device to find.</param>
    /// <param name="pid">The product id of the usb device to find.</param>
    /// <param name="serialNumber">The serial number of the usb device to find.</param>
    public UsbDeviceFinder(int vid, int pid, string serialNumber)
        : this(vid, pid, int.MaxValue, serialNumber, Guid.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class for locating usb devices by VendorID, ProuctID, and Revision code.
    /// </summary>
    /// <param name="vid">The vendor id of the usb device to find.</param>
    /// <param name="pid">The product id of the usb device to find.</param>
    /// <param name="revision">The revision number of the usb device to find.</param>
    public UsbDeviceFinder(int vid, int pid, int revision)
        : this(vid, pid, revision, null, Guid.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class for locating usb devices vendor and product ID.
    /// </summary>
    /// <param name="vid">The vendor id of the usb device to find.</param>
    /// <param name="pid">The product id of the usb device to find.</param>
    public UsbDeviceFinder(int vid, int pid)
        : this(vid, pid, int.MaxValue, null, Guid.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class for locating usb devices.
    /// </summary>
    /// <param name="vid">The vendor id of the usb device to find.</param>
    public UsbDeviceFinder(int vid)
        : this(vid, int.MaxValue, int.MaxValue, null, Guid.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class  for locating usb devices by a serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number of the usb device to find.</param>
    public UsbDeviceFinder(string serialNumber)
        : this(int.MaxValue, int.MaxValue, int.MaxValue, serialNumber, Guid.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class s for locating usb devices by a unique <see cref="Guid"/> string.
    /// </summary>
    /// <param name="deviceInterfaceGuid">The unique <see cref="Guid"/> to find.</param>
    public UsbDeviceFinder(Guid deviceInterfaceGuid)
        : this(int.MaxValue, int.MaxValue, int.MaxValue, null, deviceInterfaceGuid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class using a serialization stream to fill the <see cref="UsbDeviceFinder"/> class.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected UsbDeviceFinder(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException("info");
        }

        this.Vid = (int)info.GetValue("Vid", typeof(int));
        this.Pid = (int)info.GetValue("Pid", typeof(int));
        this.Revision = (int)info.GetValue("Revision", typeof(int));
        this.SerialNumber = (string)info.GetValue("SerialNumber", typeof(string));
        this.DeviceInterfaceGuid = (Guid)info.GetValue("DeviceInterfaceGuid", typeof(Guid));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceFinder"/> class.
    /// </summary>
    protected UsbDeviceFinder()
    {
    }

    /// <summary>
    /// Gets the device interface guid string to find, or <see cref="string.Empty"/> to ignore.
    /// </summary>
    public Guid? DeviceInterfaceGuid
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the serial number of the device to find.
    /// </summary>
    /// <remarks>
    /// Set to null to ignore.
    /// </remarks>
    public string SerialNumber
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the revision number of the device to find.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="int.MaxValue"/> to ignore.
    /// </remarks>
    public int? Revision
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the product id of the device to find.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="int.MaxValue"/> to ignore.
    /// </remarks>
    public int? Pid
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the vendor id of the device to find.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="int.MaxValue"/> to ignore.
    /// </remarks>
    public int? Vid
    {
        get;
        private set;
    }

    /// <inheritdoc/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException("info");
        }

        info.AddValue("Vid", this.Vid);
        info.AddValue("Pid", this.Pid);
        info.AddValue("Revision", this.Revision);
        info.AddValue("SerialNumber", this.SerialNumber);
        info.AddValue("DeviceInterfaceGuid", this.DeviceInterfaceGuid);
    }

    /// <summary>
    /// Dynamic predicate find function. Pass this function into any method that has a <see cref="Predicate{UsbDevice}"/> parameter.
    /// </summary>
    /// <remarks>
    /// Override this member when inheriting the <see cref="UsbDeviceFinder"/> class to change/alter the matching behavior.
    /// </remarks>
    /// <param name="usbDevice">The UsbDevice to check.</param>
    /// <returns>True if the <see cref="UsbDevice"/> instance matches the <see cref="UsbDeviceFinder"/> properties.</returns>
    public virtual bool Check(IUsbDevice usbDevice)
    {
        try
        {
            if (this.Vid != null && 
                this.Vid != int.MaxValue &&
                this.Vid.Value != usbDevice.Info.VendorId)
            {
                return false;
            }

            if (this.Pid != null && 
                this.Pid != int.MaxValue &&
                this.Pid.Value != usbDevice.Info.ProductId)
            {
                return false;
            }

            if (this.Revision != null && 
                this.Revision != int.MaxValue && 
                this.Revision.Value != usbDevice.Info.Usb)
            {
                return false;
            }

            if (this.SerialNumber != null &&
                this.SerialNumber != usbDevice.Info.SerialNumber)
            {
                return false;
            }

            return true;
        }
        catch (LibUsb.UsbException ex) when (ex.ErrorCode == Error.NotFound)
        {
            // The device has probably disconnected while we were inspecting it. Continue.
            return false;
        }
    }
}