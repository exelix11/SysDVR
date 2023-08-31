#if ANDROID_LIB
using System;
using System.IO;
using System.Text;

namespace SysDVR.Client.Core
{
    public class NativeLogger : TextWriter
    {
        readonly NativeContracts.PrintFunction Print;

        public static void Setup() 
        {
            Console.SetOut(new NativeLogger(Program.Native.Print));
        }

        private NativeLogger(NativeContracts.PrintFunction print)
        {
            Print = print;
            Print("Nativelogger has been initialized !");
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
#endif