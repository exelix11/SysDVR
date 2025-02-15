using SysDVR.Client.Core;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketHeader
    {
        // Note: to make the TCP implementation easier these should be composed of 4 identical bytes
        public const uint MagicResponse = 0xCCCCCCCC;
        public const int StructLength = 18;
        public const int MaxTransferSize = StreamInfo.MaxPayloadSize + StructLength;

        public const byte MetaIsVideo = 1 << 0;
        public const byte MetaIsAudio = 1 << 1;

        public const byte MetaIsData = 1 << 2;
        public const byte MetaIsHash = 1 << 3;
        public const byte MetaIsMultiNAL = 1 << 4;
        public const byte MetaIsError = 1 << 5;

        public uint Magic;
        public int DataSize;
        public ulong Timestamp;

        public byte Flags;
        public byte ReplaySlot;

        public readonly bool IsVideo => (Flags & MetaIsVideo) != 0;
        public readonly bool IsAudio => (Flags & MetaIsAudio) != 0;

        public readonly bool IsReplay => (Flags & MetaIsHash) != 0;
        public readonly bool IsMultiNAL => (Flags & MetaIsMultiNAL) != 0;
        public readonly bool IsError => (Flags & MetaIsError) != 0;

        public override string ToString() =>
            $"Magic: {Magic:X8} Len: {DataSize + StructLength} Bytes - ts: {Timestamp}";

        static PacketHeader()
        {
            if (Marshal.SizeOf<PacketHeader>() != StructLength)
                throw new Exception("PacketHeader struct binary size is wrong");
        }
    }

    public static class ProtocolUtil
    {
        public static readonly ushort ProtocolVersion2 = StringToVersionCode("02");
        public static readonly ushort ProtocolVersion3 = StringToVersionCode("03");

        public static ushort StringToVersionCode(string code)
        {
            if (code.Length != 2)
                throw new ArgumentException("Invalid version code");

            return (ushort)(code[0] | (code[1] << 8));
        }

        public static string VersionCodeToString(ushort code) =>
            $"{(char)(code & 0xFF)}{(char)(code >> 8)}";
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct ProtoHandshakeRequest
    {
        public static bool IsProtocolSupported(string protoString) =>
            protoString is "03" or "02";

        public static bool IsProtocolSupported(ushort protoValue) =>
            IsProtocolSupported(ProtocolUtil.VersionCodeToString(protoValue));

        public const int HelloPacketSize = 10;
        public const int StructureSize = 16;
        public const uint RequestMagic = 0xAAAAAAAA;
        public const uint HandshakeOKCode = 6;

        public uint Magic;
        public ushort Version;

        private byte MetaFlags;
        private byte VideoFlags;
        public byte AudioBatching;

        byte FeatureFlags;
        fixed byte Reserved[6];

        static int Bit(int i) => 1 << i;

        static void SetBit(ref byte b, int i, bool v)
        {
            if (v)
                b |= (byte)Bit(i);
            else
                b &= (byte)~Bit(i);
        }

        static bool GetBit(byte b, int i)
        {
            return (b & Bit(i)) != 0;
        }

        void AssertVersion(ushort version)
        {
            if (version != Version)
                throw new Exception($"This feature is not available in protocol version {ProtocolUtil.VersionCodeToString(Version)}");
        }

        public bool IsVideoPacket
        {
            get => GetBit(MetaFlags, 0);
            set => SetBit(ref MetaFlags, 0, value);
        }

        public bool IsAudioPacket
        {
            get => GetBit(MetaFlags, 1);
            set => SetBit(ref MetaFlags, 1, value);
        }

        public bool UseNalHashes
        {
            get => GetBit(VideoFlags, 0);
            set => SetBit(ref VideoFlags, 0, value);
        }

        public bool UseNalHashesOnlyForKeyframes
        {
            get => GetBit(VideoFlags, 2);
            set => SetBit(ref VideoFlags, 2, value);
        }

        public bool InjectPPSSPS
        {
            get => GetBit(VideoFlags, 1);
            set => SetBit(ref VideoFlags, 1, value);
        }

        // Requires protocol version 3
        public bool TurnOffConsoleScreen
        {
            get => GetBit(FeatureFlags, 0);
            set { if (value) AssertVersion(ProtocolUtil.ProtocolVersion3); SetBit(ref FeatureFlags, 0, value); }
        }

        // Requires protocol version 3
        public bool PerformMemoryDiagnostic
        {
            get => GetBit(FeatureFlags, 1);
            set { if (value) AssertVersion(ProtocolUtil.ProtocolVersion3); SetBit(ref FeatureFlags, 1, value); }
        }

        static ProtoHandshakeRequest()
        {
            if (Marshal.SizeOf<ProtoHandshakeRequest>() != StructureSize)
                throw new Exception("Invalid structure size, check the sysmodule source");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ProtoHandshakeResponse02
    {
        public const int Size = 4;

        public uint Result;

        static ProtoHandshakeResponse02()
        {
            if (Marshal.SizeOf<ProtoHandshakeResponse02>() != Size)
                throw new Exception("Invalid structure size, check the sysmodule source");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ProtoHandshakeResponse03
    {
        public const int Size = 72;

        public uint Result;

        // Optional memory query result
        public uint MemoryPools_QueryResult;
        public ulong MemoryPools_ApplicationSize, MemoryPools_ApplicationUsed;
        public ulong MemoryPools_AppletSize, MemoryPools_AppletUsed;
        public ulong MemoryPools_SystemSize, MemoryPools_SystemUsed;
        public ulong MemoryPools_SystemUnsafeSize, MemoryPools_SystemUnsafeUsed;

        static ProtoHandshakeResponse03()
        {
            if (Marshal.SizeOf<ProtoHandshakeResponse03>() != (int)Size)
                throw new Exception("Invalid structure size, check the sysmodule source");
        }
    }

    struct ReceivedPacket
    {
        public PacketHeader Header;
        public PoolBuffer? Buffer;

        public ReceivedPacket(PacketHeader header, PoolBuffer? buffer)
        {
            Header = header;
            Buffer = buffer;
        }
    }

    static class PacketErrorParser
    {
        public static string GetPacketErrorAsString(in ReceivedPacket packet)
        {
            if (!packet.Header.IsError)
                return "Packet is not an error";

            if (packet.Buffer is null)
                return "Packet buffer is null";

            uint ErrorType = BitConverter.ToUInt32(packet.Buffer.Span);
            uint ErrorCode = BitConverter.ToUInt32(packet.Buffer.Span[4..]);
            ulong Context1 = BitConverter.ToUInt64(packet.Buffer.Span[8..]);
            ulong Context2 = BitConverter.ToUInt64(packet.Buffer.Span[16..]);
            ulong Context3 = BitConverter.ToUInt64(packet.Buffer.Span[24..]);

            if (ErrorType == 1)
            {
                return $"Video capture failed with code 0x{ErrorCode:x} requested size was 0x{Context1:x}";
            }
            else if (ErrorType is 2 or 3)
            {
                return $"Audio capture failed with code 0x{ErrorCode:x} requested size was 0x{Context1:x}, iteration was {Context2}";
            }

            return $"Unknown error type 0x{ErrorType:x} 0x{ErrorCode:x} 0x{Context1:x} 0x{Context2:x} 0x{Context3:x}";
        }
    }

    abstract class StreamingSource : IDisposable
    {
        // This is a single object shared between the streaming target and the source(s)
        // The sources should respect the NoVideo/Audio state of the options object and not change it
        // In the case of TCP there are two different StreamingSource objects, they are configured by the manager and not by this field
        public StreamingOptions Options { get; private init; }

        protected CancellationToken Cancellation { get; private init; }

        public StreamingSource(StreamingOptions options, CancellationToken cancellation)
        {
            Options = options;
            Cancellation = cancellation;
        }

        public event Action<string> OnMessage;

        protected void ReportMessage(string message)
        {
            OnMessage?.Invoke(message);
        }

        public abstract Task Connect();
        public abstract Task StopStreaming();

        // Flush may cause a reconnection
        public abstract Task Flush();

        public abstract Task<ReceivedPacket> ReadNextPacket();

        protected abstract Task<byte[]> SendHandshakePacket(ProtoHandshakeRequest req, int expectedLength);
        protected abstract Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes);
        public abstract void Dispose();

        protected void ThrowOnHandshakeCode(string tag, uint code)
        {
            if (code == ProtoHandshakeRequest.HandshakeOKCode)
                return;

            if (code == 1) //Handshake_WrongVersion
                throw new Exception($"{tag} {Program.Strings.Errors.ConsoleRejectWrongVersion} {Program.Strings.Errors.VersionTroubleshooting}");

            // Other codes are internal checks so shouldn't happen often
            throw new Exception($"{tag} handshake failed: error code {code}");
        }

        async Task<ushort> GetCurrentVersionFromHelloPacket(StreamKind stream)
        {
            var data = await ReadHandshakeHello(stream, ProtoHandshakeRequest.HelloPacketSize).ConfigureAwait(false);

            var str = Encoding.ASCII.GetString(data);

            // TODO: add future protocol compatibility adapters here

            if (str[.."SysDVR|".Length] != "SysDVR|")
                throw new Exception($"{Program.Strings.Errors.InitialPacketError} {Program.Strings.Errors.VersionTroubleshooting}");

            if (str.Last() != '\0')
                throw new Exception("Invalid handshake hello packet (terminator)");

            var high = str["SysDVR|".Length];
            var low = str["SysDVR|".Length + 1];

            if (!char.IsAscii(high) || !char.IsAscii(low))
                throw new Exception("Invalid handshake hello packet (version)");

            return ProtocolUtil.StringToVersionCode($"{high}{low}");
        }

        // On success, this fills the options structure
        protected async Task DoHandshake(StreamKind StreamProduced)
        {
            var version = await GetCurrentVersionFromHelloPacket(StreamProduced).ConfigureAwait(false);

            // TODO: Add backwards compatibility adapters here
            if (!ProtoHandshakeRequest.IsProtocolSupported(version))
                throw new Exception($"{StreamProduced} {Program.Strings.Errors.InitialPacketWrongVersion} {Program.Strings.Errors.VersionTroubleshooting}");

            ProtoHandshakeRequest req = new();

            req.Magic = ProtoHandshakeRequest.RequestMagic;
            req.Version = version;

            req.AudioBatching = (byte)Options.AudioBatching;
            req.UseNalHashes = Options.UseNALReplay;
            req.UseNalHashesOnlyForKeyframes = Options.UseNALReplayOnlyOnKeyframes;
            req.InjectPPSSPS = true;

            req.IsVideoPacket = StreamProduced is StreamKind.Both or StreamKind.Video;
            req.IsAudioPacket = StreamProduced is StreamKind.Both or StreamKind.Audio;

            var responseSize = ProtoHandshakeResponse02.Size;
            if (req.Version >= ProtocolUtil.ProtocolVersion3)
            {
                responseSize = ProtoHandshakeResponse03.Size;

                req.PerformMemoryDiagnostic = Program.Options.Debug.Log;
                req.TurnOffConsoleScreen = Options.TurnOffConsoleScreen;
            }

            var res = await SendHandshakePacket(req, responseSize).ConfigureAwait(false);

            if (req.Version >= ProtocolUtil.ProtocolVersion3)
                ParseResponse03(res);
            else
                ParseResponse02(res);
        }

        void ParseResponse02(Span<byte> data)
        {
            if (data.Length != ProtoHandshakeResponse02.Size)
                throw new Exception($"Invalid handshake response size: {data.Length} (expected {ProtoHandshakeResponse02.Size})");

            var parsed = MemoryMarshal.Read<ProtoHandshakeResponse02>(data);
            
            ThrowOnHandshakeCode("console_02", parsed.Result);
        }

        void ParseResponse03(Span<byte> data)
        {
            if (data.Length != ProtoHandshakeResponse03.Size)
                throw new Exception($"Invalid handshake response size: {data.Length} (expected {ProtoHandshakeResponse03.Size})");

            var parsed = MemoryMarshal.Read<ProtoHandshakeResponse03>(data);
            ThrowOnHandshakeCode("console_03", parsed.Result);

            if (Program.Options.Debug.Log)
            {
                StringBuilder sb = new();
                sb.AppendLine($"Console memory report: result {parsed.MemoryPools_QueryResult:x}");

                if (parsed.MemoryPools_QueryResult == uint.MaxValue)
                    sb.AppendLine("Error code indicates that report was not requested");
                else if (parsed.MemoryPools_QueryResult != 0)
                    sb.AppendLine("Error code indicates a libnx error");

                void add(string name, ulong total, ulong used) =>
                    sb.AppendLine($"\t{name} pool: total={total} used={used} ({(total == 0 ? 0 : (used / total) * 100)}% used)");

                add("Application", parsed.MemoryPools_ApplicationSize, parsed.MemoryPools_ApplicationUsed);
                add("Applet", parsed.MemoryPools_AppletSize, parsed.MemoryPools_AppletUsed);
                add("System", parsed.MemoryPools_SystemSize, parsed.MemoryPools_SystemUsed);
                add("SystemUnsafe", parsed.MemoryPools_SystemUnsafeSize, parsed.MemoryPools_SystemUnsafeUsed);

                Program.DebugLog(sb.ToString());
            }
        }

        static protected bool ValidatePacketHeader(in PacketHeader header)
        {
            if (header.Magic != PacketHeader.MagicResponse)
                return false;

            if (header.DataSize > PacketHeader.MaxTransferSize)
                return false;

            return true;
        }
    }
}
