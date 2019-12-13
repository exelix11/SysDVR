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

namespace UsbStream
{
	class Program
	{
		public static UsbContext LibUsbCtx = null;

		static UsbDevice GetDevice(LibUsbDotNet.LogLevel level) 
		{
			if (LibUsbCtx != null)
				throw new Exception("Libusb has already been initialized");

			LibUsbCtx = new UsbContext();
			LibUsbCtx.SetDebugLevel(level);
			var usbDeviceCollection = LibUsbCtx.List();
			var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3006 && d.VendorId == 0x057e);

			if (selectedDevice == null)
				return null;

			selectedDevice.Open();

			return new UsbDevice(selectedDevice);
		}

		static void PrintGuide()
		{
			Console.WriteLine("Usage: \r\n" +
					"UsbStream video <stream config for video> audio <stream config for audio>\r\n" +
					"You can omit the stream you don't want\r\n" +
					"Stream config is one of the following:\r\n" +
					" - tcp <port> : stream the data over a the network on the specified port.\r\n" +
					" - file <file name> : stores the received data to a file\r\n" +
					"   The format is raw h264 data for video and uncompressed s16le stereo 48kHz samples for sound\r\n" +
					" - stdin <executable path> args <program arguments> : Pipes the received data to another program, <executable path> is the other program's path and <program arguments> are the args to pass to the target program, you can omit args if the program doesn't need any configuration\r\n" +
					" - mpv <mpv player path> : same as stdin but automatically configures args for mpv. On windows use mpv.com instead of mpv.exe, omitting the extension will automatically use the right one\r\n" +
					"Streaming both video and audio at the same time could cause performance issues.\r\n" +
					"Note that tcp mode will wait until a program connects\r\n\r\n" +
					"Example commands: \r\n" +
					"UsbStream audio mpv C:/programs/mpv/mpv : Plays audio via mpv located at C:/programs/mpv/mpv, video is ignored\r\n" +
					"UsbStream video mpv ./mpv audio mpv ./mpv : Plays video and audio via mpv (path has to be specified twice)\r\n" +
					"UsbStream video mpv ./mpv args \"--cache=no --cache-secs=0\" : Plays video in mpv disabling cache, audio is ignored\r\n" +
					"UsbStream video tcp 1337 audio file C:/audio.raw : Streams video over port 1337 while saving audio to disk\r\n\r\n" +
					"Opening raw files in mpv: \r\n" +
					"mpv videofile.264 --no-correct-pts --fps=30 --cache=no --cache-secs=0\r\n" +
					"mpv audiofile.raw --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000\r\n" +
					"(you can also use tcp://localhost:<port> instead of the file name to open the tcp stream)\r\n\r\n" +
					"Info to keep in mind:\r\n" +
					"Streaming works only with games that have game recording enabled.\r\n" +
					"If the video is very delayed or lagging try going to the home menu for a few seconds to force it to re-synchronize.\r\n" +
					"After disconnecting and reconnecting the usb wire the stream may not start right back, go to the home menu for a few seconds to let the sysmodule drop the last usb packets.\r\n\r\n" +
					"Experimental/Debug options:\r\n" +
					"--desync-fix : flush incoming packets and delay further requests to avoid desync when the bandwidth goes under a certain threshold\r\n" +
					"--print-stats : print the average transfer speed and loop count for each thread every second\r\n" +
					"--usb-warn : print warnings from libusb\r\n" +
					"--usb-debug : print verbose output from libusb");
			Console.ReadLine();
		}

		static void Main(string[] args)
		{
			Console.WriteLine("UsbStream - 2.0 by exelix");
			Console.WriteLine("https://github.com/exelix11/SysDVR \r\n");
			if (args.Length < 3)
			{
				PrintGuide();
				return;
			}

			IOutTarget VTarget = null;
			IOutTarget ATarget = null;
			bool PrintStats = false;
			bool DesyncFix = false;
			LogLevel UsbLogLevel = LogLevel.Error;

			void ParseTargetArgs(int baseIndex, ref IOutTarget t)
			{
				switch (args[baseIndex + 1])
				{
					case "mpv":
					case "stdin":
						{
							string fargs = "";
							if (args[baseIndex + 1] == "mpv") 
								fargs = args[baseIndex] == "video" ? "- --no-correct-pts --fps=30 " : "- --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000 ";

							if (args.Length > baseIndex + 4 && args[baseIndex + 3] == "args")
								fargs += args[baseIndex + 4];

							t = new StdInTarget(args[baseIndex + 2], fargs);

							break;
						}
					case "tcp":
						{
							int port = int.Parse(args[baseIndex + 2]);
							Console.WriteLine($"Waiting for a client to connect on {port} ...");
							t = new TCPTarget(System.Net.IPAddress.Any, port);
							break;
						}
					case "file":
						t = new OutFileTarget(args[baseIndex + 2]);
						break;
#if DEBUG
					case "log":
						t = new LoggingTarget($"F:/{args[baseIndex]}.binlog");
						break;
#endif
					default:
						throw new Exception($"{args[baseIndex + 1]} is not a valid video mode");
				}
			}

			bool HasArg(string arg) => Array.IndexOf(args, arg) != -1;

			{
				int index = Array.IndexOf(args, "video");
				if (index >= 0) ParseTargetArgs(index, ref VTarget);
				index = Array.IndexOf(args, "audio");
				if (index >= 0) ParseTargetArgs(index, ref ATarget);

				DesyncFix = HasArg("--desync-fix");
				PrintStats = HasArg("--print-stats");
				if (HasArg("--usb-warn")) UsbLogLevel = LogLevel.Info;
				if (HasArg("--usb-debug")) UsbLogLevel = LogLevel.Debug;
			}

			if (VTarget == null && ATarget == null)
			{
				Console.WriteLine("Specify at least a video or audio target");
				return;
			}

			var stream = GetDevice(UsbLogLevel);
			if (stream == null)
			{
				Console.WriteLine("Device not found, did you configure the drivers properly ?");
				return;
			}

			CancellationTokenSource StopThreads = new CancellationTokenSource();

			VideoStreamThread Video = null;
			if (VTarget != null)
				Video = new VideoStreamThread(StopThreads.Token, VTarget, stream.OpenStreamDefault(), PrintStats, DesyncFix);
				
			AudioStreamThread Audio = null;
			if (ATarget != null)
				Audio = new AudioStreamThread(StopThreads.Token, ATarget, stream.OpenStreamAlt(), PrintStats);	

			Video?.StartThread();
			Audio?.StartThread();

			Console.WriteLine("Starting stream, press return to stop");
			Console.ReadLine();
			Console.WriteLine("Terminating threads...");
			StopThreads.Cancel();

			Video?.JoinThread();
			Audio?.JoinThread();

			Video?.Dispose();
			Audio?.Dispose();

			LibUsbCtx.Dispose();
		}
	}
}
