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
using System.Runtime.Serialization;

namespace LibUsbDotNet.LibUsb;

/// <summary>
/// Exceptions thrown from libusb <see cref="Error"/>s.
/// </summary>
[Serializable]
public class UsbException : Exception
{
    public UsbException()
    {
    }

    /// <summary>
    /// Throw a <see cref="UsbException"/> for the given <see cref="Error"/>.
    /// </summary>
    /// <param name="errorCode"></param>
    public UsbException(Error errorCode)
        : this(GetErrorMessage(errorCode))
    {
        this.ErrorCode = errorCode;
        this.HResult = (int)errorCode;
    }

    /// <inheritdoc />
    public UsbException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public UsbException(string message, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// The libusb <see cref="Error"/> code.
    /// </summary>
    public Error ErrorCode
    {
        get;
        private set;
    }

    private static string GetErrorMessage(Error errorCode)
    {
        IntPtr errorString = NativeMethods.StrError(errorCode);

        if (errorString != IntPtr.Zero)
        {
            // From the documentation: 'The caller must not free() the returned string.'
            return Marshal.PtrToStringAnsi(errorString);
        }
        else
        {
            return $"An unknown error with code {(int)errorCode} has occurred.";
        }
    }
}