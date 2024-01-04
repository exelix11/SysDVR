using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        public uint Magic;
        public int DataSize;
        public ulong Timestamp;

        public byte Flags;
        public byte ReplaySlot;

        public readonly bool IsVideo => (Flags & MetaIsVideo) != 0;
        public readonly bool IsAudio => (Flags & MetaIsAudio) != 0;

        public readonly bool IsReplay => (Flags & MetaIsHash) != 0;
        public readonly bool IsMultiNAL => (Flags & MetaIsMultiNAL) != 0;

        public override string ToString() =>
            $"Magic: {Magic:X8} Len: {DataSize + StructLength} Bytes - ts: {Timestamp}";

        static PacketHeader()
        {
            if (Marshal.SizeOf<PacketHeader>() != StructLength)
                throw new Exception("PacketHeader struct binary size is wrong");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct ProtoHandshakeRequest
    {
        const char ProtoHigh = '0';
        const char ProtoLow = '2';

        public static readonly string CurrentProtocolString = $"{ProtoHigh}{ProtoLow}";
        public const ushort CurrentProtocolVersion = ProtoHigh | (ProtoLow << 8);

        public const int HelloPacketSize = 10;
        public const int StructureSize = 16;
        public const uint RequestMagic = 0xAAAAAAAA;
        public const uint HandshakeOKCode = 6;

        public uint Magic;
        public ushort Version;
        
        private byte MetaFlags;
        private byte VideoFlags;
        public byte AudioBatching;

        fixed byte Reserved[7];

        static int Bit(int i) => 1 << i;

        static void SetBit(ref byte b, int i, bool v)
        {
            if (v)
                b |= (byte)Bit(i);
            else
                b &= (byte)~Bit(i);
        }

        public bool IsVideoPacket
        {
            get => (MetaFlags & Bit(0)) != 0;
            set => SetBit(ref MetaFlags, 0, value);
        }

        public bool IsAudioPacket
        {
            get => (MetaFlags & Bit(1)) != 0;
            set => SetBit(ref MetaFlags, 1, value);
        }

        public bool UseNalhashes
        {
            get => (VideoFlags & Bit(0)) != 0;
            set => SetBit(ref VideoFlags, 0, value);
        }

        public bool UseNalHashesOnlyForKeyframes
        {
            get => (VideoFlags & Bit(2)) != 0;
            set => SetBit(ref VideoFlags, 2, value);
        }

        public bool InjectPPSSPS
        {
            get => (VideoFlags & Bit(1)) != 0;
            set => SetBit(ref VideoFlags, 1, value);
        }

        static ProtoHandshakeRequest() 
        {
            if (Marshal.SizeOf<ProtoHandshakeRequest>() != StructureSize)
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

	abstract class StreamingSource : IDisposable
    {
        // Note that the source should respect the target output type,
        // this means that by the time it's added to a StreamManager
        // this field should match the NoAudio/NoVideo state of the target
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

        protected abstract Task<uint> SendHandshakePacket(ProtoHandshakeRequest req);
        protected abstract Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes);
        public abstract void Dispose();

		protected void ThrowOnHandshakeCode(string tag, uint code)
        {
            if (code == ProtoHandshakeRequest.HandshakeOKCode)
                return;

            if (code == 1) //Handshake_WrongVersion
                throw new Exception($"{tag} handshake failed: Wrong protocol version. Update SysDVR");

            // Other codes are internal checks so shouldn't happen often
            throw new Exception($"{tag} handshake failed: error code {code}");
        }

        async Task<ushort> GetCurrentVersionFromHelloPacket(StreamKind stream) 
        {
            var data = await ReadHandshakeHello(stream, ProtoHandshakeRequest.HelloPacketSize).ConfigureAwait(false);

            var str = Encoding.ASCII.GetString(data);

            // TODO: add future protocol compatibility adapters here

            if (str[.. "SysDVR|".Length] != "SysDVR|")
				throw new Exception("Invalid handshake hello packet (header)");

            if (str.Last() != '\0')
				throw new Exception("Invalid handshake hello packet (terminator)");

            var high = str["SysDVR|".Length];
            var low = str["SysDVR|".Length + 1];

            if (!char.IsAscii(high) || !char.IsAscii(low))
                throw new Exception("Invalid handshake hello packet (version)");

            return (ushort)(high | (low << 8));
        }

        protected async Task DoHandshake(StreamKind StreamProduced)
        {
            var version = await GetCurrentVersionFromHelloPacket(StreamProduced).ConfigureAwait(false);

            // TODO: Add backwards compatibility adapters here
            if (version != ProtoHandshakeRequest.CurrentProtocolVersion)
                throw new Exception($"{StreamProduced} handshake failed: Wrong protocol version. Update SysDVR.");

            ProtoHandshakeRequest req = new();

            req.Magic = ProtoHandshakeRequest.RequestMagic;
            req.Version = ProtoHandshakeRequest.CurrentProtocolVersion;

            req.AudioBatching = (byte)Options.AudioBatching;
            req.UseNalhashes = Options.UseNALReplay;
            req.UseNalHashesOnlyForKeyframes = Options.UseNALReplayOnlyOnKeyframes;
            req.InjectPPSSPS = true;

            req.IsVideoPacket = StreamProduced is StreamKind.Both or StreamKind.Video;
            req.IsAudioPacket = StreamProduced is StreamKind.Both or StreamKind.Audio;

            uint res = await SendHandshakePacket(req).ConfigureAwait(false);
            ThrowOnHandshakeCode("Console", res);
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
