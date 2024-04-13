using LibUsbDotNet.LibUsb;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client
{
	public class CommandLineOptions
	{
		abstract record Option(string Name, bool StopParsing);
		record OptionNoArg(string Name, Action<CommandLineOptions> Handle, bool StopParsing = false) : Option(Name, StopParsing);
		record OptionArg(string Name, Action<CommandLineOptions, string> Handle, bool StopParsing = false) : Option(Name, StopParsing);

		static readonly Option[] Options = new Option[] 
		{
			new OptionArg("--libdir", (x, v) => x.LibDir = v),
			new OptionArg("--debug", (x, v) => x.DebugFlags = v),
			
			new OptionNoArg("--help", x => x.Help = true, true),
			new OptionNoArg("-h", x => x.Help = true, true),

			new OptionNoArg("--version", x => x.Version = true, true),
			new OptionNoArg("-v", x => x.Version = true, true),

			new OptionNoArg("--show-decoders", x => x.ShowDecoders = true, true),
			new OptionNoArg("--debug-list", x => x.DebugList = true, true),
			
			new OptionNoArg("--no-audio", x => x.NoAudio = true),
			new OptionNoArg("--no-video", x => x.NoVideo = true),
			new OptionNoArg("--fullscreen", x => x.LaunchFullscreen = true),
			new OptionArg("--title", (x, v) => x.WindowTitle = v),
			
			new OptionArg("--serial", (x, v) => x.ConsoleSerial = v),

			new OptionArg("--volume", (x, v) => x.Volume = v),
			new OptionNoArg("--screen-off", (x) => x.TurnOfConsoleScreen = true),
			
			// Legacy usb arg
			new OptionArg("--usb-serial", (x, v) => x.ConsoleSerial = v),
			new OptionNoArg("--usb-debug", x => x.UsbLogging = UsbLogLevel.Debug),
			new OptionNoArg("--usb-warn", x => x.UsbLogging = UsbLogLevel.Warning),

			// Advanced options
			new OptionArg("--decoder", (x, v) => x.RequestedDecoderName = v),
			new OptionNoArg("--software-render", (x) => x.SoftwareRendering = true),

			// Deprecated RTSP options
			new OptionNoArg("--rtsp", x => x.RTSPDeprecationWarning = true),
			new OptionNoArg("--rtsp-port", x => x.RTSPDeprecationWarning = true),
			new OptionNoArg("--rtsp-rtsp-any-addr", x => x.RTSPDeprecationWarning = true),

			// Deprecated stdout options
			new OptionNoArg("--mpv", x => x.LowLatencyDeprecationWarning = true),
			new OptionNoArg("--stdout", x => x.LowLatencyDeprecationWarning = true),

			// Deprecated file option
			new OptionNoArg("--file", x => x.FileDeprecationWarning = true),

#if !ANDROID_LIB
			// This only makes sense on desktop systems
			new OptionNoArg("--legacy", x => x.LegacyPlayer= true),
#endif
		};

		record DebugOptionArg(string Name, string Description, Action<DebugOptions> Handle);
		readonly static DebugOptionArg[] DebugOptions = new DebugOptionArg[] {
			new ("log", "Enable verbose logging, auto enabled when a debugger is connected", x => x.Log = true),
			new ("stats", "Print received packets info in real time", x => x.Stats = true),
			new ("keyframe", "Decode NALs and print IDR frames", x => x.Keyframe = true),
			new ("nal", "Decode all NALs and print the type", x => x.Nal = true),
			new ("nosync", "Disable A/V synchronization", x => x.NoSync = true),
			new ("noprot", "Disable dll-injection protection on Windows", x => x.NoProt = true),
			new ("dynlib", "Enable verbose logging for native library loading", x => x.DynLib = true),
		};

		public static CommandLineOptions Parse(string[] args)
		{
			var opt = new CommandLineOptions();

			opt.QuickLaunch = args.Length == 0;
			if (opt.QuickLaunch) return opt;

			int argParseStart = 0;

			// We must handle all the following cases:
			//		Debug option but no streaming     client.exe --debug log
			//		Streaming with other options      client.exe usb --debug log ...
			//		Error on invalid streaming type   client.exe invalid --debug log ...
			if (!args[0].StartsWith("--"))
			{
				opt.StreamingMode = args[0] switch
				{
					"bridge" => StreamMode.Network,
					"usb" => StreamMode.Usb,
					"stub" => StreamMode.Stub,
					_ => throw new Exception($"Unknown streaming mode {args[0]}")
				};

				argParseStart = 1;
			}

			// In network mode also take the ip to connect to, this is now optional and if not specified it will connect to the first console that is detected by the network scanner
			if (opt.StreamingMode == StreamMode.Network && args.Length > 1 && !args[1].StartsWith("--"))
			{
				opt.NetStreamHostname = args[1];
				argParseStart = 2;
			}

			for (int i = argParseStart; i < args.Length; i++)
			{
				var hanlde = Options.FirstOrDefault(x => x.Name == args[i]);
				if (hanlde == null)
					continue; // Ignore unknown options to keep accepting old incompatible args

				if (hanlde is OptionNoArg noArg)
					noArg.Handle(opt);
				else if (hanlde is OptionArg arg)
					arg.Handle(opt, args[++i]);
			}		

			return opt;
		}

		// Special command line options that interrupt the normal cli parse flow
		public bool ShowDecoders;
		public bool Help;
		public bool Version;
		public bool DebugList;
		public bool QuickLaunch;

		// Streaming options
		public enum StreamMode 
		{
			None,
			Network,
			Usb,
			Stub,
		};
		
		public StreamMode StreamingMode = StreamMode.None;

		public bool NoVideo;
		public bool NoAudio;

		public bool RTSPDeprecationWarning;
		public bool LowLatencyDeprecationWarning;
		public bool FileDeprecationWarning;

		public string? ConsoleSerial;
		public string? NetStreamHostname;

		public bool LaunchFullscreen;

		public string? Volume;
		public bool? TurnOfConsoleScreen;

		// Other advanced options
		public string? LibDir;
		public string? DebugFlags;
		public string? RequestedDecoderName;
		public string? WindowTitle;
		public UsbLogLevel? UsbLogging;

		public bool LegacyPlayer;

		public bool SoftwareRendering;

		public void ApplyOptionOverrides() 
		{
			DynamicLibraryLoader.LibLoaderOverride = LibDir;

			if (NoVideo)
				Program.Options.Streaming.Kind = StreamKind.Audio;
			if (NoAudio)
				Program.Options.Streaming.Kind = StreamKind.Video;

			if (UsbLogging.HasValue)
				Program.Options.UsbLogging = UsbLogging.Value;

			if (DebugFlags is not null)
			{
				var dbg = DebugFlags.Split(',')
					.Select(x => DebugOptions.FirstOrDefault(y => y.Name == x))
					.Where(x => x is not null)
					.ToList();

				dbg.ForEach(x => x.Handle(Program.Options.Debug));

				// Force logging in case a debug option is specified
				if (dbg.Any())
					Program.Options.Debug.Log = true;
			}

			Program.Options.DecoderName = RequestedDecoderName;

			if (SoftwareRendering)
				Program.Options.ForceSoftwareRenderer = true;

			if (Volume is not null && int.TryParse(Volume, out var vol))
			{ 
				vol = Math.Clamp(vol, 0, 100);
				Program.Options.DefaultVolume = vol;
			}

			if (TurnOfConsoleScreen.HasValue)
				Program.Options.Streaming.TurnOffConsoleScreen = TurnOfConsoleScreen.Value;
		}

		public void PrintDeprecationWarnings() 
		{
			if (FileDeprecationWarning)
				Console.WriteLine("The --file option has been removed starting from SysDVR 6.0, you can use the gameplay recording feature whitin the new player instead.");
			if (LowLatencyDeprecationWarning)
				Console.WriteLine("The --mpv and --stdout options have been removed starting from SysDVR 6.0, you should use the default player.");
			if (RTSPDeprecationWarning)
				Console.WriteLine("The --rtsp options have been removed starting from SysDVR 6.0, to stram over RTSP use simple network mode from your console.");
		}

		public static string GetDebugFlagsList() 
		{
			return "Available debug options: \n" +
				string.Join("\n", DebugOptions.Select(x => $"\t{x.Name}: {x.Description}"));

		}

		public const string HelpMessage = @"Usage:
SysDVR-Client.exe <Stream source> [Source options] [Stream options] [Output options]

Stream sources:
	The source mode is how the client connects to SysDVR running on the console. Make sure to set the correct mode with SysDVR-Settings.
	`usb` : Connects to SysDVR via USB, used if no source is specified. Remember to setup the driver as explained on the guide
	`bridge [IP address]` : Connects to SysDVR via network at the specified IP address, requires a strong connection between the PC and switch (LAN or full signal wireless). If the IP address is not specified the client will connect to the first console it detects, you can also use the --serial option to pick a specific one by serial

	Note that the `Simple network mode` option in SysDVR-Settings does not require the client, you must open it directly in a video player.

Source options:
	`--serial NX0000000` : When multiple consoles are plugged in via USB or are detected via LAN scanning use this option to automatically select one by serial number. 
		This also matches partial serials starting from the end, for example NX012345 will be matched by doing --usb-serial 45
	`--usb-warn` : Enables printing warnings from the usb stack, use it to debug USB issues
	`--usb-debug` : Same as `--usb-warn` but more detailed
	`--screen-off` : Turn off the console screen during streaming

Stream options:
	`--no-video` : Disable video streaming, only streams audio
	`--no-audio` : Disable audio streaming, only streams video

Player options:
	Since 6.0 SysDVR-Client only comes with the built-in player, the following options are available:
	`--volume <value>` : Set the player volume to a number between 0 and 100
	`--hw-acc` : Try to use hardware acceleration for decoding, this option uses the first detected decoder, it's recommended to manually specify the decoder name with --decoder
	`--decoder <name>` : Use a specific decoder for ffmpeg decoding, you can see all supported codecs with --show-decoders
	`--fullscreen` : Start in full screen mode. Press F11 to toggle manually
	`--title <some text>` : Adds the argument string to the title of the player window
	`--legacy` : Use the old player without the GUI, not all the other options are available when this is used
	`--debug <debug options>` : Enables debug options. Multiple options are comma separated for example: --debug log,stats
		When a debugger is attached `log` is enabled by default.

Extra options:
	These options will not stream, they just print the output and then quit.
	`--show-decoders` : Prints all video codecs available for the built-in video player
	`--version` : Prints the version
	`--libdir` : Overrides the dynami library loading path, use only if dotnet can't locate your ffmpeg/avcoded/SDL2 libraries automatically
                 this option effect depend on your platform, some libraries may still be handled by dotnet, it's better to use your loader path environment variable.
	`--debug-list` : Prints all available debug options.
	`--software-render` : Launches SDL with software rendering, this runs very poorly and it is only meant as a debug option

Command examples:
	SysDVR-Client.exe usb
		Connects to switch via USB and streams video and audio
		
	SysDVR-Client.exe bridge 192.168.1.20 --no-video
		Connects to switch via network at 192.168.1.20 and streams only the audio
";

	}
}
