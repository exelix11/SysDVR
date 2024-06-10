using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
	internal class StringTableMetadata
	{
		[JsonIgnore]
		public string FileName;

		// This is the font that will be loaded to render UI-text, you can use this field to provide a custom font for rendering unsupported characters
		public string FontName = "OpenSans.ttf";

		// These are just metadata fields shown in the settings page and do not affect the behavior of translation loading
		public string TranslationName = "English";
		public string TranslationAuthor = "SysDVR";

		// This is a list of locales that will be used to to load translations
		// The values come from the CultureInfo.Name property of c# https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=net-8.0#system-globalization-cultureinfo-name
		// See examples at http://www.codedigest.com/CodeDigest/207-Get-All-Language-Country-Code-List-for-all-Culture-in-C---ASP-Net.aspx
		public string[] SystemLocale = ["en-us", "en-gb"];

		public enum GlyphRange 
		{
			NotSpecified,
			//ChineseFull, // This has too many characters and the font texture is not loaded by SDL, as a workaround we manually build the glyph table from the actually used characters
			ChineseSimplifiedCommon,
			Cyrillic,
			Default,
			Greek,
			Japanese,
			Korean,
			Thai,
			Vietnamese
		}

		// This is the glyph range as defined in ImGUi, see https://github.com/ocornut/imgui/blob/master/docs/FONTS.md#fonts-loading-instructions
		public GlyphRange ImGuiGlyphRange = GlyphRange.NotSpecified;
	}

	// This class is used to translate the client UI in other languages
	// This file only contains the default language which is used as a fallback when a translation is missing
	// You should modify the reference json file in the Resources/Translations folder
	// Please read the translation guide at <TBD>
	internal class StringTable : StringTableMetadata
	{
		// The following are "legacy player" strings, they are only printed to the console when using the legacy player
		internal class LegacyPlayerTable
		{
			public string Starting = "Starting in legacy mode...";
			public string AudioOnly = "No video output requested, press return to quit";
			public string Started =
				"Starting to stream, keyboard shortcuts list:\n" +
				"\t- F11: toggle full screen\n" +
				"\t- esc: quit\n" +
				"\t- return: Print debug information";

			public string MultipleDevicesLine1 = "Multiple devices found:";
			// Do not translate command line options such as --serial
			public string MultipleDevicesLine2 = "The first one will be used, you can select a specific one by using the --serial option in the command line";
			
			public string DeviceNotFoundRetry = "Device not found, trying again in 5 seconds...";
			public string UsbDeviceNotFound = "No usb device found";
			
			public string NoStreamingError = "No streaming has been requested. For help, use --help";
			public string TcpWithoutIPError = "TCP mode without a target IP is not supported in legacy mode, use --help for help";

			public string CommandLineError = "Invalid command line. The legacy player only supports a subset of options, use --help for help";
		}

		// The following are general strings used in multiple places
		internal class GeneralTable 
		{
			public string PopupCloseButton = "Close";
			public string YesButton = "Yes";
			public string NoButton = "No";
			public string CancelButton = "Cancel";
			public string ApplyButton = "Apply";
			public string BackButton = "Go back";
			public string PopupErrorTitle = "Error";
			
			// This is shown before the actual text of the error
			// the error text might come from the system and already be in the user's language
			public string ErrorMessage = "Error:";

			// Shown when the user requested to not show serial numbers in the UI
			public string PlaceholderSerial = "(serial hidden)";
		}

		// The following are strings used by the main GUI page
		internal class HomePageTable 
		{
			public string InitializationErrorTitle = "Initialization error";
			public string NetworkButton = "Network mode";
			public string USBButton = "USB mode";
			public string ChannelLabel = "Select the streaming mode";
			public string ChannelVideo = "Video only";
			public string ChannelAudio = "Audio only";
			public string ChannelBoth = "Stream Both";

			public string SettingsButton = "Settings";
			public string GuideButton = "Guide";
			public string GithubButton = "Github page";

			// The following are only shown on android
			public string FileAccess = "Warning: File access permission was not granted, saving recordings may fail.";
			public string FileAccessRequestButton = "Request permission";

			// The following are only shown on windows
			public string DriverInstallButton = "USB driver";
		}

		// Strings related to the page shown while a connection is being made or the connection failed
		internal class ConnectionTable 
		{
			public string Title = "Connecting";
			public string Error = "Fatal error";

			public string AutoConnect = "SysDVR will connect automatically to the first valid device";
			// {0} is replaced with the user-provided serial
			public string AutoConnectWithSerial = "SysDVR will connect automatically to the console with serial containing: {0}";
			public string AutoConnectCancelButton = "Cancel auto conect";

			public string DeviceNotCompatible =
				"The selected device is not compatible with this version of the client.\n" +
				"Make sure you're using the same version of SysDVR on both the console and this device.";

			// {0} and {1} are replaced with the respecitve number of attempts
			public string ConnectionInProgress = "Connecting to console (attempt {0} out of {1}";

			// Shown when the client is connecting to a console from a manual input (eg, by ip address)
			public string ManualConnection = "Manual connection";
		}

		// Strings related to the network scan page
		internal class NetworkScanTable
		{
			public string Title = "Searching for network devices...";
			public string ManualConnectLabel = "Can't find your device ?";
			public string ManualConnectButton = "Use IP address";

			public string ConnectByIPLabel = "This is the local IP address of your console, it should look like 192.168.X.Y. You can find it in the console settings or in the SysDVR-Settings homebrew.\nIf you can't connect make sure you enabled TCP bridge mode on the console.";
			public string ConnectByIPButton = "Connect";
			public string IpConnectDialogTitle = "Enter console IP address";

			// This is shown only on PC devices
			public string FirewallLabel = "Remember to allow SysDVR client in your firewall or else it won't be able to detect consoles. Some networks may block device discovery, in that case you will have to connect by IP.";
		}

		// The following strings are used for the settings page
		internal class SettingsTable
		{
			public string Heading = "These settings are automatically applied for the current session, you can however save them so they become persistent";
			public string SaveButton = "Save changes";
			public string ResetButton = "Reset defaults";
			public string RestartWarnLabel = "Some changes may require to restart SysDVR";

			public string Tab_General = "General";
			public string Tab_Performance = "Performance";
			public string Tab_Advanced = "Advanced";

			public string HideSerials = "Hide console serials from GUI";
			public string Hotkeys = "Enable hotkeys in the player view";
			public string RecordingsOutputPath = "Video recordings output path:";
			public string RecordingsOutputDialogTitle = "Select the video recording output path";
			public string ScreenshotOutputPath = "Screenshots output path:";
			public string ScreenshotOutputDialogTitle = "Select the screenshots output path";
			public string ChangePathButton = "Change";
			// This option is for windows only
			public string ScreenshotToClipboard = "Copy screenshots to the clipboard instead of saving as files (Press SHIFT to override during capture)";

			public string DefaultVolume = "Default audio volume";

			public string ScaleMode = "Scale mode";
			public string ScaleMode_Linear = "Linear (default)";
			public string ScaleMode_Narest = "Nearest (low overhead)";
			public string ScaleMode_Best = "Best (high quality, up to the system)";

			public string AudioMode = "Audio player mode";
			public string AudioMode_Auto = "Automatic (default)";
			public string AudioMode_Sync = "Synchronized";
			public string AudioMode_Compatible = "Compatible (try it if you have audio issues)";

			public string DefaultStreaming = "Default streaming mode";
			public string DefaultStreaming_Both = "Both channels";
			public string DefaultStreaming_Video = "Video only";
			public string DefaultStreaming_Audio = "Audio only";

			public string SaveFailedError = "Failed to save settings";

			public string PerformanceRenderingLabel = "These options affect the rendering pipeline of the client, when enabling 'uncapped' modes SysDVR-client will sync to the vsync event of your device, this should remove any latency due to the rendering pipeline but mey use more power on mobile devices.";
			public string UncapStreaming = "Uncap streaming framerate";
			public string UncapGUI = "Uncap GUI framerate";
			
			public string PerformanceStreamingLabel = "These options affect the straming quality of SysDVR, the defaults are usually fine";
			
			// Technical note: batching is the term used to group together multiple audio packets as a single one to reduce transfer overhead but increase latency
			public string AudioBatching = "Audio batching";

			// Technical note: NAL is the name of packets in the H264 video format
			public string CachePackets = "Cache video packets (NAL) locally and replay them when needed";
			public string CachePacketsKeyframes = "Apply packet cache only to keyframes (H264 IDR frames)";

			public string ForceSDLSoftwareRenderer = "Force SDL software rendering";
			public string PrintRealtimeLogs = "Print real-time streaming information";
			public string VerboseDebugging = "Enable verbose logging";
			public string DisableSynchronization = "Disable Audio/Video synchronization";
			
			public string DisableBacklight = "Turn off the console screen backlight during streaming";
			public string DisableBacklightWarn = "This feature is experimental and may cause issues. You can lock and unlock the console to restore the screen.";
			
			public string AnalyzeKeyframes = "Analyze keyframe NALs during the stream";
			public string AnalyzeNALs = "Analyze every NAL packet";
			
			public string GuiScale = "GUI scale";

			public string DecoderPopupTitle = "Select video decoder";
			public string DecoderChangeButton = "Configure hardware-accelerated decoder";
			// {0} is replaced with the current decoder name
			public string DecoderResetButton = "Disable hardware decoder ({0})";
			public string DecderPopupHeading = "Select a decoder to use";
			public string DecderPopupContent = "Note that not all decoders may be compatible with your system, revert this option in case of issues.\n" +
					"You can get more decoders by obtaining a custom build of ffmpeg libraries (libavcodec) and replacing the one included in SysDVR-Client.\n\n" +
					"This feature is intended for mini-PCs like Raspberry pi where software decoding might not be enough. On desktop PCs and smartphones this option should not be used.";

			public string InvalidPathError = "The selected path does not exist, try again.";
			public string PathSelectDialog = "Select path";
		}

		internal class PlayerTable
		{
			public string ConfirmQuitPopupTitle = "Confirm quit";
			public string ConfirmQuitLabel = "Are you sure you want to quit?";

			public string Shortcuts =
					"Keyboard shortcuts:\n" +
					" - S : capture screenshot\n" +
					" - R : start/stop recording\n" +
					" - F : toggle full screen\n" +
					" - Up/Down : change audio volume\n" +
					" - Esc : quit";

			public string AudioOnlyMode = "Streaming is set to audio-only mode.";
			
			public string TakeScreenshot = "Screenshot";
			public string StartRecording = "Start recording";
			public string StopRecording = "Stop recording";
			
			// {0} is replaced with the path of the destination file
			public string RecordingStartedMessage = "Recording to {0}";
			public string RecordingSuccessMessage = "Finished recording";
			
			public string StopStreaming = "Stop streaming";
			public string DebugInfo = "Debug info";
			public string EnterFullScreen = "Full screen";
			public string HideOverlayLabel = "Tap anywhere to hide the overlay";

			public string ScreenshotSavedToClip = "Screenshot saved to clipboard";
			// {0} is replaced with the path
			public string ScreenshotSaved = "Screenshot saved to {0}";
			
			public string AndroidPermissionError = "Make sure you enabled file access permissions to the app !";

			// {0} is replaced with the name of the decoder requested by the user
			// {1} is replaced with name of the decoder that has been chosen
			public string CustomDecoderError = "Decoder {0} not found, using {1} instead";
			
			// {0} is replaced with the name of the decoder requested by the user
			public string CustomDecoderEnabled = "Using custom decoder {0}, in case of issues try disabling this option to use the default one.";

			// {0} is replaced with the name of the codec
			public string PlayerInitializationMessage = "Initializing video player with {0} codec.";

			// {0} is replaced with the volume percentage
			public string VolumePercent = "Volume {0} %";
		}

		internal class UsbPageTable 
		{
			public string Title = "Connect over USB";
			public string NoDevicesLabel = "No USB devices found.";
			public string NoDevicesHelp = "Make sure you have SysDVR installed on your console and that it's running in USB mode.";
			
			public string NoDevicesHelpWindows = "The first time you run SysDVR on Windows you must install the USB driver, follow the guide.";
			public string NoDevicesHelpLinux = "To properly use USB mode on Linux you need to install the needed udev rule according to the guide";
			public string NoDevicesHelpAndroid = "In case of issues make sure your device supports USB OTG, note that certain USB C to C cables are known to cause issues, try with a OTG adapter on your phone USB port";

			public string RefreshButton = "Refresh device list";
		}

		internal class ErrorTable
		{
			public string SettingsPathMissing = "Couldn't find a valid path to store settings";
			
			// This message does not appear as is but is appended to the following messages 
			public string VersionTroubleshooting = "Make sure to use the same version on both your PC and console, you may need to reboot your console.";
			public string ConsoleRejectWrongVersion = "Your console rejected the connection because it is running a different SysDVR version.";
			public string InitialPacketError = "Error initiating communication, your console may be on an incompatible SysDVR version.";
			public string InitialPacketWrongVersion = "Your console reports to be using an incompatible SysDVR version.";
			
			public string DisconnectUnknownError = "The console was disconnected but no error was reported.";

			// {0} is replaced with the internal error code
			public string UsbResetMessage = "USB warning: Couldn't communicate with the console ({0}). Resetting the connection...";
			public string UsbTimeoutError = "USB warning: Couldn't communicate with the console. Try entering a compatible game, unplugging your console or restarting it.";
		}

		// The followign is windows only
		internal class UsbDriverTable
		{
			public string Title = "Driver install";
			public string Description = "SysDVR now uses standard Android ADB drivers\n" +
				"The driver is signed by Google, so it is safe to install and doesn't require any complex install steps.";

			public string InstallButton = "Install";
			public string CheckAgainButton = "Check status again";
			
			public string InstallInfo = "If you choose to install the driver it will be downloaded from";
			public string FileHashInfo = "The expected SHA256 hash of the zip file is";

			public string Installing = "Installation in progress...";

			public string StatusDownload = "Downloading the driver...";
			public string StatusInstall = "Installing the driver...";
			public string StatusClenaup = "Deleting temporary files...";
			public string StatusDone = "Done";
			
			public string DetectOk = "It seems the driver is already installed, unless you face issues, you DON'T need to install it again.";
			public string DetectNotInstalled = "It seems the driver is not installed, you need to install it to use SysDVR.";
			public string DetectNoDevice = "It seems that Windows has never detected the SysDVR device ID. Enable USB mode in SysDVR-Settings and connect your console. Note that USB-C to C cables may not work.";

			public string InstallSucceeded = "The installation was succesful";
			public string InstallFailed = "Installation failed";

			// The system error message is appended after this string
			public string DetectionFailed = "Error while detecting the driver state";
		}

		public LegacyPlayerTable LegacyPlayer = new();
		public ConnectionTable Connection = new();
		public HomePageTable HomePage = new();
		public GeneralTable General = new();
		public NetworkScanTable NetworkScan = new();
		public SettingsTable Settings = new();
		public PlayerTable Player = new();
		public UsbPageTable Usb = new();
		public ErrorTable Errors = new();
		public UsbDriverTable UsbDriver = new();

		// Abuse the json metadata to get all strings in the table without the need for real reflection
		public IEnumerable<string> GetAllStringForFontBuilding() 
		{
			var objects = StringTableSerializer.Default.SysDVRStringTable.Properties
				.Where(prop => prop.Get != null)
				.Select(prop => (prop.PropertyType, prop.Get(this)))
				.ToArray();

			return objects.SelectMany(x =>
				StringTableSerializer.Default.GetTypeInfo(x.Item1)
					.Properties
					.Where(prop => prop.Get != null)
					.Where(prop => prop.PropertyType == typeof(string))
					.Select(prop => (string?)prop.Get(x.Item2))
					.Where(str => str != null)
			);
		}

		// Used internally to produce the reference english translation file
		internal string Serialize() 
		{
			return JsonSerializer.Serialize(this, StringTableSerializer.Default.SysDVRStringTable);
		}
	}

	// When using AOT we must use source generation for json serialization since reflection is not available
	[JsonSourceGenerationOptions(
		WriteIndented = true,
		PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
		IncludeFields = true,
		UseStringEnumConverter = true,
		IgnoreReadOnlyProperties = true)]
	[JsonSerializable(typeof(StringTableMetadata), TypeInfoPropertyName = "SysDVRStringTableMetadata")]
	[JsonSerializable(typeof(StringTable), TypeInfoPropertyName = "SysDVRStringTable")]
	internal partial class StringTableSerializer : JsonSerializerContext
	{
	}
}
