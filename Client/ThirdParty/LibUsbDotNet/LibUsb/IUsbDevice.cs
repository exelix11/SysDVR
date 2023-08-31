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
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// The <see cref="IUsbDevice"/> interface contains members needed to configure a USB device for use.
/// </summary>
/// <example>
/// This example uses the <see cref="IUsbDevice"/> interface to select the desired configuration and interface
/// for usb devices that require it.
/// <code source="../../Examples/Read.Write/ReadWrite.cs" lang="cs"/>
/// </example>
public interface IUsbDevice : IDisposable
{
    /// <summary>
    /// Gets the underlying device handle. The handle is populated when you open the device
    /// using <see cref="Open"/>, and cleared when you close the device using <see cref="Close"/>.
    /// </summary>
    DeviceHandle DeviceHandle { get; }

    /// <summary>
    /// Gets the available configurations for this <see cref="UsbDevice"/>
    /// </summary>
    /// <remarks>
    /// The first time this property is accessed it will query the <see cref="UsbDevice"/> for all configurations.  Subsequent request will return a cached copy of all configurations.
    /// </remarks>
    ReadOnlyCollection<UsbConfigInfo> Configs { get; }

    /// <summary>
    /// Gets the actual device descriptor the the current <see cref="UsbDevice"/>.
    /// </summary>
    UsbDeviceInfo Info { get; }

    /// <summary>
    /// Gets a value indicating whether the device handle is valid.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Gets the USB devices active configuration value.
    /// </summary>
    /// <returns>
    /// The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.
    /// </returns>
    int Configuration { get; }

    /// <summary>
    /// Gets the Vendor ID (PID) of the vendor of this device.
    /// </summary>
    ushort VendorId { get; }

    /// <summary>
    /// Gets the Product ID (PID) of this device.
    /// </summary>
    ushort ProductId { get; }

    /// <summary>
    /// Closes and frees device resources.
    /// </summary>
    void Close();

    /// <summary>
    /// Gets a specific descriptor from the device. See <see cref="DescriptorType"/> for more information.
    /// </summary>
    /// <param name="descriptorType">The descriptor type ID to retrieve; this is usually one of the <see cref="DescriptorType"/> enumerations.</param>
    /// <param name="index">Descriptor index.</param>
    /// <param name="langId">Descriptor language id.</param>
    /// <param name="buffer">Memory to store the returned descriptor in.</param>
    /// <param name="bufferLength">Length of the buffer parameter in bytes.</param>
    /// <param name="transferLength">The number of bytes transferred to buffer upon success.</param>
    /// <returns>True on success.</returns>
    bool GetDescriptor(byte descriptorType, byte index, short langId, IntPtr buffer, int bufferLength, out int transferLength);

    /// <summary>
    /// Gets a specific descriptor from the device. See <see cref="DescriptorType"/> for more information.
    /// </summary>
    /// <param name="descriptorType">The descriptor type ID to retrieve; this is usually one of the <see cref="DescriptorType"/> enumerations.</param>
    /// <param name="index">Descriptor index.</param>
    /// <param name="langId">Descriptor language id.</param>
    /// <param name="buffer">Memory to store the returned descriptor in.</param>
    /// <param name="bufferLength">Length of the buffer parameter in bytes.</param>
    /// <param name="transferLength">The number of bytes transferred to buffer upon success.</param>
    /// <returns>True on success.</returns>
    bool GetDescriptor(byte descriptorType, byte index, short langId, object buffer, int bufferLength, out int transferLength);

    /// <summary>
    /// Asking for the zero'th index is special - it returns a string
    /// descriptor that contains all the language IDs supported by the
    /// device. Typically there aren't many - often only one. The
    /// language IDs are 16 bit numbers, and they start at the third byte
    /// in the descriptor. See USB 2.0 specification, section 9.6.7, for
    /// more information on this.
    /// </summary>
    /// <returns>A collection of LCIDs that the current <see cref="UsbDevice"/> supports.</returns>
    bool GetLangIDs(out short[] langIDs);

    /// <summary>
    /// Gets a string descriptor from the device.
    /// </summary>
    /// <param name="stringData">Buffer to store the returned string in upon success.</param>
    /// <param name="langId">The language ID to retrieve the string in. (0x409 for english).</param>
    /// <param name="stringIndex">The string index to retrieve.</param>
    /// <returns>True on success.</returns>
    bool GetString(out string stringData, short langId, byte stringIndex);

    /// <summary>
    /// Retrieve a descriptor from a device.
    /// </summary>
    /// <param name="descriptorIndex">
    /// The index of the descriptor to retieve.
    /// </param>
    /// <param name="failSilently">
    /// <see langword="true"/> to return <see langword="null"/> when the descriptor could not be
    /// received; <see langword="false"/> to throw an <see cref="UsbException"/> instead.
    /// </param>
    /// <returns>
    /// The value of the requested descriptor.
    /// </returns>
    string GetStringDescriptor(byte descriptorIndex, bool failSilently = false);

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
    bool TryGetConfigDescriptor(byte configIndex, out UsbConfigInfo descriptor);

    /// <summary>
    /// sets the alternate interface number for the previously claimed interface. <see cref="IUsbDevice.ClaimInterface"/>
    /// </summary>
    /// <param name="alternateID">The alternate interface number.</param>
    /// <returns>True on success.</returns>
    bool SetAltInterface(int alternateID);

