using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    public static class Log
    {
        public static void Setup(in NativeInitBlock init) 
        {
            if (init.PrintFunction != IntPtr.Zero)
            {
                Console.SetOut(new AndroidTextOut(init.PrintFunction));
            }
        }
    }

    public class AndroidTextOut : TextWriter
    {
        delegate void PrintImpl([MarshalAs(UnmanagedType.LPStr)] string func);
        readonly PrintImpl Print;

        public AndroidTextOut(IntPtr print)
        {
            Print = Marshal.GetDelegateForFunctionPointer<PrintImpl>(print);
            Print("Native console out is set");
        }

        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(string? value)
        {
            if (value.EndsWith("\n")) 
                value = value[..^1];

            Print(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer[^1] == '\n') --count;
            Write(new string(buffer, index, count));
        }

        public override void Write(char value)
        {
            base.Write(value.ToString());
        }
    }
}
