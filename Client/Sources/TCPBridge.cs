using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
	class TCPBridgeSource : IStreamingSource
	{
		public bool Logging { get; set; }

		const int MaxConnectionAttempts = 5;
		const int ConnFailDelayMs = 2000;

		readonly StreamKind Kind;
		readonly string IpAddress;
		readonly int Port;

		CancellationToken token;
		TcpClient Client;

		public TCPBridgeSource(string ip, StreamKind kind)
		{
			Kind = kind;
			IpAddress = ip;
			Port = kind == StreamKind.Video ? 9911 : 9922;
		}

		NetworkStream Stream;
		public void WaitForConnection()
		{
			Client = new TcpClient();

			try
			{
				Client.ReceiveBufferSize = PacketHeader.MaxTransferSize * 2;
				Client.NoDelay = true;
			}
			catch (Exception ex)
			{
				if (Logging)
					Console.WriteLine("Info: Failed to set TcpClient options: " + ex);
			}

			Exception ReportException = null;
			for (int i = 0; Stream == null && i < MaxConnectionAttempts; i++) 
			{
				if (i != 0 || Logging) // Don't show error for the first attempt
					Console.WriteLine($"[{Kind} stream] Connecting to console (attempt {i}/{MaxConnectionAttempts})...");

				try
				{
					Client.ConnectAsync(IpAddress, Port, token).GetAwaiter().GetResult();
					if (Client.Connected)
						Stream = Client.GetStream();
				}
				catch (Exception ex) 
				{
					ReportException = ex;
					Thread.Sleep(ConnFailDelayMs);
				}
			}

			if (Stream == null)
			{
				Console.WriteLine($"Connection to {Kind} stream failed. Throwing exception.");
				throw ReportException ?? new Exception("No exception provided");
			}
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
				return ReadPayload(buffer, PacketHeader.StructLength);
			}
			else 
			{
				// TCPBridge is a raw stream of data, search for an header
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
