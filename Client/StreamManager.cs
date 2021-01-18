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

	interface IOutStream
	{
		void UseCancellationToken(CancellationToken tok);

		void SendData(byte[] data, UInt64 ts) => SendData(data, 0, data.Length, ts);
		void SendData(byte[] data, int offset, int size, UInt64 ts);

		void SendAndFreeData(PoolBuffer block, UInt64 ts)
		{
			SendData(block.Buffer, 0, block.Length, ts);
			block.Free();
		}
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
