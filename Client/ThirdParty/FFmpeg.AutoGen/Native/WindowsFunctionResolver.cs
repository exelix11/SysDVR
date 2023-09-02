using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen.Native;

public class WindowsFunctionResolver : FunctionResolverBase
{
    private const string Kernel32 = "kernel32";

    protected override string GetNativeLibraryName(string libraryName, int version) => $"{libraryName}-{version}.dll";

    protected override IntPtr LoadNativeLibrary(string libraryName) => LoadLibrary(libraryName);
    protected override IntPtr FindFunctionPointer(IntPtr nativeLibraryHandle, string functionName) => GetProcAddress(nativeLibraryHandle, functionName);


    [DllImport(Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    /// <summary>
    ///     Loads the specified module into the address space of the calling process. The specified module may cause other
    ///     modules to be loaded.
    /// </summary>
    /// <param name="dllToLoad">
    ///     <para>
    ///         The name of the module. This can be either a library module (a <c>.dll</c> file) or an executable module (an
    ///         <c>.exe</c> file).
    ///         The name specified is the file name of the module and is not related to the name stored in the library module
    ///         itself,
    ///         as specified by the LIBRARY keyword in the module-definition (<c>.def</c>) file.
    ///     </para>
    ///     <para>
    ///         If the string specifies a full path, the function searches only that path for the module.
    ///     </para>
    ///     <para>
    ///         If the string specifies a relative path or a module name without a path, the function uses a standard search
    ///         strategy
    ///         to find the module; for more information, see the Remarks.
    ///     </para>
    ///     <para>
    ///         If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes
    ///         (<c>\</c>),
    ///         not forward slashes (<c>/</c>). For more information about paths, see Naming a File or Directory.
    ///     </para>
    ///     <para>
    ///         If the string specifies a module name without a path and the file name extension is omitted, the function
    ///         appends the
    ///         default library extension <c>.dll</c> to the module name. To prevent the function from appending <c>.dll</c> to
    ///         the module name,
    ///         include a trailing point character (<c>.</c>) in the module name string.
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the module.
    ///     If the function fails, the return value is <see cref="IntPtr.Zero" />. To get extended error information, call
    ///     <see cref="Marshal.GetLastWin32Error" />.
    /// </returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175(v=vs.85).aspx" />
    [DllImport(Kernel32, SetLastError = true)]
    public static extern IntPtr LoadLibrary(string dllToLoad);
}
