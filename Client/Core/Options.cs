using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    public enum UsbLogMode 
    {
        Default,
        Warn,
        Debug,
    }

    public enum ScaleMode 
    {
        Linear,
        Nearest,
        Best
    }

    public class Options
    {
        public bool UncapStreaming;
        public bool UncapGUI;

        // Usb logging options
        public UsbLogMode UsbLogging = UsbLogMode.Default;

        // Ffmpeg options
        public bool HardwareAccel;
        public string? DecoderName;

        // SDL options
        public ScaleMode RendererScale = ScaleMode.Linear;

        public string ScaleHintForSDL => RendererScale switch
        {
            ScaleMode.Linear => "linear",
            ScaleMode.Nearest => "nearest",
            ScaleMode.Best => "best",
            _ => throw new NotImplementedException(),
        };
    }
}
