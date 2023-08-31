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

using System;
using System.Runtime.InteropServices;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Unix mono.net timeval structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct UnixNativeTimeval
{
    private IntPtr mTvSecInternal;
    private IntPtr mTvUSecInternal;

    /// <summary>
    /// Default <see cref="UnixNativeTimeval"/>.
    /// </summary>
    public static UnixNativeTimeval Default
    {
        get { return new UnixNativeTimeval(2, 0); }
    }

    /// <summary>
    /// Timeval seconds property.
    /// </summary>
    public long tv_sec
    {
        get { return this.mTvSecInternal.ToInt64(); }
        set { this.mTvSecInternal = new IntPtr(value); }
    }

    /// <summary>
    /// Timeval milliseconds property.
    /// </summary>
    public long tv_usec
    {
        get { return this.mTvUSecInternal.ToInt64(); }
        set { this.mTvUSecInternal = new IntPtr(value); }
    }

    /// <summary>
    /// Timeval constructor.
    /// </summary>
    /// <param name="tvSec">seconds</param>
    /// <param name="tvUsec">milliseconds</param>
    public UnixNativeTimeval(long tvSec, long tvUsec)
    {
        this.mTvSecInternal = new IntPtr(tvSec);
        this.mTvUSecInternal = new IntPtr(tvUsec);
    }
}