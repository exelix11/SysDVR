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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// A collection of <see cref="UsbDevice"/> objects. All devices in this collection are disposed
/// of when youd dispose the collection.
/// </summary>
public class UsbDeviceCollection : ReadOnlyCollection<IUsbDevice>, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceCollection"/> class.
    /// </summary>
    /// <param name="list">
    /// The underlying list of devices.
    /// </param>
    public UsbDeviceCollection(IList<IUsbDevice> list)
        : base(list)
    {
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var device in this)
        {
            device.Dispose();
        }
    }
}