﻿#if DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using SysDVR.Client.Core;

namespace SysDVR.Client.Sources
{
	// Playsback a recording made with LoggingTarget, useful for developing without a console
	class RecordedSource : StreamingSource
	{
		readonly string FileName;
		BinaryReader Source;

        public RecordedSource(string fileName, StreamingOptions opt, CancellationToken cancel) : base(opt, cancel) 
		{
			FileName = fileName;
		}

		public override Task Flush() => 
            throw new Exception("Flush is not supported");

		public override Task StopStreaming() => Task.CompletedTask;

		Stopwatch sw = new Stopwatch();

		public override Task Connect()
		{
			Source = new BinaryReader(File.OpenRead(FileName));
			return Task.CompletedTask;
		}

        public override async Task<ReceivedPacket> ReadNextPacket()
        {
            if (Source.PeekChar() < 0)
                throw new Exception("File empty");

            var headerBin = Source.ReadBytes(PacketHeader.StructLength);
            var header = MemoryMarshal.Read<PacketHeader>(headerBin);
            var body = PoolBuffer.Rent(header.DataSize);

            await Source.BaseStream.ReadAsync(body.Memory, Cancellation);

            var tsDelay = Source.ReadUInt32();

            sw.Stop();
            if (sw.ElapsedMilliseconds < tsDelay)
                await Task.Delay((int)(tsDelay - sw.ElapsedMilliseconds));

            sw.Restart();
            return new ReceivedPacket(header, body);
        }

        protected override Task<uint> SendHandshakePacket(ProtoHandshakeRequest req)
        {
            return Task.FromResult(ProtoHandshakeRequest.HandshakeOKCode);
        }
    }
}
#endif