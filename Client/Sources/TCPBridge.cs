using LibUsbDotNet;
using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
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

		const int TcpBridgeVideoPort = 9911;
		const int TcpBridgeAudioPort = 9922;
        
		int Port => 
			SourceKind == StreamKind.Video ? TcpBridgeVideoPort : TcpBridgeAudioPort;

        readonly string IpAddress;
		readonly byte HeaderMagicByte;

		CancellationToken Token;
		Socket Sock;

		public TCPBridgeSource(string ip, StreamKind kind)
		{
            if (kind == StreamKind.Both)
                throw new Exception("Tcp bridge can't stream both channels over a single connection");

            SourceKind = kind;
			IpAddress = ip;

			// Assumes that the magic bytes are composed of 4 identical bytes
			HeaderMagicByte = (byte)((kind == StreamKind.Video ? PacketHeader.MagicResponseVideo : PacketHeader.MagicResponseAudio) & 0xFF);			
        }

		public void WaitForConnection()
		{
            Sock?.Close();
            Sock?.Dispose();

            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try {
				Sock.ReceiveBufferSize = PacketHeader.MaxTransferSize;
			}
			catch { }

			Exception ReportException = null;
			for (int i = 0; i < MaxConnectionAttempts && !Token.IsCancellationRequested; i++) 
			{
				if (i != 0 || Logging) // Don't show error for the first attempt
					Console.WriteLine($"[{SourceKind} stream] Connecting to console (attempt {i}/{MaxConnectionAttempts})...");

				try
				{
					Sock.ConnectAsync(IpAddress, Port, Token).GetAwaiter().GetResult();
					if (Sock.Connected)
					{
						// Assume the connection starts in-sync and fall back to resync code only on failure
						InSync = true;
						break;
					}
				}
				catch (Exception ex) 
				{
					ReportException = ex;
					Thread.Sleep(ConnFailDelayMs);
				}
			}

			if (Token.IsCancellationRequested)
				return;

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

		bool ReportedLostConnection = false;
        public void Flush()
        {
            if (Token.IsCancellationRequested)
                return;

            InSync = false;
			if (ReportedLostConnection)
			{
				ReportedLostConnection = false;
				Console.WriteLine($"{SourceKind} stream connection lost, reconnecting...");
				WaitForConnection();
            }
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
				Console.WriteLine($"{SourceKind} Resyncing....");

                // TCPBridge is a raw stream of data, search for an header
                for (int i = 0; i < 4 && !Token.IsCancellationRequested;)
                {
                    ReadExact(buffer.AsSpan().Slice(i, 1));
					if (buffer[i] != HeaderMagicByte)
						i = 0;
					else 
						i++;
                }

				if (Token.IsCancellationRequested)
					return false;

                InSync = true;
                return ReadExact(buffer.AsSpan().Slice(4, PacketHeader.StructLength - 4));
            }
        }

        bool ReadExact(Span<byte> data)
		{
			try
			{
				while (data.Length > 0)
				{
					int r = Sock.Receive(data);
					if (r <= 0)
					{
						ReportedLostConnection = true;
                        return false;
					}

					data = data.Slice(r);
				}
			}
			catch 
			{
				return false;
			}

			return true;
		}

		public bool ReadPayload(byte[] buffer, int length)
		{
			return ReadExact(buffer.AsSpan().Slice(0, length));
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
