//#define EXCEPTION_DEBUG

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    [StructLayout(LayoutKind.Sequential)]
    struct PacketHeader
    {
        public uint Magic;
        public int DataSize;
        public ulong Timestamp;

        public override string ToString() =>
            $"Magic: {Magic:X8} Len: {DataSize + StructLength} Bytes - ts: {Timestamp}";

        public const int StructLength = 16;

        // Note: to make the TCP implementation easier these should be composed of 4 identical bytes
        public const uint MagicResponseVideo = 0xDDDDDDDD;
        public const uint MagicResponseAudio = 0xEEEEEEEE;

        public const int MaxTransferSize = StreamInfo.MaxPayloadSize + StructLength;

        static PacketHeader()
        {
            if (Marshal.SizeOf<PacketHeader>() != StructLength)
                throw new Exception("PacketHeader struct binary size is wrong");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsVideo() => Magic == MagicResponseVideo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAudio() => Magic == MagicResponseAudio;
    }

    interface IStreamingSource
    {
        // Note that the source should respect the target output type,
        // this means that by the time it's added to a StreamManager
        // this field should match the NoAudio/NoVideo state of the target
        StreamKind SourceKind { get; }

        event Action<string> OnError;
        event Action<Exception> OnFatalError;

        Task ConnectAsync(CancellationToken token);
        void StopStreaming();

        void Flush();

        bool ReadHeader(byte[] buffer);
        bool ReadPayload(byte[] buffer, int length);
    }

    abstract class StreamThread : IDisposable
    {
        Thread DeviceThread;
        CancellationTokenSource Cancel;

        readonly BaseStreamManager Manager;
        readonly public IStreamingSource Source;
        readonly public StreamKind Kind;

        protected abstract void SetCancellationToken(CancellationToken token);
        protected abstract bool DataReceived(in PacketHeader header, PoolBuffer body);

#if MEASURE_STATS
		public ulong ReceivedBytes { get; private set; }
		public DateTime FirstByteTs { get; private set; }

		public void StatsReceivedData(int length)
        {
			if (ReceivedBytes == 0)
				FirstByteTs = DateTime.Now;
			ReceivedBytes += (uint)length + PacketHeader.StructLength;
        }
#endif

        protected StreamThread(IStreamingSource source, BaseStreamManager manager)
        {
            Source = source;
            Kind = Source.SourceKind;
            Manager = manager;

            source.OnError += Manager.ReportError;
            source.OnFatalError += Manager.ReportFatalError;
        }

        public void Start()
        {
            Cancel = new CancellationTokenSource();

            DeviceThread = new Thread(() => DeviceThreadMain(Cancel.Token));
            DeviceThread.Name = "DeviceThread for " + Kind;

            DeviceThread.Start();
        }

        public void Stop()
        {
            Cancel.Cancel();
            Source.StopStreaming();
        }

        public void Join()
        {
            DeviceThread.Join();
        }

        TimeTrace trace = new TimeTrace();
        void DeviceThreadMain(CancellationToken token)
        {
            var logStats = DebugOptions.Current.Stats;
            var logDbg = DebugOptions.Current.Log;

            SetCancellationToken(token);

            var HeaderData = new byte[PacketHeader.StructLength];
            ref var Header = ref MemoryMarshal.Cast<byte, PacketHeader>(HeaderData)[0];
            try
            {
            loop_again:
                while (!token.IsCancellationRequested)
                {
                    while (!Source.ReadHeader(HeaderData))
                    {
                        Source.Flush();
                        goto loop_again;
                    }

                    if (logStats)
                        Manager.ReportError($"[{Kind}] {Header}");

                    if (Header.Magic is not PacketHeader.MagicResponseAudio and not PacketHeader.MagicResponseVideo)
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Wrong header magic: {Header.Magic:X}");

                        Source.Flush();
                        continue;
                    }

                    if (Header.DataSize > StreamInfo.MaxPayloadSize)
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");

                        Source.Flush();
                        continue;
                    }

                    var Data = PoolBuffer.Rent(Header.DataSize);
                    if (!Source.ReadPayload(Data.RawBuffer, Header.DataSize))
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Read payload failed.");

                        Source.Flush();
                        Data.Free();
                        continue;
                    }

                    if (!DataReceived(Header, Data))
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] DataReceived rejected the packet, header magic was {Header.Magic:X}");

                        Data.Free();
                        continue;
                    }
                }

            }
#if !EXCEPTION_DEBUG || RELEASE
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                    Manager.ReportFatalError(ex);
            }
#endif
        }

        public void Dispose()
        {
            if (DeviceThread.IsAlive)
                throw new Exception($"{Kind} Thread is still running");

            Cancel.Dispose();
        }
    }

    class SingleStreamThread : StreamThread
    {
        readonly OutStream Target;

        public SingleStreamThread(IStreamingSource source, OutStream target, BaseStreamManager manager) : base(source, manager)
        {
            Target = target;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
            Target.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
            var valid =
                Kind == StreamKind.Video && header.IsVideo() ||
                Kind == StreamKind.Audio && header.IsAudio();

            if (!valid)
                return false;

            Target.SendData(body, header.Timestamp);

            return true;
        }
    }

    class MultiStreamThread : StreamThread
    {
        readonly OutStream VideoTarget;
        readonly OutStream AudioTarget;

        public MultiStreamThread(IStreamingSource source, OutStream videoTarget, OutStream audioTarget, BaseStreamManager manager) : base(source, manager)
        {
            if (source.SourceKind != StreamKind.Both)
                throw new Exception("Source must be able to provide both streams");

            VideoTarget = videoTarget;
            AudioTarget = audioTarget;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
            VideoTarget.UseCancellationToken(token);
            AudioTarget.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
            if (header.IsVideo())
                VideoTarget.SendData(body, header.Timestamp);
            else if (header.IsAudio())
                AudioTarget.SendData(body, header.Timestamp);
            else
                return false;

            return true;
        }
    }
}
