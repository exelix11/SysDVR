using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace NetStream
{
	class UsbDevice 
	{
		private IUsbDevice dev;

		public UsbDevice(IUsbDevice device)
		{
			dev = device;
		}

		public UsbDevStream OpenStreamDefault() => OpenStream(WriteEndpointID.Ep01, ReadEndpointID.Ep01);
		public UsbDevStream OpenStreamAlt() => OpenStream(WriteEndpointID.Ep02, ReadEndpointID.Ep02);
		public UsbDevStream OpenStream(WriteEndpointID WriteEp, ReadEndpointID ReadEp) => new UsbDevStream(dev, WriteEp, ReadEp);
	}

	class UsbDevStream : Stream
	{
		private IUsbDevice device;

		private ReadEndpointID ReadPipe;
		private WriteEndpointID WritePipe;

		private UsbEndpointWriter writer;
		private UsbEndpointReader reader;

		public int  MillisTimeout = 300;

		public unsafe UsbDevStream(IUsbDevice dev, WriteEndpointID writePipe, ReadEndpointID readPipe)
		{
			device = dev;
			WritePipe = writePipe;
			ReadPipe = readPipe;

			device.ClaimInterface(device.Configs[0].Interfaces[(int)WritePipe - 1].Number);

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

		public override void Write(byte[] buffer, int offset, int count)
		{
			writer.Write(buffer, offset, count, MillisTimeout, out int written);
			if (written != count)
			{
				Console.WriteLine("Warning: writing to the device failed");
				Flush();
				System.Threading.Thread.Sleep(1000);
			}
		}

		public override void Flush()
		{
			reader.ReadFlush();
		}
	}
}
