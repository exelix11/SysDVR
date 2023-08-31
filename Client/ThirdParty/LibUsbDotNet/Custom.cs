using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibUsbDotNet
{
    public enum LibUsbOption : int
    {
        LOG_LEVEL = 0,
        USE_USBDK = 1,
        NO_DEVICE_DISCOVERY = 2
    }

    internal static unsafe partial class NativeMethods
    {
        [DllImport(LibUsbNativeLibrary, EntryPoint = "libusb_set_option")]
        public static extern Error SetOption(IntPtr ctx, LibUsbOption option, nint arg);
        
        [DllImport(LibUsbNativeLibrary, EntryPoint = "libusb_wrap_sys_device")]
        public static extern Error WrapSystemHanlde(Context ctx, nint nativeHandle, out IntPtr libusbHandle);
    }
}
