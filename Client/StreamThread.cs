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

		// Note: to make the TCP implementation easier these should be composed of 4 identical bytes
        public const UInt32 MagicResponseVideo = 0xDDDDDDDD;
        public const UInt32 MagicResponseAudio = 0xEEEEEEEE;

        public const int MaxTransferSize = StreamInfo.MaxPayloadSize + StructLength;

		static PacketHeader()
		{
			if (Marshal.SizeOf<PacketHeader>() != StructLength)
				throw new Exception("PacketHeader struct binary size is wrong");
		}
	}

	interface IStreamingSource
	{
		StreamKind SourceKind { get; }

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
		CancellationTokenSource Cancel;

		public IStreamingSource Source { get; private init; }

		private IOutStream MainTarget { get; init; }
		private IOutStream? SecondaryTarget { get; init; }

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

		private StreamThread(StreamKind kind)
		{
			Kind = kind;
		}

		public static StreamThread ForSingleStream(IStreamingSource source, IOutStream target)
		{
			var ret = new StreamThread(source.SourceKind) 
			{ 
				MainTarget = target,
				Source = source
			};
            return ret;
		}

		public static StreamThread ForBothStreams(IStreamingSource source, IOutStream videoTarget, IOutStream audioTarget)
		{
			if (source.SourceKind != StreamKind.Both)
				throw new Exception("Source must be able to provide both streams");

            var ret = new StreamThread(StreamKind.Both) { 
				MainTarget = videoTarget, 
				SecondaryTarget = audioTarget ,
				Source = source
			};

            return ret;
        }

		public void Start() 
		{
			Cancel = new CancellationTokenSource();

			DeviceThread = new Thread(() => DeviceThreadMain(Cancel.Token));
			DeviceThread.Name = "DeviceThread for " + Kind;

			DeviceThread.Start();
		}

		public void Stop()
		{
			Cancel.Cancel();
			Source.StopStreaming();
		}

		public void Join()
		{
			DeviceThread.Join();
		}

		TimeTrace trace = new TimeTrace();
		void DeviceThreadMain(CancellationToken token)
		{
			var log = Logging;

			Source.Logging = log;
			Source.UseCancellationToken(token);
			
			MainTarget.UseCancellationToken(token);
			SecondaryTarget?.UseCancellationToken(token);

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

					if (Header.Magic is not PacketHeader.MagicResponseAudio and not PacketHeader.MagicResponseVideo)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Wrong header magic: {Header.Magic:X}");
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					if (Header.DataSize > StreamInfo.MaxPayloadSize)
					{
						if (log)
							Console.WriteLine($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					var Data = PoolBuffer.Rent(Header.DataSize);
					if (!Source.ReadPayload(Data.RawBuffer, Header.DataSize))
					{
						Source.Flush();
						Data.Free();
						Thread.Sleep(10);
						continue;
					}

					if (Kind == StreamKind.Both)
					{
						if (Header.Magic == PacketHeader.MagicResponseVideo)
                            MainTarget.SendData(Data, Header.Timestamp);
						else if (Header.Magic == PacketHeader.MagicResponseAudio)
							SecondaryTarget!.SendData(Data, Header.Timestamp);
                    }
					else if (Kind == StreamKind.Video && Header.Magic == PacketHeader.MagicResponseVideo)
						MainTarget.SendData(Data, Header.Timestamp);
					else if (Kind == StreamKind.Audio && Header.Magic == PacketHeader.MagicResponseAudio)
						MainTarget.SendData(Data, Header.Timestamp);
					else {
                        if (log)
                            Console.WriteLine($"[{Kind}] Wrong header magic, expected different one: {Header.Magic:X}");
                        
						Source.Flush();
                        Data.Free();
                        Thread.Sleep(10);
                        continue;
                    }
                }

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
			if (DeviceThread.IsAlive)
				throw new Exception($"{Kind} Thread is still running");

			Cancel.Dispose();
		}
	}
}
