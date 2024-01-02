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
using System.Runtime.InteropServices;

namespace LibUsbDotNet.Main;

/// <summary>
/// Used for allocating a <see cref="GCHandle"/> to access the underlying pointer of an object.
/// </summary>
public class PinnedHandle : IDisposable
{
    // unmanaged memory pointer to the resource
    private IntPtr handle = IntPtr.Zero;

    // Whether or not mGCHandle will need freed when disposing.
    private bool isGCHandleOwner;

    // Alllocated in the ctor if needed
    private GCHandle mGCHandle;

    // Track whether Dispose has been called.
    private bool disposed;

    /// <summary>
    /// Creates a pinned object.
    /// </summary>
    /// <param name="objectToPin">
    /// The object can be any blittable class, or array.  If a <see cref="GCHandle"/> is passed it will be used "as-is" and no pinning will take place.
    /// </param>
    public PinnedHandle(object objectToPin)
    {
        if (!ReferenceEquals(objectToPin, null))
        {
            if (objectToPin is GCHandle)
            {
                // This object is already a GCHandle, just use its AddrOfPinnedObject()
                // This class will not free this GCHandle when dispposing.
                this.mGCHandle = (GCHandle)objectToPin;
                this.handle = this.mGCHandle.AddrOfPinnedObject();
            }
            else if (objectToPin is IntPtr)
            {
                // This object is an IntPtr, the user is manging this on his own.
                this.handle = (IntPtr)objectToPin;
            }
            else
            {
                // This is a blittable class or structure, or an array fo blittable class or structures.
                this.mGCHandle = GCHandle.Alloc(objectToPin, GCHandleType.Pinned);
                this.handle = this.mGCHandle.AddrOfPinnedObject();

                // This class will free the GcHandle when its disposed.
                this.isGCHandleOwner = true;
            }
        }
    }

    /// <summary>
    /// Gets the raw pointer in memory of the pinned object.
    /// </summary>
    public IntPtr Handle
    {
        get { return this.handle; }
    }

    #region IDisposable Members

    /// <summary>
    /// Frees and disposes the <see cref="GCHandle"/> for this pinned object.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public void Dispose()
    {
        this.Dispose(true);

        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SupressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Dispose of the <see cref="PinnedHandle"/>.
    /// </summary>
    /// <remarks>
    /// Dispose(bool disposing) executes in two distinct scenarios. 
    /// If disposing equals true, the method has been called directly 
    /// or indirectly by a user's code. Managed and unmanaged resources 
    /// can be disposed. 
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            if (this.isGCHandleOwner && this.handle != IntPtr.Zero)
            {
                this.isGCHandleOwner = false;
                this.handle = IntPtr.Zero;
                this.mGCHandle.Free();
            }

            // disposing has been done.
            this.disposed = true;

        }
    }
    #endregion

    ~PinnedHandle()
    {
        // This destructor will run only if the Dispose method
        // does not get called.

        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(false) is optimal in terms of
        // readability and maintainability.
        this.Dispose(false);
    }
}