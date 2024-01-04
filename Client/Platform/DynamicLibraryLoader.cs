using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SysDVR.Client.Platform
{
    internal static class DynamicLibraryLoader
    {
        // If something goes wrong and we want to show a critical warning to the user, we'll set this (assuming we get that far)
        public static string? CriticalWarning = null;

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
                if (Program.IsWindows)
                    return "win";
                if (Program.IsMacOs)
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
                
            if (Program.IsWindows) libext = ".dll";
            else if (Program.IsMacOs) libext = ".dylib";

            if (!libraryName.EndsWith(libext))
                libraryName += libext;

            // Is an override set ?
            if (!string.IsNullOrWhiteSpace(LibLoaderOverride))
                yield return Path.Combine(LibLoaderOverride, libraryName);

            // Is this a library we provide ?
            yield return Path.Combine(BundledOsNativeFolder, libraryName);

            // Flatpak uses this path
            if (Program.IsLinux)
                yield return Path.Combine("/app/lib", libraryName);

            // Maybe it's in the working directory
            yield return libraryName;
        }

        readonly static ConcurrentDictionary<string, IntPtr> LibraryCache = new();
        static IntPtr CachedLibraryloader(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
			if (LibraryCache.TryGetValue(libraryName, out var result))
				return result;

            var loaded = TryLoadLibrary(libraryName);

            if (loaded == IntPtr.Zero && Program.Options.Debug.DynLib)
                Console.WriteLine($"CachedLibraryloader failed to resolve {libraryName}");
            
            // Cache the failure so we don't try again
            // Dotnet will try to load the library again using its own resolver if we return IntPtr.Zero
            LibraryCache.TryAdd(libraryName, loaded);

            return loaded;
		}

		public static IntPtr TryLoadLibrary(string libraryName)
        {
            var debug = Program.Options.Debug.DynLib;
            
            var paths = FindNativeLibrary(libraryName)
                .Concat(FindNativeLibrary("lib" + libraryName));

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
                    Console.WriteLine($"Failed to load {candidate}");
            }

            // Try to load the library from the OS.
            // This is not included in the above search because it's filtered by the File.Exists
            {
				if (debug)
					Console.WriteLine($"Attempt to load {libraryName} without a path...");

				var result = IntPtr.Zero;

                if (NativeLibrary.TryLoad(libraryName, out result))
                    return result;

                if (NativeLibrary.TryLoad($"lib{libraryName}", out result))
                    return result;
            }

            // Try again, but without the version number
            if (StripVersionNumber(libraryName) is string withoutNumber)
            {
                return TryLoadLibrary(withoutNumber);
            }

            return IntPtr.Zero;
        }

        public static void Initialize()
        {
#if ANDROID_LIB
            AndroidQuerySysPaths(out var native, out var managed);
            if (!AndroidCheckDependencies(native, managed))
               throw new Exception("Native android dependencies are missing, possibly they are missing from the APK path. Note that on android SysDVR supports only arm64 builds.");
#else
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), CachedLibraryloader);

            if (Program.IsMacOs)
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64 && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    CriticalWarning =
                        "SysDVR detected that your mac has an apple silicon CPU but you are running the intel version of SysDVR.\n" +
                        "Running SysDVR in rosetta is not supported and will likely not work proparly.\n" +
                        "If you're using a standalone build of SysDVR (ver 6.0+) download the right one for your system. If you're running SysDVR through dotnet make sure to use a native arm64 dotnet build.";

                    Console.WriteLine(CriticalWarning);
                    Console.ResetColor();
                }
            }
#endif
        }
    }
}
