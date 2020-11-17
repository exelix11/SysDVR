//#define EXCEPTION_DEBUG

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client
{
	[StructLayout(LayoutKind.Sequential)]
	struct PacketHeader
	{
		public UInt32 Magic;
		public Int32 DataSize;
		public UInt64 Timestamp;

		public override string ToString() =>
			$"Len: {DataSize} Bytes - ts: {Timestamp}";

		public const int StructLength = 16;
		public const UInt32 DefaultMagic = 0xAAAAAAAA;

		public const int MaxTransferSize = StreamInfo.VideoPayloadSize + StructLength;

		static PacketHeader()
		{
			if (Marshal.SizeOf<PacketHeader>() != StructLength)
				throw new Exception("PacketHeader struct binary size is wrong");
		}
	}

	interface IStreamingSource
	{
		bool Logging { get; set; }
		void UseCancellationToken(CancellationToken tok);
		
		void WaitForConnection();
		void StopStreaming();
		void Flush();

		bool ReadHeader(byte[] buffer);
		bool ReadPayload(byte[] buffer, int length);
	}

#if DEBUG
	class StubSource : IStreamingSource
	{
		public bool Logging { get; set; }

		public void Flush()
		{
			
		}

		public bool ReadHeader(byte[] buffer) =>
			throw new NotImplementedException();

		public bool ReadPayload(byte[] buffer, int length) =>
			throw new NotImplementedException();

		public void StopStreaming()
		{
			
		}

		public void UseCancellationToken(CancellationToken tok)
		{

		}

		public void WaitForConnection()
		{
			while (true)
				Thread.Sleep(1000);
		}
	}
#endif

	class StreamingThread : IDisposable
	{
		public static bool Logging;

		Thread DeviceThread;
		Thread TargetThread;
		CancellationTokenSource Cancel;

		public IOutTarget Target { get; set; }
		public IStreamingSource Source { get; set; }

		public StreamKind Kind { get; private set; }

		public StreamingThread(StreamKind kind, IOutTarget target)
		{
			Kind = kind;
			Target = target;
		}

		public void Start() 
		{
			Cancel = new CancellationTokenSource();

			DeviceThread = new Thread(() => TargetThreadMain(Cancel.Token));
			TargetThread = new Thread(() => DeviceThreadMain(Cancel.Token));

			DeviceThread.Start();
			TargetThread.Start();
		}

		public void Stop()
		{
			Cancel.Cancel();
			Source.StopStreaming();
		}

		public void Join()
		{
			TargetThread.Join();
			DeviceThread.Join();
		}

		BlockingCollection<(ulong, PoolBuffer)> queue = new BlockingCollection<(ulong, PoolBuffer)>(5);
		void DeviceThreadMain(CancellationToken token)
		{
			try
			{
				foreach (var o in queue.GetConsumingEnumerable(token))
				{
					var (ts, data) = o;

					Target.SendData(data, ts);
					// The target is expected to free data
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
#if !EXCEPTION_DEBUG || RELEASE
			catch (Exception ex)
			{
				Console.WriteLine($"Terminating SendToTargetThread for {Kind} due to {ex}");
			}
#endif
		}

		void TargetThreadMain(CancellationToken token)
		{
			var log = Logging;

			Source.Logging = log;
			Source.UseCancellationToken(token);
			Target.UseCancellationToken(token);

			var HeaderData = new byte[PacketHeader.StructLength];
			ref var Header = ref MemoryMarshal.Cast<byte, PacketHeader>(HeaderData)[0];
			try
			{
				Source.WaitForConnection();
				while (!token.IsCancellationRequested)
				{
					while (!Source.ReadHeader(HeaderData))
					{
						Thread.Sleep(10);
						continue;
					}

					if (log)
						Console.WriteLine($"[{Kind}] {Header}");

					if (Header.Magic != PacketHeader.DefaultMagic)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Wrong header magic: {Header.Magic:X}");
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					if (Header.DataSize > StreamInfo.VideoPayloadSize)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					var Data = PoolBuffer.Rent(Header.DataSize);
					if (!Source.ReadPayload(Data.Buffer, Header.DataSize))
					{
						Source.Flush();
						Data.Free();
						Thread.Sleep(10);
						continue;
					}

					queue.Add((Header.Timestamp, Data), token);
				}

				queue.CompleteAdding();
			}
			catch (OperationCanceledException)
			{
				return;
			}
#if !EXCEPTION_DEBUG || RELEASE
			catch (Exception ex)
			{
				if (!token.IsCancellationRequested)
					Console.WriteLine($"Terminating ReceiveFromDeviceThread for {Kind} due to {ex}");
			}
#endif
		}

		public void Dispose()
		{
			if (DeviceThread.IsAlive || TargetThread.IsAlive)
				throw new Exception($"{Kind} Thread is still running");

			Cancel.Dispose();
			queue.Dispose();
		}
	}
}
