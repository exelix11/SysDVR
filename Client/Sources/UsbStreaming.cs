using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SysDVR.Client.Sources
{
	// Making UsbContext non static will remove requirement on libusb, it will be loaded only if actually using USB, this is useful on architectures where it's not supported
	// This class should be used as a singleton
	class UsbContext : IDisposable
	{
		public class SysDvrDevice : IDisposable
		{
			readonly UsbContext Context;
			public readonly string Serial;
			public IUsbDevice DeviceHandle { get; private set; }

			public SysDvrDevice(UsbContext ctx, string serial, IUsbDevice dev)
			{
				Context = ctx;
				Serial = serial;
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
				var dev = Context.FindSysdvrDevices().Where(X => X.Serial == Serial).FirstOrDefault();
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

        public enum LogLevel 
		{
			Error,
			Warning,
			Debug,
			None
		}

		readonly LibUsbDotNet.LibUsb.UsbContext LibUsbCtx = null;
		static bool Initialized = false;

		private LogLevel _debugLevel;
		public LogLevel DebugLevel {
			set {
				_debugLevel = value;
				LibUsbCtx.SetDebugLevel(value switch
				{
					LogLevel.Error => LibUsbDotNet.LogLevel.Error,
					LogLevel.Warning => LibUsbDotNet.LogLevel.Info,
					LogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
					_ => LibUsbDotNet.LogLevel.None,
				});
			}
			get => _debugLevel;
		}
				
		public UsbContext(LogLevel logLevel = LogLevel.Error) 
		{
			if (Initialized)
				throw new Exception("UsbContext can only be initialized once");

			Initialized = true;
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

		public IReadOnlyList<SysDvrDevice> FindSysdvrDevices()
		{
			// THis is hacky but libusb can't seem to get the device serial without opening it first
			// If the device is already opened by another instance of sysdvr it will print an error, suppress it by temporarily changing the log level 
			var old = DebugLevel;
			DebugLevel = LogLevel.None;

			var res = LibUsbCtx.List().Where(MatchSysdvrDevice).Select(x =>
			{
				try
				{
					if (!x.TryOpen())
						return null;

					var serial = x.Info.SerialNumber.ToLower().Trim();
					x.Close();

					if (!serial.StartsWith("sysdvr:"))
						return null;

					return new SysDvrDevice(this, serial[7..], x);
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

		private bool disposedValue;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					LibUsbCtx.Dispose();
				}

				Initialized = false;
				disposedValue = true;
			}
		}

		~UsbContext()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	class UsbStreamingSource : IStreamingSource
	{
		protected static readonly byte[] MagicRequestVideo = { 0xBB, 0xBB, 0xBB, 0xBB };
		protected static readonly byte[] MagicRequestAudio = { 0xCC, 0xCC, 0xCC, 0xCC };
		protected static readonly byte[] MagicRequestBoth = { 0xAA, 0xAA, 0xAA, 0xAA };

		public DebugOptions Logging { get; set; }
		CancellationToken Token; 

		// TODO: Remove tracing code
		readonly private TimeTrace tracer = new();
		
		protected TimeTrace BeginTrace(string extra = "", [CallerMemberName] string funcName = null) => 
			tracer.Begin("usb", extra, funcName);

		readonly UsbContext.SysDvrDevice device;
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

        public StreamKind SourceKind => (HasVideo, HasAudio) switch
        {
            (true, true) => StreamKind.Both,
            (true, false) => StreamKind.Video,
            (false, true) => StreamKind.Audio,
            _ => throw new Exception("Invalid state")
        };

        public UsbStreamingSource(UsbContext.SysDvrDevice device, bool hasVideo, bool hasAudio)
		{
			this.device = device;
			(reader, writer) = device.Open();

			HasVideo = hasVideo;
			HasAudio = hasAudio;
		}

		public void StopStreaming()
		{
			device.Close();
			device.Dispose();
		}

		void Reconnect(string reason) 
		{
            Console.WriteLine($"USB warning: Couldn't communicate with the console ({reason}). Resetting the connection...");
			Thread.Sleep(3000);
            if (device.TryReconnect())
            {
                (reader, writer) = device.Open();
            }
        }

		public void WaitForConnection() 
		{
			bool printedTimeoutWarningOnce = false;
            while (!Token.IsCancellationRequested) 
			{
				//using var trace = BeginTrace();
#if DEBUG
				Console.WriteLine($"Sending USB connection request {BitConverter.ToString(RequestMagic)}");
#endif
				LibUsbDotNet.Error err = LibUsbDotNet.Error.Success;
				try 
				{
					err = writer.Write(RequestMagic, 1000, out int _);
                }
				catch (Exception e)
				{
					Reconnect(e.Message);
					continue;
				}

				if (err != LibUsbDotNet.Error.Success)
				{
					if (err != LibUsbDotNet.Error.Timeout)
					{
						// We probably need reconnecting
						Reconnect(err.ToString());
                    }
                    else if (!printedTimeoutWarningOnce)
					{
						printedTimeoutWarningOnce = true;
                        Console.WriteLine($"USB warning: Couldn't communicate with the console ({err}). Try entering a compatible game, unplugging your console or restarting it.");
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
                if (Logging.Log)
                    Console.WriteLine($"Warning: libusb error {err} while reading header");
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

        public void UseCancellationToken(CancellationToken tok)
		{
			Token = tok;
		}
	}
}
