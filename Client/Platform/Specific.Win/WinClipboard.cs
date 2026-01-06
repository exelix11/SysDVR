using SysDVR.Client.Targets;
using System;
using System.Runtime.InteropServices;

namespace SysDVR.Client.Platform.Specific.Win
{
	internal class WinClipboard
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool CloseClipboard();

		[DllImport("user32.dll")]
		static extern IntPtr SetClipboardData(uint uFormat, HGLOBAL hMem);

		const int CF_DIB = 8;

		[StructLayout(LayoutKind.Sequential)]
		struct BITMAPINFOHEADER
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}

        [StructLayout(LayoutKind.Sequential, Size = 4)]
        struct RGBQUAD
		{
			public byte rgbBlue;
			public byte rgbGreen;
			public byte rgbRed;
			public byte rgbReserved;
		}

		const int GMEM_MOVEABLE = 0x0002;

#pragma warning disable CS0649
		struct HGLOBAL { public IntPtr Value; }
#pragma warning restore CS0649

		[DllImport("kernel32.dll")]
		static extern HGLOBAL GlobalAlloc(uint uFlags, nint dwBytes);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(HGLOBAL hMem);

		[DllImport("kernel32.dll")]
		static extern bool GlobalUnlock(HGLOBAL hMem);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalFree(HGLOBAL hMem);

		static unsafe HGLOBAL CaptureToBitmapInfo(SDLCapture cap)
		{
			var byteLen = Marshal.SizeOf<BITMAPINFOHEADER>() +
				Marshal.SizeOf<RGBQUAD>() * cap.Width * cap.Height;

			var alloc = GlobalAlloc(GMEM_MOVEABLE, byteLen);
			var buf = GlobalLock(alloc);

			var header = (BITMAPINFOHEADER*)buf.ToPointer();
			
			*header = new BITMAPINFOHEADER()
			{
				biSize = Marshal.SizeOf<BITMAPINFOHEADER>(),
				biWidth = cap.Width,
				biHeight = -cap.Height,
				biPlanes = 1,
				biBitCount = 32,
				biCompression = 0, // BI_RGB
				biSizeImage = 0
			};

			RGBQUAD* pixels = (RGBQUAD*)cap.surface->pixels;
			
			RGBQUAD* body = (RGBQUAD*)(buf + header->biSize);
			for (int i = 0; i < cap.Width * cap.Height; i++)
				body[i] = pixels[i];

			GlobalUnlock(alloc);

			return alloc;
		}

		static bool BitmapinfoToClipboard(HGLOBAL bitmap) 
		{
			Program.SdlCtx.BugCheckThreadId();

			var win = GetActiveWindow();
			if (win == 0)
			{
				Program.DebugLog("Failed to get the current active window");
				return false;
			}

			if (!OpenClipboard(win))
			{
				Program.DebugLog("Failed to open the clipboard");
				return false;
			}

			var set = SetClipboardData(CF_DIB, bitmap);
			
			CloseClipboard();

			if (set == IntPtr.Zero)
			{
				Program.DebugLog("Failed to set clipboard data");
				return false;
			}

			return true;
		}

		public static bool CopyCapture(SDLCapture cap)
		{
			var img = CaptureToBitmapInfo(cap);
			if (!BitmapinfoToClipboard(img))
			{
				// If setting the clipboard fails we still have ownership of img and must free it 
				GlobalFree(img);
				return false;
			}

			return true;
		}
	}
}
