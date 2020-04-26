using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
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
			var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x3006 && d.VendorId == 0x057e);
		
			if (selectedDevice == null)
				return null;
		
			selectedDevice.Open();
			selectedDevice.ClaimInterface(0);

			return selectedDevice;
		}
	}

	class UsbStreamingSource : StreamingSource
	{
		IUsbDevice device;
		UsbEndpointReader reader;

		public UsbStreamingSource(IUsbDevice device, StreamKind kind)
		{
			this.device = device;
			reader = device.OpenEndpointReader(kind == StreamKind.Video ? ReadEndpointID.Ep01 : ReadEndpointID.Ep02);
		}

		public int ReadBytes(byte[] buffer, int offset, int length)
		{			
			reader.Read(buffer, offset, length, 1000, out int transferLen).ThrowOnError();
			return transferLen;
		}

		public void WaitForConnection()
		{
			// If reader opened successfully we're connected
		}

		public void StopStreaming()
		{
			//Reader can't be disposed (?)
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			//Usb operations have timeouts so this is not needed
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
