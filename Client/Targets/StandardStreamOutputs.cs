using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SysDVR.Client.Core
{
    class MpvStdinManager : BaseStreamManager
    {
        private const string BaseArgs = "--profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --untimed --cache-pause=no --no-correct-pts";

        private static OutStream GetVTarget(StreamKind kind, string path) =>
            kind == StreamKind.Video ? StreamTarget.ForProcess(kind, path, "- --fps=30 " + BaseArgs) : null;

        private static OutStream GetATarget(StreamKind kind, string path) =>
            kind == StreamKind.Audio ? StreamTarget.ForProcess(kind, path, "- --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000 " + BaseArgs) : null;

        public MpvStdinManager(StreamKind kind, string path) :
            base(GetVTarget(kind, path), GetATarget(kind, path))
        {
            if (kind == StreamKind.Both)
                throw new Exception("MpvStdinManager can only handle one stream");
        }
    }

    class StdOutManager : BaseStreamManager
    {
        private static OutStream GetVTarget(StreamKind kind) =>
            kind == StreamKind.Video ? StreamTarget.ForStdOut(kind) : null;

        private static OutStream GetATarget(StreamKind kind) =>
            kind == StreamKind.Audio ? StreamTarget.ForStdOut(kind) : null;

        public StdOutManager(StreamKind kind) :
            base(GetVTarget(kind), GetATarget(kind))
        {
            if (kind == StreamKind.Both)
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

        public override void Dispose()
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
