using LibUsbDotNet;
using SysDVR.Client.Core;
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
    class TCPBridgeSource : StreamingSource
	{
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
        bool CommunicationException = false;

        bool InSync = false;

        public TCPBridgeSource(DeviceInfo ip, StreamKind kind)
		{
            if (kind == StreamKind.Both)
                throw new Exception("Tcp bridge can't stream both channels over a single connection");

            SourceKind = kind;
			IpAddress = ip.ConnectionString;

            HeaderMagicByte = unchecked((byte)PacketHeader.MagicResponse);
        }

        public override async Task ConnectAsync(CancellationToken tok)
        {
            Token = tok;
            Sock?.Close();
            Sock?.Dispose();
            CommunicationException = false;

            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Sock.ReceiveBufferSize = PacketHeader.MaxTransferSize;
                Sock.NoDelay = true;
            }
            catch { }

            Exception ReportException = null;
            for (int i = 0; i < MaxConnectionAttempts && !Token.IsCancellationRequested; i++)
            {
                if (i != 0 || DebugOptions.Current.Log) // Don't show error for the first attempt
                    ReportMessage($"[{SourceKind} stream] Connecting to console (attempt {i}/{MaxConnectionAttempts})...");

                try
                {
                    await Sock.ConnectAsync(IpAddress, Port, Token).ConfigureAwait(false);
                    if (Sock.Connected)
                    {
                        DoHandshake();

                        // Assume the connection starts in-sync and fall back to resync code only on failure
                        InSync = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Sock.Disconnect(true);
                    ReportException = ex;
                    await Task.Delay(ConnFailDelayMs).ConfigureAwait(false);
                }
            }

            if (Token.IsCancellationRequested)
                return;

            if (!Sock.Connected)
            {
                ReportMessage($"Connection to {SourceKind} stream failed. Throwing exception.");
                throw ReportException ?? new Exception("No exception provided");
            }
        }

		public override void StopStreaming()
		{
			Sock?.Close();
            Sock?.Dispose();
		}

		public override void Flush() 
		{
            if (Token.IsCancellationRequested)
                return;

            InSync = false;
			if (CommunicationException)
			{
				Sock?.Close();
				Sock?.Dispose();
				Sock = null;

                ReportMessage($"{SourceKind} stream connection lost, reconnecting...");
                Thread.Sleep(800);
				
                ConnectAsync(Token).GetAwaiter().GetResult();
			}
        }

        public override bool ReadHeader(byte[] buffer)
        {
            // What is this ? Isn't TCP supposed to be reliable ?
			// Well turs out we have small-ish socket buffers on the console side and certain packets like video
			// can be much bigger, this causes non-recoverable packet drops. This only happens in extreme situations
			// so we just handle the error by ignoring dropped packets looking for an header
            if (InSync)
            {
                return ReadPayload(buffer, PacketHeader.StructLength);
            }
            else
            {
				if (DebugOptions.Current.Log)
                    ReportMessage($"{SourceKind} Resyncing....");

                // TCPBridge is a raw stream of data, search for an header
                for (int i = 0; i < 4 && !Token.IsCancellationRequested;)
                {
					if (!ReadExact(buffer.AsSpan().Slice(i, 1)))
						return false;

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
                        CommunicationException = true;
                        return false;
					}

					data = data.Slice(r);
				}
			}
			catch 
			{
                CommunicationException = true;
                return false;
			}

			return true;
		}

		public override bool ReadPayload(byte[] buffer, int length)
		{
			return ReadExact(buffer.AsSpan().Slice(0, length));
		}

        public override bool WriteData(byte[] buffer)
        {
            try
            {
                return Sock.Send(buffer) == buffer.Length;
            }
            catch
            {
                CommunicationException = true;
                return false;
            }
        }

        // There is no difference for the TCP backend
        public override bool ReadRaw(byte[] buffer, int length) =>
            ReadPayload(buffer, length);
    }
}
