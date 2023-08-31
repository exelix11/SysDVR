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

// Contains the functionality related to kernel driver support.
public partial class UsbDevice
{
    /// <summary>
    /// Determine if this platform supports detaching the kernel driver.
    /// </summary>
    /// <returns>True if kernel driver detach supported, false otherwise.</returns>
    public bool SupportsDetachKernelDriver() => 
        NativeMethods.HasCapability((uint)Capability.SupportsDetachKernelDriver) != 0;

    /// <summary>
    /// Determine if a kernel driver is active on an interface.
    /// </summary>
    /// <remarks>
    /// If a kernel driver is active, you cannot claim the interface, and libusb will be unable to perform I/O.
    /// This functionality is not available on Windows.
    /// </remarks>
    /// <param name="interfaceNumber">The interface to check.</param>
    /// <returns>True if kernel driver active, false otherwise.</returns>
    public bool IsKernelDriverActive(int interfaceNumber)
    {
        EnsureOpen();
        return NativeMethods.KernelDriverActive(DeviceHandle, interfaceNumber).GetValueOrThrow() == 1;
    }

    /// <summary>
    /// Detach a kernel driver from an interface.
    /// </summary>
    /// <remarks>
    /// If successful, you will then be able to claim the interface and perform I/O.
    /// This functionality is not available on Windows.
    /// Note that libusb itself also talks to the device through a special kernel driver,
    /// if this driver is already attached to the device, this call will not detach it and throw a
    /// <see cref="Error.NotFound"/> <see cref="UsbException"/>
    /// </remarks>
    /// <param name="interfaceNumber">The interface to detach the driver from.</param>
    public void DetachKernelDriver(int interfaceNumber)
    {
        EnsureOpen();
        NativeMethods.DetachKernelDriver(DeviceHandle, interfaceNumber).ThrowOnError();
    }

    /// <summary>
    /// Re-attach an interface's kernel driver, which was previously detached using <see cref="DetachKernelDriver"/>.
    /// </summary>
    /// <remarks>This functionality is not available on Windows.</remarks>
    /// <param name="interfaceNumber">The interface to attach the driver from.</param>
    public void AttachKernelDriver(int interfaceNumber)
    {
        EnsureOpen();
        NativeMethods.AttachKernelDriver(DeviceHandle, interfaceNumber).ThrowOnError();
    }

    /// <summary>
    /// Enable/disable libusb's automatic kernel driver detachment.
    /// </summary>
    /// <remarks>
    /// When this is enabled libusb will automatically detach the kernel driver on an interface when claiming the
    /// interface, and attach it when releasing the interface.
    /// Automatic kernel driver detachment is disabled on newly opened device handles by default.
    /// </remarks>
    /// <param name="autoDetach">Whether to enable or disable auto kernel driver detachment.</param>
    /// <returns>True if the current platform supports kernel driver detachment, false otherwise.</returns>
    public bool SetAutoDetachKernelDriver(bool autoDetach)
    {
        EnsureOpen();
        return NativeMethods.SetAutoDetachKernelDriver(DeviceHandle, autoDetach ? 1 : 0) == Error.Success;
    }
}