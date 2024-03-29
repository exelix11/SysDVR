﻿using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static SDL2.SDL;
using static SDL2.SDL_image;
using static System.Net.Mime.MediaTypeNames;

namespace SysDVR.Client.Targets
{
	internal unsafe class SDLCapture : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SDL_Surface 
        {
            public uint flags;
            public SDL_PixelFormat* format;
            public int w, h;
            public int pitch;
            public IntPtr pixels;
            // There are more fields but we don't care about those
        }

		public readonly int Height;
		public readonly int Width;
        public readonly SDL_Surface* surface;

		bool disposed = false;

        private SDLCapture(int width, int height, SDL_Surface* surface) 
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            this.surface = surface;
			Width = width;
			Height = height;
        }

        ~SDLCapture()
        {
			if (!disposed)
				throw new Exception("Capture not disposed");
		}

		public void Dispose()
		{
			Program.SdlCtx.BugCheckThreadId();
			SDL_FreeSurface((IntPtr)surface);
			disposed = true;
			GC.SuppressFinalize(this);
		}

		public static SDLCapture CaptureTexture(IntPtr texture)
        {
			Program.SdlCtx.BugCheckThreadId();

			SDL_QueryTexture(texture, out var format, out _, out int width, out int height);
			var surface = (SDL_Surface*)SDL_CreateRGBSurface(0, width, height, 32, 0, 0, 0, 0)
					.AssertNotNull(SDL_GetError)
					.ToPointer();

			try
			{
				var renderer = Program.SdlCtx.RendererHandle;
				var tex = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ABGR8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height)
					.AssertNotNull(SDL_GetError);

				try
				{
					var target = SDL_GetRenderTarget(renderer);
					try
					{
						SDL_SetRenderTarget(renderer, tex).AssertZero(SDL_GetError);
						var rect = new SDL_Rect { x = 0, y = 0, w = width, h = height };
						SDL_RenderCopy(renderer, texture, ref rect, ref rect).AssertZero(SDL_GetError);
						SDL_RenderReadPixels(renderer, ref rect, surface->format->format, surface->pixels, surface->pitch).AssertZero(SDL_GetError);
						return new SDLCapture(width, height, surface);
					}
					finally
					{
						SDL_SetRenderTarget(renderer, target);
					}
				}
				finally
				{
					SDL_DestroyTexture(tex);
				}
			}
			catch
			{
				SDL_FreeSurface((IntPtr)surface);
				throw;
			}
		}

		public static unsafe void ExportTexture(IntPtr texture, string savePath) 
        {
			using var capture = CaptureTexture(texture);
			IMG_SavePNG((IntPtr)capture.surface, savePath).AssertZero(IMG_GetError);
		}
	}
}
