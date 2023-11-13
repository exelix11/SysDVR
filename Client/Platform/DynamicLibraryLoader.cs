using FFmpeg.AutoGen;
using SDL2;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SysDVR.Client.Platform
{
    internal static class DynamicLibraryLoader
    {
        // If something goes wrong and we want to show a critical warning to the user, we'll set this (assuming we get that far)
        public static string? CriticalWarning;

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
                    return "osx";
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
#endif

        static readonly Regex versionNumberRegex = new Regex(@"^(.*?)[\.\-][\d\.]+\.(dylib|dll)$", RegexOptions.IgnoreCase);
        static string? StripVersionNumber(string libraryName)
        {
            var match = versionNumberRegex.Match(libraryName);
            if (match.Success)
                return match.Groups[1].Value + Path.GetExtension(libraryName);
            else
                return null;
        }

        static IEnumerable<string> FindNativeLibrary(string libraryName)
        {
            var libext = ".so";
                
            if (OperatingSystem.IsWindows()) libext = ".dll";
            else if (OperatingSystem.IsMacOS()) libext = ".dylib";

            if (!libraryName.EndsWith(libext))
                libraryName += libext;

            // Is an override set ?
            if (!string.IsNullOrWhiteSpace(LibLoaderOverride))
                yield return Path.Combine(LibLoaderOverride, libraryName);

            // Is this a library we provide ?
            yield return Path.Combine(BundledOsNativeFolder, libraryName);

            // Flatpak uses this path
            if (OperatingSystem.IsLinux())
                yield return Path.Combine("/app/lib", libraryName);

            // Maybe it's in the working directory
            yield return libraryName;
        }

        public static IntPtr TryLoadLibrary(string name)
        {
            return BundledLibraryLoader(name, null, null);
        }

        static IntPtr BundledLibraryLoader(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var paths = FindNativeLibrary(libraryName)
                .Concat(FindNativeLibrary("lib" + libraryName));

            var debug = Program.Options.Debug.DynLib;

            foreach (var candidate in paths)
            {
                if (!File.Exists(candidate))
                {
                    if (debug)
                        Console.WriteLine($"Candidate library does not exist {candidate}");
                    continue;
                }

                if (debug)
                    Console.WriteLine($"Trying to load {candidate}");

                if (NativeLibrary.TryLoad(candidate, out var result))
                    return result;

                if (debug)
                    Console.WriteLine($"Failrd to load {candidate}");
            }

            // Try to load the library from the OS.
            // This is not included in the above search because it's filtered by the File.Exists
            {
                IntPtr result = IntPtr.Zero;

                if (NativeLibrary.TryLoad(libraryName, out result))
                    return result;

                if (NativeLibrary.TryLoad($"lib{libraryName}", out result))
                    return result;
            }

            // Try again, but without the version number
            if (StripVersionNumber(libraryName) is string withoutNumber)
            {
                return BundledLibraryLoader(withoutNumber, assembly, searchPath);
            }

            Console.WriteLine("Failed to load " +  libraryName);
            return IntPtr.Zero;
        }

        public static void Initialize()
        {
#if ANDROID_LIB
            AndroidQuerySysPaths(out var native, out var managed);
            if (!AndroidCheckDependencies(native, managed))
               throw new Exception("Native android dependencies are missing, possibly they are missing from the APK path. Note that on android SysDVR supports only arm64 builds.");
#else
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), BundledLibraryLoader);

            if (OperatingSystem.IsMacOS())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64 && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    CriticalWarning =
                        "You're using the intel version of dotnet on an arm mac, this is not supported and will likely not work." + Environment.NewLine +
                        "Delete intel dotnet and install the native arm64 one.";

                    Console.WriteLine(CriticalWarning);
                    Console.ResetColor();
                }
            }
#endif
        }
    }
}
