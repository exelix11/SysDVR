
//#define PRINT_DEBUG
#define PLAY_STATS

using libusbK;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetStream
{
	class Program
	{
		static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
		static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

		enum StreamKind 
		{
			Video,
			Audio
		};

		static readonly byte[] REQMagic_VIDEO = BitConverter.GetBytes(0xAAAAAAAA);
		static readonly byte[] REQMagic_AUDIO = BitConverter.GetBytes(0xBBBBBBBB);
		const int VbufMaxSz = 0x32000;
		const int AbufMaxSz = 0x1000 * 10;

		interface IOutTarget : IDisposable
		{
			public void SendData(byte[] data) => SendData(data, 0, data.Length);
			void SendData(byte[] data, int offset, int size);
		}

		class OutFileTarget : IOutTarget
		{
			FileStream Vfs;

			public OutFileTarget(string fname) 
			{
				Vfs = File.Open(fname, FileMode.Create);
			}

			public void Dispose()
			{
				Vfs.Close();
				Vfs.Dispose();
			}

			public void SendData(byte[] data, int offset, int size)
			{
				Vfs.Write(data, offset, size);
			}
		}

		class TCPTarget : IOutTarget
		{
			Socket VidSoc;

			public TCPTarget(System.Net.IPAddress addr, int port)
			{
				var v = new TcpListener(addr, port);
				v.Start();
				VidSoc = v.AcceptSocket();
				v.Stop();
			}

			public void Dispose()
			{
				VidSoc.Close();
				VidSoc.Dispose();
			}
					   
			public void SendData(byte[] data, int offset, int size)
			{
				VidSoc.Send(data, offset, size, SocketFlags.None);
			}
		}

		class StdInMPV : IOutTarget
		{
			Process proc;

			public StdInMPV(string path, string args)
			{
				ProcessStartInfo p = new ProcessStartInfo()
				{
					Arguments = " - " + args,
					FileName = path,
					RedirectStandardInput = true,
				};
				proc = Process.Start(p);
			}

			public void Dispose()
			{
				if (!proc.HasExited)
					proc.Kill(); 
			}

			public void SendData(byte[] data, int offset, int size)
			{
				proc.StandardInput.BaseStream.Write(data, offset, size);
			}
		}

		static UsbDevice GetDevice() 
		{
			var pat = new KLST_PATTERN_MATCH { DeviceID = @"USB\VID_057E&PID_3000" };
			var lst = new LstK(0, ref pat);
			lst.MoveNext(out var dinfo);
			return new UsbDevice(dinfo);
		}


		static bool StreamLoop(IOutTarget Target, UsbDevStream stream, StreamKind kind)
		{
#if PLAY_STATS
			Stopwatch ThreadTimer = new Stopwatch();
			long TransfersPerSecond = 0;
			long BytesPerSecond = 0;
			ThreadTimer.Start();

			void UpdatePlayStats() 
			{
				if (ThreadTimer.ElapsedMilliseconds < 1000) return;
				ThreadTimer.Stop();
				Console.WriteLine($"{kind} stream: {TransfersPerSecond} - {BytesPerSecond / ThreadTimer.ElapsedMilliseconds} KB/s");
				TransfersPerSecond = 0;
				BytesPerSecond = 0;
				ThreadTimer.Restart();
			}
#endif

			byte[] ReqMagic = kind == StreamKind.Video ? REQMagic_VIDEO : REQMagic_AUDIO;
			int MaxBufSize = kind == StreamKind.Video ? VbufMaxSz : AbufMaxSz;

			//For video start by sending an SPS and PPS packet to set the resolution, these packets are only sent when launching a game
			//Not sure if they're the same for every game, likely yes due to hardware encoding
			if (kind == StreamKind.Video)
			{
				Target.SendData(SPS);
				Target.SendData(PPS);
			}

			byte[] SizeBuf = new byte[4];
			ArrayPool<byte> sh = ArrayPool<byte>.Create();
			byte[] data = null;

			uint ReadToSharedArray() 
			{
				stream.Read(SizeBuf);
				var size = BitConverter.ToUInt32(SizeBuf);
				if (size > MaxBufSize || size == 0) return size;

				data = sh.Rent((int)size);
				stream.Read(data, 0, (int)size);

				return size;
			}

			void FreeBuffer() 
			{
				if (data != null) sh.Return(data);
				data = null;
			}

			try
			{
				while (!Console.KeyAvailable)
				{
					stream.Write(ReqMagic);

					var size = ReadToSharedArray();
					if (size > MaxBufSize || size == 0)
					{
						Console.WriteLine($"Discarding {kind} packet of size {size}");
						stream.Flush();
					}
					else
					{
						Target.SendData(data, 0, (int)size);
						FreeBuffer();
						#if PRINT_DEBUG
							Console.WriteLine($"video {size}");
						#endif
						
						#if PLAY_STATS
							TransfersPerSecond++;
							BytesPerSecond += size;
							UpdatePlayStats();
						#endif
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("There was an exception: " + ex.ToString());
				FreeBuffer();
			}
			return false;
		}

		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine("Usage: \r\n" +
				"NetStream <video method> <video file/port/mpv path> <audio method> <audio file/port/mpv path>\r\n" +
				"Method is either mpv, file, tcp or null (only for audio)\r\n" +
				"Note that tcp initialization will block till a program connects\r\n\r\n" +
				"Useful commands: \r\n" +
				"mpv tcp://localhost:1337 --no-correct-pts --fps=30 --cache=no --cache-secs=0\r\n" +
				"mpv tcp://localhost:1338 --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000");
				return;
			}

			IOutTarget VTarget = null;
			IOutTarget ATarget = null;

			int AudioArgsStart = 2;

			if (args[0] == "file")
				VTarget = new OutFileTarget(args[1]);
			else if (args[0] == "tcp")
				VTarget = new TCPTarget(System.Net.IPAddress.Any, int.Parse(args[1]));
			else if (args[0] == "mpv")
				VTarget = new StdInMPV(args[1], "--no-correct-pts --fps=30 --cache=no --cache-secs=0");
			else if (args[0] == "null")
			{
				VTarget = null;
				AudioArgsStart = 1;
			}
			else throw new Exception("Unknown video method");

			if (args[AudioArgsStart] == "file")
				ATarget = new OutFileTarget(args[AudioArgsStart + 1]);
			else if (args[AudioArgsStart] == "tcp")
				ATarget = new TCPTarget(System.Net.IPAddress.Any, int.Parse(args[AudioArgsStart + 1]));
			else if (args[AudioArgsStart] == "null")
				ATarget = null;
			else if (args[AudioArgsStart] == "mpv")
				ATarget = new StdInMPV(args[AudioArgsStart + 1], "--no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000");
			else throw new Exception("Unknown audio method");

			var stream = GetDevice();

			Thread VideoThread = null;
			if (VTarget != null)
				VideoThread = new Thread(() => StreamLoop(VTarget, stream.OpenStreamDefault(), StreamKind.Video));

			Thread AudioThread = null;
			if (ATarget != null)
				AudioThread = new Thread(() => StreamLoop(ATarget, stream.OpenStreamAlt(), StreamKind.Audio));

			VideoThread?.Start();
			AudioThread?.Start();

			VideoThread?.Join();
			VTarget?.Dispose();
			AudioThread?.Join();
			ATarget?.Dispose();

		}
	}

	static class Exten 
	{
		public static bool Matches(this byte[] arr, byte[] Magic) =>
			arr.Matches(0, Magic, 0, Magic.Length);

		public static bool Matches(this byte[] arr, int arrOfset, byte[] Magic, int MagicOffset, int length)
		{
			for (int i = 0; i < length; i++)
				if (arr[arrOfset + i] != Magic[MagicOffset + i])
					return false;
			return true;
		}
	}

}
