using LibUsbDotNet;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SysDVR.Client.Core
{
    public static class StreamInfo
    {
        public static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
        public static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

        // This is VideoPayloadSize, which is also the max payload possible over the SysDVR protocol
        public const int MaxPayloadSize = 0x50000;

        public const int VideoWidth = 1280;
        public const int VideoHeight = 720;

        public const int AudioPayloadSize = 0x1000;
        public const int MaxAudioBatching = 3;

        public const int AudioChannels = 2;
        public const int AudioSampleRate = 48000;
        public const int AudioSampleSize = 2;

        // Doesn't accoutn for batching, there may be more samples than this
        public const int MinAudioSamplesPerPayload = AudioPayloadSize / (AudioChannels * AudioSampleSize);
    }

    public enum ConnectionType 
    {
        Net,
        Usb
    }

    public struct DvrProtocolVersion
    {
        public readonly uint AsInteger;
        public readonly string AsString;

        public DvrProtocolVersion(uint asInteger)
        {
            AsInteger = asInteger;
        }
    }

    public class DeviceInfo
    {
        private readonly string AdvertisementString;
        private readonly int ProtocolVersion;
        
        public readonly ConnectionType Source;
        public readonly string ConnectionString;
        public readonly bool IsManualConnection;
        public readonly string Version;
        public readonly string Serial;

        public readonly string TextRepresentation;

        static ReadOnlySpan<byte> TrimNullBytes(ReadOnlySpan<byte> source) 
        {
            var nullStart = source.IndexOf((byte)0);
            if (nullStart == -1)
                return source;

            return source.Slice(0, nullStart);
        }

        public bool CheckProtocolVersion(int current)
        {
            if (IsManualConnection)
                return true;

            return ProtocolVersion == current;
        }

        private DeviceInfo(string ip)
        {
            this.Source = ConnectionType.Net;
            this.ConnectionString = ip;
            this.IsManualConnection = true;

            this.AdvertisementString = "";
            this.Version = "0.0";
            this.ProtocolVersion = 0;
            this.Serial = "00000000";

            this.TextRepresentation = $"Unknown SysDVR @ {ConnectionString}";
        }

        public DeviceInfo(ConnectionType source, string advertisementString, string connectionString)
        {
            this.Source = source;
            this.AdvertisementString = advertisementString;
            this.ConnectionString = connectionString;
            this.IsManualConnection = false;

            // Example beacon format:
            //      SysDVR|6.0|00|NX00000000

            var parts = advertisementString.Split('|');

            if (parts[0] != "SysDVR")
                throw new Exception("Invalid format");

            Version = parts[1];
            
            var protover = parts[2];
            if (protover.Length != "00".Length)
                throw new Exception("Invalid protocol version format");

            ProtocolVersion = int.Parse(protover, System.Globalization.NumberStyles.HexNumber);

            Serial = parts[3];

            var printSerial = Program.Options.HideSerials ? "(serial hidden)" : Serial;

            TextRepresentation = Source == ConnectionType.Net ?
                $"SysDVR {Version} - {printSerial} @ {ConnectionString}" :
                $"SysDVR {Version} - {printSerial} @ USB";
        }

        public override string ToString() => 
            TextRepresentation;

        public static DeviceInfo? TryParse(ConnectionType source, string advertisementString, string connectionString)
        {
            try {
                return new DeviceInfo(source, advertisementString, connectionString);
            }
            catch {
                return null;
            }
        }

        public static DeviceInfo? TryParse(ConnectionType source, byte[] advertisementPacket, string connectionString)
        {
            try
            {
                var str = Encoding.UTF8.GetString(TrimNullBytes(advertisementPacket));
                return new DeviceInfo(source, str, connectionString);
            }
            catch
            {
                return null;
            }
        }

        public static DeviceInfo ForIp(string ipAddress)
        {
            return new DeviceInfo(ipAddress);    
        }
    }
}
