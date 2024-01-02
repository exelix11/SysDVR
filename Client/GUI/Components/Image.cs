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

namespace SysDVR.Client.GUI.Components
{
    public class Image : IDisposable
    {
        public readonly nint Texture;
        public readonly int Width;
        public readonly int Height;

        internal bool Disposed = false;

        private Image(nint texture)
        {
            Texture = texture;
            SDL.SDL_QueryTexture(Texture, out _, out _, out Width, out Height);
            Program.Instance.OnExit += Dispose;
        }

        public static Image FromFile(string filename)
        {
            var tex = SDL_image.IMG_LoadTexture(Program.SdlCtx.RendererHandle, filename);
            if (tex == nint.Zero)
                throw new Exception($"Loading image {filename} failed: {SDL_image.IMG_GetError()}");

            return new Image(tex);
        }

        // W / w = H / h  h = w * H / W
        public int ScaleHeight(int width) => Height * width / Width;
        public float ScaleHeight(float width) => Height * width / Width;

        public int ScaleWidth(int height) => Width * height / Height;
        public float ScaleWidth(float height) => Width * height / Height;

        public void Dispose()
        {
            if (Disposed) return;
            SDL.SDL_DestroyTexture(Texture);
            Disposed = true;
            Program.Instance.OnExit -= Dispose;
        }
    }

    internal class LazyImage
    {
        public readonly string Filename;
        private Image image = null;

        public LazyImage(string filename)
        {
            Filename = filename;
        }

        public nint Texture => Get().Texture;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Image Get()
        {
            if (image == null || image.Disposed)
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
