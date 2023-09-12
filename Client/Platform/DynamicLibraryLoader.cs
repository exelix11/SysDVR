using FFmpeg.AutoGen;
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

#if ANDROID_LIB
        static readonly string[] InvalidBuildHash = new[]
        {
            "Y29tLmdvb2dsZS5hbmRyb2lkLmZlZWRiYWNr",
            "Y29tLmFuZHJvaWQudmVuZGluZw=="
        };

        static void AndroidQuerySysPaths(out string nativePath, out string managedPath)
        {
            byte[] Info = new byte[4096];
            Program.Native.SysGetDynamicLibInfo(Info, Info.Length);
            var str = Encoding.Unicode.GetString(Info, 0, Info.Length).Split('\0', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            managedPath = str[0];
            nativePath = str.Length > 1 ? str[1] : "";
        }

        static bool AndroidCheckDependencies(string native, string managed) 
        {
            native = Convert.ToBase64String(Encoding.ASCII.GetBytes(native));
            // These build hashes from releases are known bad because they don't contain all the required symbols
            // When using CI android native libs should be built with the same toolchain as the one used to build the app
            if (InvalidBuildHash.Contains(native))
                return false;

            // no check on managed paths for now...
            return true;
        }

#else
        // TODO: figure out mac os support, it's currently broken
        static IEnumerable<string> FindMacOSLibrary(string libraryName)
        {
            // TODO: Account for overide and https://apple.stackexchange.com/questions/40704/homebrew-installed-libraries-how-do-i-use-them
            // if (OperatingSystem.IsMacOS())
            //     return RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "/opt/homebrew/lib/" : "/usr/local/lib/";

            var names = new[] {
                Path.Combine(BundledOsNativeFolder, $"lib{libraryName}.dylib"),
                Path.Combine(BundledOsNativeFolder, $"{libraryName}.dylib"),
                $"lib{libraryName}.dylib",
                $"{libraryName}.dylib",
                libraryName
            };

            return names;
        }

        static IEnumerable<string> FindNativeLibrary(string libraryName)
        {
            var libext = OperatingSystem.IsWindows() ? ".dll" : ".so";
            
            if (!libraryName.EndsWith(libext))
                libraryName += libext;

            // Is an override set ?
            if (!string.IsNullOrWhiteSpace(LibLoaderOverride))
                yield return Path.Combine(LibLoaderOverride, libraryName);

            // Is this a library we provide ?
            yield return Path.Combine(BundledOsNativeFolder, libraryName);

            // Otherwsie let the OS handle it
            yield return libraryName;
        }

        public static IntPtr TryLoadLibrary(string name) 
        {
            return BundledLibraryLoader(name, null, null);
        }

        static IntPtr BundledLibraryLoader(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            foreach (var candidate in FindNativeLibrary(libraryName).Where(File.Exists))
            {
                if (NativeLibrary.TryLoad(candidate , out var result))
                    return result;

                Console.WriteLine($"Failrd to load {candidate}");
            }

            Console.WriteLine($"Failed to find library: {libraryName}");
            return IntPtr.Zero;
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

            var thisExePath = Path.GetDirectoryName(AppContext.BaseDirectory);

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
#endif

        public static void Initialize()
        {
#if ANDROID_LIB
            AndroidQuerySysPaths(out var native, out var managed);
            if (!AndroidCheckDependencies(native, managed))
               throw new Exception("Native android dependencies are missing, possibly they are missing from the APK path. Note that on android SysDVR supports only arm64 builds.");
#else
            // TODO: All of this has to be re-tested since now we bundle our own dependencies
            
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), BundledLibraryLoader);

            if (OperatingSystem.IsMacOS())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64 && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You're using the intel version of dotnet on an arm mac, this is not supported and will likely not work.");
                    Console.WriteLine("Delete intel dotnet and install the native arm64 one.");
                    Console.ResetColor();
                }

                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), MacOsLibraryLoader);
                SetupMacOSLibrarySymlinks();
            }
#endif
        }
    }
}
