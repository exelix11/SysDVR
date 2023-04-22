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
		public DebugOptions Logging { get; set; }
		public BinaryReader Source { get; set; }

        public StreamKind SourceKind { get; private init; }

        public RecordedSource(StreamKind kind, string basePath)
		{
			if (kind == StreamKind.Both)
				throw new Exception("Can't use both streams in RecordedSource");

			SourceKind = kind;

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

			var sourceMagic = Source.ReadUInt32();

			if (sourceMagic != PacketHeader.MagicResponseAudio && sourceMagic != PacketHeader.MagicResponseVideo)
				throw new Exception("");

			var header = MemoryMarshal.Cast<byte, PacketHeader>(buffer.AsSpan());
			ref var h = ref header[0];
			
			sw.Restart();

			ms = Source.ReadInt64();
			h.Magic = sourceMagic;
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