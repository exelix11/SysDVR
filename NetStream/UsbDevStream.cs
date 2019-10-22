using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace UsbStream
{
	public class UsbDevice 
	{
		private IUsbDevice dev;

		public UsbDevice(IUsbDevice device)
		{
			dev = device;
			device.ClaimInterface(device.Configs[0].Interfaces[0].Number);
		}

		public UsbDevStream OpenStreamDefault() => OpenStream(WriteEndpointID.Ep01, ReadEndpointID.Ep01);
		public UsbDevStream OpenStreamAlt() => OpenStream(WriteEndpointID.Ep02, ReadEndpointID.Ep02);
		public UsbDevStream OpenStream(WriteEndpointID WriteEp, ReadEndpointID ReadEp) => new UsbDevStream(dev, WriteEp, ReadEp);
	}

	public class UsbDevStream : Stream
	{
		private IUsbDevice device;

		private ReadEndpointID ReadPipe;
		private WriteEndpointID WritePipe;

		private UsbEndpointWriter writer;
		private UsbEndpointReader reader;

		public int MillisTimeout = 100000;

		public UsbDevStream(IUsbDevice dev, WriteEndpointID writePipe, ReadEndpointID readPipe)
		{
			device = dev;
			WritePipe = writePipe;
			ReadPipe = readPipe;

			writer = device.OpenEndpointWriter(writePipe);
			reader = device.OpenEndpointReader(readPipe);

			Flush();
		}

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => true;
		public override long Length => throw new NotImplementedException();
		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int Read(byte[] buffer, int offset, int count)
		{
			reader.Read(buffer, offset, count, MillisTimeout, out int read);
			return read;
		}

		public override long Seek(long offset, SeekOrigin origin) =>
			throw new NotImplementedException();

		public override void SetLength(long value) => 
			throw new NotImplementedException();

		public int WriteWResult(byte[] buffer) => WriteWResult(buffer, 0, buffer.Length);

		public int WriteWResult(byte[] buffer, int offset, int count)
		{
			writer.Write(buffer, offset, count, MillisTimeout, out int written);
			return written;
		}

		public override void Write(byte[] buffer, int offset, int count) => WriteWResult(buffer, offset, count);

		public override void Flush()
		{
			reader.ReadFlush();
		}
	}
}
