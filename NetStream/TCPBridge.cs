using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace UsbStream
{
	internal class TCPBridgeManager : RTSP.SysDvrRTSPServer 
	{
		TCPBridgeThread Video, Audio;
		CancellationTokenSource tok;

		public TCPBridgeManager(bool video, bool audio, IPAddress source) : base(video, audio, false)
		{
		
		}

		public override void Begin()
		{
			Video?.Begin();
			Audio?.Begin();
			base.Begin();
		}

		public override void Stop()
		{
			tok.Cancel();
			Video?.Join();
			Audio?.Join();
			base.Stop();
		}

		protected override void Dispose(bool Managed)
		{
			base.Dispose(Managed);
			if (!Managed) return;
			Video?.Dispose();
			Audio?.Dispose();
		}
	}

	abstract class TCPBridgeThread : IDisposable
	{
		public readonly StreamKind Kind;
		protected readonly int BufferSize;
		protected readonly CancellationToken Token;
		protected Thread thread;

		protected TCPBridgeThread(StreamKind kind, int bufSz)
		{
			this.Kind = kind;
			BufferSize = bufSz;
			Buffer = new byte[bufSz];
		}

		public void Begin()
		{
			thread = new Thread(MainLoop);
			thread.Start();
		}

		byte[] Buffer = null;
		private void MainLoop() 
		{
			
		}

		public void Join() => thread.Join();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool Managed)
		{
			if (!Managed) return;
			
			if (thread.IsAlive)
				thread.Abort();
		}

	}
}
