using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

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
		public const int ABatchingMax = 4;
		public const int VPayloadMax = 0x32000;
		public const int MaxTransferSize = VPayloadMax + StructLength;

		static PacketHeader()
		{
			if (Marshal.SizeOf<PacketHeader>() != StructLength)
				throw new Exception("PacketHeader struct binary size is wrong");
		}
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

	abstract class RTSPStreamManager : RTSP.SysDvrRTSPServer
	{
		protected StreamingThread VideoThread, AudioThread;

		public RTSPStreamManager(bool videoSupport, bool audioSupport, bool localOnly, int port) : base(videoSupport, audioSupport, localOnly, port) 
		{

		}

		public override void Begin()
		{
			VideoThread?.Start();
			AudioThread?.Start();
			base.Begin();
		}

		public override void Stop()
		{
			VideoThread?.Stop();
			AudioThread?.Stop();
			base.Stop();
		}
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
			StreamThread.Join();
		}

		private void ThreadMain()
		{
			var log = Logging;

			CancellationToken token = Cancel.Token;
			ArrayPool<byte> pool = ArrayPool<byte>.Create();

			Source.UseCancellationToken(token);
			
#if RELEASE
			try
			{
#endif
			Source.WaitForConnection();

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

				var Data = pool.Rent(Header.DataSize);
				if (!Source.ReadPayload(Data, Header.DataSize))
				{
					Source.Flush();
					System.Threading.Thread.Sleep(10);
					continue;
				}
				Target.SendData(Data, Header.Timestamp);
				pool.Return(Data);
			}
#if RELEASE
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Terminating {Kind} thread due to {ex}");
			}
#endif

			Source.StopStreaming();
		}		
	}
}
