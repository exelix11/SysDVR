using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.RTSP
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

		public override void Stop() {
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
		static int FindNextNALHeader(Span<byte> span)
		{
			for (int i = 2; i < span.Length; i++)
				if (span[i - 2] == 0 && span[i - 1] == 0 && span[i] == 1)
					return i + 1;

			return -1;
		}

		static Span<byte> AdvanceToStart(Span<byte> span)
		{
            var offset = FindNextNALHeader(span);
			if (offset == -1)
				return Span<byte>.Empty;
			else
				return span.Slice(offset);
        }

		override public void SendData(PoolBuffer block, ulong ts)
		{			
			var tsMs = ts / 1000;

			Span<byte> data = AdvanceToStart(block);
			while (!data.IsEmpty)
			{
				var next = FindNextNALHeader(data);
				if (next == -1)
				{
					InvokeEvent(data, tsMs);
					break;
				}

				var sendSlice = data.Slice(0, next - 3);
                InvokeEvent(sendSlice, tsMs);

                data = data.Slice(next);
            }
            
			block.Free();
		}
	}
}
