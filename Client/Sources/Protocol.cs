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
    [StructLayout(LayoutKind.Sequential)]
    struct PacketHeader
    {
        // Note: to make the TCP implementation easier these should be composed of 4 identical bytes
        public const uint MagicResponse = 0xCCCCCCCC;
        public const int StructLength = 18;
        public const int MaxTransferSize = StreamInfo.MaxPayloadSize + StructLength;

        public uint Magic;
        public int DataSize;
        public ulong Timestamp;

        byte Flags;
        byte flags1;

        public readonly bool IsVideo => (Flags & 1) == 0;
        public readonly bool IsAudio => (Flags & 1) == 1;

        public readonly bool IsHash => ((Flags >> 1) & 3) == 1;

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

        byte Meta;
        public byte AudioBatching;
        byte Video;

        fixed byte Reserved[7];

        static void SetBit(ref byte b, int i, bool v)
        {
            if (v)
                b |= (byte)(1 << i);
            else
                b &= (byte)~(1 << i);
        }

        public bool IsVideoPacket
        {
            get => (Meta & 2) == 1;
            set => SetBit(ref Meta, 2, value);
        }

        public bool IsAudioPacket
        {
            get => (Meta & 1) == 1;
            set => SetBit(ref Meta, 1, value);
        }

        public bool UseNalhashes 
        {
            get => (Video & 1) == 1;
            set => SetBit(ref Video, 1, value);
        }

        public bool InjectPPSSPS
        {
            get => (Video & 2) == 1;
            set => SetBit(ref Video, 2, value);
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
        public StreamKind SourceKind { get; protected init; }

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

            req.AudioBatching = 2;
            req.UseNalhashes = false;
            req.InjectPPSSPS = true;

            req.IsVideoPacket = SourceKind is StreamKind.Both or StreamKind.Video;
            req.IsAudioPacket = SourceKind is StreamKind.Both or StreamKind.Audio;

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
