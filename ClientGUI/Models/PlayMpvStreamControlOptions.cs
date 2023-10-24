using Newtonsoft.Json;

namespace SysDVRClientGUI.Models
{
    internal sealed class PlayMpvStreamControlOptions
    {
        [JsonProperty("mpvPath")]
        public string MpvPath { get; set; }
    }
}
