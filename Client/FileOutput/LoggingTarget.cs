#if DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SysDVR.Client.FileOutput
{
	class LoggingTarget : IOutStream
	{
		readonly BinaryWriter bin;
		readonly MemoryStream mem = new MemoryStream();
		readonly string filename;

		public LoggingTarget(string filename)
		{
			this.filename = filename;
			bin = new BinaryWriter(mem);
		}

		public void WriteToDisk()
		{
			File.WriteAllBytes(filename, mem.ToArray());
		}

		Stopwatch sw = new Stopwatch();

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			Console.WriteLine($"{filename} - ts: {ts}");
			bin.Write(0xAAAAAAAA);
			bin.Write(sw.ElapsedMilliseconds);
			bin.Write(ts);
			bin.Write(size);
			bin.Write(data, offset, size);
			sw.Restart();
		}

		public void UseCancellationToken(CancellationToken tok) { }
	}

	class LoggingManager : BaseStreamManager
	{
		public LoggingManager(string VPath, string APath) : base(
			VPath != null ? new LoggingTarget(VPath) : null,
			APath != null ? new LoggingTarget(APath) : null)
		{

		}

		public override void Stop()
		{
			base.Stop();
			(VideoTarget as LoggingTarget)?.WriteToDisk();
			(AudioTarget as LoggingTarget)?.WriteToDisk();
		}
	}
}
#endif
