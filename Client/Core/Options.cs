using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SysDVR.Client.Core
{
    public enum UsbLogLevel
    {
        Error,
        Warning,
        Debug,
        None
    }

    public enum SDLScaleMode 
    {
        Linear,
        Nearest,
        Best
    }

    public enum SDLAudioMode 
    {
        Compatible,
        Default,
        Auto
    }

    public class Options
    {
        public bool UncapStreaming;
        public bool UncapGUI;
        public string RecordingsPath = DefaultPlatformVideoPath();
        public string ScreenshotsPath = DefaultPlatformPicturePath();
        public bool HideSerials;

        public bool PlayerHotkeys = true;

        // (Windows only) Capture screenshots to clipboard by default
        public bool Windows_ScreenToClip = false;

		// Usb logging options
		[JsonIgnore]
		public UsbLogLevel UsbLogging = UsbLogLevel.Error;

        // Ffmpeg options
        public bool HardwareAccel;
        
        // Mark as json ignore the ones that can only be set via command line
        [JsonIgnore]
        public string? DecoderName;

        // SDL options
        public bool ForceSoftwareRenderer;
        public SDLScaleMode RendererScale = SDLScaleMode.Linear;
        public SDLAudioMode AudioPlayerMode = SDLAudioMode.Auto;

        // Sysmodule options
        public StreamingOptions Streaming = new();

		// Debug settings
		public DebugOptions Debug = new();

		public string ScaleHintForSDL => RendererScale switch
        {
            SDLScaleMode.Linear => "linear",
            SDLScaleMode.Nearest => "nearest",
            SDLScaleMode.Best => "best",
            _ => throw new NotImplementedException(),
        };

        public string GetFilePathForVideo()
        {
            var format = $"SysDVR_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.mp4";
            return Path.Combine(RecordingsPath, format);
        }

        public string GetFilePathForScreenshot()
        {
            var format = $"SysDVR_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";
            return Path.Combine(ScreenshotsPath, format);
        }

        static string LinuxFallbackPath(string wantedFolderName) 
        {
            var wanted = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), wantedFolderName);
            
            if (!Directory.Exists(wanted))
            {
                wanted = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
                
                if (!Directory.Exists(wanted))
                {
                    wanted = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");

                    // WSL doesn't have the other folders by default
                    if (!Directory.Exists(wanted))
                        wanted = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                }
            }

            return wanted;
        }

        static string DefaultPlatformVideoPath()
        {
#if ANDROID_LIB
            return "/sdcard/Movies";  
#else
            if (Program.IsWindows)
                return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else
                // like you realise ~/Videos is a thing on linux right
                //                                                  -Blecc
                return LinuxFallbackPath("Videos");
#endif
        }

        static string DefaultPlatformPicturePath()
        {
#if ANDROID_LIB
            return "/sdcard/Pictures";  
#else
            if (Program.IsWindows)
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                return LinuxFallbackPath("Pictures");
#endif
        }

        public string SerializeToJson() 
        {
            return JsonSerializer.Serialize(this, OptionsJsonSerializer.Default.SysDVROptions);
		}

        public static Options FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Options>(json, OptionsJsonSerializer.Default.SysDVROptions)!;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to deserialize options from json: " + ex);
                return new Options();
            }
        }
    }

    // When using AOT we must use source generation for json serialization since reflection is not available
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IncludeFields = true, 
        IgnoreReadOnlyProperties = true)]
    [JsonSerializable(typeof(Options), TypeInfoPropertyName = "SysDVROptions")]
    internal partial class OptionsJsonSerializer : JsonSerializerContext 
    {
    }
}
