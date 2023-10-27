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

        const byte MetaIsVideo = 1 << 0;
        const byte MetaIsAudio = 1 << 1;
        
        const byte MetaIsData = 1 << 2;
        const byte MetaIsHash = 1 << 3;
        const byte MetaIsMultiNAL = 1 << 4;

        public uint Magic;
        public int DataSize;
        public ulong Timestamp;

        byte Flags;
        byte Reserved;

        public readonly bool IsVideo => (Flags & MetaIsVideo) != 0;
        public readonly bool IsAudio => (Flags & MetaIsAudio) != 0;

        public readonly bool IsHash => (Flags & MetaIsHash) != 0;
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
        const char ProtoLow = '1';

        public static readonly string CurrentProtocolString = $"{ProtoHigh}{ProtoLow}";
        public const ushort CurrentProtocolVersion = ProtoHigh | (ProtoLow << 8);

        public const uint RequestMagic = 0xAAAAAAAA;

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
            if (Marshal.SizeOf<ProtoHandshakeRequest>() != 16)
                throw new Exception("Invalid structure size, check the sysmodule source");
        }
    }

    abstract class StreamingSource
    {
        // Note that the source should respect the target output type,
        // this means that by the time it's added to a StreamManager
        // this field should match the NoAudio/NoVideo state of the target
        public StreamingOptions Options { get; private init; }
        
        public StreamingSource(StreamingOptions options)
        {       
            Options = options;
        }

        // This field delcares which particular stream will this instance produce
        // it may be different from Options.Kind when multiple streams are needed for both channels
        // Example: TCP uses two sources each from a different socket while USB uses a single source
        public StreamKind StreamProduced { get; protected init; }

        public event Action<string> OnMessage;

        protected void ReportMessage(string message)
        {
            OnMessage?.Invoke(message);
        }

        public abstract Task ConnectAsync(CancellationToken token);
        public abstract void StopStreaming();

        public abstract void Flush();

        public abstract bool ReadHeader(byte[] buffer);
        public abstract bool ReadPayload(byte[] buffer, int length);
        
        public abstract bool ReadRaw(byte[] buffer, int length);
        public abstract bool WriteData(byte[] buffer);

        protected void DoHandshake()
        {
            ProtoHandshakeRequest req = new();
            
            req.Magic = ProtoHandshakeRequest.RequestMagic;
            req.Version = ProtoHandshakeRequest.CurrentProtocolVersion;

            req.AudioBatching = (byte)Options.AudioBatching;
            req.UseNalhashes = Options.UseNALHashing;
            req.UseNalHashesOnlyForKeyframes = Options.UseNALHashingOnlyOnKeyframes;
            req.InjectPPSSPS = true;

            req.IsVideoPacket = StreamProduced is StreamKind.Both or StreamKind.Video;
            req.IsAudioPacket = StreamProduced is StreamKind.Both or StreamKind.Audio;

            var buf = new byte[Marshal.SizeOf<ProtoHandshakeRequest>()];
            MemoryMarshal.Write(buf, ref req);

            if (!WriteData(buf))
                throw new Exception("Handshake failed while sending the request");

            if (!ReadRaw(buf, 4))
                throw new Exception("Handshake failed while receiving the result");

            uint res = BitConverter.ToUInt32(buf);
            if (res != 6)
                throw new Exception("Handshake failed with error code " + res);
        }
    }
}
