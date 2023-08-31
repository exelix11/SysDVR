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

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Static class to determine the native libusb version.
/// </summary>
public static class NativeLibraryVersion
{
    static unsafe NativeLibraryVersion()
    {
        var version = NativeMethods.GetVersion();
        Major = version->Major;
        Minor = version->Minor;
        Micro = version->Micro;
        Nano = version->Nano;
    }
    
    /// <summary>
    ///  Library major version.
    /// </summary>
    public static ushort Major { get; }

    /// <summary>
    ///  Library minor version.
    /// </summary>
    public static ushort Minor { get; }

    /// <summary>
    ///  Library micro version.
    /// </summary>
    public static ushort Micro { get; }

    /// <summary>
    ///  Library nano version.
    /// </summary>
    public static ushort Nano { get; }

    /// <summary>
    /// libusb version with nano included
    /// </summary>
    public static string FullVersion 
        => $"{Major}.{Minor}.{Micro}.{Nano}";
    
    /// <summary>
    /// libusb version
    /// </summary>
    public static string Version 
        => $"{Major}.{Minor}.{Micro}";
}