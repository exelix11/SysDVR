//#define EXCEPTION_DEBUG

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
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
			$"Magic: {Magic:X8} Len: {DataSize + StructLength} Bytes - ts: {Timestamp}";

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

	abstract class StreamThread : IDisposable
	{
		public static bool Logging;

		Thread DeviceThread;
		CancellationTokenSource Cancel;

		readonly public IStreamingSource Source;
		readonly public StreamKind Kind;

		protected abstract void SetCancellationToken(CancellationToken token);
		protected abstract bool DataReceived(in PacketHeader header, PoolBuffer body);

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

		protected StreamThread(IStreamingSource source)
		{
			Source = source;
			Kind = Source.SourceKind;
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
			var logStats = Logging;
			var logDbg = logStats || Debugger.IsAttached;

			Source.Logging = logStats;
			Source.UseCancellationToken(token);
			
			SetCancellationToken(token);

			var HeaderData = new byte[PacketHeader.StructLength];
			ref var Header = ref MemoryMarshal.Cast<byte, PacketHeader>(HeaderData)[0];
			try
			{
				Source.WaitForConnection();
			loop_again:
				while (!token.IsCancellationRequested)
				{
					while (!Source.ReadHeader(HeaderData))
					{
						Source.Flush();
						Thread.Sleep(10);
						goto loop_again;
					}

					if (logStats)
						Console.WriteLine($"[{Kind}] {Header}");

					if (Header.Magic is not PacketHeader.MagicResponseAudio and not PacketHeader.MagicResponseVideo)
					{
						if (logDbg)
							Console.WriteLine($"[{Kind}] Wrong header magic: {Header.Magic:X}");
						
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					if (Header.DataSize > StreamInfo.MaxPayloadSize)
					{
						if (logDbg)
							Console.WriteLine($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");
						
						Source.Flush();
						Thread.Sleep(10);
						continue;
					}

					var Data = PoolBuffer.Rent(Header.DataSize);
					if (!Source.ReadPayload(Data.RawBuffer, Header.DataSize))
					{
                        if (logDbg)
                            Console.WriteLine($"[{Kind}] Read payload failed.");
                        
						Source.Flush();
						Data.Free();
						Thread.Sleep(10);
						continue;
					}

					if (!DataReceived(Header, Data)) 
					{
                        if (logDbg)
                            Console.WriteLine($"[{Kind}] DataReceived rejected the packet, header magic was {Header.Magic:X}");
                        
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

    class SingleStreamThread : StreamThread
    {
		readonly IOutStream Target;

        public SingleStreamThread(IStreamingSource source, IOutStream target) : base(source)
        {
			Target = target;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
            Target.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
			var valid =
				(Kind == StreamKind.Video && header.Magic == PacketHeader.MagicResponseVideo) ||
				(Kind == StreamKind.Audio && header.Magic == PacketHeader.MagicResponseAudio);

			if (!valid)
				return false;

			Target.SendData(body, header.Timestamp);

			return true;
        }
    }

    class MultiStreamThread : StreamThread
    {
        readonly IOutStream VideoTarget;
        readonly IOutStream AudioTarget;

        public MultiStreamThread(IStreamingSource source, IOutStream videoTarget, IOutStream audioTarget) : base(source)
        {
            if (source.SourceKind != StreamKind.Both)
                throw new Exception("Source must be able to provide both streams");

            VideoTarget = videoTarget;
			AudioTarget = audioTarget;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
			VideoTarget.UseCancellationToken(token);
			AudioTarget.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
			if (header.Magic == PacketHeader.MagicResponseVideo)
				VideoTarget.SendData(body, header.Timestamp);
			else if (header.Magic == PacketHeader.MagicResponseAudio)
				AudioTarget.SendData(body, header.Timestamp);
			else 
				return false;

			return true;
        }
    }
}
