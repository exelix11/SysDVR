using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Platform
{
    internal static class DynamicLibraryLoader
    {
        static string ArchName => RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "-x64",
            Architecture.X86 => "-x86",
            Architecture.Arm => "-arm",
            Architecture.Arm64 => "-arm64",
            _ => ""
        };

        static string OsName
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return "win";
                if (OperatingSystem.IsMacOS())
                    return "mac";
                else return "linux";
            }
        }

        static string BundledOsNativeFolder => Path.Combine(Resources.RuntimesFolder, $"{OsName}{ArchName}", "native");

        public static string? LibLoaderOverride = null;

        static string OsLibFolder
        {
            get
            {
                if (LibLoaderOverride is not null)
                    return LibLoaderOverride;

                if (OperatingSystem.IsWindows())
                    return BundledOsNativeFolder;

                // Should we really account for misconfigured end user PCs ? See https://apple.stackexchange.com/questions/40704/homebrew-installed-libraries-how-do-i-use-them
                if (OperatingSystem.IsMacOS())
                    return RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "/opt/homebrew/lib/" : "/usr/local/lib/";

                // On linux we have to rely on the dlopen implementation to find the libs wherever they are 
                return string.Empty;
            }
        }

        static IEnumerable<string> FindMacOSLibrary(string libraryName)
        {
            var names = new[] {
                Path.Combine(OsLibFolder, $"lib{libraryName}.dylib"),
                Path.Combine(OsLibFolder, $"{libraryName}.dylib"),
                Path.Combine(BundledOsNativeFolder, $"lib{libraryName}.dylib"),
                Path.Combine(BundledOsNativeFolder, $"{libraryName}.dylib"),
                $"lib{libraryName}.dylib",
                $"{libraryName}.dylib",
                libraryName
            };

            return names;
        }

        static IntPtr MacOsLibraryLoader(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr result = IntPtr.Zero;
            foreach (var name in FindMacOSLibrary(libraryName))
                if (NativeLibrary.TryLoad(name, out result))
                    break;

            if (result == IntPtr.Zero)
                Console.Error.WriteLine($"Warning: couldn't load {libraryName} for {assembly.FullName} ({searchPath}).");

            return result;
        }

        static void SetupMacOSLibrarySymlinks()
        {
            // This is a terrible hack but seems to work, we'll create symlinks to the OS libraries in the program folder
            // The alternative is to fork libusbdotnet to add a way to load its native lib from a cusstom folder
            // See https://github.com/exelix11/SysDVR/issues/192

            var thisExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // We only need to link libusb as ffmpeg has a global root path variable and for SDL we set the custom NativeLoader callback
            var libNames = new[] {
                        ("libusb-1.0", "libusb-1.0.dylib")
                    };

            foreach (var (libName, fileName) in libNames)
            {
                if (File.Exists(Path.Combine(thisExePath, fileName)))
                    continue;

                var path = FindMacOSLibrary(libName).Where(File.Exists).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(path))
                    Console.Error.WriteLine($"Couldn't find a library to symlink: {libName} ({fileName}). You might need to install it with brew.");
                else
                    File.CreateSymbolicLink(Path.Combine(thisExePath, fileName), path);
            }
        }

        public static void Initialize()
        {
            ffmpeg.RootPath = OsLibFolder;
            if (OperatingSystem.IsMacOS())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64 && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You're using the intel version of dotnet on an arm mac, this is not supported and will likely not work.");
                    Console.WriteLine("Delete intel dotnet and install the native arm64 one.");
                    Console.ResetColor();
                }

                NativeLibrary.SetDllImportResolver(typeof(SDL2.SDL).Assembly, MacOsLibraryLoader);
                SetupMacOSLibrarySymlinks();
            }
        }
    }
}
