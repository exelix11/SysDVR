using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
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
		
		public static LibUsbDotNet.LogLevel LogLevel = LibUsbDotNet.LogLevel.Error;
		
		public static IUsbDevice GetSysDVRDevice()
		{
			if (LibUsbCtx != null)
				throw new Exception("Libusb has already been initialized");
		
			LibUsbCtx = new UsbContext();
			LibUsbCtx.SetDebugLevel(LogLevel);
			var usbDeviceCollection = LibUsbCtx.List();
			var dev = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3006 && d.VendorId == 0x057e);

			if (dev == null)
				throw new Exception("Device not found");

			dev.Open();
			dev.ClaimInterface(dev.Configs[0].Interfaces[0].Number);

			return dev;
		}
	}

	class UsbStreamingSource : StreamingSource
	{
		IUsbDevice device;
		UsbEndpointReader reader;
		UsbEndpointWriter writer;
		CancellationToken Token;

		public UsbStreamingSource(IUsbDevice device, StreamKind kind)
		{
			this.device = device;
			reader = device.OpenEndpointReader(kind == StreamKind.Video ? ReadEndpointID.Ep01 : ReadEndpointID.Ep02);
			writer = device.OpenEndpointWriter(kind == StreamKind.Video ? WriteEndpointID.Ep01 : WriteEndpointID.Ep02);
		}

		public void WaitForConnection()
		{
			
		}

		public void StopStreaming()
		{
			//Reader can't be disposed (?)
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			Token = tok;
		}

		public void Flush()
		{
			reader.ReadFlush();
		}

		static readonly byte[] magic = { 0xAA, 0xAA, 0xAA, 0xAA };
		public void ReadHeader(byte[] buffer)
		{
			int received = 0;
			do
			{
				writer.Write(magic, 100, out int _).ThrowOnError();
				reader.Read(buffer, 0, PacketHeader.StructLength, 100, out received).ThrowOnError();
			} while (received != PacketHeader.StructLength && !Token.IsCancellationRequested);
		}

		public bool ReadPayload(byte[] buffer, int length) 
		{
			int received = 0;
			do {	
				reader.Read(buffer, 0, length, 100, out int sz);
				if (sz == 0)
					return false;
				received += sz;
			} while (received < length && !Token.IsCancellationRequested);
			return true;
		}
	}

	class UsbStreamManager : RTSPStreamManager
	{ 
		public UsbStreamManager(bool hasVideo, bool hasAudio, int port) : base(hasVideo, hasAudio, false, port)
		{
			var device = UsbHelper.GetSysDVRDevice();

			if (hasVideo)
				VideoThread = new StreamingThread(Video, StreamKind.Video, new UsbStreamingSource(device, StreamKind.Video));
			if (hasAudio)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, new UsbStreamingSource(device, StreamKind.Audio));
		}
	}

	class LegacyUsbStreamManager : IMutliStreamManager
	{
		public IOutTarget Video { get; set; }
		public IOutTarget Audio { get; set; }

		protected StreamingThread VideoThread, AudioThread;

		public LegacyUsbStreamManager(IOutTarget video, IOutTarget audio)
		{
			var device = UsbHelper.GetSysDVRDevice();

			Video = video;
			Audio = audio;

			if (Video != null)
				VideoThread = new StreamingThread(Video, StreamKind.Video, new UsbStreamingSource(device, StreamKind.Video));
			if (Audio != null)
				AudioThread = new StreamingThread(Audio, StreamKind.Audio, new UsbStreamingSource(device, StreamKind.Audio));
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
