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
	// Making UsbContext non static will remove requirement on libusb, it will be loaded only if actually using USB, this is useful on architectures where it's not supported
	// This class should be used as a singleton
	class UsbContext
	{
		public class DvrUsbConnection : IDisposable
		{
			readonly UsbContext Context;
			public readonly DeviceInfo Info;

			public IUsbDevice DeviceHandle { get; private set; }

			public DvrUsbConnection(UsbContext ctx, DeviceInfo info, IUsbDevice dev)
			{
				Context = ctx;
				Info = info;
				DeviceHandle = dev;
			}

			public (UsbEndpointReader, UsbEndpointWriter) Open()
			{
				DeviceHandle.Open();

				if (!DeviceHandle.ClaimInterface(0))
					throw new Exception($"Couldn't claim device interface");

				var (epIn, epOut) = (ReadEndpointID.Ep01, WriteEndpointID.Ep01);

				var reader = DeviceHandle.OpenEndpointReader(epIn, PacketHeader.MaxTransferSize, EndpointType.Bulk);
				var writer = DeviceHandle.OpenEndpointWriter(epOut, EndpointType.Bulk);

				return (reader, writer);
			}

			public void Close()
			{
				DeviceHandle.Close();
			}

			public bool TryReconnect()
			{
				Close();
				var dev = Context.FindSysdvrDevices().Where(X => X.Info.Serial == Info.Serial).FirstOrDefault();
				if (dev == null)
					return false;

				DeviceHandle = dev.DeviceHandle;
				return true;
			}

			public void Dispose()
			{
				DeviceHandle.Dispose();
			}
		}

		static LibUsbDotNet.LibUsb.UsbContext LibUsbCtx = null;

		private UsbLogLevel _debugLevel;
		public UsbLogLevel DebugLevel
		{
			set
			{
				_debugLevel = value;
				LibUsbCtx.SetDebugLevel(value switch
				{
					UsbLogLevel.Error => LibUsbDotNet.LogLevel.Error,
					UsbLogLevel.Warning => LibUsbDotNet.LogLevel.Info,
					UsbLogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
					_ => LibUsbDotNet.LogLevel.None,
				});
			}
			get => _debugLevel;
		}

		public UsbContext(UsbLogLevel logLevel = UsbLogLevel.Error)
		{
			if (LibUsbCtx == null)
				LibUsbCtx = new LibUsbDotNet.LibUsb.UsbContext();
		
			DebugLevel = logLevel;
		}

		static bool MatchSysdvrDevice(IUsbDevice device)
		{
			try
			{
				return device.VendorId == 0x18D1 && device.ProductId == 0x4EE0;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Warning: failed to query device ID " + ex);
				return false;
			}
		}

		public IReadOnlyList<DvrUsbConnection> FindSysdvrDevices()
		{
			// THis is hacky but libusb can't seem to get the device serial without opening it first
			// If the device is already opened by another instance of sysdvr it will print an error, suppress it by temporarily changing the log level 
			var old = DebugLevel;
			DebugLevel = UsbLogLevel.None;

			var res = LibUsbCtx.FindAll(MatchSysdvrDevice).Select(x =>
			{
				try
				{
					if (!x.TryOpen())
						return null;

					var serial = x.Info.SerialNumber.Trim();
					x.Close();

					var dev = DeviceInfo.TryParse(ConnectionType.Usb, serial, "usb device");
					if (dev == null)
                        return null;

					return new DvrUsbConnection(this, dev, x);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Warning: failed to query device serial " + ex);
					return null;
				}
			}).Where(x => x != null).ToArray();

			DebugLevel = old;
			return res;
		}
	}

	class UsbStreamingSource : IStreamingSource
	{
		protected static readonly byte[] MagicRequestVideo = { 0xBB, 0xBB, 0xBB, 0xBB };
		protected static readonly byte[] MagicRequestAudio = { 0xCC, 0xCC, 0xCC, 0xCC };
		protected static readonly byte[] MagicRequestBoth = { 0xAA, 0xAA, 0xAA, 0xAA };

        public event Action<string> OnMessage;

		CancellationToken Token;

		// TODO: Remove tracing code
		readonly private TimeTrace tracer = new();

		protected TimeTrace BeginTrace(string extra = "", [CallerMemberName] string funcName = null) =>
			tracer.Begin("usb", extra, funcName);

		readonly UsbContext.DvrUsbConnection device;
		protected UsbEndpointReader reader;
		protected UsbEndpointWriter writer;

		readonly bool HasAudio;
		readonly bool HasVideo;

		byte[] RequestMagic => (HasVideo, HasAudio) switch
		{
			(true, true) => MagicRequestBoth,
			(true, false) => MagicRequestVideo,
			(false, true) => MagicRequestAudio,
			_ => throw new Exception("Invalid state")
		};

		public StreamKind SourceKind { get; private set; }

		public UsbStreamingSource(UsbContext.DvrUsbConnection device, StreamKind kind)
        {
			this.device = device;
			SourceKind = kind;

			HasVideo = kind is StreamKind.Both or StreamKind.Video;
			HasAudio = kind is StreamKind.Both or StreamKind.Audio;

            (reader, writer) = device.Open();
        }

		public void StopStreaming()
		{
			device.Close();
			device.Dispose();
		}

		bool Reconnect(string reason)
		{
            OnMessage?.Invoke($"USB warning: Couldn't communicate with the console ({reason}). Resetting the connection...");
			Thread.Sleep(3000);
			if (device.TryReconnect())
			{
				(reader, writer) = device.Open();
				return true;
			}
			return false;
		}

        public Task ConnectAsync(CancellationToken token)
        {
            Token = token;
			return Task.Run(WaitForConnection, token);
        }

        void WaitForConnection()
		{
			bool printedTimeoutWarningOnce = false;
			bool connected = true;
			while (!Token.IsCancellationRequested)
			{
                //using var trace = BeginTrace();
#if DEBUG
                OnMessage?.Invoke($"Sending USB connection request {BitConverter.ToString(RequestMagic)}");
#endif
				LibUsbDotNet.Error err = LibUsbDotNet.Error.Success;
				try
				{
					if (!connected)
						err = LibUsbDotNet.Error.NoDevice;
					else
						err = writer.Write(RequestMagic, 1000, out int _);
				}
				catch (Exception e)
				{
					connected = Reconnect(e.Message);
					continue;
				}

				if (err != LibUsbDotNet.Error.Success)
				{
					if (err != LibUsbDotNet.Error.Timeout)
					{
						// We probably need reconnecting
						connected = Reconnect(err.ToString());
					}
					else if (!printedTimeoutWarningOnce)
					{
						printedTimeoutWarningOnce = true;
                        OnMessage?.Invoke($"USB warning: Couldn't communicate with the console ({err}). Try entering a compatible game, unplugging your console or restarting it.");
					}

					if (!Token.IsCancellationRequested)
						Thread.Sleep(3000);

					continue;
				}

				return;
			}
		}

		public virtual void Flush()
		{
			if (Token.IsCancellationRequested)
				return;

			// Wait some time so the switch side timeouts
			Thread.Sleep(1500);

			// Then attempt to connect again
			WaitForConnection();
		}

		// Not all USB implementations buffer data in the OS side,
		// to support libusb we manually define the backing buffer and read everything in one shot
		// At some point there was a different implementation for WinUSB since that does support buffering
		// But it makes little sense to keep them separate as it doesn't grant any performance improvement.
		private int ReadSize = 0;
		private byte[] ReadBuffer = new byte[PacketHeader.MaxTransferSize];

        public bool ReadHeader(byte[] buffer)
		{
			//using var trace = BeginTrace();

			var err = reader.Read(ReadBuffer, 0, PacketHeader.MaxTransferSize, 800, out ReadSize);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (DebugOptions.Current.Log)
                    OnMessage?.Invoke($"Warning: libusb error {err} while reading header");

				return false;
			}

			Buffer.BlockCopy(ReadBuffer, 0, buffer, 0, PacketHeader.StructLength);

			return true;
		}

		public bool ReadPayload(byte[] buffer, int length)
		{
			if (length > ReadSize - PacketHeader.StructLength)
				return false;

			Buffer.BlockCopy(ReadBuffer, PacketHeader.StructLength, buffer, 0, length);

			return true;
		}
    }
}
