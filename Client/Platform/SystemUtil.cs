using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Platform
{
    internal static class SystemUtil
    {
        public static bool OpenURL(string url)
        {
#if ANDROID_LIB
            return Program.Native.SysOpenURL(url);
#else
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                }
                catch { }
                return false;
            }
            return true;
#endif
        }

        const string SettingsFileName = "options.json";

        public static string? LoadSettingsString() 
        {
            var file = Path.Combine(Resources.SettingsStorePath, SettingsFileName);
            if (File.Exists(file))
                return File.ReadAllText(file);
            
            return null;
        }

        public static void StoreSettingsString(string json) 
        {
			var file = Path.Combine(Resources.SettingsStorePath, SettingsFileName);
			File.WriteAllText(file, json);
        }
    }
}
