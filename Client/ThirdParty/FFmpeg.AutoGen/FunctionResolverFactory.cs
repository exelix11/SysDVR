using FFmpeg.AutoGen.Native;
using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public static class FunctionResolverFactory
{
    public static IFunctionResolver Create()
    {
        // OperatingSystem.IsAndroid() doesn't seem to work when compiling with bflat
#if ANDROID_LIB
        return new AndroidFunctionResolver();
#else

        if (OperatingSystem.IsWindows())
            return new WindowsFunctionResolver();
        else if (OperatingSystem.IsAndroid())
            return new AndroidFunctionResolver();
        else if (OperatingSystem.IsMacOS())
            return new MacFunctionResolver();
        else if (OperatingSystem.IsLinux())
            return new LinuxFunctionResolver();
        else
            throw new PlatformNotSupportedException();
#endif
    }
}
