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

using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Endpoint members common to Read, Write, Bulk, and Interrupt <see cref="T:LibUsbDotNet.Main.EndpointType"/>.
/// </summary>
public abstract class UsbEndpointBase
{
    private readonly byte mEpNum;
    private readonly IUsbDevice mUsbDevice;
    private readonly byte alternateInterfaceID;
    private UsbEndpointInfo mUsbEndpointInfo;
    private EndpointType mEndpointType;
    private UsbInterfaceInfo mUsbInterfacetInfo;

    internal UsbEndpointBase(IUsbDevice usbDevice, byte alternateInterfaceID, byte epNum, EndpointType endpointType)
    {
        this.mUsbDevice = usbDevice;
        this.alternateInterfaceID = alternateInterfaceID;
        this.mEpNum = epNum;
        this.mEndpointType = endpointType;
    }

    /// <summary>
    /// Gets the <see cref="UsbDevice"/> class this endpoint belongs to.
    /// </summary>
    public IUsbDevice Device
    {
        get { return this.mUsbDevice; }
    }

    /// <summary>
    /// Gets the endpoint ID for this <see cref="UsbEndpointBase"/> class.
    /// </summary>
    public byte EpNum
    {
        get
        {
            return this.mEpNum;
        }
    }

    /// <summary>
    /// Gets the <see cref="EndpointType"/> for this endpoint.
    /// </summary>
    public EndpointType Type
    {
        get { return this.mEndpointType; }
    }

    /// <summary>
    /// Gets the <see cref="UsbEndpointInfo"/> descriptor for this endpoint.
    /// </summary>
    public UsbEndpointInfo EndpointInfo
    {
        get
        {
            if (ReferenceEquals(this.mUsbEndpointInfo, null))
            {
                if (!LookupEndpointInfo(this.Device.Configs[0], this.alternateInterfaceID, this.mEpNum, out this.mUsbInterfacetInfo, out this.mUsbEndpointInfo))
                {
                    // throw new UsbException(this, String.Format("Failed locating endpoint {0} for the current usb configuration.", mEpNum));
                    return null;
                }
            }

            return this.mUsbEndpointInfo;
        }
    }

    /// <summary>
    /// Synchronous bulk/interrupt transfer function.
    /// </summary>
    /// <param name="buffer">An <see cref="IntPtr"/> to a caller-allocated buffer.</param>
    /// <param name="offset">Position in buffer that transferring begins.</param>
    /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>True on success.</returns>
    public virtual unsafe Error Transfer(Span<byte> buffer, int offset, int length, int timeout, out int transferLength)
    {
        int transferred = 0;

        if (this.Device == null)
            throw new Exception("Device is null");

        if (this.Device.DeviceHandle == null)
            throw new Exception("DeviceHandle is null");

        if (this.Device.DeviceHandle.DangerousGetHandle() == IntPtr.Zero)
            throw new Exception("DeviceHandle handle is null");

        Error returnValue;
        switch (this.mEndpointType)
        {
            case EndpointType.Bulk:
                fixed (byte* bufferPtr = &MemoryMarshal.GetReference(buffer))
                    returnValue = NativeMethods.BulkTransfer(this.Device.DeviceHandle, this.mEpNum, bufferPtr + offset, length, ref transferred, (uint)timeout);
                transferLength = transferred;
                return returnValue;

            case EndpointType.Interrupt:
                fixed (byte* bufferPtr = &MemoryMarshal.GetReference(buffer))
                    returnValue = NativeMethods.InterruptTransfer(this.Device.DeviceHandle, this.mEpNum, bufferPtr + offset, length, ref transferred, (uint)timeout);
                transferLength = transferred;
                return returnValue;

            case EndpointType.Isochronous:
                throw new NotSupportedException($"{EndpointType.Isochronous} not supported yet.");
            case EndpointType.Control:
                throw new NotSupportedException(
                    $"Do not use {nameof(Transfer)} for synchronous control transfers, use {nameof(UsbDevice.ControlTransfer)}");
            default:
                throw new ArgumentOutOfRangeException(nameof(mEndpointType), $"Not an {typeof(EndpointType)}");
        }
    }
        
