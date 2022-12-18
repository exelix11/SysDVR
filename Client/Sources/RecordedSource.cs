#if DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SysDVR.Client.Sources
{
	// Playsback a recording made with LoggingTarget, useful for developing without a console
	class RecordedSource : IStreamingSource
	{
		public bool Logging { get; set; }
		public BinaryReader Source { get; set; }

		public RecordedSource(StreamKind kind, string basePath)
		{
			var name = kind == StreamKind.Video ? "video.h264" : "audio.raw";
			Source = new BinaryReader(File.OpenRead(Path.Combine(basePath, name)));
		}

		public void Flush()
		{

		}

		Int64 ms;
		public bool ReadHeader(byte[] buffer)
		{
			if (Source.PeekChar() < 0)
				return false;

			if (Source.ReadUInt32() != 0xAAAAAAAA)
				throw new Exception("");

			var header = MemoryMarshal.Cast<byte, PacketHeader>(buffer.AsSpan());
			ref var h = ref header[0];
			
			sw.Restart();

			ms = Source.ReadInt64();
			h.Magic = PacketHeader.DefaultMagic;
			h.Timestamp = Source.ReadUInt64();
			h.DataSize = Source.ReadInt32();

			return true;
		}

		Stopwatch sw = new Stopwatch();
		public bool ReadPayload(byte[] buffer, int length)
		{					
			sw.Stop();
			if (sw.ElapsedMilliseconds < ms)
				System.Threading.Thread.Sleep((int)(ms - sw.ElapsedMilliseconds));

			Source.Read(buffer, 0, length);
			return true;
		}

		public void StopStreaming()
		{
			
		}

		public void UseCancellationToken(CancellationToken tok)
		{

		}

		public void WaitForConnection()
		{

		}
	}
}
#endif