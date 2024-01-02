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
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Contains methods for retrieving data from a <see cref="EndpointType.Bulk"/> or <see cref="EndpointType.Interrupt"/> endpoint using the overloaded <see cref="Read(byte[],int,out int)"/> functions.
/// </summary>
public partial class UsbEndpointReader : UsbEndpointBase
{
    private int mReadBufferSize;

    /// <summary>
    /// Class for reading data from a USB endpoint.
    /// </summary>
    /// <param name="usbDevice">Device the endpoint belongs to.</param>
    /// <param name="readBufferSize">TODO: Remove this parameter.</param>
    /// <param name="alternateInterfaceID">The alternate interface to read from.</param>
    /// <param name="readEndpointID">The endpoint id to read from.</param>
    /// <param name="endpointType">The <see cref="EndpointType"/> of the endpoint.</param>
    public UsbEndpointReader(IUsbDevice usbDevice, int readBufferSize, byte alternateInterfaceID, ReadEndpointID readEndpointID, EndpointType endpointType)
        : base(usbDevice, alternateInterfaceID, (byte)readEndpointID, endpointType)
    {
        this.mReadBufferSize = readBufferSize;
    }

    /// <summary>
    /// TODO: Remove this property.
    /// </summary>
    /// <remarks>
    /// This value can be bypassed using the second parameter of the <see cref="UsbDevice.OpenEndpointReader(LibUsbDotNet.Main.ReadEndpointID,int)"/> method.
    /// The default is 4096.
    /// </remarks>
    public static int DefReadBufferSize { get; set; } = 4096;

    /// <summary>
    /// Reads data from the current <see cref="UsbEndpointReader"/>.
    /// </summary>
    /// <param name="buffer">The buffer to store the received data in.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Read(byte[] buffer, int timeout, out int transferLength) 
        => Read(buffer, 0, buffer.Length, timeout, out transferLength);

    /// <summary>
    /// Reads data from the current <see cref="UsbEndpointReader"/>.
    /// </summary>
    /// <param name="buffer">The buffer to store the received data in.</param>
    /// <param name="offset">The position in buffer to start storing the data.</param>
    /// <param name="count">The maximum number of bytes to receive.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Read(byte[] buffer, int offset, int count, int timeout, out int transferLength) 
        => Transfer(buffer, offset, count, timeout, out transferLength);

    /// <summary>
    /// Reads data from the current <see cref="UsbEndpointReader"/>.
    /// </summary>
    /// <param name="buffer">The buffer to store the received data in.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Read(Span<byte> buffer, int timeout, out int transferLength) 
        => Read(buffer, 0, buffer.Length, timeout, out transferLength);

    /// <summary>
    /// Reads data from the current <see cref="UsbEndpointReader"/>.
    /// </summary>
    /// <param name="buffer">The buffer to store the received data in.</param>
    /// <param name="offset">The position in buffer to start storing the data.</param>
    /// <param name="count">The maximum number of bytes to receive.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Read(Span<byte> buffer, int offset, int count, int timeout, out int transferLength) 
        => Transfer(buffer, offset, count, timeout, out transferLength);

    /// <summary>
    /// Reads/discards data from the endpoint until no more data is available.
    /// </summary>
    /// <returns>Always returns <see cref="Error.Success"/> </returns>
    public virtual Error ReadFlush()
    {
        byte[] bufDummy = new byte[64];

        int iBufCount = 0;
        while (this.Read(bufDummy, 10, out _) == Error.Success && iBufCount < 128)
        {
            iBufCount++;
        }

        return Error.Success;
    }
}