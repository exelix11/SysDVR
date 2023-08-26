using System.Runtime.CompilerServices;

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
}
