using FFmpeg.AutoGen;
using LibUsbDotNet;
using SysDVR.Client.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    class TCPBridgeSource : StreamingSource
    {
        const int MaxConnectionAttempts = 5;
        const int ConnFailDelayMs = 2000;

        const int TcpBridgeVideoPort = 9911;
        const int TcpBridgeAudioPort = 9922;

        readonly TcpBridgeSubStream? videoStream;
        readonly TcpBridgeSubStream? audioStream;

        readonly Channel<ReceivedPacket> poolBuffers = Channel.CreateBounded<ReceivedPacket>(
            new BoundedChannelOptions(30)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,
                SingleReader = true
            },
            x => x.Buffer?.Free()
        );

        public TCPBridgeSource(DeviceInfo device, CancellationToken tok, StreamingOptions opt) : base(opt, tok)
        {
            if (Options.HasVideo)
                videoStream = new(device.ConnectionString, StreamKind.Video, tok, OnBuffer, ReportMessage);

            if (Options.HasAudio)
                audioStream = new(device.ConnectionString, StreamKind.Audio, tok, OnBuffer, ReportMessage);
        }

        async void OnBuffer(ReceivedPacket buffer)
        {
            try
            {
                await poolBuffers.Writer.WriteAsync(buffer, Cancellation);
            }
            catch
            {
                // This will throw on cancel
            }
        }

        async Task WaitAll(Task? first, Task? second)
        {
            if (first is null && second is null)
                return;

            if (first is null)
                await second!.ConfigureAwait(false);
            else if (second is null)
                await first!.ConfigureAwait(false);
            else
            {
                await Task.WhenAll(first, second).ConfigureAwait(false);

                if (first.Exception is not null) throw first.Exception;
                if (second.Exception is not null) throw second.Exception;
            }
        }

        public override async Task Connect()
        {
            List<Task> tasks = new();

            await WaitAll(videoStream?.Connect(), audioStream?.Connect()).ConfigureAwait(false);

            await WaitAll(
                videoStream is null ? null : DoHandshake(StreamKind.Video),
                audioStream is null ? null : DoHandshake(StreamKind.Audio)
            ).ConfigureAwait(false);

            if (videoStream is not null)
                videoStream.PendingLoop = videoStream.ProcessLoop().ContinueWith(x => LoopFinished(videoStream, x));

            if (audioStream is not null)
                audioStream.PendingLoop = audioStream.ProcessLoop().ContinueWith(x => LoopFinished(audioStream, x));
        }

        async void LoopFinished(TcpBridgeSubStream stream, Task task)
        {
            if (Cancellation.IsCancellationRequested)
                return;

            if (task.Exception is not null)
                ReportMessage($"{stream.StreamName} connection error: {task.Exception.InnerException}");
            else // Shouldn't happen unless the loop was cancelled
                ReportMessage($"{stream.StreamName} disconnected but no error was reported.");

            // Just try to reconnect until the user cancels the operation
            while (!Cancellation.IsCancellationRequested)
            {
                try
                {
                    await stream.Connect().ConfigureAwait(false);
                    // throws if handshake fails
                    await DoHandshake(stream.Channel).ConfigureAwait(false);
                    // Then restart the loop
                    stream.PendingLoop = stream.ProcessLoop().ContinueWith(x => LoopFinished(stream, x));
                    break;
                }
                catch
                {
                    ReportMessage($"{stream.StreamName} Fatal error: {task.Exception}");
                }
            }
        }

        public override async Task StopStreaming()
        {
            if (!Cancellation.IsCancellationRequested)
                throw new Exception("StopStreaming called but the cancellation was not requested");

            if (videoStream?.PendingLoop is not null)
                await videoStream.PendingLoop.ConfigureAwait(false);

            if (audioStream?.PendingLoop is not null)
                await audioStream.PendingLoop.ConfigureAwait(false);
        }

        public override Task Flush()
        {
            if (Cancellation.IsCancellationRequested)
                return Task.CompletedTask;

            videoStream?.Flush();
            audioStream?.Flush();

            return Task.CompletedTask;
        }

        public override async Task<ReceivedPacket> ReadNextPacket()
        {
            return await poolBuffers.Reader.ReadAsync(Cancellation).ConfigureAwait(false);
        }

        protected override async Task<uint> SendHandshakePacket(ProtoHandshakeRequest req)
        {
            byte[] buffer = new byte[ProtoHandshakeRequest.StructureSize];
            MemoryMarshal.Write(buffer, in req);

            var stream = req.IsVideoPacket ? videoStream : audioStream;

            // This is only invoked from the Connect() method so stream is always guranteed to not be null
            var h = await stream!.DoHandshake(buffer).ConfigureAwait(false);
            return h;
        }

		protected override async Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes)
		{
            byte[] buffer = new byte[maxBytes];
			
            var s = stream switch { 
                StreamKind.Video => videoStream,
                StreamKind.Audio => audioStream,
                _ => throw new Exception("Invalid stream kind")
            };

            await s.ReadExact(buffer, 0, maxBytes).ConfigureAwait(false);
            return buffer;
		}

		public override void Dispose()
		{
            videoStream?.Dispose();
            audioStream?.Dispose();
		}

		// In TCP bridge each stream is carried by its own socket
		class TcpBridgeSubStream : IDisposable
        {
            public bool IsConnected => socket?.Connected ?? false;
            public readonly string StreamName;
            public readonly StreamKind Channel;

            public Task? PendingLoop;

            readonly Action<ReceivedPacket> onBuffer;
            readonly Action<string> reportMessage;

            CancellationToken cancel;
            byte[] headerBuffer = new byte[PacketHeader.StructLength];
            string host;
            int port = 0;
            bool inSync = true;
            Socket socket = null!;

            public TcpBridgeSubStream(string host, StreamKind kind, CancellationToken token, Action<ReceivedPacket> onBuffer, Action<string> reportMessage)
            {
                if (kind == StreamKind.Both)
                    throw new Exception("Invalid stream kind");

                cancel = token;
                this.onBuffer = onBuffer;
                this.host = host;
                this.Channel = kind;
                this.port = kind == StreamKind.Video ? TcpBridgeVideoPort : TcpBridgeAudioPort;
                this.StreamName = kind == StreamKind.Video ? "Video stream" : "Audio stream";
                this.reportMessage = reportMessage;
            }

            public async Task Connect()
            {
                socket?.Dispose();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.ReceiveTimeout = 2000;
                    socket.SendTimeout = 2000;
                    socket.ReceiveBufferSize = PacketHeader.MaxTransferSize;
                    socket.NoDelay = true;
                }
                catch { }

                for (int i = 0; i < MaxConnectionAttempts && !cancel.IsCancellationRequested; i++)
                {
                    reportMessage($"[{StreamName}] Connecting to console (attempt {i}/{MaxConnectionAttempts})...");

                    try
                    {
                        await socket.ConnectAsync(host, port, cancel).ConfigureAwait(false);
                        if (socket.Connected)
                            return;
                    }
                    catch
                    {
                        if (i == MaxConnectionAttempts)
                            throw;

                        await Task.Delay(ConnFailDelayMs, cancel).ConfigureAwait(false);
                    }
                }
            }

            public void Flush()
            {
                inSync = false;
            }

            internal async Task ReadExact(byte[] data, int offset, int length)
            {
                while (length > 0)
                {
                    int r = await socket.ReceiveAsync(new ArraySegment<byte>(data, offset, length), cancel).ConfigureAwait(false);
                    if (r <= 0)
                        throw new Exception("Socket communication erorr");

                    length -= r;
                    offset += r;
                }
            }

            async Task ResyncStream()
            {
                // This looks weird but since we have small socket buffers on the console side sometimes even TCCP may lose packets
                // It's rare and only under bad conditions but we can save the connection by performing a manual resync
                // Just read bytes until we find a valid header then continue normally

                byte[] data = new byte[1];
                int counter = 0;
                var magicBypte = (byte)(PacketHeader.MagicResponse & 0xFF);

                if (Program.Options.Debug.Log)
                    Console.WriteLine($"TCP {StreamName} performing resync...");

                while (!cancel.IsCancellationRequested)
                {
                    await ReadExact(data, 0, data.Length).ConfigureAwait(false);
                    if (data[0] == magicBypte)
                    {
                        if (++counter == 4)
                        {
                            inSync = true;
                            return;
                        }
                    }
                    else counter = 0;
                }
            }

            async Task<(bool, ReceivedPacket)> ReadData()
            {
                if (!inSync)
                {
                    await ResyncStream().ConfigureAwait(false);
                    // If we just resynched the stream the header was cut, read only the remainig part of the packet
                    await ReadExact(headerBuffer, 4, PacketHeader.StructLength - 4).ConfigureAwait(false);
                    // Then fix the magic value manually
                    BitConverter.TryWriteBytes(headerBuffer, PacketHeader.MagicResponse);
                }
                else
                    await ReadExact(headerBuffer, 0, PacketHeader.StructLength).ConfigureAwait(false);

                var header = MemoryMarshal.Read<PacketHeader>(headerBuffer);
                if (!ValidatePacketHeader(in header))
                    return (false, default);

                PoolBuffer? data = null;
                if (header.DataSize != 0)
                {
                    data = PoolBuffer.Rent(header.DataSize);
                    await ReadExact(data.RawBuffer, 0, header.DataSize).ConfigureAwait(false);
                }

                return (true, new ReceivedPacket(header, data));
            }

            public async Task<uint> DoHandshake(byte[] reqBuffer)
            {
                if (await socket.SendAsync(reqBuffer, cancel).ConfigureAwait(false) <= 0)
                    throw new Exception("Socket send error");

                await ReadExact(headerBuffer, 0, sizeof(uint)).ConfigureAwait(false);

                return MemoryMarshal.Read<uint>(headerBuffer);
            }

            public async Task ProcessLoop()
            {
                try
                {
                    while (!cancel.IsCancellationRequested)
                    {
                        var (result, data) = await ReadData().ConfigureAwait(false);
                        if (!result)
                            Flush();
                        else
                            onBuffer(data);
                    }
                }
                finally
                {
                    socket.Dispose();
                }
            }

			public void Dispose()
			{
                socket?.Dispose();
			}
		}
    }
}
