#region License
/* SDL2# - C# Wrapper for SDL2
 *
 * Copyright (c) 2013-2021 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
#endregion

namespace SDL2
{
	public static class SDL_gfx
	{
		#region SDL2# Variables

		/* Used by DllImport to load the native library. */
		private const string nativeLibName = "SDL2_gfx";

		#endregion
		
		public const double M_PI = 3.1415926535897932384626433832795;
		
		#region SDL2_gfxPrimitives.h
		
		public const uint SDL2_GFXPRIMITIVES_MAJOR = 1;
		public const uint SDL2_GFXPRIMITIVES_MINOR = 0;
		public const uint SDL2_GFXPRIMITIVES_MICRO = 1;

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pixelColor(IntPtr renderer, short x, short y, uint color);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pixelRGBA(IntPtr renderer, short x, short y, byte r, byte g, byte b, byte a);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int hlineColor(IntPtr renderer, short x1, short x2, short y, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int hlineRGBA(IntPtr renderer, short x1, short x2, short y, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int vlineColor(IntPtr renderer, short x, short y1, short y2, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int vlineRGBA(IntPtr renderer, short x, short y1, short y2, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rectangleColor(IntPtr renderer, short x1, short y1, short x2, short y2, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rectangleRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, byte r, byte g, byte b, byte a);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int roundedRectangleColor(IntPtr renderer, short x1, short y1, short x2, short y2, short rad, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int roundedRectangleRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, short rad, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int boxColor(IntPtr renderer, short x1, short y1, short x2, short y2, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int boxRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int roundedBoxColor(IntPtr renderer, short x1, short y1, short x2, short y2, short rad, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int roundedBoxRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, short rad, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int lineColor(IntPtr renderer, short x1, short y1, short x2, short y2, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int lineRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aalineColor(IntPtr renderer, short x1, short y1, short x2, short y2, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aalineRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int thickLineColor(IntPtr renderer, short x1, short y1, short x2, short y2, byte width, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int thickLineRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, byte width, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int circleColor(IntPtr renderer, short x, short y, short rad, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int circleRGBA(IntPtr renderer, short x, short y, short rad, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int arcColor(IntPtr renderer, short x, short y, short rad, short start, short end, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int arcRGBA(IntPtr renderer, short x, short y, short rad, short start, short end, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aacircleColor(IntPtr renderer, short x, short y, short rad, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aacircleRGBA(IntPtr renderer, short x, short y, short rad, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledCircleColor(IntPtr renderer, short x, short y, short rad, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledCircleRGBA(IntPtr renderer, short x, short y, short rad, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int ellipseColor(IntPtr renderer, short x, short y, short rx, short ry, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int ellipseRGBA(IntPtr renderer, short x, short y, short rx, short ry, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aaellipseColor(IntPtr renderer, short x, short y, short rx, short ry, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aaellipseRGBA(IntPtr renderer, short x, short y, short rx, short ry, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledEllipseColor(IntPtr renderer, short x, short y, short rx, short ry, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledEllipseRGBA(IntPtr renderer, short x, short y, short rx, short ry, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pieColor(IntPtr renderer, short x, short y, short rad, short start, short end, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pieRGBA(IntPtr renderer, short x, short y, short rad, short start, short end, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledPieColor(IntPtr renderer, short x, short y, short rad, short start, short end, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledPieRGBA(IntPtr renderer, short x, short y, short rad, short start, short end, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int trigonColor(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int trigonRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aatrigonColor(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aatrigonRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledTrigonColor(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledTrigonRGBA(IntPtr renderer, short x1, short y1, short x2, short y2, short x3, short y3, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int polygonColor(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int polygonRGBA(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aapolygonColor(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int aapolygonRGBA(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledPolygonColor(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int filledPolygonRGBA(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int texturedPolygon(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, IntPtr texture, int texture_dx, int texture_dy);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int bezierColor(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, int s, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int bezierRGBA(IntPtr renderer, [In] short[] vx, [In] short[] vy, int n, int s, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gfxPrimitivesSetFont([In] byte[] fontdata, uint cw, uint ch);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gfxPrimitivesSetFontRotation(uint rotation);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int characterColor(IntPtr renderer, short x, short y, char c, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int characterRGBA(IntPtr renderer, short x, short y, char c, byte r, byte g, byte b, byte a);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int stringColor(IntPtr renderer, short x, short y, string s, uint color);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int stringRGBA(IntPtr renderer, short x, short y, string s, byte r, byte g, byte b, byte a);

		#endregion

		#region SDL2_rotozoom.h

		public const int SMOOTHING_OFF = 0;
		public const int SMOOTHING_ON = 1;
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr rotozoomSurface(IntPtr src, double angle, double zoom, int smooth);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr rotozoomSurfaceXY(IntPtr src, double angle, double zoomx, double zoomy, int smooth);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void rotozoomSurfaceSize(int width, int height, double angle, double zoom, out int dstwidth, out int dstheight);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void rotozoomSurfaceSizeXY(int width, int height, double angle, double zoomx, double zoomy, out int dstwidth, out int dstheight);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr zoomSurface(IntPtr src, double zoomx, double zoomy, int smooth);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void zoomSurfaceSize(int width, int height, double zoomx, double zoomy, out int dstwidth, out int dstheight);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr shrinkSurface(IntPtr src, int factorx, int factory);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr rotateSurface90Degrees(IntPtr src, int numClockwiseTurns);

		#endregion

		#region SDL2_framerate.h

		public const int FPS_UPPER_LIMIT = 200;
		public const int FPS_LOWER_LIMIT = 1;
		public const int FPS_DEFAULT = 30;
		
		[StructLayout(LayoutKind.Sequential)]
		public struct FPSmanager
		{
			public uint framecount;
			public float rateticks;
			public uint baseticks;
			public uint lastticks;
			public uint rate;
		}
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDL_initFramerate(ref FPSmanager manager);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_setFramerate(ref FPSmanager manager, uint rate);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_getFramerate(ref FPSmanager manager);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_getFramecount(ref FPSmanager manager);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint SDL_framerateDelay(ref FPSmanager manager);

		#endregion

		#region SDL2_imageFilter.h

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMMXdetect();
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDL_imageFilterMMXoff();
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDL_imageFilterMMXon();

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterAdd([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMean([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterSub([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterAbsDiff([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMult([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMultNor([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMultDivby2([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMultDivby4([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterBitAnd([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterBitOr([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterDiv([In] byte[] src1, [In] byte[] src2, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterBitNegation([In] byte[] src1, [Out] byte[] dest, uint length);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterAddByte([In] byte[] src1, [Out] byte[] dest, uint length, byte c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterAddUint([In] byte[] src1, [Out] byte[] dest, uint length, uint c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterAddByteToHalf([In] byte[] src1, [Out] byte[] dest, uint length, byte c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterSubByte([In] byte[] src1, [Out] byte[] dest, uint length, byte c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterSubUint([In] byte[] src1, [Out] byte[] dest, uint length, uint c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftRight([In] byte[] src1, [Out] byte[] dest, uint length, byte n);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftRightUint([In] byte[] src1, [Out] byte[] dest, uint length, byte n);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterMultByByte([In] byte[] src1, [Out] byte[] dest, uint length, byte c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftRightAndMultByByte([In] byte[] src1, [Out] byte[] dest, uint length, byte n, byte c);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftLeftByte([In] byte[] src1, [Out] byte[] dest, uint length, byte n);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftLeftUint([In] byte[] src1, [Out] byte[] dest, uint length, byte n);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterShiftLeft([In] byte[] src1, [Out] byte[] dest, uint length, byte n);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterBinarizeUsingThreshold([In] byte[] src1, [Out] byte[] dest, uint length, byte t);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterClipToRange([In] byte[] src1, [Out] byte[] dest, uint length, byte tmin, byte tmax);
		
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_imageFilterNormalizeLinear([In] byte[] src1, [Out] byte[] dest, uint length, int cmin, int cmax, int nmin, int nmax);

		#endregion
	}
}
