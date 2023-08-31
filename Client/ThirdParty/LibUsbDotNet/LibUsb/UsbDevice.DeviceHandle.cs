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

using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Main;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LibUsbDotNet.LibUsb;

// Implementation of functionality which wraps around a DeviceHandle.
public partial class UsbDevice
{
    /// <summary>
    /// The underlying device handle. The handle is populated when you open the device
    /// using <see cref="Open"/>, and cleared when you close the device using <see cref="Close"/>.
    /// </summary>
    private DeviceHandle deviceHandle;

    /// <inheritdoc/>
    public DeviceHandle DeviceHandle
    {
        get { return this.deviceHandle; }
    }

    /// <summary>
    /// Gets a value indicating whether the device has been opened. You can perform I/O on a
    /// device when it is open.
    /// </summary>
    public bool IsOpen
    {
        get
        {
            return this.deviceHandle != null;
        }
    }

    /// <summary>
    /// Gets the <c>bConfigurationValue</c> of the currently active configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You could formulate your own control request to obtain this information, but this function
    /// has the advantage that it may be able to retrieve the information from operating system caches
    /// (no I/O involved).
    /// </para>
    /// <para>
    /// If the OS does not cache this information, then this function will block while a control
    /// transfer is submitted to retrieve the information.
    /// </para>
    /// <para>
    /// This property will return a value of 0 in the config output parameter if the device is
    /// in unconfigured state.
    /// </para>
    /// </remarks>
    public int Configuration
    {
        get
        {
            this.EnsureNotDisposed();
            this.EnsureOpen();

            int config = 0;
            NativeMethods.GetConfiguration(this.deviceHandle, ref config).ThrowOnError();
            return config;
        }
    }

