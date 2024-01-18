using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SysDVR.Client.Platform
{
	internal static class Resources
	{
#if ANDROID_LIB
        // Android is such a shit platform that you can't fopen(), all resources must be read like this
        // thank god SDL already wraps its stupid interface
        public static byte[] ReadResouce(string path)
        {
            Console.WriteLine($"Loading resource {path}");

            var file = SDL.SDL_RWFromFile(path, "r").AssertNotNull(SDL.SDL_GetError);

            var len = (int)SDL.SDL_RWsize(file);
            var buf = new byte[len];

            var read = SDL.SDL_RWread(file, buf, 1, len);
            SDL.SDL_RWclose(file);

            if (read != len)
                throw new Exception($"Loading resource {path} failed: {SDL.SDL_GetError()}");
            
            return buf;
        }
        
        public static bool ResourceExists(string path) 
        {
            try 
            {
                LoadResource(path);
            }
            catch 
            {
                return false;
            }

            return true;
        }

        static string BasePath = "";        
        static string ResourcePath(string x) => x;

        public static bool HasDiskAccessPermission() 
        {
            if (!Program.Native.PlatformSupportsDiskAccess)
                return false;

            if (Program.Native.GetFileAccessInfo(out var canWrite, out _))
                return canWrite;
            else 
                Console.WriteLine("GetFileAccessInfo failed");

            return false;
        }

        public static bool CanRequestDiskAccessPermission() 
        {
            if (!Program.Native.PlatformSupportsDiskAccess)
                return false;

            if (Program.Native.GetFileAccessInfo(out _, out var canRequest))
                return canRequest;
            else
                Console.WriteLine("GetFileAccessInfo failed");

            return false;
        }

        public static void RequestDiskAccessPermission()
        {
            if (!Program.Native.PlatformSupportsDiskAccess)
                throw new NotImplementedException();

            if (!Program.Native.GetFileAccessInfo(out var hasPermission, out var canRequest))
                throw new NotImplementedException();

            if (hasPermission)
                return;

            if (!canRequest)
                return;

            Program.Native.RequestFileAccess();
        }
        
        private static string? _settingsStorePath = null;
        public static string SettingsStorePath()
        {
            if (_settingsStorePath is null)
                _settingsStorePath = Program.Native.GetSettingsStoragePath?.Invoke() ?? "";

            return _settingsStorePath;
        }

#else
		static string BasePath = Path.Combine(AppContext.BaseDirectory, "runtimes");
		static string ResourcePath(string x) => Path.Combine(BasePath, "resources", x);

		public static string? SettingsStorePath()
		{
			string? path = null;
			if (Program.IsContainerApp)
			{
				// https://github.com/exelix11/SysDVR/issues/255
				if (Program.IsLinux)
				{
					path = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
					if (string.IsNullOrWhiteSpace(path))
						path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".sysdvr");
				}
			}
			else
				path = AppContext.BaseDirectory;

			return path;
		}

		public static byte[] ReadResouce(string path) => File.ReadAllBytes(path);
		public static bool ResourceExists(string path) => File.Exists(path);

		public static bool HasDiskAccessPermission() => true;
		public static bool CanRequestDiskAccessPermission() => true;
		public static void RequestDiskAccessPermission() { }

        static string[] GetTranslationFiles()
        {
            if (!Directory.Exists(ResourcePath("strings")))
                return [];

            return Directory.GetFiles(ResourcePath("strings"), "*.json");
		}
#endif
		public static string RuntimesFolder => BasePath;
		public static string MainFont { get; private set; } = ResourcePath("fonts/OpenSans.ttf");
		public static string LoadingImage => ResourcePath("loading.yuv");

		public readonly static LazyImage Logo = new LazyImage(ResourcePath("logo.png"));
		public readonly static LazyImage UsbIcon = new LazyImage(ResourcePath("ico_usb.png"));
		public readonly static LazyImage WifiIcon = new LazyImage(ResourcePath("ico_wifi.png"));

		public static string? GetBuildId()
		{
			try
			{
				// Reflection doesn't usually work with AOT but this is a special case
				return typeof(Program).Assembly
					.GetCustomAttributes<AssemblyMetadataAttribute>()
					.FirstOrDefault(a => a.Key == "BuildCommit")?.Value;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Couldn't load build id file" + ex.ToString());
			}

			return null;
		}

        public static StringTableMetadata[] GetAvailableTranslations()
        {
            var files = GetTranslationFiles();
			var result = new List<StringTableMetadata>();

			foreach (var file in files)
            {
                try
                {
                    var table = JsonSerializer.Deserialize<StringTableMetadata>(ReadResouce(file), StringTableSerializer.Default.SysDVRStringTableMetadata);
                    if (table is null)
                        continue;

                    if (table.SystemLocale.Length == 0)
                    {
                        Console.WriteLine($"Translation {table.TranslationName} was not loaded due to a missing system locale");
                        continue;
                    }

					table.FileName = file;
					result.Add(table);
				}
                catch (Exception ex)
                {
                    Program.DebugLog($"Failed to load translation {file} : {ex}");    
                }
			}

			return result.ToArray();
        }

        public static StringTable? LoadtranslationFromAssetName(string fullAssetName)
        {
            return JsonSerializer.Deserialize<StringTable>(ReadResouce(fullAssetName), StringTableSerializer.Default.SysDVRStringTable) 
                ?? throw new Exception($"Failed to deserialize {fullAssetName}");
        }

        public static bool OverrideMainFont(string fontName)
        {
            var font = ResourcePath(fontName);
            if (!ResourceExists(font))
            {
				return false;
			}

			MainFont = font;
			return true;
        }
	}
}
