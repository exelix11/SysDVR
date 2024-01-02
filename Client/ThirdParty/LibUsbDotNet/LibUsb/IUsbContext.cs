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
//
//

using LibUsbDotNet.Main;
using System;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Initializes a new libusb context. You can access most of libusb's functionality through context. Multiple
/// contexts operate in isolation from each other.
/// </summary>
/// <seealso href="http://libusb.sourceforge.net/api-1.0/libusb_contexts.html"/>
public interface IUsbContext : IDisposable
{
    /// <summary>
    /// Set log message verbosity.
    /// </summary>
    /// <param name="level">
    /// The requested debug level.
    /// </param>
    void SetDebugLevel(LogLevel level);

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
    UsbDeviceCollection List();

    /// <summary>
    /// Finds a specific device.
    /// </summary>
    /// <param name="finder">
    /// A finder which contains the parameters of the device you want to find.
    /// </param>
    /// <returns>
    /// If found, the requested device. Otherwise, <see langword="null"/>.
    /// </returns>
    IUsbDevice Find(UsbDeviceFinder finder);

    /// <summary>
    /// Finds a specific device.
    /// </summary>
    /// <param name="predicate">
    /// A predicate which specifies which device you want to find.
    /// </param>
    /// <returns>
    /// If found, the requested device. Otherwise, <see langword="null"/>.
    /// </returns>
    IUsbDevice Find(Func<IUsbDevice, bool> predicate);

    /// <summary>
    /// Finds all devices which match a given criterium.
    /// </summary>
    /// <param name="finder">
    /// A finder which contains the parameters of the device you want to find.
    /// </param>
    /// <returns>
    /// A list of devices which match the criteria you've specified.
    /// </returns>
    UsbDeviceCollection FindAll(UsbDeviceFinder finder);

    /// <summary>
    /// Finds all devices which match a given criterium.
    /// </summary>
    /// <param name="predicate">
    /// A finder which contains the parameters of the device you want to find.
    /// </param>
    /// <returns>
    /// A list of devices which match the criteria you've specified.
    /// </returns>
    UsbDeviceCollection FindAll(Func<IUsbDevice, bool> predicate);

    /// <summary>
    /// Start the event handling thread.
    /// </summary>
    void StartHandlingEvents();

    /// <summary>
    /// Stop the event handling thread.
    /// </summary>
    void StopHandlingEvents();
}