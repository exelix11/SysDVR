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
		Video,
		Audio
	};

	struct PoolBuffer
	{
		private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

		public int Length { get; private set; }
		private byte[] _buffer;		

		public byte[] Buffer => _buffer ?? throw new Exception("The buffer has been freed");

		public void Free() 
		{
			pool.Return(Buffer);
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
			new Span<byte>(Buffer, 0, Length);

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
		protected StreamThread VideoThread, AudioThread;
		private bool disposedValue;

		public IStreamingSource VideoSource { get => VideoThread?.Source; set { if (VideoThread != null) VideoThread.Source = value; } }
		public IStreamingSource AudioSource { get => AudioThread?.Source; set { if (AudioThread != null) AudioThread.Source = value; } }

		public IOutStream VideoTarget => VideoThread?.Target;
		public IOutStream AudioTarget => AudioThread?.Target;

		public bool HasVideo => VideoThread != null;
		public bool HasAudio => AudioThread != null;

		public BaseStreamManager(IOutStream VideoTarget, IOutStream AudioTarget)
		{
			if (VideoTarget != null)
				VideoThread = new StreamThread(StreamKind.Video, VideoTarget);
			if (AudioTarget != null)
				AudioThread = new StreamThread(StreamKind.Audio, AudioTarget);
		}

		public virtual void Begin()
		{
			VideoThread?.Start();
			AudioThread?.Start();
		}

		public virtual void Stop()
		{
			VideoThread?.Stop();
			AudioThread?.Stop();

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

			VideoThread?.Join();
			AudioThread?.Join();
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
					VideoThread?.Dispose();
					AudioThread?.Dispose();

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
