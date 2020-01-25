using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UsbStream
{
	internal class TCPBridgeManager : RTSP.SysDvrRTSPServer 
	{
		TCPBridgeThread VideoThread, AudioThread;
		CancellationTokenSource tok;

		public TCPBridgeManager(bool hasVideo, bool hasAudio, string source) : base(hasVideo, hasAudio, false)
		{
			tok = new CancellationTokenSource();
			var t = tok.Token;
			if (hasVideo)
				VideoThread = new TCPBridgeThread(StreamKind.Video, Video, source, 6666, t);
			if (hasAudio)
				AudioThread = new TCPBridgeThread(StreamKind.Audio, Audio, source, 6667, t);
		}

		public override void Begin()
		{
			VideoThread?.Begin();
			AudioThread?.Begin();
			base.Begin();
		}

		public override void Stop()
		{
			tok.Cancel();
			VideoThread?.CloseSocket();
			AudioThread?.CloseSocket();
			VideoThread?.Join();
			AudioThread?.Join();
			base.Stop();
		}

		protected override void Dispose(bool Managed)
		{
			if (!Managed) return;
			VideoThread?.Dispose();
			AudioThread?.Dispose();
			base.Dispose(Managed);
		}
	}

	class TCPBridgeThread : IDisposable
	{
		public readonly StreamKind Kind;
		protected readonly CancellationToken Token;
		protected Thread thread;

		protected IOutTarget Target;

		protected string Ip;
		protected int Port;
		protected TcpClient cli;
		public TCPBridgeThread(StreamKind kind, IOutTarget target, string ip, int port, CancellationToken Token)
		{
			this.Kind = kind;
			this.Token = Token;
			Ip = ip;
			Port = port;
			Target = target;
		}

		public void Begin()
		{
			thread = new Thread(MainLoop);
			thread.Start();
		}

		public void CloseSocket() 
		{
			cli?.Close();
		}

		readonly ArrayPool<byte> pool = ArrayPool<byte>.Create();
		readonly byte[] tempBuffer = new byte[8];
		const byte magic = 0x11;
		const int magicLen = 4;
		private async void MainLoop()
		{
			cli = new TcpClient(Ip, Port);
			var stream = cli.GetStream();
			Target.InitializeStreaming();

			var bin = new BinaryReader(stream);
			try
			{
				while (!Token.IsCancellationRequested)
				{
					int read = 0;

					{
						int magicCount = 0;
						while (magicCount != magicLen && !Token.IsCancellationRequested)
						{
							read = await stream.ReadAsync(tempBuffer, 0, 1, Token);
							if (read <= 0)
								return;
							if (tempBuffer[0] == magic) magicCount++;
						}
						if (Token.IsCancellationRequested)
							return;
					}

					UInt64 ts = bin.ReadUInt64();
					Int32 sz = bin.ReadInt32();
					if (sz < 0)
						continue;

					byte[] data = bin.ReadBytes(sz);

					Target.SendData(data, 0, sz, ts);
#if DEBUG
					Console.WriteLine($"[{Kind}] received {sz} ts {ts}");
#endif
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Closing {Kind} thread due to {ex.GetType().Name}...");
			}
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
