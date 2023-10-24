using Newtonsoft.Json;

namespace SysDVRClientGUI.Models
{
    internal sealed class RtspStreamControlOptions
    {
        [JsonProperty("mpvPath")]
        public string MpvPath { get; set; }

        [JsonProperty("lowLatency")]
        public bool LowLatency { get; set; }

        [JsonProperty("unlimited")]
        public bool Untimed { get; set; }
    }
}
