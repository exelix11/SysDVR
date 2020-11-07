using System;
using System.IO;
using System.Text;

namespace SysDVR.Client
{
	class Program
	{
		static string VersionString() 
		{
			var Version = typeof(Program).Assembly.GetName().Version;
			if (Version == null) return "<unknown version>";
			StringBuilder str = new StringBuilder();
			str.Append(Version.Major);
			str.Append(".");
			str.Append(Version.Minor);

			if (Version.Revision != 0)
			{
				str.Append(".");
				str.Append(Version.Revision);
			}

			return str.ToString();
		}

		static void PrintGuide(bool full)
		{
			if (!full) {
				Console.WriteLine("Basic usage:\r\n" +
						"Simply launching this exectuable will show this message and launch the RTSP server via USB.\r\n" +
						"Use 'SysDVR-Client usb' to stream directly, add '--no-audio' or '--no-video' to disable one of the streams\r\n" +
						"To stream in TCP Bridge mode launch 'SysDVR-Client bridge <switch ip address>'\r\n" +
						"There are more advanced options, you can see them with 'SysDVR-Client --help'\r\n" +
						"Press enter to continue.\r\n");
				Console.ReadLine();
				return;
			}

			Console.WriteLine(
@"Usage:
SysDVR-Client.exe <Stream source> [Source options] [Stream options] [Output options]

Stream sources:
	`usb` : Connects to SysDVR via USB, used if no source is specified. Remember to setup the driver as explained on the guide
	`bridge <IP address>` : Connects to SysDVR via network at the specified IP address, requires a strong connection between the PC and switch (LAN or full signal wireless)

Source options:
	`--print-stats` : Logs received data size and errors
	`--no-winusb` : Forces the LibUsb backend on windows, you must use this option in case you installed LibUsb-win32 as the SysDVR driver (it's recommended to use WinUsb)
	`--usb-warn` : Enables printing warnings from the usb stack, use it to debug USB issues
	`--usb-debug` : Same as `--usb-warn` but more detailed

Stream options:
	`--no-video` : Disable video streaming, only streams audio
	`--no-audio` : Disable audio streaming, only streams video

Output options:
	Low-latency streaming options
	`--mpv <mpv path>` : Streams the specified channel to mpv via stdin, only works with one channel, if no stream option is specified `--no-audio` will be used.
	`--stdout` : Streams the specified channel to stdout, only works with one channel, if no stream option is specified `--no-audio` will be used.
	Storage options
	`--file <folder path>` : Stores to the specified folder the streams, video will be saved as `video.h264` and audio as `audio.raw`, existing files will be overwritten.
	If you don't specify any of these SysDVR-Client will stream via RTSP, the following are RTSP options
	`--rtsp-port <port number>` : Port used to stream via RTSP (default is 6666)
	`--rtsp-any-addr` : The RTSP socket will be open for INADDR_ANY, this means other devices in your local network can connect to your pc by IP and watch the stream

Command examples:
	SysDVR-Client.exe usb
		Connects to switch via USB and streams video and audio over rtsp at rtsp://127.0.0.1:6666/
		
	SysDVR-Client.exe bridge 192.168.1.20 --no-video --rtsp-port 9090
		Connects to switch via network at 192.168.1.20 and streams the audio over rtsp at rtsp://127.0.0.1:9090/

	SysDVR-Client.exe usb --mpv `C:\Program Files\mpv\mpv.com`
		Connects to switch via USB and streams the video in low-latency mode via mpv
");
		}

		static void Main(string[] args)
		{
			bool HasArg(string arg) => Array.IndexOf(args, arg) != -1;
			bool StreamStdout = HasArg("--stdout");

			if (StreamStdout)
				Console.SetOut(Console.Error);

			Console.WriteLine($"SysDVR-Client - {VersionString()} by exelix");
			Console.WriteLine("https://github.com/exelix11/SysDVR \r\n");
			if (args.Length < 1)
				PrintGuide(false);
			else if (args[0].Contains("help"))
			{
				PrintGuide(true);
				return;
			}

			BaseStreamManager StreamManager;
			bool NoAudio, NoVideo;

			string ArgValue(string arg) 
			{
				int index = Array.IndexOf(args, arg);
				if (index == -1) return null;
				if (args.Length <= index + 1) return null;
				return args[index + 1];
			}

			int? ArgValueInt(string arg) 
			{
				var a = ArgValue(arg);
				if (int.TryParse(a, out int res))
					return res;
				return null;
			}

			if (HasArg("--usb-warn")) UsbContext.Logging = UsbContext.LogLevel.Warning;
			if (HasArg("--usb-debug")) UsbContext.Logging = UsbContext.LogLevel.Debug;

			NoAudio = HasArg("--no-audio");
			NoVideo = HasArg("--no-video");
			StreamingThread.Logging = HasArg("--print-stats");
			UsbContext.ForceLibUsb = HasArg("--no-winusb");

			if (NoVideo && NoAudio)
			{
				Console.WriteLine("Specify at least a video or audio target");
				return;
			}

			if (StreamStdout)
			{
				if (!NoVideo && !NoAudio)
					NoAudio = true;
				StreamManager = new StdOutManager(NoAudio ? StreamKind.Video : StreamKind.Audio);
			}
			else if (HasArg("--mpv"))
			{
				string mpvPath = ArgValue("--mpv");
				if (mpvPath == null || !File.Exists(mpvPath))
				{
					Console.WriteLine("The specified mpv path is not valid");
					return;
				}
				if (!NoVideo && !NoAudio)
					NoAudio = true;
				StreamManager = new MpvStdinManager(NoAudio ? StreamKind.Video : StreamKind.Audio, mpvPath);
			}
			else if (HasArg("--file"))
			{
				string diskPath = ArgValue("--file").Replace("\"", "");
				if (diskPath == null || !Directory.Exists(diskPath))
				{
					Console.WriteLine("The specified directory is not valid");
					return;
				}
				StreamManager = new SaveToDiskManager(!NoVideo, !NoAudio, diskPath);
			}
#if DEBUG
			else if (HasArg("--debug"))
			{
				string path = ArgValue("--debug");
				StreamManager = new LoggingManager(NoVideo ? null : Path.Combine(path, "video.h264"), NoAudio ? null : Path.Combine(path, "audio.raw"));
			}
#endif
			else if (HasArg("--rtsp"))
			{
				int port = ArgValueInt("--rtsp-port") ?? 6666;
				if (port <= 1024)
					Console.WriteLine("Warning: ports lower than 1024 are usually reserved and may require administrator/root privileges");
				StreamManager = new RTSP.SysDvrRTSPManager(!NoVideo, !NoAudio, !HasArg("--rtsp-any-addr"), port);
			}
			else // Stream to the built-in player by default
			{
				StreamManager = new Player.PlayerManager(!NoVideo, !NoAudio);
			}

			if (args.Length == 0 || args[0] == "usb")
			{
				var ctx = UsbContext.GetInstance();

				if (!NoVideo)
					StreamManager.VideoSource = ctx.MakeStreamingSource(StreamKind.Video);
				if (!NoAudio)
					StreamManager.AudioSource = ctx.MakeStreamingSource(StreamKind.Audio);
			}
			else if (args[0] == "bridge")
			{
				if (args.Length < 2)
				{
					Console.WriteLine("Specify an ip address for bridge mode");
					return;
				}

				string ip = args[1];

				if (!NoVideo)
					StreamManager.VideoSource = new TCPBridgeSource(ip, StreamKind.Video);
				if (!NoAudio)
					StreamManager.AudioSource = new TCPBridgeSource(ip, StreamKind.Audio);
			}
			else
			{
				Console.WriteLine("Invalid source");
				return;
			}

			StartStreaming(StreamManager);
		}

		static void StartStreaming(BaseStreamManager Streams)
		{
			Console.WriteLine("Starting stream, press return to stop");
			Streams.Begin();

			Console.ReadLine();
			Console.WriteLine("Terminating threads...");

			Streams.Stop();
		}
	}
}
