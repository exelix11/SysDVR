using Newtonsoft.Json;
using System;
using System.Net;

namespace SysDVRClientGUI.Logic
{
    internal class IpAddressJsonConverter : JsonConverter<IPAddress>
    {
        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == default)
            {
                return default;
            }
            return IPAddress.Parse((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
