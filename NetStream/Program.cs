//#define PRINT_DEBUG
#define PLAY_STATS

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

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
		const int AbufMaxSz = 0x1000 * 12;

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
			using (var context = new UsbContext())
			{
				context.SetDebugLevel(LogLevel.Error);

				//Get a list of all connected devices
				var usbDeviceCollection = context.List();

				//Narrow down the device by vendor and pid
				var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3000 && d.VendorId == 0x057e);

				if (selectedDevice == null)
					throw new Exception("Device not found");

				//Open the device
				selectedDevice.Open();

				return new UsbDevice(selectedDevice);
			}

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

			int ReadToSharedArray() 
			{
				SizeBuf[0] = SizeBuf[1] = SizeBuf[2] = SizeBuf[3] = 0;
				stream.Read(SizeBuf);
				var size = BitConverter.ToUInt32(SizeBuf);
				if (size > MaxBufSize || size == 0) return 0;

				data = sh.Rent((int)size);
				int actualsize = stream.Read(data, 0, (int)size);
				if (actualsize != size) Console.WriteLine("Warning: Reported size doesn't match received size");
				return actualsize;
			}

			void FreeBuffer() 
			{
				if (data != null) sh.Return(data);
				data = null;
			}

			#if !DEBUG
			try
			{
			#endif
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
#if !DEBUG
				
			}
			catch (Exception ex)
			{
				Console.WriteLine("There was an exception: " + ex.ToString());
				FreeBuffer();
			}
#endif
			return false;
		}

		static void Main(string[] args)
		{
			Console.WriteLine("SysVStream - 1.0 by exelix");
			Console.WriteLine("https://github.com/exelix11/SysVStream \r\n");
			if (args.Length < 3)
			{
				Console.WriteLine("Usage: \r\n" +
				"NetStream <video config> <audio conifg>\r\n" +
				"Supported configurations are: \r\n" +
				" - tcp <port> : stream the data over a the network on the specified port.\r\n" +
				" - file <file name> : stores the received data to a file\r\n" +
				"   The format is raw h264 data for video and uncompressed s16le stereo 48kHz samples for sound\r\n" +
				" - mpv <mpv player path> : streams the received data to mpv player via stdin\r\n" +
				" - null : ignore this stream\r\n" +
				"It is not recommended to stream audio and video at the same time\r\n" +
				"Note that tcp initialization will block untill a program connects\r\n\r\n" +
				"Example commands: \r\n" +
				"NetStream null mpv C:/programs/mpv/mpv : Plays audio via mpv located at C:/programs/mpv/mpv, video is ignored\r\n" +
				"NetStream mpv C:/programs/mpv/mpv mpv C:/programs/mpv/mpv : Plays video and audio via mpv (path has to be specified twice)\r\n" +
				"NetStream tcp 1337 file C:/audio.raw : Streams video over port 1337 while saving audio to disk\r\n\r\n" +
				"Opening raw files in mpv: \r\n" +
				"mpv videofile.264 --no-correct-pts --fps=30 --cache=no --cache-secs=0\r\n" +
				"mpv audiofile.raw --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000\r\n" +
				"(you can also use tcp://localhost:<port> instead of the file name to open the tcp stream)\r\n\r\n" +
				"Info to keep in mind:\r\n" +
				"Streaming works only with games that have game recording enabled.\r\n" +
				"If the video is very delayed or lagging try going to the home menu for a few seconds to force it to re-synchronize.\r\n" +
				"After disconnecting and reconnecting the usb wire the stream may not start right back, go to the home menu for a few seconds to let the sysmodule drop the last usb packets.");
				Console.ReadLine();
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
				VTarget = new StdInMPV(args[1], "--no-correct-pts --fps=30");
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
}
