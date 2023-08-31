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
using System.Runtime.InteropServices;

#pragma warning disable 649

namespace LibUsbDotNet.Descriptors;

/// <summary> Base class for all usb descriptors structures.
/// </summary>
/// <remarks> This is the actual descriptor as described in the USB 2.0 Specifications.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public abstract class UsbDescriptor
{
    /// <summary>
    /// String value used to seperate the name/value pairs for all ToString overloads of the descriptor classes.
    /// </summary>
    public const string ToStringParamValueSeperator = ":";

    /// <summary>
    /// String value used to seperate the name/value groups for all ToString overloads of the descriptor classes.
    /// </summary>
    public const string ToStringFieldSeperator = "\r\n";

    /// <summary>
    /// Total size of this structure in bytes.
    /// </summary>
    public static readonly int Size = Marshal.SizeOf(typeof(UsbDescriptor));

    /// <summary>
    /// Length of structure reported by the associated usb device.
    /// </summary>
    private byte length;

    /// <summary>
    /// Type of structure reported by the associated usb device.
    /// </summary>
    private DescriptorType descriptorType;

    /// <inheritdoc/>
    public override string ToString()
    {
        object[] values = { this.length, this.descriptorType };
        string[] names = { "Length", "DescriptorType" };

        return Helper.ToString(string.Empty, names, ToStringParamValueSeperator, values, ToStringFieldSeperator);
    }
}