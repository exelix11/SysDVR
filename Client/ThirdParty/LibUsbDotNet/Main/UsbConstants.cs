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

namespace LibUsbDotNet.Main;

/// <summary> Various USB constants.
/// </summary>
public static class UsbConstants
{
    /// <summary>
    /// Default timeout for all USB IO operations.
    /// </summary>
    public const int DefaultTimeout = 1000;

    /// <summary>
    /// Maximum number of USB devices connected to the driver at once.
    /// </summary>
    public const int MaxDeviceCount = 256;

    /// <summary>
    /// Endpoint direction mask.
    /// </summary>
    public const byte EndpointDirectionMask = 0x80;

    /// <summary>
    /// Endpoint number mask.
    /// </summary>
    public const byte EndpointNumberMask = 0xf;
}