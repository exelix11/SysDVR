using SysDVR.Client.RTSP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client
{
    public record DebugOptions(bool Stats, bool Log, bool Keyframe, bool Nal, bool Fps, bool NoSync)
    {
        public static DebugOptions Current = new DebugOptions(false, Debugger.IsAttached, false, false, false, false);

        public bool RequiresH264Analysis => Keyframe || Nal;

        public static DebugOptions Parse(string? options)
        {
            if (string.IsNullOrEmpty(options))
                return Current;

            bool stats = false, log = false, keyframe = false, nal = false, fps = false, nosync = false;
            foreach (var opt in options.Split(','))
            {
                switch (opt)
                {
                    case "stats":
                        stats = true;
                        break;
                    case "log":
                        log = true;
                        break;
                    case "keyframe":
                        keyframe = true;
                        break;
                    case "nal":
                        nal = true;
                        break;
                    case "fps":
                        fps = true;
                        break;
                    case "nosync":
                        nosync = true;
                        break;
                    default:
                        throw new Exception($"Unknown debug option: {opt}");
                }
            }
            return new DebugOptions(stats, log, keyframe, nal, fps, nosync);
        }
    }

    class FramerateCounter 
    {
        Stopwatch sw = new();
        int frames = 0;

        public void Start() 
        {
            sw.Restart();
        }

        public void OnFrame()
        {
            unchecked { frames++; }
        }

        public bool GetFps(out int fps) 
        {
            if (sw.ElapsedMilliseconds > 1000)
            {
                fps = (int)(frames * 1000 / sw.ElapsedMilliseconds);
                frames = 0;
                sw.Restart();
                return true;
            }

            fps = 0;
            return false;
        }
    }

    class H264LoggingWrapperTarget : IOutStream, IDisposable
    {
        public readonly IOutStream Inner;

        public H264LoggingWrapperTarget(IOutStream inner)
        {
            Inner = inner;
        }

        public void UseCancellationToken(CancellationToken tok)
        {
            Inner.UseCancellationToken(tok);
        }

        DateTime lastKeyframe = DateTime.Now;
        DateTime lastNal = DateTime.Now;
        StringBuilder sb = new();
        void IOutStream.SendData(PoolBuffer block, ulong ts)
        {
            sb.Clear();
            sb.Append('[');

            bool firstInSeq = true;
            foreach (var (start, length) in H264Util.EnumerateNals(block.ArraySegment))
            {
                var nal = block.Span.Slice(start, length);
                if (DebugOptions.Current.Nal)
                {
                    if (firstInSeq)
                    {
                        var now = DateTime.Now;
                        var diff = (int)((now - lastNal).TotalMilliseconds);
                        sb.AppendFormat("{0}ms ", diff);
                        lastNal = now;
                        firstInSeq = false;
                    }

                    sb.AppendFormat("{0:x} ", (nal[0] & 0x1F));
                }

                if (DebugOptions.Current.Keyframe)
                {
                    // IDR frame
                    if ((nal[0] & 0x1F) == 5)
                    {
                        var now = DateTime.Now;
                        var diff = now - lastKeyframe;
                        sb.AppendFormat("kf {0}ms ", (int)diff.TotalMilliseconds);
                        lastKeyframe = now;
                    }
                }
            }

            if (sb.Length != 1) 
            { 
                sb.Append("] ");
                Console.Write(sb.ToString());
            }

            Inner.SendData(block, ts);
        }

        public void Dispose()
        {
            if (Inner is IDisposable i)
                i.Dispose();
        }
    }
}
