﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PInvoke.SetupApi;

namespace SysDVRClientGUI.DriverInstall
{
    public enum DriverStatus {
        Unknown,
        NotInstalled,
        Installed
    }

    internal static partial class DriverHelper
    {
        [LibraryImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsUserAnAdmin();

        private static unsafe string DriverInfoToString(SP_DRVINFO_DATA data)
        {
            return new string(data.ProviderName) + " " + new string(data.Description)
                + " " + new string(data.MfgName) + " " + data.DriverType.ToString();
        }

        // See https://stackoverflow.com/questions/27144063/check-if-a-windows-driver-exists-for-a-given-device-id/
        public static DriverStatus GetDriverInfo()
        {
            var hdevInfo = SetupDiGetClassDevs(null as Guid?, "USB\\VID_18D1&PID_4EE0", IntPtr.Zero, GetClassDevsFlags.DIGCF_ALLCLASSES);
            if (hdevInfo.IsInvalid)
            {
                Trace.WriteLine("Warning: GetDriverInfo SetupDiGetClassDevs failed");
                return DriverStatus.Unknown;
            }

            var devinfo = SP_DEVINFO_DATA.Create();
            if (!SetupDiEnumDeviceInfo(hdevInfo, 0, ref devinfo))
            {
                Trace.WriteLine("Warning: GetDriverInfo SetupDiEnumDeviceInfo failed");
                return DriverStatus.Unknown;
            }

            if (!SetupDiBuildDriverInfoList(hdevInfo, devinfo, DriverType.SPDIT_COMPATDRIVER))
            {
                Trace.WriteLine("Warning: GetDriverInfo SetupDiBuildDriverInfoList failed");
                return DriverStatus.Unknown;
            }

            var drvdata = SP_DRVINFO_DATA.Create();
            string drvInfo = "";
            uint i = 0;
            while (SetupDiEnumDriverInfo(hdevInfo, devinfo, DriverType.SPDIT_COMPATDRIVER, i++, ref drvdata))
            {
                drvInfo += DriverInfoToString(drvdata) + "\r\n\r\n";
            }
            
             Trace.WriteLine($"Current SysDVR driver: {drvInfo}");

             return drvInfo.ToLower().Contains("android") ? DriverStatus.Installed : DriverStatus.NotInstalled;
        }

        public static async Task DownloadDriver() 
        {
            if (Directory.Exists("usb_driver_r13-windows"))
            {
                if (File.Exists("usb_driver_r13-windows\\usb_driver\\android_winusb.inf"))
                    return;

                Directory.Delete("usb_driver_r13-windows", true);
            }

            byte[] driver, hash;

            using (var cli = new HttpClient())
                driver = await cli.GetByteArrayAsync("https://dl.google.com/android/repository/usb_driver_r13-windows.zip");

            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(driver);

            var str = BitConverter.ToString(hash).Replace("-", "").ToLower();

            var expected = "360b01d3dfb6c41621a3a64ae570dfac2c9a40cca1b5a1f136ae90d02f5e9e0b";
            if (str != expected)
            {
                throw new Exception($"The downloaded driver hash doesn't match, try again or open an issue on GitHub.");
            }

            File.WriteAllBytes("usb_driver_r13-windows.zip", driver);
            
            ZipFile.ExtractToDirectory("usb_driver_r13-windows.zip", "usb_driver_r13-windows");
            File.Delete("usb_driver_r13-windows.zip");

            if (!File.Exists("usb_driver_r13-windows\\usb_driver\\android_winusb.inf"))
            {
                throw new Exception($"The downloaded archive doesn't contain all the needed files.");
            }
        }

        public static void DeleteTempDir() 
        {
            if (Directory.Exists("usb_driver_r13-windows"))
            {
                Directory.Delete("usb_driver_r13-windows", true);
            }
        }

        public static void InstallDriver()
        {
            var info = new ProcessStartInfo()
            {
                FileName = "infdefaultinstall",
                Arguments = $"\"{Path.GetFullPath("usb_driver_r13-windows\\usb_driver\\android_winusb.inf")}\""
            };

            Process.Start(info).WaitForExit();
        }
    }
}