    /// <inheritdoc/>
    public void SetConfiguration(int config)
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        NativeMethods.SetConfiguration(this.deviceHandle, config).ThrowOnError();
    }

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
    public unsafe string GetStringDescriptor(byte descriptorIndex, bool failSilently = false)
    {
        if (failSilently && !this.IsOpen)
        {
            return null;
        }

        this.EnsureNotDisposed();
        this.EnsureOpen();

        if (descriptorIndex == 0)
            return null;

        Span<byte> buffer = stackalloc byte[1024];
        int length;
        fixed (byte* ptr = &MemoryMarshal.GetReference(buffer))
            length = (int)NativeMethods.GetStringDescriptorAscii(this.deviceHandle, descriptorIndex, ptr,
                buffer.Length);

        if (length < 0)
        {
            if (failSilently)
                return null;

            ((Error)length).ThrowOnError();
        }

        return Encoding.ASCII.GetString(buffer[..length].ToArray());
    }

    /// <inheritdoc/>
    public unsafe int ControlTransfer(UsbSetupPacket setupPacket)
    {
        return this.ControlTransfer(setupPacket, null, 0, 0);
    }

    /// <inheritdoc/>
    public unsafe int ControlTransfer(UsbSetupPacket setupPacket, byte[] buffer, int offset, int length)
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        int result = 0;

        if (length > 0)
        {
            fixed (byte* data = &buffer[0])
            {
                result = NativeMethods.ControlTransfer(
                    this.deviceHandle,
                    setupPacket.RequestType,
                    setupPacket.Request,
                    (ushort)setupPacket.Value,
                    (ushort)setupPacket.Index,
                    data,
                    (ushort)length,
                    UsbConstants.DefaultTimeout);
            }
        }
        else
        {
            result = NativeMethods.ControlTransfer(
                this.deviceHandle,
                setupPacket.RequestType,
                setupPacket.Request,
                (ushort)setupPacket.Value,
                (ushort)setupPacket.Index,
                null,
                0,
                UsbConstants.DefaultTimeout);
        }

        if (result >= 0)
        {
            return result;
        }
        else
        {
            throw new UsbException((Error)result);
        }
    }

    /// <inheritdoc/>
    public unsafe bool GetDescriptor(byte descriptorType, byte index, short langId, IntPtr buffer, int bufferLength, out int transferLength)
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        int ret = NativeMethods.ControlTransfer(
            this.deviceHandle,
            (byte)EndpointDirection.In,
            (byte)StandardRequest.GetDescriptor,
            (ushort)((descriptorType << 8) | index),
            0,
            (byte*)buffer.ToPointer(),
            (ushort)bufferLength,
            1000);

        if (ret < 0)
        {
            throw new UsbException((Error)ret);
        }

        transferLength = ret;
        return true;
    }

    /// <inheritdoc/>
    public bool GetDescriptor(byte descriptorType, byte index, short langId, object buffer, int bufferLength, out int transferLength)
    {
        using (PinnedHandle p = new PinnedHandle(buffer))
        {
            return this.GetDescriptor(descriptorType, index, langId, p.Handle, bufferLength, out transferLength);
        }
    }

    /// <inheritdoc/>
    public bool GetLangIDs(out short[] langIDs)
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        LangStringDescriptor sd = new LangStringDescriptor(UsbDescriptor.Size + (16 * sizeof(short)));

        int ret;
        bool bSuccess = this.GetDescriptor((byte)DescriptorType.String, 0, 0, sd.Ptr, sd.MaxSize, out ret);
        bSuccess = sd.Get(out langIDs);
        sd.Free();
        return bSuccess;
    }

    /// <inheritdoc/>
    public bool GetString(out string stringData, short langId, byte stringIndex)
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        stringData = null;
        int iTransferLength;
        LangStringDescriptor sd = new LangStringDescriptor(255);
        bool bSuccess = this.GetDescriptor((byte)DescriptorType.String, stringIndex, langId, sd.Ptr, sd.MaxSize, out iTransferLength);
        if (bSuccess && iTransferLength > UsbDescriptor.Size && sd.Length == iTransferLength)
        {
            bSuccess = sd.Get(out stringData);
        }

        return bSuccess;
    }

    /// <inheritdoc/>
    public void ResetDevice()
    {
        this.EnsureNotDisposed();
        this.EnsureOpen();

        NativeMethods.ResetDevice(this.deviceHandle).ThrowOnError();
    }

    /// <summary>
    /// Opens a device, allowing you to perform I/O on this device.
    /// </summary>
    public void Open()
    {
        this.OpenNative().ThrowOnError();
    }

    /// <inheritdoc/>
    public bool TryOpen()
    {
        return this.OpenNative() == Error.Success;
    }

    /// <summary>
    /// Closes the device.
    /// </summary>
    public void Close()
    {
        this.EnsureNotDisposed();

        if (!this.IsOpen)
            return;

        bool shouldStopHandlingEvents = originatingContext.OpenDevices.Count == 1;

        if (shouldStopHandlingEvents)
            Interlocked.Exchange(ref originatingContext.stopHandlingEvents, 1);
            
        this.deviceHandle.Dispose();
        this.deviceHandle = null;

        if (shouldStopHandlingEvents) 
            originatingContext.StopHandlingEvents();
            
        if (!originatingContext.IsDisposing)
            originatingContext.OpenDevices.Remove(this);
    }

    /// <summary>
    /// Throws a <see cref="UsbException"/> if the device is not open.
    /// </summary>
    protected void EnsureOpen()
    {
        if (!this.IsOpen)
        {
            throw new UsbException("The device has not been opened. You need to call Open() first.");
        }
    }

    public Error OpenWrapped(DeviceHandle deviceHandle) 
    {
        this.EnsureNotDisposed();

        if (this.IsOpen)
        {
            return Error.Success;
        }

        this.deviceHandle = deviceHandle;
        this.descriptor = null;
        if (originatingContext.OpenDevices.Count == 0)
            originatingContext.StartHandlingEvents();
        if (!originatingContext.IsDisposing)
            originatingContext.OpenDevices.Add(this);

        return Error.Success;
    }

    private Error OpenNative()
    {
        this.EnsureNotDisposed();

        if (this.IsOpen)
        {
            return Error.Success;
        }

        IntPtr deviceHandle = IntPtr.Zero;
        var ret = NativeMethods.Open(this.device, ref deviceHandle);
            
        if (ret == Error.Success)
        {
            return OpenWrapped(DeviceHandle.DangerousCreate(deviceHandle));
        }

        return ret;
    }
}