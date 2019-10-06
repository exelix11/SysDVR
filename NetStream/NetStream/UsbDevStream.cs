using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using libusbK;

namespace NetStream
{
	class UsbDevStream : Stream
	{
		private KLST_DEVINFO_HANDLE handle;
		private UsbK device;

		private const byte ReadPipe = 0x81;
		private const byte WritePipe = 0x1;

		public UsbDevStream(KLST_DEVINFO_HANDLE dev)
		{
			handle = dev;
			device = new UsbK(handle);
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
				Debugger.Break();
		}

		public override void Flush()
		{
			device.FlushPipe(WritePipe);
			device.FlushPipe(ReadPipe);
		}
	}
}
