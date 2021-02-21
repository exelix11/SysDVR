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
		
		protected void InvokeEvent(Span<byte> Data, ulong tsMsec) => DataAvailable(Data, tsMsec);

		public abstract void SendData(PoolBuffer block, ulong ts);

		public void UseCancellationToken(CancellationToken tok) { }
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
		static Span<byte> FindNalOffset(Span<byte> span)
		{
			int nalOffset = -1;
			int nextOffset = -1;

			for (int i = 2; i < span.Length; i++)
				if (span[i] == 1 && span[i - 1] == 0 && span[i - 2] == 0)
				{
					if (nalOffset == -1) nalOffset = i + 1;
					else
					{
						nextOffset = i - 1;
						while (nextOffset >= 0 && span[nextOffset] == 0) nextOffset--;
						break;
					}
				}

			if (nalOffset == -1) return null;
			if (nextOffset == -1) return span.Slice(nalOffset);

			return span.Slice(nalOffset, nextOffset - nalOffset);
		}

		override public void SendData(PoolBuffer block, ulong ts)
		{
			Span<byte> data = block;
			var nal = FindNalOffset(data);
			while (nal != null)
			{
				InvokeEvent(nal, ts / 1000);
				nal = FindNalOffset(nal);
			}
			block.Free();
		}
	}
}
