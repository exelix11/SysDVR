using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Windows
{
    // https://github.com/exelix11/SysDVR/issues/235
    // Discord causes sysdvr to crash when it injects its dll
    // Debugging the issue seems to be an invalid icall (CFG guard) from SDL_RenderPresent
    // In practice it's not our issue and we're implementing poorman's anticheat to stop discord from messing with our process
    // TODO: This may triggr antiviruses, consider obfucating this code....
    internal static class AntiInject
    {
        // We only care about LoadLibraryA and W
        // Note that i'm not using their full names to avoid giving suspicious indicators to antimalware software
        delegate IntPtr A_Delegate([MarshalAs(UnmanagedType.LPStr)] string filename);
        delegate IntPtr W_Delegate([MarshalAs(UnmanagedType.LPWStr)] string filename);

        // Callbakcs to the real system implementation
        static A_Delegate A_Real;
        static W_Delegate W_Real;

        // Delegates handles to keep GC from collecting them
        static W_Delegate W_Handle;
        static A_Delegate A_Handle;

        // When set to true we reject any attemt at dll injection
        // Note that there are smarter ways to do this such as querying the thread start address to figure out if this is an injected thread
        // but since we know our dependencies the player can just toggle this after everything is initialized and ready to start (other streaming modes don't enable this currently)
        public static bool Reject = false;

        private static IntPtr Impl_LoadA([MarshalAs(UnmanagedType.LPStr)] string filename)
        {
            if (!Reject)
                return A_Real(filename);

            Console.WriteLine($"Anti-Injection blocked {filename}...");
            return IntPtr.Zero;
        }

        private static IntPtr Impl_LoadW([MarshalAs(UnmanagedType.LPWStr)] string filename)
        {
            if (!Reject)
                return W_Real(filename);

            Console.WriteLine($"Anti-Injection blocked {filename}...");
            return IntPtr.Zero;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr LoadLibraryA(string filename);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(
            IntPtr hProcess,
            ulong lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
             IntPtr hProcess,
             IntPtr lpBaseAddress,
             byte[] lpBuffer,
             int nSize,
             out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        // We don't support 32 bits
        private static byte[] MakeStubX64(ulong address)
        {
            var stub = new byte[14];

            // movabs r11, address
            stub[0] = 0x49; 
            stub[1] = 0xbb;
            Buffer.BlockCopy(BitConverter.GetBytes(address), 0, stub, 2, 8);

            // jmp r11
            stub[10] = 0x41; 
            stub[11] = 0xff;
            stub[12] = 0xe3;
            
            return stub;
        }

        private static void ApplyRedirection<T>(ulong from, T to, out T handle)
        {
            handle = to;
            var ptr = (ulong)Marshal.GetFunctionPointerForDelegate(handle);
            var call = MakeStubX64(ptr);

            // What's memory protection anyway ?
            // "WriteProcessMemory tries really hard"
            //      - one sprcific blogpost from MS
            if (!WriteProcessMemory(GetCurrentProcess(), from, call, call.Length, out _))
                throw new Exception($"Anti-inject failed to replace {from:X}");
        }

        private static IntPtr Lookup(string module, string name)
        {
            var res = GetProcAddress(LoadLibraryA(module), name);
            if (res == IntPtr.Zero)
                throw new Exception($"Could not find {module}!{name}");
            return res;
        }

        static bool EnsureMinWin(IntPtr address)
        {
            var original = new byte[16];
            ReadProcessMemory(GetCurrentProcess(), address, original, 16, out _);
            return original.Skip(7).All(x => x == 0xCC);
        }

        public static void Initialize() 
        {
            if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
            {
                if (DebugOptions.Current.Log)
                    Console.WriteLine("[AntiInj] not enabled due to unsupported OS/CPU");

                return;
            }

            try
            {
                // This relies on windows 10+ "minwin" api sets implementation which make hooking easier
                // This is not a good example for function hooking and you shouldn't use it
                // only works cause discord uses classical dll injection, don't take it as a good practice
                // (actually you should avoid messing with system dlls full stop)
                A_Real = Marshal.GetDelegateForFunctionPointer<A_Delegate>(Lookup("KernelBase.dll", "LoadLibraryA"));
                W_Real = Marshal.GetDelegateForFunctionPointer<W_Delegate>(Lookup("KernelBase.dll", "LoadLibraryW"));
            }
            catch 
            {
                if (DebugOptions.Current.Log)
                    Console.WriteLine("[AntiInj] not enabled due to missing kernelbase exports (old windows ?)");

                return;
            }

            IntPtr a, w;

            try
            {
                a = Lookup("Kernel32.dll", "LoadLibraryA");
                w = Lookup("Kernel32.dll", "LoadLibraryW");
            }
            catch 
            {
                if (DebugOptions.Current.Log)
                    Console.WriteLine("[AntiInj] not enabled due to missing kernel32 exports, this should never happen....");

                return;
            }

            if (!EnsureMinWin(a) || !EnsureMinWin(w))
            {
                if (DebugOptions.Current.Log)
                    Console.WriteLine("[AntiInj] not enabled due to unexpected kernel32 layout");

                return;
            }
            
            // Don't handle exceptions: If these fail just crash cause we likely messed up the memory layout
            ApplyRedirection((ulong)a, Impl_LoadA, out A_Handle);
            ApplyRedirection((ulong)w, Impl_LoadW, out W_Handle);

            if (DebugOptions.Current.Log)
                Console.WriteLine("[AntiInj] Ready !");
        }
    }
}
