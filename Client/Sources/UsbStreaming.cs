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
			device.Close();
			device.Dispose();
			return Task.CompletedTask;
		}

		bool Reconnect(string reason)
		{
			ReportMessage($"USB warning: Couldn't communicate with the console ({reason}). Resetting the connection...");
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
                    ReportMessage($"USB warning: Couldn't communicate with the console. Try entering a compatible game, unplugging your console or restarting it.");

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

        protected override async Task<uint> SendHandshakePacket(ProtoHandshakeRequest req)
        {
			var buffer = new byte[ProtoHandshakeRequest.StructureSize];
			MemoryMarshal.Write(buffer, ref req);

			var (err, transfer) = await writer.WriteAsync(buffer, 1500).ConfigureAwait(false);

            if (err != LibUsbDotNet.Error.Success || transfer != buffer.Length)
            	throw new Exception($"libusb write handshake failed, result: {err} len: {transfer}");

            (err, transfer) = await reader.ReadAsync(buffer, 0, 4, 1500).ConfigureAwait(false);
            if (err != LibUsbDotNet.Error.Success || transfer != 4)
                throw new Exception($"libusb receive handshake failed, result: {err} len: {transfer}");

			return BitConverter.ToUInt32(buffer, 0);
        }
    }
}
