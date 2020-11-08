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
	public static class SDL_image
	{
		#region SDL2# Variables

		/* Used by DllImport to load the native library. */
		private const string nativeLibName = "SDL2_image";

		#endregion

		#region SDL_image.h

		/* Similar to the headers, this is the version we're expecting to be
		 * running with. You will likely want to check this somewhere in your
		 * program!
		 */
		public const int SDL_IMAGE_MAJOR_VERSION =	2;
		public const int SDL_IMAGE_MINOR_VERSION =	0;
		public const int SDL_IMAGE_PATCHLEVEL =		6;

		[Flags]
		public enum IMG_InitFlags
		{
			IMG_INIT_JPG =	0x00000001,
			IMG_INIT_PNG =	0x00000002,
			IMG_INIT_TIF =	0x00000004,
			IMG_INIT_WEBP =	0x00000008
		}

		public static void SDL_IMAGE_VERSION(out SDL.SDL_version X)
		{
			X.major = SDL_IMAGE_MAJOR_VERSION;
			X.minor = SDL_IMAGE_MINOR_VERSION;
			X.patch = SDL_IMAGE_PATCHLEVEL;
		}

		[DllImport(nativeLibName, EntryPoint = "IMG_Linked_Version", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr INTERNAL_IMG_Linked_Version();
		public static SDL.SDL_version IMG_Linked_Version()
		{
			SDL.SDL_version result;
			IntPtr result_ptr = INTERNAL_IMG_Linked_Version();
			result = (SDL.SDL_version) Marshal.PtrToStructure(
				result_ptr,
				typeof(SDL.SDL_version)
			);
			return result;
		}

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int IMG_Init(IMG_InitFlags flags);

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void IMG_Quit();

		/* IntPtr refers to an SDL_Surface* */
		[DllImport(nativeLibName, EntryPoint = "IMG_Load", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_IMG_Load(
			byte* file
		);
		public static unsafe IntPtr IMG_Load(string file)
		{
			byte* utf8File = SDL.Utf8Encode(file);
			IntPtr handle = INTERNAL_IMG_Load(
				utf8File
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return handle;
		}

		/* src refers to an SDL_RWops*, IntPtr to an SDL_Surface* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_Load_RW(
			IntPtr src,
			int freesrc
		);

		/* src refers to an SDL_RWops*, IntPtr to an SDL_Surface* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, EntryPoint = "IMG_LoadTyped_RW", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_IMG_LoadTyped_RW(
			IntPtr src,
			int freesrc,
			byte* type
		);
		public static unsafe IntPtr IMG_LoadTyped_RW(
			IntPtr src,
			int freesrc,
			string type
		) {
			int utf8TypeBufSize = SDL.Utf8Size(type);
			byte* utf8Type = stackalloc byte[utf8TypeBufSize];
			return INTERNAL_IMG_LoadTyped_RW(
				src,
				freesrc,
				SDL.Utf8Encode(type, utf8Type, utf8TypeBufSize)
			);
		}

		/* IntPtr refers to an SDL_Texture*, renderer to an SDL_Renderer* */
		[DllImport(nativeLibName, EntryPoint = "IMG_LoadTexture", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_IMG_LoadTexture(
			IntPtr renderer,
			byte* file
		);
		public static unsafe IntPtr IMG_LoadTexture(
			IntPtr renderer,
			string file
		) {
			byte* utf8File = SDL.Utf8Encode(file);
			IntPtr handle = INTERNAL_IMG_LoadTexture(
				renderer,
				utf8File
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return handle;
		}

		/* renderer refers to an SDL_Renderer*.
		 * src refers to an SDL_RWops*.
		 * IntPtr to an SDL_Texture*.
		 */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_LoadTexture_RW(
			IntPtr renderer,
			IntPtr src,
			int freesrc
		);

		/* renderer refers to an SDL_Renderer*.
		 * src refers to an SDL_RWops*.
		 * IntPtr to an SDL_Texture*.
		 */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, EntryPoint = "IMG_LoadTextureTyped_RW", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr INTERNAL_IMG_LoadTextureTyped_RW(
			IntPtr renderer,
			IntPtr src,
			int freesrc,
			byte* type
		);
		public static unsafe IntPtr IMG_LoadTextureTyped_RW(
			IntPtr renderer,
			IntPtr src,
			int freesrc,
			string type
		) {
			byte* utf8Type = SDL.Utf8Encode(type);
			IntPtr handle = INTERNAL_IMG_LoadTextureTyped_RW(
				renderer,
				src,
				freesrc,
				utf8Type
			);
			Marshal.FreeHGlobal((IntPtr) utf8Type);
			return handle;
		}

		/* IntPtr refers to an SDL_Surface* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_ReadXPMFromArray(
			[In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
				string[] xpm
		);

		/* surface refers to an SDL_Surface* */
		[DllImport(nativeLibName, EntryPoint = "IMG_SavePNG", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe int INTERNAL_IMG_SavePNG(
			IntPtr surface,
			byte* file
		);
		public static unsafe int IMG_SavePNG(IntPtr surface, string file)
		{
			byte* utf8File = SDL.Utf8Encode(file);
			int result = INTERNAL_IMG_SavePNG(
				surface,
				utf8File
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return result;
		}

		/* surface refers to an SDL_Surface*, dst to an SDL_RWops* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int IMG_SavePNG_RW(
			IntPtr surface,
			IntPtr dst,
			int freedst
		);

		/* surface refers to an SDL_Surface* */
		[DllImport(nativeLibName, EntryPoint = "IMG_SaveJPG", CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe int INTERNAL_IMG_SaveJPG(
			IntPtr surface,
			byte* file,
			int quality
		);
		public static unsafe int IMG_SaveJPG(IntPtr surface, string file, int quality)
		{
			byte* utf8File = SDL.Utf8Encode(file);
			int result = INTERNAL_IMG_SaveJPG(
				surface,
				utf8File,
				quality
			);
			Marshal.FreeHGlobal((IntPtr) utf8File);
			return result;
		}

		/* surface refers to an SDL_Surface*, dst to an SDL_RWops* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int IMG_SaveJPG_RW(
			IntPtr surface,
			IntPtr dst,
			int freedst,
			int quality
		);

		#region Animated Image Support

		/* This region is only available in 2.0.6 or higher. */

		public struct IMG_Animation
		{
			public int w;
			public int h;
			public IntPtr frames; /* SDL_Surface** */
			public IntPtr delays; /* int* */
		}

		/* IntPtr refers to an IMG_Animation* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_LoadAnimation(
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string file
		);

		/* IntPtr refers to an IMG_Animation*, src to an SDL_RWops* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_LoadAnimation_RW(
			IntPtr src,
			int freesrc
		);

		/* IntPtr refers to an IMG_Animation*, src to an SDL_RWops* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_LoadAnimationTyped_RW(
			IntPtr src,
			int freesrc,
			[In()] [MarshalAs(UnmanagedType.LPStr)]
				string type
		);

		/* anim refers to an IMG_Animation* */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void IMG_FreeAnimation(IntPtr anim);

		/* IntPtr refers to an IMG_Animation*, src to an SDL_RWops* */
		/* THIS IS A PUBLIC RWops FUNCTION! */
		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr IMG_LoadGIFAnimation_RW(IntPtr src);

		#endregion

		#endregion
	}
}