    /// <summary>
    /// Gets the alternate interface number for the previously claimed interface. <see cref="IUsbDevice.ClaimInterface"/>
    /// </summary>
    /// <param name="alternateID">The alternate interface number.</param>
    /// <returns>True on success.</returns>
    bool GetAltInterface(out int alternateID);

    /// <summary>
    /// Opens/re-opens this USB device instance for communication.
    /// </summary>
    void Open();

    /// <summary>
    /// Attempts to open this device.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the device could be opened successfully;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool TryOpen();

    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <param name="readBufferSize">TODO: Remove this parameter.</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize);

    /// <summary>
    /// Opens an endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <param name="readBufferSize">TODO: Remove this parameter.</param>
    /// <param name="endpointType">The type of endpoint to open.</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize, EndpointType endpointType);

    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
    /// </summary>
    /// <param name="readEndpointID">Endpoint number for read operations.</param>
    /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
    UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID);

    /// <summary>
    /// Opens a <see cref="EndpointType.Bulk"/> endpoint for writing
    /// </summary>
    /// <param name="writeEndpointID">Endpoint number for read operations.</param>
    /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
    UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID);

    /// <summary>
    /// Opens an endpoint for writing
    /// </summary>
    /// <param name="writeEndpointID">Endpoint number for read operations.</param>
    /// <param name="endpointType">The type of endpoint to open.</param>
    /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
    UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID, EndpointType endpointType);

    /// <summary>
    /// Sets the USB devices active configuration value.
    /// </summary>
    /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
    /// <remarks>
    /// A USB device can have several different configurations, but only one active configuration.
    /// </remarks>
    void SetConfiguration(int config);

    /// <summary>
    /// Gets the selected alternate interface of the specified interface.
    /// </summary>
    /// <param name="interfaceID">The interface settings number (index) to retrieve the selected alternate interface setting for.</param>
    /// <param name="selectedAltInterfaceID">The alternate interface setting selected for use with the specified interface.</param>
    void GetAltInterfaceSetting(byte interfaceID, out byte selectedAltInterfaceID);

    /// <summary>
    /// Claims the specified interface of the device.
    /// </summary>
    /// <param name="interfaceID">The interface to claim.</param>
    /// <returns>True on success.</returns>
    bool ClaimInterface(int interfaceID);

    /// <summary>
    /// Releases an interface that was previously claimed with <see cref="ClaimInterface"/>.
    /// </summary>
    /// <param name="interfaceID">The interface to release.</param>
    /// <returns>True on success.</returns>
    bool ReleaseInterface(int interfaceID);

    /// <summary>
    /// Sends a usb device reset command.
    /// </summary>
    /// <remarks>
    /// After calling <see cref="ResetDevice"/>, the <see cref="UsbDevice"/> instance is disposed and
    /// no longer usable.  A new <see cref="UsbDevice"/> instance must be obtained from the device list.
    /// </remarks>
    void ResetDevice();

    /// <summary>
    /// Transmits control data over a default control endpoint.
    /// </summary>
    /// <param name="setupPacket">
    /// An 8-byte setup packet which contains parameters for the control request.
    /// See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information.
    /// </param>
    /// <returns>
    /// The number of bytes sent or received (depends on the direction of the control transfer).
    /// </returns>
    int ControlTransfer(UsbSetupPacket setupPacket);
        
    /// <summary>
    /// Asynchronously transmits control data over a default control endpoint.
    /// </summary>
    /// <param name="setupPacket">
    /// An 8-byte setup packet which contains parameters for the control request.
    /// See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information.
    /// </param>
    /// <returns>
    /// The number of bytes sent or received (depends on the direction of the control transfer).
    /// </returns>
    Task<int> ControlTransferAsync(UsbSetupPacket setupPacket);

    /// <summary>
    /// Transmits control data over a default control endpoint.
    /// </summary>
    /// <param name="setupPacket">
    /// An 8-byte setup packet which contains parameters for the control request.
    /// See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information.
    /// </param>
    /// <param name="buffer">
    /// Data to be sent/received from the device.
    /// </param>
    /// <param name="offset">
    /// The offset of the first byte of the data to send.
    /// </param>
    /// <param name="length">
    /// Length of the buffer param.
    /// </param>
    /// <returns>The number of bytes sent or received (depends on the direction of the control transfer).
    /// </returns>
    int ControlTransfer(UsbSetupPacket setupPacket, byte[] buffer, int offset, int length);
        
    /// <summary>
    /// Asynchronously transmits control data over a default control endpoint.
    /// </summary>
    /// <param name="setupPacket">
    /// An 8-byte setup packet which contains parameters for the control request.
    /// See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information.
    /// </param>
    /// <param name="buffer">
    /// Data to be sent/received from the device.
    /// </param>
    /// <param name="offset">
    /// The offset of the first byte of the data to send.
    /// </param>
    /// <param name="length">
    /// Length of the buffer param.
    /// </param>
    /// <returns>The number of bytes sent or received (depends on the direction of the control transfer).
    /// </returns>
    Task<int> ControlTransferAsync(UsbSetupPacket setupPacket, byte[] buffer, int offset, int length);

    /// <summary>
    /// Creates a clone of this device.
    /// </summary>
    /// <returns>
    /// A new <see cref="UsbDevice"/> which represents a clone of this device.
    /// </returns>
    IUsbDevice Clone();
}