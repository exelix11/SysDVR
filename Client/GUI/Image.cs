using ImGuiNET;
using SDL2;
using SysDVR.Client.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    public class Image : IDisposable
    {
        public readonly IntPtr Texture;
        public readonly int Width;
        public readonly int Height;

        internal bool Disposed = false;

        private Image(IntPtr texture)
        {
            this.Texture = texture;
            SDL.SDL_QueryTexture(Texture, out _, out _, out Width, out Height);
        }

        public static Image FromFile(string filename)
        {
            var tex = SDL_image.IMG_LoadTexture(Program.Instance.SdlRenderer, filename);
            if (tex == IntPtr.Zero)
                throw new Exception($"Loading image {filename} failed: {SDL_image.IMG_GetError()}");

            return new Image(tex);
        }

        // W / w = H / h  h = w * H / W
        public int ScaleHeight(int width) => Height * width / Width;

        public int ScaleWidth(int height) => Width * height / Height;

        public void Dispose()
        {
            if (Disposed) return;
            SDL.SDL_DestroyTexture(Texture);
            Disposed = true;
        }
    }

    internal class LazyImage
    {
        public readonly string Filename;
        private Image image = null;

        public LazyImage(string filename)
        {
            this.Filename = filename;
        }

        public IntPtr Texture => Get().Texture;
        public int Width { get; private set; }
        public int Height {get; private set; }

        public Image Get()
        {
            if (image == null)
            {
                image = Image.FromFile(Filename);
                Width = image.Width;
                Height = image.Height;
            }

            return image;
        }

        static public implicit operator Image(LazyImage i) => i.Get();

        public void Free() 
        {
            image?.Dispose();
            image = null;
        }
    }
}
