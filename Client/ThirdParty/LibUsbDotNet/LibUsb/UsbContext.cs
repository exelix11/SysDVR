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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// An instance of the libusb API. You can use multiple <see cref="UsbContext"/> which are independent
/// from each other.
/// </summary>
public class UsbContext : IUsbContext
{
    /// <summary>
    /// The native context.
    /// </summary>
    private readonly Context context;

    /// <summary>
    /// Thread for event handling.
    /// </summary>
    private Thread eventHandlingThread;
        
    /// <summary>
    /// ID of the underlying <see cref="Context"/>.
    /// </summary>
    public string HandleId => context.ToString();

    /// <summary>
    /// Tracks whether this context has been disposed of, or not.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Tracking list of all devices that are open on this context.
    /// </summary>
    internal List<UsbDevice> OpenDevices { get; }

    /// <summary>
    /// Read-only list of all devices that are open on this context.
    /// </summary>
    public ReadOnlyCollection<UsbDevice> ReadOnlyOpenDevices => 
        new ReadOnlyCollection<UsbDevice>(OpenDevices);

    /// <summary>
    /// Tracks when the context is currently being disposed.
    /// <remarks>
    /// Used for preventing the devices in <see cref="OpenDevices"/> from modifying the list when they are closed in <see cref="Dispose()"/>.
    /// </remarks>
    /// </summary>
    internal bool IsDisposing { get; private set; }

    /// <summary>
    /// Allows the event handling thread to return when set to 1.
    /// </summary>
    internal int stopHandlingEvents;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbContext"/> class.
    /// </summary>
    public UsbContext()
    {
        IntPtr contextHandle = IntPtr.Zero;
        NativeMethods.Init(ref contextHandle).ThrowOnError();
        this.context = Context.DangerousCreate(contextHandle);
        OpenDevices = new List<UsbDevice>();
    }

    ~UsbContext()
    {
        // Put cleanup code in Dispose(bool disposing).
        this.Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Put cleanup code in Dispose(bool disposing).
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void SetDebugLevel(LogLevel level)
    {
        NativeMethods.SetDebug(this.context, (int)level);
    }

    /// <summary>
    /// Returns a list of USB devices currently attached to the system.
    /// </summary>
    /// <returns>
    /// A <see cref="UsbDeviceCollection"/> which contains the devices currently
    /// attached to the system.</returns>
    /// <remarks>
    /// <para>
    /// This is your entry point into finding a USB device to operate.
    /// </para>
    /// <para>
    /// You are expected to dispose all the devices once you are done with them. Disposing the <see cref="UsbDeviceCollection"/>
    /// will dispose all devices in that collection. You can <see cref="UsbDevice.Clone"/> a device to get a copy of the device
    /// which you can use after you've disposed the <see cref="UsbDeviceCollection"/>.
    /// </para>
    /// </remarks>
    public unsafe UsbDeviceCollection List()
    {
        IntPtr* list;
        var deviceCount = NativeMethods.GetDeviceList(this.context, &list);

        Collection<IUsbDevice> devices = new Collection<IUsbDevice>();

        for (int i = 0; i < deviceCount.ToInt32(); i++)
        {
            Device device = Device.DangerousCreate(list[i]);
            devices.Add(new UsbDevice(device, this));
        }

        NativeMethods.FreeDeviceList(list, unrefDevices: 0 /* Do not unreference the devices */);

        return new UsbDeviceCollection(devices);
    }

    /// <inheritdoc/>
    public IUsbDevice Find(UsbDeviceFinder finder)
    {
        if (finder == null)
        {
            throw new ArgumentNullException(nameof(finder));
        }

        return this.Find(finder.Check);
    }

    /// <inheritdoc/>
    public IUsbDevice Find(Func<IUsbDevice, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        using (var list = this.List())
        {
            foreach (var device in list)
            {
                if (predicate(device))
                {
                    return device.Clone();
                }
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public UsbDeviceCollection FindAll(UsbDeviceFinder finder)
    {
        if (finder == null)
        {
            throw new ArgumentNullException(nameof(finder));
        }

        return this.FindAll(finder.Check);
    }

    /// <inheritdoc/>
    public UsbDeviceCollection FindAll(Func<IUsbDevice, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        var matchingDevices = new List<IUsbDevice>();

        using (var list = this.List())
        {
            foreach (var device in list)
            {
                if (predicate(device))
                {
                    matchingDevices.Add(device.Clone());
                }
            }
        }

        UsbDeviceCollection devices = new UsbDeviceCollection(matchingDevices);
        return devices;
    }

    /// <summary>
    /// Starts the event handling thread.
    /// </summary>
    public void StartHandlingEvents()
    {
        if (this.eventHandlingThread != null)
            return;
            
        this.eventHandlingThread = new Thread(this.HandleEvents)
        {
            IsBackground = true
        };

        this.stopHandlingEvents = 0;
        this.eventHandlingThread.Start();
    }

    /// <summary>
    /// Attempts to stop the event handling thread.
    /// </summary>
    /// <remarks>
    /// Note that <see cref="NativeMethods.HandleEventsCompleted"/> must be woken up an event in order for this method to return.
    /// Ideally this will happen when the last device in <see cref="OpenDevices"/> is closed.
    /// </remarks>
    public void StopHandlingEvents()
    {
        if (this.eventHandlingThread == null)
            return;
            
        this.eventHandlingThread.Join();
        this.eventHandlingThread = null;
    }

    protected virtual void Dispose(bool disposeManagedObjects)
    {
        if (IsDisposed) 
            return;
            
        IsDisposing = true;
            
        // Not sure what should go here, what resources should be cleaned up by explicit Dispose but not in the finalizer?
        if (disposeManagedObjects)
        {
            // Dispose managed state (managed objects).
        }

        // Close any devices still open on this context.
        foreach (var openDevice in OpenDevices)
        {
            openDevice.Dispose();
        }
            
        OpenDevices.Clear();

        // Ideally this shouldn't be necessary, as StopHandlingEvents should be called when the last open device is closed.
        if (this.stopHandlingEvents == 0) 
            StopHandlingEvents();
            
        // Dispose of underlying context handle.
        context.Dispose();
            
        IsDisposed = true;
    }

    private void HandleEvents()
    {
        while (this.stopHandlingEvents == 0)
        {
            NativeMethods.HandleEventsCompleted(this.context, ref stopHandlingEvents).ThrowOnError();
        }
    }

    public static void SetGlobalOption(LibUsbOption option, nint arg)
    {
        NativeMethods.SetOption(IntPtr.Zero, option, arg).ThrowOnError();
    }

    public IUsbDevice WrapNativeDeviceHandle(nint nativeHandle)
    {
        NativeMethods.WrapSystemHanlde(context, nativeHandle, out var handle).ThrowOnError();
        var deviceHandle = DeviceHandle.DangerousCreate(handle);

        // GetDevice does not increment the reference count, so we don't need to decrement it.
        var device = Device.DangerousCreate(NativeMethods.GetDeviceRaw(deviceHandle), false);

        var devObj = new UsbDevice(device, this);
        devObj.OpenWrapped(deviceHandle);

        return devObj;
    }
}