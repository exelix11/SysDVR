using Newtonsoft.Json;

namespace SysDVRClientGUI.Models
{
    internal sealed class AdvancedOptions
    {
        [JsonProperty("logtransferinfo")]
        public bool LogTransferInfo { get; set; }

        [JsonProperty("ingoreaudiovideosync")]
        public bool IngoreAudioVideoSync { get; set; }

        [JsonProperty("logstatusmsgs")]
        public bool LogStatusMsgs { get; set; }

        [JsonProperty("printlibusbwarnings ")]
        public bool PrintLibUsbWarnings { get; set; }

        [JsonProperty("printlibusbdebug")]
        public bool PrintLibUsbDebug { get; set; }
    }
}
