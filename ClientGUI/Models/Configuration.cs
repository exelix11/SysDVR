using Newtonsoft.Json;
using SysDVRClientGUI.Logic;
using System.Net;

namespace SysDVRClientGUI.Models
{
    internal sealed class Configuration
    {
        [JsonProperty("userLanguage")]
        public string UserLanguage { get; set; }

        [JsonProperty("showAdvancedOptions")]
        public bool ShowAdvancedOptions { get; set; }

        [JsonProperty("channelsToStream")]
        public StreamKind ChannelsToStream { get; set; } = StreamKind.Both;

        [JsonProperty("streamSource")]
        public StreamSource StreamSource { get; set; } = StreamSource.Usb;

        [JsonProperty("ipaddress")]
        [JsonConverter(typeof(IpAddressJsonConverter))]
        public IPAddress IpAddress { get; set; }

        [JsonProperty("streamMode")]
        public StreamMode StreamMode { get; set; }

        [JsonProperty("advancedOptions")]
        public AdvancedOptions AdvancedOptions { get; set; } = new();

        [JsonProperty("playStreamControlOptions")]
        public PlayStreamControlOptions PlayStreamControlOptions { get; set; } = new();

        [JsonProperty("playMpvStreamControlOptions")]
        public PlayMpvStreamControlOptions PlayMpvStreamControlOptions { get; set; } = new();

        [JsonProperty("fileStreamControlOptions")]
        public FileStreamControlOptions FileStreamControlOptions { get; set; } = new();

        [JsonProperty("rtspStreamControlOptions")]
        public RtspStreamControlOptions RtspStreamControlOptions { get; set; } = new();
    }
}
