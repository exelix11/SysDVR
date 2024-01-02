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

using LibUsbDotNet.Main;
using System;

namespace LibUsbDotNet.LibUsb;

/// <summary>Contains methods for writing data to a <see cref="EndpointType.Bulk"/> or <see cref="EndpointType.Interrupt"/> endpoint using the overloaded <see cref="Write(byte[],int,out int)"/> functions.
/// </summary>
public partial class UsbEndpointWriter : UsbEndpointBase
{
    public UsbEndpointWriter(IUsbDevice usbDevice, byte alternateInterfaceID, WriteEndpointID writeEndpointID, EndpointType endpointType)
        : base(usbDevice, alternateInterfaceID, (byte)writeEndpointID, endpointType)
    {
    }

    /// <summary>
    /// Writes data to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Write(byte[] buffer, int timeout, out int transferLength) 
        => Write(buffer, 0, buffer.Length, timeout, out transferLength);

    /// <summary>
    /// Writes data to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="offset">The position in buffer to start writing the data from.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Write(byte[] buffer, int offset, int count, int timeout, out int transferLength) => 
        Transfer(buffer, offset, count, timeout, out transferLength);

    /// <summary>
    /// Writes data to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Write(Span<byte> buffer, int timeout, out int transferLength) => 
        Write(buffer, 0, buffer.Length, timeout, out transferLength);

    /// <summary>
    /// Writes data to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="offset">The position in buffer to start writing the data from.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <param name="transferLength">Number of bytes actually transferred.</param>
    /// <returns>
    /// <see cref="Error"/>.<see cref="Error.Success"/> on success.
    /// </returns>
    public virtual Error Write(Span<byte> buffer, int offset, int count, int timeout, out int transferLength) => 
        Transfer(buffer, offset, count, timeout, out transferLength);
}