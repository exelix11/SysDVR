using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;

namespace SysDVR.Client.Targets.RTSP
{
    class SysDvrRTSPManager : BaseStreamManager
    {
        RtspServer server;

        public SysDvrRTSPManager(bool video, bool audio, bool localOnly, int port) :
            base(video ? new SysDVRVideoRTSPTarget() : null, audio ? new SysDVRAudioRTSPTarget() : null)
        {
            server = new RtspServer(port, VideoTarget as SysDVRVideoRTSPTarget, AudioTarget as SysDVRAudioRTSPTarget, localOnly);
        }

        public override void Begin()
        {
            server.StartListenerThread();
            base.Begin();
        }

        public override void Stop()
        {
            server.StopListen();
            base.Stop();
        }
    }

    abstract class SysDvrRTSPTarget : IOutStream
    {
        public delegate void DataAvailableFn(Span<byte> Data, ulong tsMsec);
        public event DataAvailableFn DataAvailable;
        public CancellationToken Cancellation;

        protected void InvokeEvent(Span<byte> Data, ulong tsMsec) => DataAvailable(Data, tsMsec);

        public abstract void SendData(PoolBuffer block, ulong ts);

        public void UseCancellationToken(CancellationToken tok) { Cancellation = tok; }
    }

    class SysDVRAudioRTSPTarget : SysDvrRTSPTarget
    {
        public override void SendData(PoolBuffer block, ulong ts)
        {
            InvokeEvent(block, ts / 1000);
            block.Free();
        }
    }

    class SysDVRVideoRTSPTarget : SysDvrRTSPTarget
    {
        override public void SendData(PoolBuffer block, ulong ts)
        {
            var tsMs = ts / 1000;

            var dataSpan = block.Span;
            foreach (var (start, length) in H264Util.EnumerateNals(block.ArraySegment))
            {
                var data = dataSpan.Slice(start, length);
                InvokeEvent(data, tsMs);
            }

            block.Free();
        }
    }
}
