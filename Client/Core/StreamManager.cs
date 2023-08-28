using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SysDVR.Client.Core
{
    enum StreamKind
    {
        Both,
        Video,
        Audio
    };

    struct PoolBuffer
    {
        private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        public int Length { get; private set; }
        private byte[] _buffer;
        private int refcount;

        public byte[] RawBuffer => _buffer ?? throw new Exception("The buffer has been freed");

        public void Reference() 
        {
            refcount++;
        }

        public void Free()
        {
            --refcount;

#if DEBUG
            if (refcount < 0)
                throw new Exception("Buffer refcount is negative");
#endif

            if (refcount == 0)
            {
                pool.Return(RawBuffer);
                _buffer = null;
            }
        }

        public static PoolBuffer Rent(int len) =>
            new PoolBuffer(pool.Rent(len), len);

        private PoolBuffer(byte[] buf, int len)
        {
            Length = len;
            _buffer = buf;
            refcount = 1;
        }

        public Span<byte> Span =>
            new Span<byte>(RawBuffer, 0, Length);

        public ArraySegment<byte> ArraySegment =>
            new ArraySegment<byte>(RawBuffer, 0, Length);

        public static implicit operator Span<byte>(PoolBuffer o) => o.Span;
    }

    abstract class OutStream : IDisposable
    {
        protected OutStream? Next;
        protected bool OwnsNext;
        protected CancellationToken Cancel;

        public void ChainStream(OutStream next, bool ownsNext)
        {
            if (Next is not null && OwnsNext)
                Next.Dispose();

            OwnsNext = ownsNext;
            Next = next;
        }

        protected virtual void UseCancellationTokenImpl(CancellationToken tok)
        {
            Cancel = tok;
        }

        protected abstract void SendDataImpl(PoolBuffer block, ulong ts);

        // This must be called before sending any data
        public void UseCancellationToken(CancellationToken tok)
        {
            UseCancellationTokenImpl(tok);
            Next?.UseCancellationToken(tok);
        }

        public void SendData(PoolBuffer block, ulong ts)
        {
            // Reference counting:
            //      block starts with refcount = 1
            //      SendData() increases it before calling the impl
            //      Each impl calls Free() when it doesn't need it anymore (even asynchronously)
            //      The final block in the chain calls Free() to remove the initial refcount, any pending refs are cleared by async processes
            //      When ref == 0 the block is freed
            block.Reference();
            SendDataImpl(block, ts);

            if (Next is not null)
                Next.SendData(block, ts);
            else
                block.Free();
        }

        public virtual void Dispose() 
        {
            if (OwnsNext)
                Next?.Dispose();
        }
    }

    abstract class BaseStreamManager : IDisposable
    {
        private bool disposedValue;

        // Usb streaming may require a single thread
        private StreamThread Thread1;
        private StreamThread? Thread2;

        protected OutStream VideoTarget { get; set; }
        protected OutStream AudioTarget { get; set; }

        public StreamKind? Streams { get; private set; }

        public bool HasVideo => Streams is StreamKind.Both or StreamKind.Video;
        public bool HasAudio => Streams is StreamKind.Both or StreamKind.Audio;

        public BaseStreamManager(OutStream videoTarget, OutStream audioTarget)
        {
            VideoTarget = videoTarget;
            AudioTarget = audioTarget;
        }

        public void AddSource(IStreamingSource source)
        {
            if (source.SourceKind == StreamKind.Video)
            {
                if (HasVideo) throw new Exception("Already has a video source");
                Thread1 = new SingleStreamThread(source, VideoTarget);

                Streams = HasAudio ? StreamKind.Both : StreamKind.Video;
            }
            else if (source.SourceKind == StreamKind.Audio)
            {
                if (HasAudio) throw new Exception("Already has an audio source");
                Thread2 = new SingleStreamThread(source, AudioTarget);

                Streams = HasVideo ? StreamKind.Both : StreamKind.Audio;
            }
            else if (source.SourceKind == StreamKind.Both)
            {
                if (HasAudio || HasVideo) throw new Exception("Already has a multi source");
                Thread1 = new MultiStreamThread(source, VideoTarget, AudioTarget);

                Streams = StreamKind.Both;
            }
        }

        public virtual void Begin()
        {
            // Sanity checks
            if (Streams is null)
                throw new Exception("No streams have been set");

            if (Streams == StreamKind.Video && AudioTarget != null)
                throw new Exception("There should be no audio target for a video only stream");
            if (Streams == StreamKind.Audio && VideoTarget != null)
                throw new Exception("There should be no video target for an audio only stream");
            if (Streams == StreamKind.Both && (VideoTarget == null || AudioTarget == null))
                throw new Exception("One or more targets are not set");

            if (DebugOptions.Current.RequiresH264Analysis && VideoTarget is not null)
                VideoTarget.ChainStream(new H264LoggingTarget(), true);

            Thread1?.Start();
            Thread2?.Start();
        }

        public virtual void Stop()
        {
            Thread1?.Stop();
            Thread2?.Stop();

#if MEASURE_STATS
			var vDiff = DateTime.Now - VideoThread.FirstByteTs;
			var aDiff = DateTime.Now - AudioThread.FirstByteTs;
			var total = VideoThread.ReceivedBytes + AudioThread.ReceivedBytes;

			var max = vDiff > aDiff ? vDiff : aDiff;

			Console.WriteLine($"MEASURE_STATS: received {total} bytes in {max.TotalSeconds} s of streaming, avg of {total / max.TotalSeconds} B/s.");
			Console.WriteLine($"Per thread stats:");
			Console.WriteLine($"\tVideo: {VideoThread.ReceivedBytes} bytes in {vDiff.TotalSeconds} s, avg of {VideoThread.ReceivedBytes / vDiff.TotalSeconds} B/s");
			Console.WriteLine($"\tAudio: {AudioThread.ReceivedBytes} bytes in {aDiff.TotalSeconds} s, avg of {AudioThread.ReceivedBytes / aDiff.TotalSeconds} B/s");
#endif

            Thread1?.Join();
            Thread2?.Join();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Thread1?.Dispose();
                    Thread2?.Dispose();

                    if (VideoTarget is IDisposable iv)
                        iv.Dispose();
                    if (AudioTarget is IDisposable ia)
                        ia.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
