#region License
/* SDL2# - C# Wrapper for SDL2
 *
 * Copyright (c) 2013-2020 Ethan Lee.
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
	public static class SDL_ttf
	{
		#region SDL2# Variables

		/* Used by DllImport to load the native library. */
		private const string nativeLibName = "SDL2_ttf";

		#endregion

		#region SDL_ttf.h

		/* Similar to the headers, this is the version we're expecting to be
		 * running with. You will likely want to check this somewhere in your
		 * program!
		 */
		public const int SDL_TTF_MAJOR_VERSION =	2;
		public const int SDL_TTF_MINOR_VERSION =	0;
		public const int SDL_TTF_PATCHLEVEL =		16;

		public const int UNICODE_BOM_NATIVE =	0xFEFF;
		public const int UNICODE_BOM_SWAPPED =	0xFFFE;

		public const int TTF_STYLE_NORMAL =		0x00;
		public const int TTF_STYLE_BOLD =		0x01;
		public const int TTF_STYLE_ITALIC =		0x02;
		public const int TTF_STYLE_UNDERLINE =		0x04;
		public const int TTF_STYLE_STRIKETHROUGH =	0x08;

		public const int TTF_HINTING_NORMAL =		0;
		public const int TTF_HINTING_LIGHT =		1;
		public const int TTF_HINTING_MONO =		2;
		public const int TTF_HINTING_NONE =		3;
		public const int TTF_HINTING_LIGHT_SUBPIXEL =	4; /* >= 2.0.16 */

		public static void SDL_TTF_VERSION(out SDL.SDL_version X)
		{
			X.major = SDL_TTF_MAJOR_VERSION;
			X.minor = SDL_TTF_MINOR_VERSION;
			X.patch = SDL_TTF_PATCHLEVEL;
		}

		[DllImport(nativeLibName, EntryPoint = "TTF_LinkedVersion", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr INTERNAL_TTF_LinkedVersion();
		public static SDL.SDL_version TTF_LinkedVersion()
		{
			SDL.SDL_version result;
			IntPtr result_ptr = INTERNAL_TTF_LinkedVersion();
			result = (SDL.SDL_version) Marshal.PtrToStructure(
				result_ptr,
				typeof(SDL.SDL_version)
			);
			return result;
		}

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_ByteSwappedUNICODE(int swapped);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_Init();

		/* IntPtr refers to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_OpenFont", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_OpenFont(
			byte* file,
			int ptsize
		);
		public static unsafe IntPtr TTF_OpenFont(string file, int ptsize)
		{
			byte* utf8File = SDL.Utf8Encode(file);
			IntPtr handle = INTERNAL_TTF_OpenFont(
				utf8File,
				ptsize
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return handle;
		}

		/* src refers to an SDL_RWops*, IntPtr to a TTF_Font* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_OpenFontRW(
			IntPtr src,
			int freesrc,
			int ptsize
		);

		/* IntPtr refers to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_OpenFontIndex", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_OpenFontIndex(
			byte* file,
			int ptsize,
			long index
		);
		public static unsafe IntPtr TTF_OpenFontIndex(
			string file,
			int ptsize,
			long index
		) {
			byte* utf8File = SDL.Utf8Encode(file);
			IntPtr handle = INTERNAL_TTF_OpenFontIndex(
				utf8File,
				ptsize,
				index
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return handle;
		}

		/* src refers to an SDL_RWops*, IntPtr to a TTF_Font* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_OpenFontIndexRW(
			IntPtr src,
			int freesrc,
			int ptsize,
			long index
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SetFontSize(
			IntPtr font,
			int ptsize
		);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontStyle(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_SetFontStyle(IntPtr font, int style);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontOutline(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_SetFontOutline(IntPtr font, int outline);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontHinting(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_SetFontHinting(IntPtr font, int hinting);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_FontHeight(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_FontAscent(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_FontDescent(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_FontLineSkip(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontKerning(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_SetFontKerning(IntPtr font, int allowed);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern long TTF_FontFaces(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_FontFaceIsFixedWidth(IntPtr font);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_FontFaceFamilyName", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr INTERNAL_TTF_FontFaceFamilyName(
			IntPtr font
		);
		public static string TTF_FontFaceFamilyName(IntPtr font)
		{
			return SDL.UTF8_ToManaged(
				INTERNAL_TTF_FontFaceFamilyName(font)
			);
		}

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_FontFaceStyleName", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr INTERNAL_TTF_FontFaceStyleName(
			IntPtr font
		);
		public static string TTF_FontFaceStyleName(IntPtr font)
		{
			return SDL.UTF8_ToManaged(
				INTERNAL_TTF_FontFaceStyleName(font)
			);
		}

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GlyphIsProvided(IntPtr font, ushort ch);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GlyphIsProvided32(IntPtr font, uint ch);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GlyphMetrics(
			IntPtr font,
			ushort ch,
			out int minx,
			out int maxx,
			out int miny,
			out int maxy,
			out int advance
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GlyphMetrics32(
			IntPtr font,
			uint ch,
			out int minx,
			out int maxx,
			out int miny,
			out int maxy,
			out int advance
		);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SizeText(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			out int w,
			out int h
		);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_SizeUTF8", CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe int INTERNAL_TTF_SizeUTF8(
			IntPtr font,
			byte* text,
			out int w,
			out int h
		);
		public static unsafe int TTF_SizeUTF8(
			IntPtr font,
			string text,
			out int w,
			out int h
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			int result = INTERNAL_TTF_SizeUTF8(
				font,
				utf8Text,
				out w,
				out h
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SizeUNICODE(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			out int w,
			out int h
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_MeasureText(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			int measure_width,
			out int extent,
			out int count
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, EntryPoint = "TTF_MeasureUTF8", CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe int INTERNAL_TTF_MeasureUTF8(
			IntPtr font,
			byte* text,
			int measure_width,
			out int extent,
			out int count
		);
		public static unsafe int TTF_MeasureUTF8(
			IntPtr font,
			string text,
			int measure_width,
			out int extent,
			out int count
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			int result = INTERNAL_TTF_MeasureUTF8(
				font,
				utf8Text,
				measure_width,
				out extent,
				out count
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_MeasureUNICODE(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			int measure_width,
			out int extent,
			out int count
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Solid(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Solid", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Solid(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg
		);
		public static unsafe IntPtr TTF_RenderUTF8_Solid(
			IntPtr font,
			string text,
			SDL.SDL_Color fg
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Solid(
				font,
				utf8Text,
				fg
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Solid(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Solid_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg,
			uint wrapLength
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Solid_Wrapped", CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Solid_Wrapped(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg,
			uint wrapLength
		);
		public static unsafe IntPtr TTF_RenderUTF8_Solid_Wrapped(
			IntPtr font,
			string text,
			SDL.SDL_Color fg,
			uint wrapLength
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Solid_Wrapped(
				font,
				utf8Text,
				fg,
				wrapLength
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Solid_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg,
			uint wrapLength
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph_Solid(
			IntPtr font,
			ushort ch,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph32_Solid(
			IntPtr font,
			uint ch,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Shaded(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Shaded", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Shaded(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		);
		public static unsafe IntPtr TTF_RenderUTF8_Shaded(
			IntPtr font,
			string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Shaded(
				font,
				utf8Text,
				fg,
				bg
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Shaded(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Shaded_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg,
			uint wrapLength
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Shaded_Wrapped", CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Shaded_Wrapped(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg,
			uint wrapLength
		);
		public static unsafe IntPtr TTF_RenderUTF8_Shaded_Wrapped(
			IntPtr font,
			string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg,
			uint wrapLength
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Shaded_Wrapped(
				font,
				utf8Text,
				fg,
				bg,
				wrapLength
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Shaded_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg,
			uint wrapLength
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph_Shaded(
			IntPtr font,
			ushort ch,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph32_Shaded(
			IntPtr font,
			uint ch,
			SDL.SDL_Color fg,
			SDL.SDL_Color bg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Blended(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Blended", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Blended(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg
		);
		public static unsafe IntPtr TTF_RenderUTF8_Blended(
			IntPtr font,
			string text,
			SDL.SDL_Color fg
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Blended(
				font,
				utf8Text,
				fg
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Blended(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderText_Blended_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string text,
			SDL.SDL_Color fg,
			uint wrapped
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, EntryPoint = "TTF_RenderUTF8_Blended_Wrapped", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_TTF_RenderUTF8_Blended_Wrapped(
			IntPtr font,
			byte* text,
			SDL.SDL_Color fg,
			uint wrapped
		);
		public static unsafe IntPtr TTF_RenderUTF8_Blended_Wrapped(
			IntPtr font,
			string text,
			SDL.SDL_Color fg,
			uint wrapped
		) {
			byte* utf8Text = SDL.Utf8Encode(text);
			IntPtr result = INTERNAL_TTF_RenderUTF8_Blended_Wrapped(
				font,
				utf8Text,
				fg,
				wrapped
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderUNICODE_Blended_Wrapped(
			IntPtr font,
			[In()] [MarshalAs(UnmanagedType.LPWStr)]
				string text,
			SDL.SDL_Color fg,
			uint wrapped
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph_Blended(
			IntPtr font,
			ushort ch,
			SDL.SDL_Color fg
		);

		/* IntPtr refers to an SDL_Surface*, font to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr TTF_RenderGlyph32_Blended(
			IntPtr font,
			uint ch,
			SDL.SDL_Color fg
		);

		/* Only available in 2.0.16 or higher. */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SetDirection(int direction);

		/* Only available in 2.0.16 or higher. */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_SetScript(int script);

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_CloseFont(IntPtr font);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TTF_Quit();

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_WasInit();

		/* font refers to a TTF_Font* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDL_GetFontKerningSize(
			IntPtr font,
			int prev_index,
			int index
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.15 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontKerningSizeGlyphs(
			IntPtr font,
			ushort previous_ch,
			ushort ch
		);

		/* font refers to a TTF_Font*
		 * Only available in 2.0.16 or higher.
		 */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int TTF_GetFontKerningSizeGlyphs32(
			IntPtr font,
			ushort previous_ch,
			ushort ch
		);

		#endregion
	}
}
