using SysDVR.Client.Core;
using SysDVR.Client.ThirdParty.Openh264;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SysDVR.Client.ThirdParty.Openh264.OpenH264Native;

namespace SysDVR.Client.Targets.Decoding
{
	public class PlanarYUVFrame : IDisposable
	{
		public struct Buffer 
		{
			public byte[] Data;
			public IntPtr Pinned;
			public GCHandle Handle;
		}

		readonly public int Width, Height;
		readonly public Buffer Y;
		readonly public Buffer U;
		readonly public Buffer V;

		// These point into the pinned buffers above ad the exact start of the Y, U, V planes
		public IntPtr YDecoded, UDecoded, VDecoded;
		public int YLineSize, ULineSize, VLineSize;

		static Buffer MakeBuffer(int size) 
		{
			var data = new byte[size];
			var pinned = GCHandle.Alloc(data, GCHandleType.Pinned);
			return new Buffer { 
				Data = data, 
				Pinned = pinned.AddrOfPinnedObject(), 
				Handle = pinned 
			};
		}

		public PlanarYUVFrame()
		{
			Width = StreamInfo.VideoWidth;
			Height = StreamInfo.VideoHeight;

			Y = MakeBuffer(Width * Height);
			U = MakeBuffer(Width * Height / 4);
			V = MakeBuffer(Width * Height / 4);
		}

		public void Dispose()
		{
			Y.Handle.Free();
			U.Handle.Free();
			V.Handle.Free();
		}
	}
}
