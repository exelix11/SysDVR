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

	interface IUnmanagedMemory
	{
		byte[] Buffer { get; }
		int Length { get; }

		void Free();
	}

	struct PoolBuffer : IUnmanagedMemory
	{
		private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

		bool freed;
		readonly byte[] buffer;
		public byte[] Buffer => !freed ? buffer : throw new Exception("The buffer has been freed");
		public int Length { get; private set; }

		public void Free() 
		{
			freed = true;
			pool.Return(buffer);
		}

		public static PoolBuffer Rent(int len) =>
			new PoolBuffer(pool.Rent(len), len);

		private PoolBuffer(byte[] buf, int len)
		{
			freed = false;
			Length = len;
			buffer = buf;
		}
	}

	interface IOutTarget
	{
		void UseCancellationToken(CancellationToken tok);

		void SendData(byte[] data, UInt64 ts) => SendData(data, 0, data.Length, ts);
		void SendData(byte[] data, int offset, int size, UInt64 ts);

		void SendData(PoolBuffer block, UInt64 ts)
		{
			SendData(block.Buffer, 0, block.Length, ts);
			block.Free();
		}
	}

	abstract class BaseStreamManager : IDisposable
	{
		protected StreamingThread VideoThread, AudioThread;
		private bool disposedValue;

		public IStreamingSource VideoSource { get => VideoThread?.Source; set { if (VideoThread != null) VideoThread.Source = value; } }
		public IStreamingSource AudioSource { get => AudioThread?.Source; set { if (AudioThread != null) AudioThread.Source = value; } }

		public IOutTarget VideoTarget => VideoThread?.Target;
		public IOutTarget AudioTarget => AudioThread?.Target;

		public bool HasVideo => VideoThread != null;
		public bool HasAudio => AudioThread != null;

		public BaseStreamManager(IOutTarget VideoTarget, IOutTarget AudioTarget)
		{
			if (VideoTarget != null)
				VideoThread = new StreamingThread(StreamKind.Video, VideoTarget);
			if (AudioTarget != null)
				AudioThread = new StreamingThread(StreamKind.Audio, AudioTarget);
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
			VideoThread?.Join();
			AudioThread?.Join();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					VideoThread?.Dispose();
					AudioThread?.Dispose();
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

#if DEBUG
	class LoggingTarget : IOutTarget
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

		public void UseCancellationToken(CancellationToken tok) {}
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
#endif
}
