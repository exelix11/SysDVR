using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client
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
}
