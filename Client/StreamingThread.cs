using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVRClient
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

		public const int APayloadMax = 0x1000;
		public const int VPayloadMax = 0x32000;
		public const int MaxTransferSize = VPayloadMax + StructLength;

		static PacketHeader()
		{
			if (Marshal.SizeOf<PacketHeader>() != StructLength)
				throw new Exception("PacketHeader struct binary size is wrong");
		}
	}

	public static class StreamInfo 
	{
		public static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
		public static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };
	}

	interface StreamingSource
	{
		bool Logging { get; set; }
		void UseCancellationToken(CancellationToken tok);
		
		void WaitForConnection();
		void StopStreaming();
		void Flush();

		bool ReadHeader(byte[] buffer);
		bool ReadPayload(byte[] buffer, int length);
	}

	class StreamingThread
	{
		public static bool Logging;
		
		Thread StreamThread;
		CancellationTokenSource Cancel;

		IOutTarget Target;
		StreamKind Kind;
		StreamingSource Source;

		public StreamingThread(IOutTarget target, StreamKind kind, StreamingSource source)
		{
			Target = target;
			Kind = kind;
			Source = source;
		}

		~StreamingThread() 
		{
			if (StreamThread.IsAlive)
				throw new Exception($"{Kind} Thread is still running");
		}

		public void Start() 
		{
			Cancel = new CancellationTokenSource();

			StreamThread = new Thread(ThreadMain);
			StreamThread.Start();
		}

		public void Stop()
		{
			Cancel.Cancel();
			Source.StopStreaming();
			StreamThread.Join();
		}

		private void ThreadMain()
		{
			var log = Logging;

			CancellationToken token = Cancel.Token;
			ArrayPool<byte> pool = ArrayPool<byte>.Create();
			BlockingCollection<(ulong, byte[])> queue = new BlockingCollection<(ulong, byte[])>(5);

			Source.Logging = log;
			Source.UseCancellationToken(token);
			
#if RELEASE
			try
			{
#endif
			Source.WaitForConnection();

			void ReceiveFromDevice() 
			{
				var HeaderData = new byte[PacketHeader.StructLength];
				ref var Header = ref MemoryMarshal.Cast<byte, PacketHeader>(HeaderData)[0];

				while (!token.IsCancellationRequested)
				{
					while (!Source.ReadHeader(HeaderData))
					{
						System.Threading.Thread.Sleep(10);
						continue;
					}

					if (log)
						Console.WriteLine($"[{Kind}] {Header}");

					if (Header.Magic != PacketHeader.DefaultMagic)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Wrong header magic: {Header.Magic:X}");
						Source.Flush();
						System.Threading.Thread.Sleep(10);
						continue;
					}

					if (Header.DataSize > PacketHeader.VPayloadMax)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");
						Source.Flush();
						System.Threading.Thread.Sleep(10);
						continue;
					}

					var Data = pool.Rent(Header.DataSize);
					if (!Source.ReadPayload(Data, Header.DataSize))
					{
						Source.Flush();
						System.Threading.Thread.Sleep(10);
						continue;
					}

					queue.Add((Header.Timestamp, Data));
				}
				queue.CompleteAdding();
			}

			void SendToClient() 
			{
				foreach (var o in queue.GetConsumingEnumerable())
				{
					var (ts, data) = o;

					Target.SendData(data, ts);
					pool.Return(data);
				}
			}

			Parallel.Invoke(ReceiveFromDevice, SendToClient);
#if RELEASE
			}
			catch (Exception ex)
			{
				if (!token.IsCancellationRequested)
					Console.WriteLine($"Terminating {Kind} thread due to {ex}");
			}
#endif
		}
	}
}
