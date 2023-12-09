#if WINDOWS
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace SysDVR.Client.Platform.Specific.Win
{
	internal static class SetupApi
	{
		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		private const int LINE_LEN = 256;

		[Flags]
		public enum GetClassDevsFlags
		{
			DIGCF_ALLCLASSES = 0x00000004,
			DIGCF_DEVICEINTERFACE = 0x00000010,
			DIGCF_DEFAULT = 0x00000001,
			DIGCF_PRESENT = 0x00000002,
			DIGCF_PROFILE = 0x00000008,
		}

		public class SafeDeviceInfoSetHandle : SafeHandle
		{
			public static readonly SafeDeviceInfoSetHandle Invalid = new SafeDeviceInfoSetHandle();
			public SafeDeviceInfoSetHandle()
				: base(INVALID_HANDLE_VALUE, true) { }

			public SafeDeviceInfoSetHandle(IntPtr preexistingHandle, bool ownsHandle = true)
				: base(INVALID_HANDLE_VALUE, ownsHandle)
			{
				this.SetHandle(preexistingHandle);
			}

			public override bool IsInvalid => this.handle == INVALID_HANDLE_VALUE;
			protected override bool ReleaseHandle() => SetupDiDestroyDeviceInfoList(this.handle);
		}

		[Flags]
		public enum DeviceInterfaceDataFlags : uint
		{
			SPINT_ACTIVE,
			SPINT_DEFAULT,
			SPINT_REMOVED,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct SP_DEVINFO_DATA
		{
			public int Size;
			public Guid ClassGuid;
			public uint DevInst;
			public IntPtr Reserved;
			public static unsafe SP_DEVINFO_DATA Create() => new SP_DEVINFO_DATA { Size = sizeof(SP_DEVINFO_DATA) };
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
		public unsafe struct SP_DRVINFO_DATA
		{
			public int cbSize;
			public DriverType DriverType;
			public UIntPtr Reserved;
			public fixed char Description[SetupApi.LINE_LEN];
			public fixed char MfgName[SetupApi.LINE_LEN];
			public fixed char ProviderName[SetupApi.LINE_LEN];
			public FileTime DriverDate;
			public ulong DriverVersion;
			public static SP_DRVINFO_DATA Create() => new SP_DRVINFO_DATA { cbSize = sizeof(SP_DRVINFO_DATA) };
		}

		[Flags]
		public enum DriverType : uint
		{
			SPDIT_NODRIVER = 0x00000000,
			SPDIT_CLASSDRIVER = 0x00000001,
			SPDIT_COMPATDRIVER = 0x00000002,
		}

		[DllImport(nameof(SetupApi), SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

		[DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern unsafe SafeDeviceInfoSetHandle SetupDiGetClassDevs(
			IntPtr classGuid,
			string enumerator,
			IntPtr hwndParent,
			GetClassDevsFlags flags);

		[DllImport(nameof(SetupApi), SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern unsafe bool SetupDiEnumDeviceInfo(
			SafeDeviceInfoSetHandle deviceInfoSet,
			int memberIndex,
			ref SP_DEVINFO_DATA deviceInfoData);

		[DllImport(nameof(SetupApi), SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static unsafe extern bool SetupDiBuildDriverInfoList(
			SafeDeviceInfoSetHandle deviceInfoSet,
			in SP_DEVINFO_DATA deviceInfoData,
			DriverType driverType);

		[DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static unsafe extern bool SetupDiEnumDriverInfo(
			SafeDeviceInfoSetHandle deviceInfoSet,
			in SP_DEVINFO_DATA deviceInfoData,
			DriverType driverType,
			uint memberIndex,
			ref SP_DRVINFO_DATA driverInfoData);
	}
}
#endif