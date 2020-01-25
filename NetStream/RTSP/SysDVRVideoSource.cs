using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbStream.RTSP
{
	public class SysDvrRTSPServer : IMutliStreamManager
	{
		public IOutTarget Video { get; protected set; }
		public IOutTarget Audio { get; protected set; }

		RtspServer server;

		public SysDvrRTSPServer(bool videoSupport, bool audioSupport, bool localOnly) 
		{
			SysDVRVideoRTSPTarget v = null;
			SysDVRAudioRTSPTarget a = null;

			if (videoSupport) v = new SysDVRVideoRTSPTarget();
			if (audioSupport) a = new SysDVRAudioRTSPTarget();

			Video = v;
			Audio = a;
			server = new RtspServer(6666, v, a, localOnly);
		}

		public virtual void Begin() => server.StartListenerThread();
		public virtual void Stop() => server.StopListen();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool Managed)
		{
			if (!Managed) return;
			server?.Dispose();
			Video?.Dispose();
			Audio?.Dispose();
		}
	}

	public abstract class SysDvrRTSPTarget : IOutTarget
	{
		public delegate void DataAvailableFn(Memory<byte> Data, ulong tsMsec);
		public event DataAvailableFn DataAvailable;
		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		protected void InvokeEvent(Memory<byte> Data, ulong tsMsec) => DataAvailable(Data, tsMsec);

		public abstract void SendData(byte[] data, int offset, int size, ulong ts);
		
		public void InitializeStreaming()
		{
			ClientConnected?.Invoke();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool managed)
		{

		}

		~SysDvrRTSPTarget() { Dispose(false); }
	}

	public class SysDVRAudioRTSPTarget : SysDvrRTSPTarget
	{
		public override void SendData(byte[] data, int offset, int size, ulong ts)
		{
			InvokeEvent(new Memory<byte>(data, offset, size), ts / 1000);
		}
	}

	public class SysDVRVideoRTSPTarget : SysDvrRTSPTarget
	{
		public static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
		public static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

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

		//public override void InitializeStreaming()
		//{
		//	 SendData(SPS, 0, SPS.Length, 0);
		//	 SendData(PPS, 0, PPS.Length, 0);
		//}

		public override void SendData(byte[] indata, int offset, int size, ulong ts)
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
