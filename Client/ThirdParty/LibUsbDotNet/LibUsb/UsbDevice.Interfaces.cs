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
using System.Collections.Generic;

namespace LibUsbDotNet.LibUsb;

// Implements functionality for the UsbDevice class, related to Interfaces
public partial class UsbDevice
{
    private readonly List<int> mClaimedInterfaces = new List<int>();
    private readonly byte[] usbAltInterfaceSettings = new byte[UsbConstants.MaxDeviceCount];

    /// <summary>
    /// Claims the specified interface of the device.
    /// </summary>
    /// <param name="interfaceID">The interface to claim.</param>
    /// <returns>True on success.</returns>
    public bool ClaimInterface(int interfaceID)
    {
        this.EnsureOpen();

        if (this.mClaimedInterfaces.Contains(interfaceID))
        {
            return true;
        }

        NativeMethods.ClaimInterface(this.deviceHandle, interfaceID).ThrowOnError();
        this.mClaimedInterfaces.Add(interfaceID);
        return true;
    }
    
    public bool GetAltInterface(out int alternateID)
    {
        int interfaceID = this.mClaimedInterfaces.Count == 0 ? 0 : this.mClaimedInterfaces[this.mClaimedInterfaces.Count - 1];
        return this.GetAltInterface(interfaceID, out alternateID);
    }

    /// <summary>
    /// Gets the alternate interface number for the specified interfaceID.
    /// </summary>
    /// <param name="interfaceID">The interface number of to get the alternate setting for.</param>
    /// <param name="alternateID">The currrently selected alternate interface number.</param>
    /// <returns>True on success.</returns>
    public bool GetAltInterface(int interfaceID, out int alternateID)
    {
        alternateID = this.usbAltInterfaceSettings[interfaceID & (UsbConstants.MaxDeviceCount - 1)];
        return true;
    }

    /// <summary>
    /// Gets the selected alternate interface of the specified interface.
    /// </summary>
    /// <param name="interfaceID">The interface settings number (index) to retrieve the selected alternate interface setting for.</param>
    /// <param name="selectedAltInterfaceID">The alternate interface setting selected for use with the specified interface.</param>
    public void GetAltInterfaceSetting(byte interfaceID, out byte selectedAltInterfaceID)
    {
        byte[] buf = new byte[1];
        int uTransferLength;

        UsbSetupPacket setupPkt = new UsbSetupPacket();
        setupPkt.RequestType = (byte)EndpointDirection.In | (byte)UsbRequestType.TypeStandard |
                               (byte)UsbRequestRecipient.RecipInterface;
        setupPkt.Request = (byte)StandardRequest.GetInterface;
        setupPkt.Value = 0;
        setupPkt.Index = interfaceID;
        setupPkt.Length = 1;

        uTransferLength = this.ControlTransfer(setupPkt, buf, 0, buf.Length);
        if (uTransferLength == 1)
        {
            selectedAltInterfaceID = buf[0];
        }
        else
        {
            selectedAltInterfaceID = 0;
        }
    }

    /// <summary>
    /// Releases an interface that was previously claimed with <see cref="ClaimInterface"/>.
    /// </summary>
    /// <param name="interfaceID">The interface to release.</param>
    /// <returns>True on success.</returns>
    public bool ReleaseInterface(int interfaceID)
    {
        this.EnsureOpen();

        var ret = NativeMethods.ReleaseInterface(this.deviceHandle, interfaceID);
        this.mClaimedInterfaces.Remove(interfaceID);
        ret.ThrowOnError();
        return true;
    }

    /// <summary>
    /// Sets an alternate interface for the most recent claimed interface.
    /// </summary>
    /// <param name="interfaceID">THe most recent claimed interface.</param>
    /// <param name="alternateID">The alternate interface to select for the most recent claimed interface See <see cref="ClaimInterface"/>.</param>
    /// <returns>True on success.</returns>
    public bool SetAltInterface(int interfaceID, int alternateID)
    {
        this.EnsureOpen();

        NativeMethods.SetInterfaceAltSetting(this.deviceHandle, interfaceID, alternateID).ThrowOnError();
        this.usbAltInterfaceSettings[interfaceID & (UsbConstants.MaxDeviceCount - 1)] = (byte)alternateID;
        return true;
    }

    /// <summary>
    /// Sets an alternate interface for the most recent claimed interface.
    /// </summary>
    /// <param name="alternateID">The alternate interface to select for the most recent claimed interface See <see cref="ClaimInterface"/>.</param>
    /// <returns>True on success.</returns>
    public bool SetAltInterface(int alternateID)
    {
        if (this.mClaimedInterfaces.Count == 0)
        {
            throw new UsbException("You must claim an interface before setting an alternate interface.");
        }

        return this.SetAltInterface(this.mClaimedInterfaces[this.mClaimedInterfaces.Count - 1], alternateID);
    }
}