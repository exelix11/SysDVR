using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVRClient
{
	class TCPBridgeSource : StreamingSource
	{
		public bool Logging { get; set; }

		CancellationToken token;
		TcpClient Client;
		string IpAddress;
		int Port;

		public TCPBridgeSource(string ip, StreamKind kind)
		{
			IpAddress = ip;
			Port = kind == StreamKind.Video ? 9911 : 9922;
		}

		NetworkStream Stream;
		public void WaitForConnection()
		{
			Client = new TcpClient();
			Client.ConnectAsync(IpAddress, Port, token).GetAwaiter().GetResult();
			if (Client.Connected)
				Stream = Client.GetStream();
		}

		public void StopStreaming()
		{
			Stream?.Close();
			Client?.Close();
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			token = tok;
		}

		public void Flush()
		{
			Stream.Flush();
			InSync = false;
		}

		bool InSync = false;
		public bool ReadHeader(byte[] buffer)
		{
			if (InSync)
			{
				Stream.Read(buffer, 0, PacketHeader.StructLength);
			}
			else 
			{
				//TCPBridge is a raw stream of data, search for an header
				for (int i = 0; i < 4 && !token.IsCancellationRequested; i++)
				{
					buffer[i] = (byte)Stream.ReadByte();
					if (buffer[i] != 0xAA)
						i = 0;
				}
				Stream.Read(buffer, 4, PacketHeader.StructLength - 4);
				InSync = true;
			}

			return true;
		}

		public bool ReadPayload(byte[] buffer, int length)
		{
			int received = 0;
			do
				received += Stream.Read(buffer, received, length - received);
			while (received < length);
			return true;
		}
	}

	internal class TCPBridgeManager : RTSPStreamManager
	{
		public TCPBridgeManager(bool hasVideo, bool hasAudio, string source, int port) : base(hasVideo, hasAudio, false, port)
		{
			if (hasVideo)
				VideoThread = new StreamingThread(Video, StreamKind.Video, new TCPBridgeSource(source, StreamKind.Video));
			if (hasAudio)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, new TCPBridgeSource(source, StreamKind.Audio));
		}
	}

	static internal partial class Exten 
	{
		public static async Task ConnectAsync(this TcpClient tcpClient, string host, int port, CancellationToken cancellationToken)
		{
			if (tcpClient == null)
				throw new ArgumentNullException(nameof(tcpClient));

			cancellationToken.ThrowIfCancellationRequested();

			using (cancellationToken.Register(() => tcpClient.Close()))
			{
				cancellationToken.ThrowIfCancellationRequested();
				await tcpClient.ConnectAsync(host, port).ConfigureAwait(false);				
			}
		}
	}
}
