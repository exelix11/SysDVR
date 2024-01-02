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

namespace LibUsbDotNet.Info;

/// <summary> Base class for all Usb descriptors.
/// <see cref="LibUsbDotNet.Info.UsbConfigInfo"/>, <see cref="T:LibUsbDotNet.Info.UsbEndpointInfo"/>,
/// <see cref="T:LibUsbDotNet.Info.UsbInterfaceInfo"/></summary>
/// <remarks>
/// <p>LibUsbDotNet supports and parses all the basic usb descriptors.</p><p>
/// Unknown descriptors such as driver specific class descriptors are stored as byte arrays and are accessible
/// from the <see cref="P:LibUsbDotNet.Info.UsbBaseInfo.CustomDescriptors"/> property.
/// </p>
/// </remarks>
public abstract class UsbBaseInfo
{
    /// <summary>
    /// Extra descriptors.
    /// </summary>
    protected byte[] RawDescriptors { get; set; }
#if NETSTANDARD2_0
        = new byte[] { };
#else
            = Array.Empty<byte>();
#endif

    /// <summary>
    /// Gets the device-specific custom descriptor lists.
    /// </summary>
    public virtual ReadOnlyCollection<byte> CustomDescriptors
    {
        get { return new ReadOnlyCollection<byte>(new List<byte>(this.RawDescriptors)); }
    }
}