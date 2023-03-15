using System;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
	class TCPBridgeSource : IStreamingSource
	{
		public bool Logging { get; set; }
        public StreamKind SourceKind { get; private init; }

        const int MaxConnectionAttempts = 5;
		const int ConnFailDelayMs = 2000;

		readonly string IpAddress;
		readonly int Port;
		readonly byte PacketMagicHeader;

		CancellationToken Token;
		Socket Sock;

		public TCPBridgeSource(string ip, StreamKind kind)
		{
            if (kind == StreamKind.Both)
                throw new Exception("Tcp bridge can't stream both channels over a single connection");

            SourceKind = kind;
			IpAddress = ip;
			Port = kind == StreamKind.Video ? 9911 : 9922;

			PacketMagicHeader = 
				(byte)((kind == StreamKind.Video ? PacketHeader.MagicResponseVideo : PacketHeader.MagicResponseAudio) & 0xFF);
        }

		public void WaitForConnection()
		{
			Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				Sock.ReceiveBufferSize = PacketHeader.MaxTransferSize * 2;
				Sock.NoDelay = true;
			}
			catch (Exception ex)
			{
				if (Logging)
					Console.WriteLine("Info: Failed to set TcpClient options: " + ex);
			}

			Exception ReportException = null;
			for (int i = 0; i < MaxConnectionAttempts; i++) 
			{
				if (i != 0 || Logging) // Don't show error for the first attempt
					Console.WriteLine($"[{SourceKind} stream] Connecting to console (attempt {i}/{MaxConnectionAttempts})...");

				try
				{
					Sock.ConnectAsync(IpAddress, Port, Token).GetAwaiter().GetResult();
					if (Sock.Connected)
						break;
				}
				catch (Exception ex) 
				{
					ReportException = ex;
					Thread.Sleep(ConnFailDelayMs);
				}
			}

			if (!Sock.Connected)
			{
				Console.WriteLine($"Connection to {SourceKind} stream failed. Throwing exception.");
				throw ReportException ?? new Exception("No exception provided");
			}
		}

		public void StopStreaming()
		{
			Sock?.Close();
			Sock?.Dispose();
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			Token = tok;
		}

		public void Flush()
		{
			InSync = false;
		}

		bool InSync = false;
		public bool ReadHeader(byte[] buffer)
		{
			if (InSync)
			{
				return ReadPayload(buffer, PacketHeader.StructLength);
			}
			else 
			{
				// TCPBridge is a raw stream of data, search for an header
				for (int i = 0; i < 4 && !Token.IsCancellationRequested; i++)
				{
					ReadExact(buffer.AsSpan().Slice(i, 1));
					if (buffer[i] != PacketMagicHeader)
						i = 0;
				}
				ReadExact(buffer.AsSpan().Slice(4, PacketHeader.StructLength - 4));
				InSync = true;
			}

			return true;
		}

		void ReadExact(Span<byte> data)
		{
			while (data.Length > 0) 
			{
				int r = Sock.Receive(data);
				data = data.Slice(r);
			}
		}

		public bool ReadPayload(byte[] buffer, int length)
		{
			ReadExact(buffer.AsSpan().Slice(0, length));
			return true;
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
