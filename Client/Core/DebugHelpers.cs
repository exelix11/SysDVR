using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SysDVR.Client.Core
{
    public class TimeTrace : IDisposable
    {
        Stopwatch sw = new Stopwatch();
        StringBuilder sb = new();

        public TimeTrace Begin(string kind, string extra, string funcname)
        {
            sb.Clear();
            sb.Append($"[{kind}] [{funcname}] {extra} ");
            sw.Restart();
            return this;
        }

        public void Mark(string name)
        {
            sb.Append($"{sw.ElapsedMilliseconds} ms | {name} ");
            sw.Restart();
        }

        // Abuses the using statement for RAII-like behavior, is not actually disposed
        public void Dispose()
        {
            sw.Stop();
            sb.Append($"{sw.ElapsedMilliseconds} ms");
            Console.WriteLine(sb.ToString());
        }
    }

    public static class TextEncoding 
    {
        // shh, we don't want certain string appearing in the binary as plaintext. Don't tell anyone.
        public static string ToPlainText(string data)
        {
            byte[] pattern = [0xfc, 0x14, 0x07, 0xe1, 0xe7, 0xe5, 0xbb, 0x49, 0xff, 0xb1, 0x41, 0xcd, 0xcd, 0xdf, 0x01, 0xef, 0xce, 0x34, 0x0a, 0xff, 0xaa, 0x43, 0x50, 0x01, 0xff, 0x00, 0x05, 0x00, 0x01, 0x52, 0xf4, 0xf5, 0xbb, 0x02, 0x52, 0x08, 0x01, 0xf1, 0xfb, 0xff, 0xca, 0x00, 0x00, 0xf3, 0xad, 0x04, 0x0a, 0x02, 0x00, 0x0a, 0xf2, 0xf7, 0x44, 0xbc, 0x01, 0xf9, 0x0d, 0xb4, 0xfa, 0x04, 0xb3, 0x14, 0x01, 0x01, 0x00, 0xf2, 0xfc, 0x04, 0xf2, 0xb7, 0xf3, 0xf7, 0xf2, 0x4e, 0xbf, 0x00, 0x43, 0xed, 0x05, 0x08, 0xec, 0x44, 0xcb];
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] converted = dataBytes
                .Select((x, i) => unchecked((byte)(x + pattern[i])))
                .Take(Math.Min(pattern.Length, dataBytes.Length))                
                .ToArray();
            return Encoding.UTF8.GetString(converted);
        }
    }
}
