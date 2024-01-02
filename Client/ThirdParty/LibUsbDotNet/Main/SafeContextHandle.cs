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

namespace LibUsbDotNet.Main;

/// <summary>
/// Base class for all critial handles.
/// </summary>
public abstract class SafeContextHandle : SafeHandle
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="pHandle"></param>
    /// <param name="ownsHandle"></param>
    protected SafeContextHandle(IntPtr pHandle, bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
    {
        this.SetHandle(pHandle);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="pHandleToOwn"></param>
    protected SafeContextHandle(IntPtr pHandleToOwn)
        : this(pHandleToOwn, true)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the handle value is invalid.
    /// </summary>
    /// <returns>
    /// true if the handle value is invalid; otherwise, false.
    /// </returns>
    /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    public override bool IsInvalid
    {
        get
        {
            if (this.handle != IntPtr.Zero)
            {
                return this.handle == new IntPtr(-1);
            }

            return true;
        }
    }
}