using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SysDVR.Client
{
	enum StreamKind
	{
		Both,
		Video,
		Audio
	};

	struct PoolBuffer
	{
		private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

		public int Length { get; private set; }
		private byte[] _buffer;		

		public byte[] RawBuffer => _buffer ?? throw new Exception("The buffer has been freed");

		public void Free() 
		{
			pool.Return(RawBuffer);
			_buffer = null;
		}

		public static PoolBuffer Rent(int len) =>
			new PoolBuffer(pool.Rent(len), len);

		private PoolBuffer(byte[] buf, int len)
		{
			Length = len;
			_buffer = buf;
		}

		public Span<byte> Span =>
			new Span<byte>(RawBuffer, 0, Length);

        public ArraySegment<byte> ArraySegment =>
            new ArraySegment<byte>(RawBuffer, 0, Length);

        public static implicit operator Span<byte>(PoolBuffer o) => o.Span;
	}

	interface IOutStream
	{
		void UseCancellationToken(CancellationToken tok);

		// The implementation must call Free() on the buffer
		void SendData(PoolBuffer block, UInt64 ts);
	}

	abstract class BaseStreamManager : IDisposable
	{
		private bool disposedValue;

		// Usb streaming may require a single thread
		private StreamThread Thread1;
		private StreamThread? Thread2;

		protected IOutStream VideoTarget { get;  set; }
		protected IOutStream AudioTarget { get; set; }

		public StreamKind? Streams { get; private set; }

		public bool HasVideo => Streams is StreamKind.Both or StreamKind.Video;
		public bool HasAudio => Streams is StreamKind.Both or StreamKind.Audio;

		public BaseStreamManager(IOutStream videoTarget, IOutStream audioTarget)
		{
            VideoTarget = videoTarget;
			AudioTarget = audioTarget;
		}

		IOutStream? WrapVideoTarget()
		{
            if (DebugOptions.Current.RequiresH264Analysis && VideoTarget is not null)
                return new H264LoggingWrapperTarget(VideoTarget);

			return VideoTarget;
        }

		public void AddSource(IStreamingSource source)
		{
			if (source.SourceKind == StreamKind.Video) 
			{
				if (HasVideo) throw new Exception("Already has a video source");
				Thread1 = new SingleStreamThread(source, WrapVideoTarget());

                Streams = HasAudio ? StreamKind.Both : StreamKind.Video;
            }
            else if (source.SourceKind == StreamKind.Audio)
            {
                if (HasAudio) throw new Exception("Already has an audio source");
                Thread2 = new SingleStreamThread(source, AudioTarget);

                Streams = HasVideo ? StreamKind.Both : StreamKind.Audio;
            }
            else if (source.SourceKind == StreamKind.Both)
            {
                if (HasAudio || HasVideo) throw new Exception("Already has a multi source");
                Thread1 = new MultiStreamThread(source, WrapVideoTarget(), AudioTarget);

                Streams = StreamKind.Both;
            }
        }

		public virtual void Begin()
		{
			// Sanity checks
			if (Streams is null)
				throw new Exception("No streams have been set");

			if (Streams == StreamKind.Video && AudioTarget != null)
                throw new Exception("There should be no audio target for a video only stream");
            if (Streams == StreamKind.Audio && VideoTarget != null)
                throw new Exception("There should be no video target for an audio only stream");
			if (Streams == StreamKind.Both && (VideoTarget == null || AudioTarget == null))
                throw new Exception("One or more targets are not set");

            Thread1?.Start();
			Thread2?.Start();
		}

		public virtual void Stop()
		{
			Thread1?.Stop();
			Thread2?.Stop();

#if MEASURE_STATS
			var vDiff = DateTime.Now - VideoThread.FirstByteTs;
			var aDiff = DateTime.Now - AudioThread.FirstByteTs;
			var total = VideoThread.ReceivedBytes + AudioThread.ReceivedBytes;

			var max = vDiff > aDiff ? vDiff : aDiff;

			Console.WriteLine($"MEASURE_STATS: received {total} bytes in {max.TotalSeconds} s of streaming, avg of {total / max.TotalSeconds} B/s.");
			Console.WriteLine($"Per thread stats:");
			Console.WriteLine($"\tVideo: {VideoThread.ReceivedBytes} bytes in {vDiff.TotalSeconds} s, avg of {VideoThread.ReceivedBytes / vDiff.TotalSeconds} B/s");
			Console.WriteLine($"\tAudio: {AudioThread.ReceivedBytes} bytes in {aDiff.TotalSeconds} s, avg of {AudioThread.ReceivedBytes / aDiff.TotalSeconds} B/s");
#endif

			Thread1?.Join();
			Thread2?.Join();
		}

		/*
			The default implementation just waits for a return on stdin, it is overridden by the video player.
			OSX requres SDL to run on the main thread
		*/
		public virtual void MainThread() 
		{
			Console.WriteLine("Starting stream, press return to stop.");
			Console.ReadLine();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Thread1?.Dispose();
					Thread2?.Dispose();

					if (VideoTarget is IDisposable iv)
						iv.Dispose();
					if (AudioTarget is IDisposable ia)
						ia.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
