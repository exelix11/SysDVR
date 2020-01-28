//#define BENCHMARK

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace UsbStream
{
	public abstract class StreamThread : IDisposable
	{
		public bool PrintStats = false;
		public readonly StreamKind Kind;

		protected readonly CancellationToken Token;
		protected Thread thread;

		public void Start() 
		{
			thread = new Thread(MainLoop);
			thread.Start();
		}

		public void Join() => thread.Join();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
		bool SuspendThread = false;
		public void Pause() 
		{
			lock (wait)
				SuspendThread = true;
		}

		public void Resume()
		{
			lock (wait)
				SuspendThread = false;
			wait.Set();
		}

		readonly protected UsbDevStream Device;
		readonly protected IOutTarget Target;
		readonly protected byte[] MagicPacket;
		readonly protected int MaxBufSize;

		protected virtual void StreamInitialized() { }

		protected StreamThread(CancellationToken token, StreamKind kind, UsbDevStream dev, IOutTarget target, byte[] magic, int BufMax)
		{
			Token = token;
			Kind = kind;
			Device = dev;
			Target = target;
			MagicPacket = magic;
			MaxBufSize = BufMax;
		}

		private void MainLoop()
		{
			Target.ClientConnected += StreamInitialized;
			Target.InitializeStreaming();
			ThreadTimer.Start();

#if BENCHMARK
			Stopwatch benchmark = new Stopwatch();
#endif
#if !DEBUG
			try
			{
#endif
			while (!Token.IsCancellationRequested)
			{
#if BENCHMARK
				benchmark.Restart();
#endif
				bool ShouldBlock = false;
				lock (wait)
					ShouldBlock = SuspendThread;
				if (ShouldBlock)
					wait.WaitOne();

				while (Device.WriteWResult(MagicPacket) != MagicPacket.Length)
				{
					Console.WriteLine($"Warning: Couldn't write data to device ({Kind} thread)");
					System.Threading.Thread.Sleep(1000);
					Device.Flush();
				}

				var size = ReadNextPacket();
				if (size > MaxBufSize || size <= 0)
				{
					Console.WriteLine($"Warning: Discarding packet of size {size} ({Kind} thread)");
					System.Threading.Thread.Sleep(500);
					Device.Flush();
				}
				else
				{
					Target.SendData(Data, 0, (int)size, Timestamp);
#if LOG
					Console.WriteLine($"video {size}");
#endif
					TransfersPerSecond++;
					BytesPerSecond += size;
					CheckPlayStats();
				}
#if BENCHMARK
				if (Kind == StreamKind.Video)
					Console.WriteLine(benchmark.ElapsedMilliseconds);
#endif
			}
#if !DEBUG
				
			}
			catch (Exception ex)
			{
				if (!Token.IsCancellationRequested)
					Console.WriteLine("There was an exception: " + ex.ToString());
			}
#endif
			Console.WriteLine($"{Kind} thread stopped.");
		}

		private readonly ArrayPool<byte> StreamingPool = ArrayPool<byte>.Create();
		protected byte[] Data = null;
		protected ulong Timestamp = 0;

		readonly private byte[] SizeBuf = new byte[4];
		readonly private byte[] TsBuf = new byte[8];
		private ulong firtTs = 0;
		protected int ReadNextPacket()
		{	
			if (Data != null)
			{
				StreamingPool.Return(Data);
				Data = null; 
			}

			//wait until a magic packet is received
			Device.MillisTimeout = 100;
			while (true)
			{
				SizeBuf[0] = SizeBuf[1] = SizeBuf[2] = SizeBuf[3] = 0;
				if (Device.Read(SizeBuf, 0, 4) != 4)
					if (Token.IsCancellationRequested)
						return -3;
					else
						continue;
				if (SizeBuf.SequenceEqual(MagicPacket))
					break;
			}
			
			//read the payload size
			if (Device.Read(SizeBuf, 0, 4) != 4) return -2;
			var size = BitConverter.ToUInt32(SizeBuf,0);
			if (size > MaxBufSize) return -1;
			if (size == 0) return 0;

			//read the timestamp
			if (Device.Read(TsBuf, 0, 8) != 8) return -4;
			Timestamp = BitConverter.ToUInt64(TsBuf);
			if (firtTs == 0)
				firtTs = Timestamp;

			Timestamp -= firtTs;

			//Read the actual data
			Device.MillisTimeout = 1000;
			Data = StreamingPool.Rent((int)size);
			int actualsize = Device.Read(Data, 0, (int)size);
			if (actualsize != size)
				Console.WriteLine("Warning: Reported size doesn't match received size");
			return actualsize;
		}

		private Stopwatch ThreadTimer = new Stopwatch();
		private long TransfersPerSecond = 0;
		private long BytesPerSecond = 0;
		protected void CheckPlayStats()
		{
			if (ThreadTimer.ElapsedMilliseconds < 1000) return;
			ThreadTimer.Stop();

			if (PrintStats)
				Console.WriteLine($"{Kind} stream: {TransfersPerSecond} - {BytesPerSecond / ThreadTimer.ElapsedMilliseconds} KB/s");

			TransfersPerSecond = 0;
			BytesPerSecond = 0;

			ThreadTimer.Restart();
		}

		protected virtual void Dispose(bool Explicit)
		{
			if (Explicit)
			{
				if (thread.IsAlive)
					thread.Abort();

				Target.Dispose();
			}
		}

		~StreamThread() => Dispose(false);
	}

	public class VideoStreamThread : StreamThread
	{
		static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
		static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

		static readonly byte[] REQMagic_VIDEO = BitConverter.GetBytes(0xAAAAAAAA);
		const int VbufMaxSz = 0x32000;

		public VideoStreamThread(CancellationToken token, IOutTarget StreamTarget, UsbDevStream InputDevice, bool PrintStatsArg = false) :
			base(token, StreamKind.Video, InputDevice, StreamTarget, REQMagic_VIDEO, VbufMaxSz)
		{
			PrintStats = PrintStatsArg;
		}

		//For video start by sending an SPS and PPS packet to set the resolution, these packets are only sent when launching a game
		//Not sure if they're the same for every game, likely yes due to hardware encoding
		protected override void StreamInitialized()
		{
			Target.SendData(SPS, 0);
			Target.SendData(PPS, 0);
		}
	}

	public class AudioStreamThread : StreamThread
	{
		static readonly byte[] REQMagic_AUDIO = BitConverter.GetBytes(0xBBBBBBBB);
		const int AbufMaxSz = 0x1000 * 12;

		public AudioStreamThread(CancellationToken token, IOutTarget StreamTarget, UsbDevStream InputDevice, bool PrintStatsArg = false) :
			base(token, StreamKind.Audio, InputDevice, StreamTarget, REQMagic_AUDIO, AbufMaxSz)
		{
			PrintStats = PrintStatsArg;
		}
	}
}
