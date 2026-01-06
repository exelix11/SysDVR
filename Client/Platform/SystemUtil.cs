using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

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
			var path = Resources.SettingsStorePath();

			if (string.IsNullOrWhiteSpace(path))
				throw new Exception(Program.Strings.Errors.SettingsPathMissing);

			var file = Path.Combine(path, SettingsFileName);
			if (File.Exists(file))
				return File.ReadAllText(file);

			return null;
		}

		public static void StoreSettingsString(string json)
		{
			var path = Resources.SettingsStorePath();
			if (string.IsNullOrWhiteSpace(path))
				throw new Exception(Program.Strings.Errors.SettingsPathMissing);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var file = Path.Combine(path, SettingsFileName);
			File.WriteAllText(file, json);
		}

		public static string GetLanguageCode() 
		{
			return CultureInfo.CurrentUICulture.Name;
		}
	}
}
