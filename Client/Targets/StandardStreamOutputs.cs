using SysDVR.Client.Sources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace SysDVR.Client.Core
{
    class MpvStdinManager : BaseStreamManager
    {
        private const string BaseArgs = "--profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --untimed --cache-pause=no --no-correct-pts";

        private static OutStream GetVTarget(bool enable, string path) =>
            enable ? StreamTarget.ForProcess(StreamKind.Video, path, "- --fps=30 " + BaseArgs) : null;

        private static OutStream GetATarget(bool enable, string path) =>
            enable ? StreamTarget.ForProcess(StreamKind.Audio, path, "- --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000 " + BaseArgs) : null;

        public MpvStdinManager(StreamingSource source, string path, CancellationTokenSource cancel) :
            base(source, GetVTarget(source.Options.HasVideo, path), GetATarget(source.Options.HasAudio, path), cancel)
        {
            if (source.Options.Kind == StreamKind.Both)
                throw new Exception("MpvStdinManager can only handle one stream");
        }
    }

    class StdOutManager : BaseStreamManager
    {
        private static OutStream GetVTarget(bool enable) =>
            enable ? StreamTarget.ForStdOut(StreamKind.Video) : null;

        private static OutStream GetATarget(bool enable) =>
            enable ? StreamTarget.ForStdOut(StreamKind.Audio) : null;

        public StdOutManager(StreamingSource source, CancellationTokenSource cancel) :
            base(source, GetVTarget(source.Options.HasVideo), GetATarget(source.Options.HasVideo), cancel)
        {
            if (source.Options.Kind == StreamKind.Both)
                throw new Exception("StdOutManager can only handle one stream");
        }
    }

    class StreamTarget : OutStream
    {
        Stream stream;
        Process? processHandle;
        bool firstTimeForVideo = true;

        public StreamTarget(Stream stream, bool isVideo)
        {
            this.stream = stream;

            if (!isVideo)
                firstTimeForVideo = false;
        }

        public static StreamTarget ForStdOut(StreamKind kind)
        {
            return new StreamTarget(Console.OpenStandardOutput(), kind == StreamKind.Video);
        }

        public static StreamTarget ForProcess(StreamKind kind, string path, string args)
        {
            ProcessStartInfo p = new ProcessStartInfo()
            {
                Arguments = args,
                FileName = path,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            
            var proc = Process.Start(p);

            var target = new StreamTarget(proc.StandardInput.BaseStream, kind == StreamKind.Video);
            target.processHandle = proc;

            return target;
        }

        protected override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            if (firstTimeForVideo)
            {
                stream.Write(StreamInfo.SPS);
                stream.Write(StreamInfo.PPS);
                firstTimeForVideo = false;
            }

            stream.Write(block.Span);
            stream.Flush();

            block.Free();
        }

        protected override void DisposeImpl()
        {
            if (processHandle is not null)
            {
                if (!processHandle.HasExited)
                    processHandle.Kill();

                processHandle.Dispose();
            }

            base.Dispose();
        }
    }
}
