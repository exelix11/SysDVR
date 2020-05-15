using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SysDVRClient
{
	static class UsbHelper
	{
		static UsbContext LibUsbCtx = null;
		static IUsbDevice device = null;
		static int refCount = 0;

		public static LibUsbDotNet.LogLevel LogLevel = LibUsbDotNet.LogLevel.Error;

		static IUsbDevice GetSysDVRDevice()
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
	}

	class UsbStreamingSource : StreamingSource
	{
		public bool Logging { get; set; }

		UsbEndpointReader reader;
		UsbEndpointWriter writer;

		public UsbStreamingSource(StreamKind kind)
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

		public void Flush()
		{
			reader.ReadFlush();
			ReadSize = 0;
		}

		static readonly byte[] magic = { 0xAA, 0xAA, 0xAA, 0xAA };

		//Incredibly dumb libusb workaround
		private byte[] ReadBuffer = new byte[PacketHeader.MaxTransferSize];
		private int ReadSize = 0;
		public bool ReadHeader(byte[] buffer)
		{
			var err = writer.Write(magic, 500, out int _);
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

		public bool ReadPayload(byte[] buffer, int length)
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
				VideoThread = new StreamingThread(Video, StreamKind.Video, new UsbStreamingSource(StreamKind.Video));
			if (hasAudio)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, new UsbStreamingSource(StreamKind.Audio));
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
				VideoThread = new StreamingThread(Video, StreamKind.Video, new UsbStreamingSource(StreamKind.Video));
			if (Audio != null)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, new UsbStreamingSource(StreamKind.Audio));
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
