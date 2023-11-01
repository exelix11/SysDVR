#if DEBUG
using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;
using SysDVR.Client.Sources;

namespace SysDVR.Client.Targets.FileOutput
{
    class LoggingTarget
    {
        readonly BinaryWriter bin;
        readonly MemoryStream mem = new MemoryStream();
        readonly string filename;

        readonly public OutStream VideoTarget;
        readonly public OutStream AudioTarget;

        public LoggingTarget(string filename)
        {
            this.filename = filename;
            bin = new BinaryWriter(mem);

            VideoTarget = new ChannelTarget(this, StreamKind.Video);
            AudioTarget = new ChannelTarget(this, StreamKind.Audio);
        }

        public void FlushToDisk()
        {
            File.WriteAllBytes(filename, mem.ToArray());
        }

        Stopwatch sw = new Stopwatch();

        protected void AddBuffer(StreamKind kind, PoolBuffer data, ulong ts)
        {
            lock (this)
            {
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;

                // Create a fake header
                var header = new PacketHeader();
                header.Magic = PacketHeader.MagicResponse;
                header.Timestamp = ts;
                header.DataSize = data.Length;

                if (kind == StreamKind.Video)
                    header.Flags = PacketHeader.MetaIsVideo;
                else if (kind == StreamKind.Audio)
                    header.Flags = PacketHeader.MetaIsAudio;
                else
                    throw new Exception("Unknown stream kind");

                header.Flags |= PacketHeader.MetaIsData;

                var headerBin = new byte[PacketHeader.StructLength];
                MemoryMarshal.Write(headerBin, ref header);

                bin.Write(headerBin);
                bin.Write(data.Span);
                bin.Write((uint)elapsed);
                sw.Restart();

                data.Free();
            }
        }

        class ChannelTarget : OutStream
        {
            readonly LoggingTarget parent;
            readonly StreamKind streamKind;

            public ChannelTarget(LoggingTarget parent, StreamKind streamKind)
            {
                this.parent = parent;
                this.streamKind = streamKind;
            }

            protected override void SendDataImpl(PoolBuffer block, ulong ts)
            {
                parent.AddBuffer(streamKind, block, ts);
            }
        }
    }

    class LoggingManager : BaseStreamManager
    {
        static LoggingTarget target = null!;

        static LoggingTarget GetTarget()
        {
            if (target is null)
                target = new LoggingTarget("log.bin");

            return target;
        }

        public LoggingManager(StreamingSource source, string VPath, string APath, CancellationTokenSource cancel) : base(
            source,
            VPath != null ? GetTarget().VideoTarget : null,
            APath != null ? GetTarget().AudioTarget : null,
            cancel)
        {

        }

        public override async Task Stop()
        {
            await base.Stop().ConfigureAwait(false);
            target.FlushToDisk();
        }
    }
}
#endif
