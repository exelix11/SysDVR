using Newtonsoft.Json;

namespace SysDVRClientGUI.Models
{
    internal sealed class FileStreamControlOptions
    {
        [JsonProperty("lastUsedPath")]
        public string LastUsedPath { get; set; }
    }
}
