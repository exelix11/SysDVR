using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client
{
	public static class H265Nal 
	{
		public static void PrintNalTypes(Span<byte> s) 
		{
			Console.Write("{");
			while (s.Length > 4)
			{
				if (s[0] == 0 && s[1] == 0 && s[2] == 0 && s[3] == 1)
					Console.Write($"{s[4] & 0x1F} ");
				s = s.Slice(1);
			}
			Console.Write("}");
		}
	}

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
}
