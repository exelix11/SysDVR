using System;
using System.Runtime.InteropServices;

namespace SysDVR.Client.Core
{
    public static class NativeContracts 
    {
        // Takes an ascii string, unlike all the other functions, print must be callable without attaching the thread
        public delegate void PrintFunction([MarshalAs(UnmanagedType.LPStr)] string log);

        // Mark this thread as one which might make native calls
        public delegate void NativeAttachThread();

        // Terminate a previously attached thread
        public delegate void NativeDetachThread();

        // Captures a snapshot of current devices, returns true if success
        // If success, deviceCount is set to the number of devices
        public delegate bool UsbSnapshotDevices(int vid, int pid, out int deviceCount);
        
        // Frees the device snapshot
        public delegate void UsbFreeSnapshot();

        // Returns the device serial at the given index of the last snapshot
        // the string pointer should be wchar*
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public delegate string UsbGetSnapshotDeviceSerial(int idx);

        // Gets the last usb system error as a wide string
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public delegate string UsbGetError();

        // Open a device handle
        public delegate bool UsbOpenHandle([MarshalAs(UnmanagedType.LPWStr)] string serial, out nint handle);

        // Close a device handle
        public delegate void UsbCloseHandle(nint handle);
    }

    public enum NativeError : int 
    {
        Success = 0,
        NativePtrMissing = 1,
        NativeVersionMismatch = 2,
        NativeSizeMismatch = 3,
    }

    public struct NativeInitBlock 
    {
        public const nint BlockVersion = 1;

        // General info, must be populated
        public NativeContracts.PrintFunction Print;

        // Threat management
        public NativeContracts.NativeAttachThread NativeAttachThread;
        public NativeContracts.NativeDetachThread NativeDetachThread;

        // Usb control, if any is null then usb is not supported
        public NativeContracts.UsbSnapshotDevices UsbAcquireSnapshot;
        public NativeContracts.UsbFreeSnapshot UsbReleaseSnapshot;
        public NativeContracts.UsbGetSnapshotDeviceSerial UsbGetSnapshotDeviceSerial;
        public NativeContracts.UsbOpenHandle UsbOpenHandle;
        public NativeContracts.UsbCloseHandle UsbCloseHandle;
        public NativeContracts.UsbGetError UsbGetLastError;

        public bool PlatformSupportsUsb => 
            UsbAcquireSnapshot != null && UsbReleaseSnapshot != null &&
            UsbGetSnapshotDeviceSerial != null && UsbOpenHandle != null &&
            UsbCloseHandle != null && UsbGetLastError != null;

        // This is the native representation of the init block which we must unmarshal manually
        // [MarshalAs(UnmanagedType.FunctionPtr)] doesn't seem to work on delegates...
        [StructLayout(LayoutKind.Sequential)]
        struct NativeInitRepr
        {
            public nint Version;
            public nint Sizeof;
            public IntPtr Print;

            // Threat management
            public IntPtr NativeAttachThread;
            public IntPtr NativeDetachThread;

            // Usb control, if any is null then usb is not supported
            public IntPtr UsbAcquireSnapshot;
            public IntPtr UsbReleaseSnapshot;
            public IntPtr UsbGetSnapshotDeviceSerial;
            public IntPtr UsbOpenHandle;
            public IntPtr UsbCloseHandle;
            public IntPtr UsbGetLastError;
        }

        public unsafe static NativeError Read(IntPtr ptr, out NativeInitBlock native)
        {
            if (ptr == IntPtr.Zero)
            {
                native = default;
                return NativeError.NativePtrMissing;
            }

            var repr = *(NativeInitRepr*)ptr;

            if (repr.Sizeof != Marshal.SizeOf(typeof(NativeInitRepr)))
            {
                native = default;
                return NativeError.NativeSizeMismatch;
            }

            if (repr.Version != BlockVersion)
            {
                native = default;
                return NativeError.NativeVersionMismatch;
            }
            
            native = new NativeInitBlock
            {
                Print = Marshal.GetDelegateForFunctionPointer<NativeContracts.PrintFunction>(repr.Print),

                NativeAttachThread = repr.NativeAttachThread == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.NativeAttachThread>(repr.NativeAttachThread),
                NativeDetachThread = repr.NativeDetachThread == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.NativeDetachThread>(repr.NativeDetachThread),

                UsbAcquireSnapshot = repr.UsbAcquireSnapshot == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbSnapshotDevices>(repr.UsbAcquireSnapshot),
                UsbReleaseSnapshot = repr.UsbReleaseSnapshot == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbFreeSnapshot>(repr.UsbReleaseSnapshot),
                UsbGetSnapshotDeviceSerial = repr.UsbGetSnapshotDeviceSerial == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbGetSnapshotDeviceSerial>(repr.UsbGetSnapshotDeviceSerial),
                UsbOpenHandle = repr.UsbOpenHandle == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbOpenHandle>(repr.UsbOpenHandle),
                UsbCloseHandle = repr.UsbCloseHandle == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbCloseHandle>(repr.UsbCloseHandle),
                UsbGetLastError = repr.UsbGetLastError == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.UsbGetError>(repr.UsbGetLastError),
            };

            return NativeError.Success;
        }
    }
}
