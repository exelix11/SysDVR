using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.SetupApi;

namespace SysDVR.Client.Platform.Windows
{
    public enum DriverStatus {
        Unknown,
        NotInstalled,
        Installed
    }

    internal static class DriverHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsUserAnAdmin();

        private static unsafe string DriverInfoToString(SP_DRVINFO_DATA data)
        {
            return new string(data.ProviderName) + " " + new string(data.Description);
        }

        // See https://stackoverflow.com/questions/27144063/check-if-a-windows-driver-exists-for-a-given-device-id/
        public static DriverStatus GetDriverInfo()
        {
            var hdevInfo = SetupDiGetClassDevs(null as Guid?, "USB\\VID_057E&PID_3006", IntPtr.Zero, GetClassDevsFlags.DIGCF_ALLCLASSES);
            if (hdevInfo.IsInvalid)
            {
                Console.WriteLine("Warning: GetDriverInfo SetupDiGetClassDevs failed");
                return DriverStatus.Unknown;
            }

            var devinfo = SP_DEVINFO_DATA.Create();
            if (!SetupDiEnumDeviceInfo(hdevInfo, 0, ref devinfo))
            {
                Console.WriteLine("Warning: GetDriverInfo SetupDiEnumDeviceInfo failed");
                return DriverStatus.Unknown;
            }

            if (!SetupDiBuildDriverInfoList(hdevInfo, devinfo, DriverType.SPDIT_COMPATDRIVER))
            {
                Console.WriteLine("Warning: GetDriverInfo SetupDiBuildDriverInfoList failed");
                return DriverStatus.Unknown;
            }

            var drvdata = SP_DRVINFO_DATA.Create();
            var gotDriverInfo = SetupDiEnumDriverInfo(hdevInfo, devinfo, DriverType.SPDIT_COMPATDRIVER, 0, ref drvdata);

            if (!gotDriverInfo)
            {
                var ec = Marshal.GetLastWin32Error();
                if (ec == 259) // ERROR_NO_MORE_ITEMS
                    return DriverStatus.NotInstalled;
                Console.WriteLine($"Warning: GetDriverInfo SetupDiEnumDriverInfo {ec}");
                return DriverStatus.Unknown;
            }
            else
            {
#if DEBUG
                Console.WriteLine($"Current SysDVR driver: {DriverInfoToString(drvdata)}");
#endif
                return DriverStatus.Installed;
            }
        }

        // Returns false if sysdvr should quit
        public static bool InstallDriver()
        {
            if (IsUserAnAdmin())
                return InstallDriverInternal();
            else 
            {
                Console.WriteLine("To install the driver SysDVR must be restarted as admin");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();

                var filePath = typeof(Program).Assembly.Location;
                var args = "--force-install-driver";

                // This seems to be always true
                if (filePath.EndsWith(".dll"))
                {
                    args = $"\"{filePath}\" {args}";
                    filePath = "dotnet";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = args
                };

                Process.Start(psi);

                // Hopefully it worked
                return false;
            }
        }

        static bool InstallDriverInternal() 
        {
            Console.WriteLine("Installing driver...");

            var wdiExe = Path.Combine(Program.OsNativeFolder, "wdi-simple.exe");
            if (!File.Exists(wdiExe))
            {
                Console.WriteLine("wdi-simple.exe is missing, did you download and extract SysDVR-Client properly ?");
                return false;
            }

            var psi = new ProcessStartInfo
            {
                FileName = wdiExe,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = "-n SysDVR -v 0x057e -p 0x3006 -t 0"
            };

            var p = Process.Start(psi);
            Console.WriteLine(p.StandardOutput.ReadToEnd());

            p.WaitForExit();

            if (p.ExitCode == 0)
            {
                Console.WriteLine("Driver installed successfully");
                Console.WriteLine("You may need to restart SysDVR and unplug your console for the changes to take effect");
            }

            Console.WriteLine("Press any key to continue");
            Console.Read();
            
            return true;
        }
    }
}
