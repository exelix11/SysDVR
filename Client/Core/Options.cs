using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    public enum UsbLogLevel
    {
        Error,
        Warning,
        Debug,
        None
    }

    public enum ScaleMode 
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

        // Usb logging options
        public UsbLogLevel UsbLogging = UsbLogLevel.Error;

        // Ffmpeg options
        public bool HardwareAccel;
        public string? DecoderName;

        // SDL options
        public bool ForceSoftwareRenderer;
        public ScaleMode RendererScale = ScaleMode.Linear;
        public SDLAudioMode AudioPlayerMode = SDLAudioMode.Auto;

        // Sysmodule options
        public StreamingOptions Streaming = new();

        // Debug settings
        public DebugOptions Debug = new();

		public string ScaleHintForSDL => RendererScale switch
        {
            ScaleMode.Linear => "linear",
            ScaleMode.Nearest => "nearest",
            ScaleMode.Best => "best",
            _ => throw new NotImplementedException(),
        };

        public string GetFilePathForVideo()
        {
            var format = $"SysDVR_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.mp4";
            return Path.Combine("F:\\", format);
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
            if (OperatingSystem.IsWindows())
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
            if (OperatingSystem.IsWindows())
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                return LinuxFallbackPath("Pictures");
#endif
        }
    }
}