    /// <summary>
    /// Asynchronous bulk/interrupt transfer function.
    /// </summary>
    /// <param name="buffer">Caller-allocated buffer.</param>
    /// <param name="offset">Position in buffer that transferring begins.</param>
    /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
    /// <returns>Named tuple of <see cref="Error"/> and transferLength</returns>
    protected Task<(Error error, int transferLength)> TransferAsync(Memory<byte> buffer, int offset, int length, int timeout) => 
        AsyncTransfer.TransferAsync(this.Device.DeviceHandle, this.mEpNum, this.mEndpointType, buffer, offset, length, timeout);

    /// <summary>
    /// Looks up endpoint/interface information in a configuration.
    /// </summary>
    /// <param name="currentConfigInfo">The config to seach.</param>
    /// <param name="altInterfaceID">Alternate interface id the endpoint exists in, or -1 for any alternate interface id.</param>
    /// <param name="endpointAddress">The endpoint address to look for.</param>
    /// <param name="usbInterfaceInfo">On success, the <see cref="UsbInterfaceInfo"/> class for this endpoint.</param>
    /// <param name="usbEndpointInfo">On success, the <see cref="UsbEndpointInfo"/> class for this endpoint.</param>
    /// <returns>True of the endpoint was found, otherwise false.</returns>
    public static bool LookupEndpointInfo(UsbConfigInfo currentConfigInfo, int altInterfaceID, byte endpointAddress, out UsbInterfaceInfo usbInterfaceInfo, out UsbEndpointInfo usbEndpointInfo)
    {
        bool found = false;

        usbInterfaceInfo = null;
        usbEndpointInfo = null;
        foreach (UsbInterfaceInfo interfaceInfo in currentConfigInfo.Interfaces)
        {
            if (altInterfaceID == -1 || altInterfaceID == interfaceInfo.AlternateSetting)
            {
                foreach (UsbEndpointInfo endpointInfo in interfaceInfo.Endpoints)
                {
                    if ((endpointAddress & UsbConstants.EndpointNumberMask) == 0)
                    {
                        // find first read/write endpoint
                        if ((endpointAddress & UsbConstants.EndpointDirectionMask) == 0 &&
                            (endpointInfo.EndpointAddress & UsbConstants.EndpointDirectionMask) == 0)
                        {
                            // first write endpoint
                            found = true;
                        }

                        if ((endpointAddress & UsbConstants.EndpointDirectionMask) != 0 &&
                            (endpointInfo.EndpointAddress & UsbConstants.EndpointDirectionMask) != 0)
                        {
                            // first read endpoint
                            found = true;
                        }
                    }
                    else if (endpointInfo.EndpointAddress == endpointAddress)
                    {
                        found = true;
                    }

                    if (found)
                    {
                        usbInterfaceInfo = interfaceInfo;
                        usbEndpointInfo = endpointInfo;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Looks up endpoint/interface information in a configuration.
    /// </summary>
    /// <param name="currentConfigInfo">The config to seach.</param>
    /// <param name="endpointAddress">The endpoint address to look for.</param>
    /// <param name="usbInterfaceInfo">On success, the <see cref="UsbInterfaceInfo"/> class for this endpoint.</param>
    /// <param name="usbEndpointInfo">On success, the <see cref="UsbEndpointInfo"/> class for this endpoint.</param>
    /// <returns>True of the endpoint was found, otherwise false.</returns>
    public static bool LookupEndpointInfo(UsbConfigInfo currentConfigInfo, byte endpointAddress, out UsbInterfaceInfo usbInterfaceInfo, out UsbEndpointInfo usbEndpointInfo)
    {
        return LookupEndpointInfo(currentConfigInfo, -1, endpointAddress, out usbInterfaceInfo, out usbEndpointInfo);
    }
}
