using Newtonsoft.Json;

namespace SysDVRClientGUI.Models
{
    internal sealed class PlayStreamControlOptions
    {
        [JsonProperty("hardwareAcceleration")]
        public bool HardwareAcceleration { get; set; }

        [JsonProperty("bestScaling")]
        public bool BestScaling { get; set; }
    }
}
