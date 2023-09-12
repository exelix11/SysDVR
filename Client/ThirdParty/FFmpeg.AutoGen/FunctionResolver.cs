using SysDVR.Client.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public class FunctionResolver : IFunctionResolver
{
    public static readonly Dictionary<string, string[]> LibraryDependenciesMap =
        new()
        {
            { "avcodec", new[] { "avutil", "swresample" } },
            { "avdevice", new[] { "avcodec", "avfilter", "avformat", "avutil" } },
            { "avfilter", new[] { "avcodec", "avformat", "avutil", "postproc", "swresample", "swscale" } },
            { "avformat", new[] { "avcodec", "avutil" } },
            { "avutil", new string[] { } },
            { "postproc", new[] { "avutil" } },
            { "swresample", new[] { "avutil" } },
            { "swscale", new[] { "avutil" } }
        };

    private readonly Dictionary<string, IntPtr> _loadedLibraries = new();

    private readonly object _syncRoot = new();

    public T GetFunctionDelegate<T>(string libraryName, string functionName, bool throwOnError = true)
    {
        var nativeLibraryHandle = GetOrLoadLibrary(libraryName, throwOnError);
        return GetFunctionDelegate<T>(nativeLibraryHandle, functionName, throwOnError);
    }

    public T GetFunctionDelegate<T>(IntPtr nativeLibraryHandle, string functionName, bool throwOnError)
    {
        var functionPointer = FindFunctionPointer(nativeLibraryHandle, functionName);

        if (functionPointer == IntPtr.Zero)
        {
            if (throwOnError) throw new EntryPointNotFoundException($"Could not find the entrypoint for {functionName}.");
            return default;
        }

        try
        {
            return Marshal.GetDelegateForFunctionPointer<T>(functionPointer);
        }
        catch (MarshalDirectiveException)
        {
            if (throwOnError)
                throw;
            return default;
        }
    }

    public IntPtr GetOrLoadLibrary(string libraryName, bool throwOnError)
    {
        if (_loadedLibraries.TryGetValue(libraryName, out var ptr)) return ptr;

        lock (_syncRoot)
        {
            if (_loadedLibraries.TryGetValue(libraryName, out ptr)) return ptr;

            var dependencies = LibraryDependenciesMap[libraryName];
            dependencies.Where(n => !_loadedLibraries.ContainsKey(n) && !n.Equals(libraryName))
                .ToList()
                .ForEach(n => GetOrLoadLibrary(n, false));

            var version = ffmpeg.LibraryVersionMap[libraryName];
            var nativeLibraryName = GetNativeLibraryName(libraryName, version);
            ptr = LoadNativeLibrary(nativeLibraryName);

            if (ptr != IntPtr.Zero) _loadedLibraries.Add(libraryName, ptr);
            else if (throwOnError)
            {
                throw new DllNotFoundException(
                    $"Unable to load DLL '{libraryName}.{version}': The specified module could not be found.");
            }

            return ptr;
        }
    }

    protected string GetNativeLibraryName(string libraryName, int version)
    {
        // Currently android needs a special ifdef due to bflat not supporting this API yet
#if ANDROID_LIB
        return $"lib{libraryName}.so";
#else
        if (OperatingSystem.IsWindows())
            return $"{libraryName}-{version}.dll";
        else if (OperatingSystem.IsAndroid())
            return $"lib{libraryName}.so";
        else if (OperatingSystem.IsMacOS())
            return $"lib{libraryName}.{version}.dylib";
        else if (OperatingSystem.IsLinux())
            return $"lib{libraryName}.so.{version}";

        return libraryName;
#endif
    }

    protected IntPtr LoadNativeLibrary(string libraryName) 
    {
        return DynamicLibraryLoader.TryLoadLibrary(libraryName);
    }
    
    protected IntPtr FindFunctionPointer(IntPtr nativeLibraryHandle, string functionName)
    {
        if (!NativeLibrary.TryGetExport(nativeLibraryHandle, functionName, out var addr))
            return IntPtr.Zero;

        return addr;
    }
}
