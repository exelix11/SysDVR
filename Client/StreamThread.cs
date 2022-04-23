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

	class StreamThread : IDisposable
	{
		public static bool Logging;

		Thread DeviceThread;
		Thread TargetThread;
		CancellationTokenSource Cancel;

		public IStreamingSource Source { get; set; }

		public IOutStream Target { get; private init; }
		public StreamKind Kind { get; private init; }

#if MEASURE_STATS
		public ulong ReceivedBytes { get; private set; }
		public DateTime FirstByteTs { get; private set; }

		public void StatsReceivedData(int length)
        {
			if (ReceivedBytes == 0)
				FirstByteTs = DateTime.Now;
			ReceivedBytes += (uint)length + PacketHeader.StructLength;
        }
#endif

		public StreamThread(StreamKind kind, IOutStream target)
		{
			Kind = kind;
			Target = target;
		}

		public void Start() 
		{
			Cancel = new CancellationTokenSource();

			TargetThread = new Thread(() => TargetThreadMain(Cancel.Token));
			DeviceThread = new Thread(() => DeviceThreadMain(Cancel.Token));

			TargetThread.Name = "TargetThread for " + Kind;
			DeviceThread.Name = "DeviceThread for " + Kind;

			DeviceThread.Priority = ThreadPriority.Highest;

			TargetThread.Start();
			DeviceThread.Start();
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
		void TargetThreadMain(CancellationToken token)
		{
			try
			{
				foreach (var o in queue.GetConsumingEnumerable(token))
				{
					var (ts, data) = o;

#if LOG_NAL_TYPES
					if (Kind == StreamKind.Video)
					{
						var s = data.Span;
						Console.Write("{");
						while (s.Length > 4)
						{
							if (s[0] == 0 && s[1] == 0 && s[2] == 0 && s[3] == 1)
								Console.Write($"{s[4] & 0x1F} ");								
							s = s.Slice(1);
						}
						Console.Write("}");
					}
#endif

#if MEASURE_STATS
					StatsReceivedData(data.Length);
#endif
					Target.SendData(data, ts);
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

		void DeviceThreadMain(CancellationToken token)
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
						Source.Flush();
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
