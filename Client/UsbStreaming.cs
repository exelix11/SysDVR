using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SysDVRClient
{
	static class UsbHelper
	{
		public static bool ForceLibUsb { get; set; } = false;

		static UsbContext LibUsbCtx = null;
		static IUsbDevice device = null;
		static int refCount = 0;

		public static LibUsbDotNet.LogLevel LogLevel = LibUsbDotNet.LogLevel.Error;

		private static IUsbDevice GetSysDVRDevice()
		{
			if (device != null)
				return device;

			LibUsbCtx = new UsbContext();
			LibUsbCtx.SetDebugLevel(LogLevel);
			var usbDeviceCollection = LibUsbCtx.List();
			var dev = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3006 && d.VendorId == 0x057e);

			if (dev == null)
				throw new Exception("Device not found");

			dev.Open();

			device = dev;
			return dev;
		}

		public static (UsbEndpointReader, UsbEndpointWriter) GetForInterface(StreamKind iface)
		{
			var dev = GetSysDVRDevice();

			lock (device)
			{
				if (!dev.ClaimInterface(iface == StreamKind.Video ? 0 : 1))
					throw new Exception($"Couldn't claim interface for {iface}");

				var (epIn, epOut) = iface == StreamKind.Video ? (ReadEndpointID.Ep01, WriteEndpointID.Ep01) : (ReadEndpointID.Ep02, WriteEndpointID.Ep02);

				var reader = dev.OpenEndpointReader(epIn, PacketHeader.MaxTransferSize, EndpointType.Bulk);
				var writer = dev.OpenEndpointWriter(epOut, EndpointType.Interrupt);
				refCount++;

				return (reader, writer);
			}
		}

		public static void MarkInterfaceClosed()
		{
			lock (device)
			{
				refCount--;
				if (refCount == 0)
					device.Close();
				else if (refCount < 0)
					throw new Exception("interface refCount out of range");
			}
		}

		public static UsbStreamingSourceBase MakeStreamingSource(StreamKind stream)
		{
			// LibUsb backend on linux and windows doesn't seem to buffer reads from the pipe, this causes issues and requires manually receiving everything and copying data
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !ForceLibUsb)
				return new UsbStreamingSourceWinUsb(stream);
			else 
				return new UsbStreamingSourceLibUsb(stream);
		}
	}

	abstract class UsbStreamingSourceBase : StreamingSource
	{
		protected static readonly byte[] USBMagic = { 0xAA, 0xAA, 0xAA, 0xAA };
		public bool Logging { get; set; }

		protected UsbEndpointReader reader;
		protected UsbEndpointWriter writer;

		public UsbStreamingSourceBase(StreamKind kind)
		{
			(reader, writer) = UsbHelper.GetForInterface(kind);
		}

		public void WaitForConnection()
		{

		}

		public void StopStreaming()
		{
			UsbHelper.MarkInterfaceClosed();
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

		public UsbStreamingSourceWinUsb(StreamKind kind) : base(kind) { }

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
					Console.WriteLine($"Warning: libusb error {err} while requesting data");
				return false;
			}

			err = reader.Read(buffer, 0, PacketHeader.StructLength, 100, out int _);
			if (err != LibUsbDotNet.Error.Success)
			{
				if (Logging)
					Console.WriteLine($"Warning: libusb error {err} while reading header");
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
					Console.WriteLine($"Warning: libusb error {err} while reading payload. ({sz} read)");
				return false;
			}

			return sz == length;
		}
	}

	class UsbStreamingSourceLibUsb : UsbStreamingSourceBase
	{
		public UsbStreamingSourceLibUsb(StreamKind kind) : base(kind) { }

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

	class UsbStreamManager : RTSP.RTSPStreamManager
	{
		public UsbStreamManager(bool hasVideo, bool hasAudio, int port) : base(hasVideo, hasAudio, false, port)
		{
			if (hasVideo)
				VideoThread = new StreamingThread(Video, StreamKind.Video, UsbHelper.MakeStreamingSource(StreamKind.Video));
			if (hasAudio)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, UsbHelper.MakeStreamingSource(StreamKind.Audio));
		}
	}

	class LegacyUsbStreamManager : IMutliStreamManager
	{
		public IOutTarget Video { get; set; }
		public IOutTarget Audio { get; set; }

		protected StreamingThread VideoThread, AudioThread;

		public LegacyUsbStreamManager(IOutTarget video, IOutTarget audio)
		{
			Video = video;
			Audio = audio;

			if (Video != null)
				VideoThread = new StreamingThread(Video, StreamKind.Video, UsbHelper.MakeStreamingSource(StreamKind.Video));
			if (Audio != null)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, UsbHelper.MakeStreamingSource(StreamKind.Audio));
		}

		public void Begin()
		{
			VideoThread?.Start();
			AudioThread?.Start();
		}

		public void Stop()
		{
			VideoThread?.Stop();
			AudioThread?.Stop();
		}
	}
}
