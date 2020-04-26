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

		static PacketHeader()
		{
			if (Marshal.SizeOf<PacketHeader>() != StructLength)
				throw new Exception("PacketHeader struct binary size is wrong");
		}
	}

	interface StreamingSource
	{
		public void UseCancellationToken(CancellationToken tok);
		
		public void WaitForConnection();
		public void StopStreaming();

		public int ReadBytes(byte[] buffer, int offset, int length);

		public void ReadExact(byte[] buffer, int offset, int length)
		{
			int Read = 0;
			do
			{
				Read += ReadBytes(buffer, offset + Read, length - Read);
			} while (Read < length);
		}
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

				if (log)
					Console.WriteLine($"[{Kind}] Searching first packet header...");

				//Find packet head, stream may not be aligned, look for the packet magic
				for (int i = 0; i < 4 && !token.IsCancellationRequested; i++)
				{
					Source.ReadBytes(HeaderData, i, 1);
					if (HeaderData[i] != 0xAA)
						i = 0;
				}

				if (log)
					Console.WriteLine($"[{Kind}] Header found !");

				if (token.IsCancellationRequested)
					return;

				Source.ReadExact(HeaderData, 4, 12);

				while (!token.IsCancellationRequested)
				{
					var Data = pool.Rent(Header.DataSize);

					Source.ReadExact(Data, 0, Header.DataSize);

					if (log)
						Console.WriteLine($"[{Kind}] {Header}");

					if (Header.Magic == PacketHeader.DefaultMagic)
						Target.SendData(Data, Header.Timestamp);
					else throw new Exception($"Unknown magic {Header.Magic}");

					pool.Return(Data);

					Source.ReadExact(HeaderData, 0, PacketHeader.StructLength);
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
