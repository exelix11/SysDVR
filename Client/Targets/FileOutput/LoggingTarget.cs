#if DEBUG
using System;
using System.Diagnostics;
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

        bool Written = false;
        readonly object lockObj = new object();

        public LoggingTarget(string filename)
        {
            this.filename = filename;
            bin = new BinaryWriter(mem);

            VideoTarget = new ChannelTarget(this, StreamKind.Video);
            AudioTarget = new ChannelTarget(this, StreamKind.Audio);
        }

        public void FlushToDisk()
        {
            lock (lockObj)
            {
                if (Written) return;
                Written = true;
                File.WriteAllBytes(filename, mem.ToArray());
            }
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
                MemoryMarshal.Write(headerBin, in header);

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

            protected override void DisposeImpl()
            {
                parent.FlushToDisk();
                base.DisposeImpl(); 
            }
        }
    }

    class LoggingManager
    {
        public static StreamManager Create(StreamingSource source, CancellationTokenSource cancel) 
        {
            var target = new LoggingTarget("log.bin");
            
            return new StreamManager(source, 
                source.Options.HasVideo ? target.VideoTarget : null,
                source.Options.HasAudio ? target.AudioTarget : null,
                cancel
            );
        }
    }
}
#endif
