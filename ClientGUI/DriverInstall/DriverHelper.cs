using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

    internal static class DriverHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsUserAnAdmin();

        private static unsafe string DriverInfoToString(SP_DRVINFO_DATA data)
        {
            return new string(data.ProviderName) + " " + new string(data.Description)
                + " " + new string(data.MfgName) + " " + data.DriverType.ToString();
        }

        // See https://stackoverflow.com/questions/27144063/check-if-a-windows-driver-exists-for-a-given-device-id/
        public static DriverStatus GetDriverInfo()
        {
            var hdevInfo = SetupDiGetClassDevs(null as Guid?, "USB\\VID_057E&PID_3006&MI_00", IntPtr.Zero, GetClassDevsFlags.DIGCF_ALLCLASSES);
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

             return drvInfo.Contains("libwdi") ? DriverStatus.Installed : DriverStatus.NotInstalled;
        }

        // if true sysdvr should quit
        public static bool InstallDriver()
        {
            if (IsUserAnAdmin())
            {
                InstallDriverInternal();
                return false;
            }
            else
            {
                Trace.WriteLine("User is not admin");

                var filePath = typeof(Program).Assembly.Location;
                var args = "--install-driver";

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

                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Hopefully it worked
                return true;
            }
        }

        static (int, string) RunAndGetOutput(string name, string args) {
            var psi = new ProcessStartInfo
            {
                FileName = name,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = args
            };

            var p = Process.Start(psi);
            var log = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            return (p.ExitCode, log);
        }

        static bool InstallDriverInternal() 
        {
            Trace.WriteLine("Installing driver...");

            // It seems the 32-bit version can install 64-bit drivers as well
            var wdiExe = Path.Combine(Program.OsArchGenericFolder, "wdi-simple32.exe");
            if (!File.Exists(wdiExe))
            {
                MessageBox.Show("wdi-simple32.exe is missing, did you download and extract SysDVR-Client properly ?");
                return false;
            }

            string log = "";

            var (ec, curlog) = RunAndGetOutput(wdiExe, "-n SysDVR -m exelix -v 0x057e -p 0x3006 -i 0 -t 0 -l 0");
            log += $"Installing Video interface: {ec}\r\n" + curlog;

            if (ec == 0)
            {
                (ec, curlog) = RunAndGetOutput(wdiExe, "-n SysDVR -m exelix -v 0x057e -p 0x3006 -i 1 -t 0 -l 0");
                log += $"Installing Audio interface: {ec}\r\n" + curlog;
            }

            if (ec == 0)
            {
                if (MessageBox.Show("Driver installed successfully\r\n\r\nUnplug and plug back in your console now for the changes to take effect.\r\n\r\nDo you want to open the log page ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    new DriverInstallResultForm(log).ShowDialog();
            }
            else 
            {
                MessageBox.Show("One or more errors occurred, the log will be shown");
                new DriverInstallResultForm(log).ShowDialog();
            }

            return true;
        }
    }
}
