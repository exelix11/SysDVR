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
/// Provides extension methods for the <see cref="Error"/> enumeration.
/// </summary>
public static class ErrorExtensions
{
    /// <summary>
    /// Throws a <see cref="UsbException"/> if the value of <paramref name="error"/> is not <see cref="Error.Success"/>.
    /// </summary>
    /// <param name="error">
    /// The error code based on which to throw an exception.
    /// </param>
    public static void ThrowOnError(this Error error)
    {
        if (error != Error.Success)
        {
            throw new UsbException(error);
        }
    }

    /// <summary>
    /// Gets the function's return value (if ret &gt;= 0), or throws an error if the return value was negative
    /// and indicated an error.
    /// </summary>
    /// <param name="error">
    /// The return value to inspect.
    /// </param>
    /// <returns>
    /// The function's return value (if ret &gt;= 0);.
    /// </returns>
    public static int GetValueOrThrow(this Error error)
    {
        int value = (int)error;

        if (value < 0)
        {
            throw new UsbException(error);
        }
        else
        {
            return value;
        }
    }

    public static Error ToError(TransferStatus transferStatus)
    {
        switch (transferStatus)
        {
            case TransferStatus.Completed:
                return Error.Success;
            case TransferStatus.Error:
                return Error.Pipe;
            case TransferStatus.TimedOut:
                return Error.Timeout;
            case TransferStatus.Cancelled:
                return Error.Io;
            case TransferStatus.Stall:
                return Error.Pipe;
            case TransferStatus.NoDevice:
                return Error.NoDevice;
            case TransferStatus.Overflow:
                return Error.Overflow;
            default:
                return Error.Other;
        }
    }
}