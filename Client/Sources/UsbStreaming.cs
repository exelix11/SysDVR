using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
	class UsbStreamingSource : StreamingSource
	{
		CancellationToken Token;

		readonly DvrUsbDevice device;
		protected UsbEndpointReader reader;
		protected UsbEndpointWriter writer;

		public UsbStreamingSource(DvrUsbDevice device, StreamKind kind)
        {
			this.device = device;
			SourceKind = kind;

            (reader, writer) = device.Open();
        }

		public override void StopStreaming()
		{
			device.Close();
			device.Dispose();
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

        public override Task ConnectAsync(CancellationToken token)
        {
            Token = token;
			return Task.Run(WaitForConnection, token);
        }

        void WaitForConnection()
		{
			bool printedTimeoutWarningOnce = false;
			while (!Token.IsCancellationRequested)
			{
                ReportMessage($"Sending USB connection request");
				try
				{
					DoHandshake();
				}
				catch (Exception e)
				{
                    if (!printedTimeoutWarningOnce)
                    {
                        printedTimeoutWarningOnce = true;
                        ReportMessage($"USB warning: Couldn't communicate with the console. Try entering a compatible game, unplugging your console or restarting it.");
                    }

                    if (!Token.IsCancellationRequested)
                        Thread.Sleep(3000);

                    Reconnect(e.Message);
					continue;
				}

				return;
			}
		}

		public override void Flush()
		{
			if (Token.IsCancellationRequested)
				return;

			// Wait some time so the switch side timeouts
			Thread.Sleep(3000);

			// Then attempt to connect again
			WaitForConnection();
		}

		// Not all USB implementations buffer data in the OS side,
		// to support libusb we manually define the backing buffer and read everything in one shot
		// At some point there was a different implementation for WinUSB since that does support buffering
		// But it makes little sense to keep them separate as it doesn't grant any performance improvement.
		private int ReadSize = 0;
		private byte[] ReadBuffer = new byte[PacketHeader.MaxTransferSize];

        public override bool ReadHeader(byte[] buffer)
		{
			var err = reader.Read(ReadBuffer, 0, PacketHeader.MaxTransferSize, 800, out ReadSize);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (DebugOptions.Current.Log)
                    ReportMessage($"Warning: libusb error {err} while reading header");

				return false;
			}

			Buffer.BlockCopy(ReadBuffer, 0, buffer, 0, PacketHeader.StructLength);

			return true;
		}

		public override bool ReadPayload(byte[] buffer, int length)
		{
			if (length > ReadSize - PacketHeader.StructLength)
				return false;

			Buffer.BlockCopy(ReadBuffer, PacketHeader.StructLength, buffer, 0, length);

			return true;
		}

		public override bool ReadRaw(byte[] buffer, int length)
		{
			var err = reader.Read(buffer, 0, length, 1500, out ReadSize);

            if (err != LibUsbDotNet.Error.Success && DebugOptions.Current.Log)
                ReportMessage($"Warning: libusb error {err} while reading data");

            return err == LibUsbDotNet.Error.Success;
        }

        public override bool WriteData(byte[] buffer)
        {
			var err = writer.Write(buffer, 2000, out int _);

			if (err != LibUsbDotNet.Error.Success && DebugOptions.Current.Log)
				ReportMessage($"Warning: libusb error {err} while writing data");

            return err == LibUsbDotNet.Error.Success;
        }
    }
}
