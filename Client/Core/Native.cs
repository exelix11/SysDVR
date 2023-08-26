using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeInitBlock 
    {
        public const IntPtr BlockVersion = (IntPtr)1;

        public IntPtr Version;
        public IntPtr PrintFunction;

        public unsafe static NativeInitBlock Read(IntPtr ptr) =>
            *(NativeInitBlock*)ptr;

        public static NativeInitBlock Empty = new NativeInitBlock()
        {
            Version = BlockVersion,
            PrintFunction = IntPtr.Zero
        };
    }
}
