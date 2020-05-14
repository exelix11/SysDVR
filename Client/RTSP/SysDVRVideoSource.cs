using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVRClient.RTSP
{
	class SysDvrRTSPServer : IMutliStreamManager
	{
		public IOutTarget Video { get; protected set; }
		public IOutTarget Audio { get; protected set; }

		RtspServer server;

		public SysDvrRTSPServer(bool videoSupport, bool audioSupport, bool localOnly, int port) 
		{
			SysDVRVideoRTSPTarget v = null;
			SysDVRAudioRTSPTarget a = null;

			if (videoSupport) v = new SysDVRVideoRTSPTarget();
			if (audioSupport) a = new SysDVRAudioRTSPTarget();

			Video = v;
			Audio = a;
			server = new RtspServer(port, v, a, localOnly);
		}

		public virtual void Begin() => server.StartListenerThread();
		public virtual void Stop() => server.StopListen();
	}

	abstract class SysDvrRTSPTarget : IOutTarget
	{
		public delegate void DataAvailableFn(Memory<byte> Data, ulong tsMsec);
		public event DataAvailableFn DataAvailable;
		
		protected void InvokeEvent(Memory<byte> Data, ulong tsMsec) => DataAvailable(Data, tsMsec);

		public abstract void SendData(byte[] data, int offset, int size, ulong ts);
	}

	class SysDVRAudioRTSPTarget : SysDvrRTSPTarget
	{
		public override void SendData(byte[] data, int offset, int size, ulong ts)
		{
			InvokeEvent(new Memory<byte>(data, offset, size), ts / 1000);
		}
	}

	class SysDVRVideoRTSPTarget : SysDvrRTSPTarget
	{
		static Memory<byte>? FindNalOffset(Memory<byte> data)
		{
			var span = data.Span;

			int nalOffset = -1;
			int nextOffset = -1;

			for (int i = 2; i < data.Length; i++)
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
			if (nextOffset == -1) return data.Slice(nalOffset);

			return data.Slice(nalOffset, nextOffset - nalOffset);
		}

		override public void SendData(byte[] indata, int offset, int size, ulong ts)
		{
			Memory<byte> data = new Memory<byte>(indata, offset, size);
			var nal = FindNalOffset(data);
			while (nal != null)
			{
				InvokeEvent(nal.Value, ts / 1000);
				nal = FindNalOffset(nal.Value);
			}
		}
	}
}
