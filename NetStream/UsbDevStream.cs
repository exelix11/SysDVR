using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using libusbK;

namespace NetStream
{
	class UsbDevice 
	{
		private KLST_DEVINFO_HANDLE handle;
		private UsbK device;

		public UsbDevice(KLST_DEVINFO_HANDLE dev)
		{
			handle = dev;
			device = new UsbK(handle);
		}

		public UsbDevStream OpenStreamDefault() => OpenStream(0x81, 0x1);
		public UsbDevStream OpenStreamAlt() => OpenStream(0x82, 0x2);
		public UsbDevStream OpenStream(byte readPipe, byte writePipe) => new UsbDevStream(device, readPipe, writePipe);
	}

	class UsbDevStream : Stream
	{
		private UsbK device;

		private byte ReadPipe;
		private byte WritePipe;

		public UsbDevStream(UsbK dev, byte readPipe, byte writePipe)
		{
			device = dev;
			WritePipe = writePipe;
			ReadPipe = readPipe;
			device.ResetPipe(WritePipe);
			device.ResetPipe(ReadPipe);
		}

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => true;
		public override long Length => throw new NotImplementedException();
		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int Read(byte[] buffer, int offset, int count)
		{
			device.ReadPipe(ReadPipe, Marshal.UnsafeAddrOfPinnedArrayElement(buffer,offset), count, out int outLen, IntPtr.Zero);
			return outLen;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			device.WritePipe(WritePipe, Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset), count, out int outLen, IntPtr.Zero);
			if (outLen != count)
				throw new Exception("Writing to the device failed");
		}

		public override void Flush()
		{
			device.FlushPipe(WritePipe);
			device.FlushPipe(ReadPipe);
		}
	}
}
