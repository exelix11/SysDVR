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

        public delegate bool SysOpenURL([MarshalAs(UnmanagedType.LPWStr)] string url);

        public delegate void SysGetDynamicLibInfo(byte[] buffer, int length);

        // Returns true if success, the two flags indicate if we can write files and if we can request permissions
        public delegate bool GetFileAccessPermissionInfo(out bool hasWriteAccess, out bool canRequestAccess);

        public delegate void RequestFileAccessPermission();

		[return: MarshalAs(UnmanagedType.LPStr)]
		public delegate string GetSettingsStoragePath();

        // Not an android function
		public delegate bool IterateAssetsContentCallback(nint ptr, int characters);

		public delegate void IterateAssetsContent([MarshalAs(UnmanagedType.LPWStr)] string path, IterateAssetsContentCallback callback);
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

        // Thread management, keep private and only use it for EnsureThreadAttached()
        // TODO: Currently we have no way of detaching .NET async worker threads
        NativeContracts.NativeAttachThread NativeAttachThread;
        NativeContracts.NativeDetachThread NativeDetachThread;

        // Usb control, if any is null then usb is not supported
        public NativeContracts.UsbSnapshotDevices UsbAcquireSnapshot;
        public NativeContracts.UsbFreeSnapshot UsbReleaseSnapshot;
        public NativeContracts.UsbGetSnapshotDeviceSerial UsbGetSnapshotDeviceSerial;
        public NativeContracts.UsbOpenHandle UsbOpenHandle;
        public NativeContracts.UsbCloseHandle UsbCloseHandle;
        public NativeContracts.UsbGetError UsbGetLastError;

        // System utilities, should be populated
        public NativeContracts.SysOpenURL SysOpenURL;
        public NativeContracts.SysGetDynamicLibInfo SysGetDynamicLibInfo;

        // System utilities, can be null
        public NativeContracts.GetFileAccessPermissionInfo GetFileAccessInfo;
        public NativeContracts.RequestFileAccessPermission RequestFileAccess;
        public NativeContracts.GetSettingsStoragePath GetSettingsStoragePath;
        public NativeContracts.IterateAssetsContent IterateAssetsContent;

		public bool PlatformSupportsUsb => 
            UsbAcquireSnapshot != null && UsbReleaseSnapshot != null &&
            UsbGetSnapshotDeviceSerial != null && UsbOpenHandle != null &&
            UsbCloseHandle != null && UsbGetLastError != null;

        public bool PlatformSupportsDiskAccess =>
            GetFileAccessInfo != null;

        [ThreadStatic]
        public static bool NativeThreadAttached;

        // On android certain actions require the thread to be attached to the JVM
        // However we use .NET async which may create worker threads without us knowing
        // So must call this function before making any native calls in contextes where we may be running on async workers
        // Right now, this only happens with the USB code
        public void EnsureThreadAttached() 
        {
            if (!NativeThreadAttached)
            {
                NativeThreadAttached = true;
                NativeAttachThread();
            }
        }

        public void DetachThread() 
        {
            if (NativeThreadAttached)
            {
                NativeThreadAttached = false;
                NativeDetachThread();
            }
        }

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

            // System utilities
            public IntPtr SysOpenURL;
            public IntPtr SysGetDynamicLibInfo;
            public IntPtr SysGetFileAccessInfo;
            public IntPtr SysRequestFileAccess;
            public IntPtr SysGetSettingsStoragePath;
            public IntPtr SysIterateAssetsContent;
        }

        public unsafe static NativeError Read(IntPtr ptr, out NativeInitBlock native)
        {
            if (ptr == IntPtr.Zero)
            {
                native = default;
                return NativeError.NativePtrMissing;
            }

            var repr = *(NativeInitRepr*)ptr;

            if (repr.Sizeof != Marshal.SizeOf<NativeInitRepr>())
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

                SysOpenURL = repr.SysOpenURL == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.SysOpenURL>(repr.SysOpenURL),
                SysGetDynamicLibInfo = repr.SysGetDynamicLibInfo == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.SysGetDynamicLibInfo>(repr.SysGetDynamicLibInfo),
                
                GetFileAccessInfo = repr.SysGetFileAccessInfo == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.GetFileAccessPermissionInfo>(repr.SysGetFileAccessInfo),
                RequestFileAccess = repr.SysRequestFileAccess == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.RequestFileAccessPermission>(repr.SysRequestFileAccess),
				GetSettingsStoragePath = repr.SysGetSettingsStoragePath == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.GetSettingsStoragePath>(repr.SysGetSettingsStoragePath),
                IterateAssetsContent = repr.SysIterateAssetsContent == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<NativeContracts.IterateAssetsContent>(repr.SysIterateAssetsContent),
			};

            return NativeError.Success;
        }
    }
}
