using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace SysDVR.Client.Sources
{
	// Making UsbContext non static will remove requirement on libusb, it will be loaded only if actually using USB, this is useful on architectures where it's not supported
	class UsbContext : IDisposable
	{
		static object LockObject = new object();

		public enum LogLevel 
		{
			Error,
			Warning,
			Debug
		}

		public static bool ForceLibUsb { get; set; } = false;
		public static LogLevel Logging { get; set; } = LogLevel.Error;
		int deviceReferences = 0;

		static UsbContext instance = null;
		public static UsbContext GetInstance() 
		{
			lock (LockObject)
			{
				if (instance is null)
					instance = new UsbContext();

				return instance;
			}
		}

		readonly LibUsbDotNet.LibUsb.UsbContext LibUsbCtx = null;
		public readonly IUsbDevice device = null;
		
		private UsbContext() 
		{
			LibUsbCtx = new LibUsbDotNet.LibUsb.UsbContext();
			
			LibUsbCtx.SetDebugLevel(Logging switch
			{
				LogLevel.Error => LibUsbDotNet.LogLevel.Error,
				LogLevel.Warning => LibUsbDotNet.LogLevel.Info,
				LogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
				_ => throw new NotImplementedException(),
			});
			
			var usbDeviceCollection = LibUsbCtx.List();
			var dev = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3006 && d.VendorId == 0x057e);

			if (dev == null)
				throw new Exception("Device not found");

			dev.Open();

			device = dev;
		}

		public (UsbEndpointReader, UsbEndpointWriter) GetForInterface(StreamKind iface)
		{
			lock (LockObject)
			{
				var dev = device;

				if (!dev.ClaimInterface(iface == StreamKind.Video ? 0 : 1))
					throw new Exception($"Couldn't claim interface for {iface}");

				var (epIn, epOut) = iface == StreamKind.Video ? (ReadEndpointID.Ep01, WriteEndpointID.Ep01) : (ReadEndpointID.Ep02, WriteEndpointID.Ep02);

				var reader = dev.OpenEndpointReader(epIn, PacketHeader.MaxTransferSize, EndpointType.Bulk);
				var writer = dev.OpenEndpointWriter(epOut, EndpointType.Interrupt);
				Interlocked.Increment(ref deviceReferences);

				return (reader, writer);
			}
		}

		public void MarkInterfaceClosed()
		{
			lock (LockObject)
			{
				Interlocked.Decrement(ref deviceReferences);
				if (deviceReferences == 0)
					Dispose();
				else if (deviceReferences < 0)
					throw new Exception("interface refCount out of range");
			}
		}

		public UsbStreamingSourceBase MakeStreamingSource(StreamKind stream)
		{
			// LibUsb backend on linux and windows doesn't seem to buffer reads from the pipe, this causes issues and requires manually receiving everything and copying data
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !ForceLibUsb)
				return new UsbStreamingSourceWinUsb(stream, this);
			else 
				return new UsbStreamingSourceLibUsb(stream, this);
		}

		private bool disposedValue;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					device.Close();
					LibUsbCtx.Dispose();
				}

				instance = null;
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

	abstract class UsbStreamingSourceBase : IStreamingSource
	{
		protected static readonly byte[] USBMagic = { 0xAA, 0xAA, 0xAA, 0xAA };
		public bool Logging { get; set; }

		protected UsbEndpointReader reader;
		protected UsbEndpointWriter writer;
		protected UsbContext context;

		public UsbStreamingSourceBase(StreamKind kind, UsbContext context)
		{
			this.context = context;
			(reader, writer) = context.GetForInterface(kind);
		}

		public void WaitForConnection()
		{

		}

		public void StopStreaming()
		{
			context.MarkInterfaceClosed();
		}

		public void UseCancellationToken(CancellationToken tok)
		{

		}

		public abstract void Flush();
		public abstract bool ReadHeader(byte[] buffer);
		public abstract bool ReadPayload(byte[] buffer, int length);
	}

	class UsbStreamingSourceWinUsb : UsbStreamingSourceBase
	{
		public UsbStreamingSourceWinUsb(StreamKind kind, UsbContext context) : base(kind, context) { }

		public override void Flush()
		{
			reader.ReadFlush();
		}

		public override bool ReadHeader(byte[] buffer)
		{
			var err = writer.Write(USBMagic, 100, out int _);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: winusb error {err} while requesting data");
				return false;
			}

			err = reader.Read(buffer, 0, PacketHeader.StructLength, 100, out int _);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: winusb error {err} while reading header");
				return false;
			}

			return true;
		}

		public override bool ReadPayload(byte[] buffer, int length)
		{
			var err = reader.Read(buffer, 0, length, 500, out int sz);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: winusb error {err} while reading payload. ({sz} read)");
				return false;
			}

			return sz == length;
		}
	}

	class UsbStreamingSourceLibUsb : UsbStreamingSourceBase
	{
		public UsbStreamingSourceLibUsb(StreamKind kind, UsbContext context) : base(kind, context) { }

		public override void Flush()
		{
			reader.ReadFlush();
			ReadSize = 0;
		}
		
		//Incredibly dumb libusb workaround
		private byte[] ReadBuffer = new byte[PacketHeader.MaxTransferSize];
		private int ReadSize = 0;
		public override bool ReadHeader(byte[] buffer)
		{
			var err = writer.Write(USBMagic, 500, out int _);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: libusb error {err} while requesting data");
				return false;
			}

			err = reader.Read(ReadBuffer, 0, PacketHeader.MaxTransferSize, 600, out ReadSize);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: libusb error {err} while reading header");
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
	}
}
