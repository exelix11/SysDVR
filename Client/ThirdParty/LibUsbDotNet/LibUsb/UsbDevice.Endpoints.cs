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

namespace LibUsbDotNet.LibUsb;

// Implements functionality for the UsbDevice class related to endpoints.
public partial class UsbDevice
{
    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    public UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID)
    {
        return this.OpenEndpointReader(readEndpointID, UsbEndpointReader.DefReadBufferSize);
    }

    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <param name="readBufferSize">TODO: Remove this parameter</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    public UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize)
    {
        return this.OpenEndpointReader(readEndpointID, readBufferSize, EndpointType.Bulk);
    }

    /// <summary>
    /// Opens an endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <param name="readBufferSize">TODO: Remove this parameter.</param>
    /// <param name="endpointType">The type of endpoint to open.</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    public UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize, EndpointType endpointType)
    {
        EnsureNotDisposed();
        byte altIntefaceID = this.mClaimedInterfaces.Count == 0 ? this.usbAltInterfaceSettings[0] : this.usbAltInterfaceSettings[this.mClaimedInterfaces[this.mClaimedInterfaces.Count - 1]];

        return new UsbEndpointReader(this, readBufferSize, altIntefaceID, readEndpointID, endpointType);
    }

    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for writing
    /// </summary>
    /// <param name="writeEndpointID">Endpoint number for read operations.</param>
    /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
    public UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID)
    {
        return this.OpenEndpointWriter(writeEndpointID, EndpointType.Bulk);
    }

    /// <summary>
    /// Opens an endpoint for writing
    /// </summary>
    /// <param name="writeEndpointID">Endpoint number for read operations.</param>
    /// <param name="endpointType">The type of endpoint to open.</param>
    /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
    public UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID, EndpointType endpointType)
    {
        EnsureNotDisposed();
        byte altIntefaceID = this.mClaimedInterfaces.Count == 0 ? this.usbAltInterfaceSettings[0] : this.usbAltInterfaceSettings[this.mClaimedInterfaces[this.mClaimedInterfaces.Count - 1]];

        return new UsbEndpointWriter(this, altIntefaceID, writeEndpointID, endpointType);
    }
}