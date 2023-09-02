using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public abstract class FunctionResolverBase : IFunctionResolver
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

#if NETSTANDARD2_0_OR_GREATER
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
#else
        return (T)(object)Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(T));
#endif
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
            var libraryPath = Path.Combine(ffmpeg.RootPath, nativeLibraryName);
            ptr = LoadNativeLibrary(libraryPath);

            if (ptr != IntPtr.Zero) _loadedLibraries.Add(libraryName, ptr);
            else if (throwOnError)
            {
                throw new DllNotFoundException(
                    $"Unable to load DLL '{libraryName}.{version} under {ffmpeg.RootPath}': The specified module could not be found.");
            }

            return ptr;
        }
    }

    protected abstract string GetNativeLibraryName(string libraryName, int version);
    protected abstract IntPtr LoadNativeLibrary(string libraryName);
    protected abstract IntPtr FindFunctionPointer(IntPtr nativeLibraryHandle, string functionName);
}
