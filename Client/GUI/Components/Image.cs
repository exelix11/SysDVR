using SDL2;
using SysDVR.Client.App;
using System;

namespace SysDVR.Client.GUI.Components
{
    public class Image : IDisposable
    {
        readonly ClientApp Owner;

        public readonly nint Texture;
        public readonly int Width;
        public readonly int Height;

        internal bool Disposed = false;

        private Image(ClientApp owner, nint texture)
        {
            Owner = owner;
            Texture = texture;
            SDL.SDL_QueryTexture(Texture, out _, out _, out Width, out Height);
            owner.OnExit += Dispose;
        }

        public static Image FromFile(ClientApp owner, string filename)
        {
            var tex = SDL_image.IMG_LoadTexture(Program.SdlCtx.RendererHandle, filename);
            if (tex == nint.Zero)
                throw new Exception($"Loading image {filename} failed: {SDL_image.IMG_GetError()}");

            return new Image(owner, tex);
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
            Owner.OnExit -= Dispose;
        }
    }
}
