﻿using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    class UsbStreamingSource : StreamingSource
	{
		bool closed = false;
		readonly DvrUsbDevice device;
		protected UsbEndpointReader reader;
		protected UsbEndpointWriter writer;

		public UsbStreamingSource(DvrUsbDevice device, StreamingOptions opt, CancellationToken cancel) : base(opt, cancel)
		{
			this.device = device;
			(reader, writer) = device.Open();
		}

		public override Task StopStreaming()
		{
			Dispose();
			return Task.CompletedTask;
		}

		bool Reconnect(string reason)
		{
			ReportMessage(string.Format(Program.Strings.Errors.UsbResetMessage, reason));
			if (device.TryReconnect())
			{
				(reader, writer) = device.Open();
				return true;
			}
			return false;
		}

		public override async Task Connect()
		{
			await WaitForConnection().ConfigureAwait(false);
		}

		async Task WaitForConnection()
		{
			while (!Cancellation.IsCancellationRequested)
			{
				Program.DebugLog($"Sending USB connection request");
				
				try
				{
					await DoHandshake(Options.Kind).ConfigureAwait(false);
				}
				catch (Exception e)
				{
                    ReportMessage(Program.Strings.Errors.UsbTimeoutError);

                    if (!Cancellation.IsCancellationRequested)
						await Task.Delay(1000, Cancellation).ConfigureAwait(false);

                    Reconnect(e.Message);
					continue;
				}

				return;
			}
		}

		public override async Task Flush()
		{
			if (Cancellation.IsCancellationRequested)
				return;

			// Wait some time so the switch side timeouts
			await Task.Delay(3000, Cancellation).ConfigureAwait(false);

			// Then attempt to connect again
			await WaitForConnection().ConfigureAwait(false);
		}

		private byte[] ReadBuffer = new byte[PacketHeader.MaxTransferSize];
        public override async Task<ReceivedPacket> ReadNextPacket()
        {
            var (err, read) = await reader.ReadAsync(ReadBuffer, 800).ConfigureAwait(false);
            if (err != LibUsbDotNet.Error.Success)
				throw new Exception($"Warning: libusb error {err} while reading header");

			if (read < PacketHeader.StructLength)
				throw new Exception("Libusb did not read enough data");

			var header = MemoryMarshal.Read<PacketHeader>(ReadBuffer);

			if (!ValidatePacketHeader(in header))
				throw new Exception($"Invaid packet header: {header}");

			PoolBuffer? data = null;
			if (header.DataSize != 0)
			{
				data = PoolBuffer.Rent(header.DataSize);
				ReadBuffer.AsSpan(PacketHeader.StructLength, header.DataSize).CopyTo(data.Span);
			}

            return new ReceivedPacket(header, data);
        }

		protected override async Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes)
		{
			var buffer = new byte[maxBytes];
			
			var (err, read) = await reader.ReadAsync(buffer, 800).ConfigureAwait(false);
			if (err != LibUsbDotNet.Error.Success || read != maxBytes)
				throw new Exception($"libusb receive handshake failed, result: {err} len: {read}");

			return buffer;
		}

		protected override async Task<byte[]> SendHandshakePacket(ProtoHandshakeRequest req, int expectedSize)
        {
			var buffer = new byte[ProtoHandshakeRequest.StructureSize];
			MemoryMarshal.Write(buffer, in req);

			var (err, transfer) = await writer.WriteAsync(buffer, 1500).ConfigureAwait(false);

            if (err != LibUsbDotNet.Error.Success || transfer != buffer.Length)
            	throw new Exception($"libusb write handshake failed, result: {err} len: {transfer}");

			var response = new byte[expectedSize];
            (err, transfer) = await reader.ReadAsync(response, 0, expectedSize, 1500).ConfigureAwait(false);
            if (err != LibUsbDotNet.Error.Success || transfer != expectedSize)
                throw new Exception($"libusb receive handshake failed, result: {err} len: {transfer}/{expectedSize}");

			return response;
        }

		public override void Dispose()
		{
			if (closed)
				return;

			closed = true;
			device.Close();
			device.Dispose();
		}
	}
}
