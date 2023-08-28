using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static SDL2.SDL;
using static FFmpeg.AutoGen.ffmpeg;
using System.Security.Cryptography;
using ImGuiNET;
using SysDVR.Client.Platform;
using System.Threading.Channels;

namespace SysDVR.Client.GUI
{
    class PendingUiNotif : IDisposable
    {
        public string Text;
        public bool ShouldRemove;
        
        Timer disposeTimer;

        public PendingUiNotif(string text)
        {
            Text = text;
            ShouldRemove = false;
            disposeTimer = new Timer((_) => ShouldRemove = true, null, 5000, Timeout.Infinite);
        }

        public void Dispose()
        {
            disposeTimer.Dispose();
        }
    }

    internal class PlayerView : View
    {
        readonly bool HasAudio;
        readonly bool HasVideo;
        readonly StreamSynchronizationHelper SyncHelper;
        readonly AutoResetEvent? OnNewFrame;
        readonly SDLAudioContext SDLAudio;
        readonly DecoderContext Decoder;

        readonly PlayerManager Manager;

        protected SDLContext SDL; // This must be initialized by the UI thread
        protected FormatConverterContext Converter; // Initialized only when the decoder output format doesn't match the SDL texture format

        SDL_Rect DisplayRect = new SDL_Rect();

        List<PendingUiNotif> notifications = new();
        
        Gui.CenterGroup uiOptCenter;
        Gui.Popup quitConfirm = new("Are you sure you want to quit ?");
        bool drawUi;

        void UINotifyMessage(string message)
        {
            notifications.Add(new PendingUiNotif(message));
            Console.WriteLine(message);
        }

        public override void Created()
        {
            Manager.Begin();
            SDL_PauseAudioDevice(SDLAudio.DeviceID, 0);
            base.Created();
        }

        public PlayerView(PlayerManager manager)
        {
            RenderMode = FramerateCapOptions.Target(60);

            Manager = manager;
            HasVideo = manager.VideoTarget is not null;
            HasAudio = manager.AudioTarget is not null;

            if (!HasAudio && !HasVideo)
                throw new Exception("Can't start a player with no streams");

            // SyncHelper is disabled if there is only a single stream
            // Note that it can also be disabled via a --debug flag and this is handled by the constructor
            SyncHelper = new(HasAudio && HasVideo);

            if (HasVideo)
            {
                Decoder = InitVideoDecoder();
                Decoder.SyncHelper = SyncHelper;
                manager.VideoTarget.UseContext(Decoder);
                OnNewFrame = Decoder.OnFrameEvent;

                SDL = InitSDLRenderTexture();
                InitializeLoadingTexture();
            }

            if (HasAudio)
            {
                SDLAudio = InitSDLAudio(manager.AudioTarget);
                manager.AudioTarget.UseSynchronizationHeloper(SyncHelper);
            }

            UINotifyMessage("Hello word");
        }

        public override void BackPressed()
        {
            if (!quitConfirm.HandleBackButton())
                quitConfirm.RequestOpen();
        }

        public override void Draw()
        {
            Gui.BeginWindow("Player", ImGuiWindowFlags.NoBackground);

            for (int i = 0; i < notifications.Count; i++)
            {
                var notif = notifications[i];
                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1, 0, 0, 1));
                ImGui.Text(notif.Text);
                ImGui.PopStyleColor();
                if (notif.ShouldRemove)
                {
                    notifications.RemoveAt(i);
                    notif.Dispose();
                }
            }

            if (drawUi)
                DrawOverlayMenu();

            DrawOverlayToggleArea();

            DrawQuitModal();

            ImGui.End();
        }

        void DrawOverlayToggleArea() 
        {
            var rect = new ImRect()
            {
                Min = ImGui.GetWindowPos(),
                Max = ImGui.GetWindowPos() + ImGui.GetWindowSize(),
            };

            var id = ImGui.GetID("##TepToReveal");
            ImGui.KeepAliveID(id);
            if (ImGui.ButtonBehavior(rect, id, out _, out _, ImGuiButtonFlags.MouseButtonLeft) || ImGui.IsKeyPressed(ImGuiKey.Space))
            {
                drawUi = !drawUi;
            }
        }

        void DrawOverlayMenu() 
        {
            float OverlayY = ImGui.GetWindowSize().Y;

            if (Program.Instance.IsPortrait)
            {
                OverlayY = OverlayY * 6 / 10;
                ImGui.SetCursorPosY(OverlayY + ImGui.GetStyle().WindowPadding.Y);

                var width = ImGui.GetWindowSize().X;
                var btnwidth = width * 3 / 6;

                ImGui.SetCursorPosX(width / 2 - btnwidth / 2);
                if (ImGui.Button("Start recording", new(btnwidth, 0))) ButtonStartRecording();
                
                ImGui.SetCursorPosX(width / 2 - btnwidth / 2);
                if (ImGui.Button("Stop streaming", new(btnwidth, 0))) ButtonQuit();
                
                ImGui.SetCursorPosX(width / 2 - btnwidth / 2);
                if (ImGui.Button("Show statistics", new(btnwidth, 0))) ButtonStats();
                
                ImGui.SetCursorPosX(width / 2 - btnwidth / 2);
                if (ImGui.Button("Full screen", new(btnwidth, 0))) ButtonFullscreen();
            }
            else
            {
                OverlayY = OverlayY * 5 / 6;

                ImGui.SetCursorPosY(OverlayY + ImGui.GetStyle().WindowPadding.Y);

                uiOptCenter.StartHere();
                if (ImGui.Button("Start recording")) ButtonStartRecording();
                ImGui.SameLine();
                if (ImGui.Button("Stop streaming")) ButtonQuit();
                ImGui.SameLine();
                if (ImGui.Button("Show statistics")) ButtonStats();
                ImGui.SameLine();
                if (ImGui.Button("Full screen")) ButtonFullscreen();
                uiOptCenter.EndHere();
            }

            ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - ImGui.CalcTextSize("A").Y - ImGui.GetStyle().WindowPadding.Y);
            Gui.CenterText("Tap anywhere to hide the overlay");
            
            ImGui.GetBackgroundDrawList().AddRectFilled(new(0, OverlayY), ImGui.GetWindowSize(), 0xe0000000);
        }

        void DrawQuitModal() 
        {
            if (quitConfirm.Begin())
            {
                ImGui.Text("Are you sure you want to quit?");
                ImGui.Separator();

                if (ImGui.Button("Yes"))
                {
                    quitConfirm.RequestClose();
                    Program.Instance.PopView();
                }

                ImGui.SameLine();

                if (ImGui.Button("No"))
                    quitConfirm.RequestClose();

                ImGui.EndPopup();
            }
        }

        void ButtonStartRecording() { UINotifyMessage("ButtonStartRecording"); }
        void ButtonQuit() { BackPressed(); }
        void ButtonStats() { UINotifyMessage("ButtonStats"); }
        void ButtonFullscreen() { UINotifyMessage("ButtonFullscreen"); }

        unsafe public override void RawDraw()
        {
            base.RawDraw();

            if (DecodeNextFrame())
            {
                // TODO: this call is needed only with opengl on linux (and not on every linux install i tested) where TextureUpdate must be called by the main thread,
                // Check if are there any performance improvements by moving this to the decoder thread on other OSes
                UpdateSDLTexture(Decoder.RenderFrame);
            }

            // Bypass imgui for this
            SDL_RenderCopy(Program.Instance.SdlRenderer, SDL.Texture, ref SDL.TextureSize, ref DisplayRect);

            // Signal we're presenting something to SDL to kick the decding thread
            // We don't care if we didn't actually decoded anything we just do it here
            // to do this on every vsync to avoid arbitrary sleeps on the other side
            OnNewFrame?.Set();
        }

        public override void ResolutionChanged()
        {
            const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;

            var w = (int)Program.Instance.WindowSize.X;
            var h = (int)Program.Instance.WindowSize.Y;

            // TODO: Verify what's the state of this in 6.0
            // Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
            //SDL_GetRendererOutputSize(Program.SdlRenderer, out int pixelWidth, out int pixelHeight);
            //float scaleX = pixelWidth / (float)w;
            //float scaleY = pixelHeight / (float)h;
            //SDL_RenderSetScale(SDL.Renderer, scaleX, scaleY);

            if (w >= h * Ratio)
            {
                DisplayRect.w = (int)(h * Ratio);
                DisplayRect.h = h;
            }
            else
            {
                DisplayRect.h = (int)(w / Ratio);
                DisplayRect.w = w;
            }

            DisplayRect.x = w / 2 - DisplayRect.w / 2;
            DisplayRect.y = h / 2 - DisplayRect.h / 2;
        }

        public unsafe override void Destroy()
        {
            if (HasAudio)
                SDL_PauseAudioDevice(SDLAudio.DeviceID, 1);

            Manager.Stop();
            Manager.Dispose();

            // Dispose of unmanaged resources
            if (HasAudio)
            {
                SDL_CloseAudioDevice(SDLAudio.DeviceID);
                SDLAudio.TargetHandle.Free();
            }

            if (HasVideo)
            {
                var ptr = Decoder.Frame1;
                av_frame_free(&ptr);

                ptr = Decoder.Frame2;
                av_frame_free(&ptr);

                var ptr2 = Decoder.CodecCtx;
                avcodec_free_context(&ptr2);

                if (Converter.Converter != null)
                {
                    var ptr3 = Converter.Frame;
                    av_frame_free(&ptr3);

                    sws_freeContext(Converter.Converter);
                }

                SDL_DestroyTexture(SDL.Texture);
            }

            OnNewFrame?.Dispose();
            base.Destroy();
        }

        static SDLContext InitSDLRenderTexture()
        {
            var tex = SDL_CreateTexture(Program.Instance.SdlRenderer, SDL_PIXELFORMAT_IYUV,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                StreamInfo.VideoWidth, StreamInfo.VideoHeight).AssertNotNull(SDL_GetError);

            return new SDLContext()
            {
                Initialized = true,
                TextureSize = new SDL_Rect() { x = 0, y = 0, w = StreamInfo.VideoWidth, h = StreamInfo.VideoHeight },
                Texture = tex,
                TextureLock = new object()
            };
        }

        static unsafe SDLAudioContext InitSDLAudio(AudioStreamTarget target)
        {
            var handle = GCHandle.Alloc(target, GCHandleType.Normal);

            SDL_AudioCallback callback = AudioStreamTargetNative.SDLCallback;
            SDL_AudioSpec wantedSpec = new SDL_AudioSpec()
            {
                silence = 0,
                channels = StreamInfo.AudioChannels,
                format = AUDIO_S16LSB,
                freq = StreamInfo.AudioSampleRate,
                // This value was the deafault until sysdvr 5.4, however SDL will pick its preferred buffer size since we pass SDL_AUDIO_ALLOW_SAMPLES_CHANGE, this is fine since we have our own buffering
                size = StreamInfo.AudioPayloadSize * 2,
                samples = StreamInfo.MinAudioSamplesPerPayload * 2,
                callback = callback,
                userdata = GCHandle.ToIntPtr(handle)
            };

            var deviceId = SDL_OpenAudioDevice(IntPtr.Zero, 0, ref wantedSpec, out var obtained, (int)SDL_AUDIO_ALLOW_SAMPLES_CHANGE);

            deviceId.AssertNotZero(SDL_GetError);

            if (DebugOptions.Current.Log)
            {
                Console.WriteLine($"SDL_Audio: requested samples per callback={wantedSpec.samples} obtained={obtained.samples}");
            }

            return new SDLAudioContext
            {
                DeviceID = deviceId,
                CallbackDelegate = callback, // Prevents the delegate passed to native code from being GC'd
                TargetHandle = handle
            };
        }

        unsafe DecoderContext InitVideoDecoder()
        {
            var name = Program.Options.DecoderName;
            var useHwAcc = Program.Options.HardwareAccel;
            AVCodec* codec = null;

            if (name is not null) 
            {
                codec = avcodec_find_decoder_by_name(name);

                if (codec == null)
                {
                    UINotifyMessage($"ERROR: The codec {name} Couldn't be initialized, falling back to default decoder.");
                }
                else
                {
                    UINotifyMessage("You manually set a video decoder with the --decoder option, in case of issues remove it to use the default one.");
                    return CreateDecoderContext(codec);
                }
            }

            if (useHwAcc)
            {
                name = LibavUtils.GetH264Decoders().Where(x => x.Name != "h264").FirstOrDefault()?.Name;

                if (name != null)
                {
                    codec = avcodec_find_decoder_by_name(name);
                    if (codec != null)
                        UINotifyMessage("Attempting to initialize the video player with an hardware accelerated video decoder, in case of issues try removing the --hw-acc option do disable this.");
                }
            }

            if (codec == null)
                codec = avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);

            if (codec == null)
                throw new Exception("Couldn't find any compatible video codecs");

            return CreateDecoderContext(codec);
        }

        static unsafe DecoderContext CreateDecoderContext(AVCodec* codec)
        {
            if (codec == null)
                throw new Exception("Codec can't be null");

            string codecName = Marshal.PtrToStringAnsi((IntPtr)codec->name);

            Console.WriteLine($"Initializing video player with {codecName} codec.");

            var codectx = avcodec_alloc_context3(codec);
            if (codectx == null)
                throw new Exception("Couldn't allocate a codec context");

            // These are set in ffplay
            codectx->codec_id = codec->id;
            codectx->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
            codectx->bit_rate = 0;

            // Some decoders break without this
            codectx->width = StreamInfo.VideoWidth;
            codectx->height = StreamInfo.VideoHeight;

            var (ex, sz) = LibavUtils.AllocateH264Extradata();
            codectx->extradata_size = sz;
            codectx->extradata = (byte*)ex.ToPointer();

            avcodec_open2(codectx, codec, null).AssertZero("Couldn't open the codec.");

            var pic = av_frame_alloc();
            if (pic == null)
                throw new Exception("Couldn't allocate the decoding frame");

            var pic2 = av_frame_alloc();
            if (pic2 == null)
                throw new Exception("Couldn't allocate the decoding frame");

            return new DecoderContext()
            {
                CodecCtx = codectx,
                Frame1 = pic,
                Frame2 = pic2,
                ReceiveFrame = pic,
                RenderFrame = pic2,
                CodecLock = new object(),
                OnFrameEvent = new AutoResetEvent(true)
            };
        }

        unsafe static FormatConverterContext InitializeConverter(AVCodecContext* codecctx)
        {
            AVFrame* dstframe = null;
            SwsContext* swsContext = null;

            Console.WriteLine($"Initializing converter for {codecctx->pix_fmt}");

            dstframe = av_frame_alloc();

            if (dstframe == null)
                throw new Exception("Couldn't allocate the the converted frame");

            dstframe->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;
            dstframe->width = StreamInfo.VideoWidth;
            dstframe->height = StreamInfo.VideoHeight;

            av_frame_get_buffer(dstframe, 32).AssertZero("Couldn't allocate the buffer for the converted frame");

            swsContext = sws_getContext(codecctx->width, codecctx->height, codecctx->pix_fmt,
                                        dstframe->width, dstframe->height, AVPixelFormat.AV_PIX_FMT_YUV420P,
                                        SWS_FAST_BILINEAR, null, null, null);

            if (swsContext == null)
                throw new Exception("Couldn't initialize the converter");

            return new FormatConverterContext()
            {
                Converter = swsContext,
                Frame = dstframe
            };
        }

        private unsafe void InitializeLoadingTexture()
        {
            var currentDllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] data = null;

            try {
                data = Resources.ReadResouce(Resources.LoadingImage);
            }
            catch {
                // Don't care
            }
            
            if (data == null)
            {
                // Hardcoded buffer size for a 1280x720 YUV texture
                data = new byte[0x1517F0];
                // Fill with YUV white
                data.AsSpan(0, 0xE1000).Fill(0xFF);
                data.AsSpan(0xE1000, 0x119400 - 0xE1000).Fill(0x7F);
                data.AsSpan(0x119400).Fill(0x80);
            }

            fixed (byte* ptr = data)
                SDL_UpdateTexture(SDL.Texture, ref SDL.TextureSize, new IntPtr(ptr), 1280);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int av_ceil_rshift(int a, int b) =>
            -(-a >> b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void UpdateSDLTexture(AVFrame* pic)
        {
            if (pic->linesize[0] > 0 && pic->linesize[1] > 0 && pic->linesize[2] > 0)
            {
                SDL_UpdateYUVTexture(SDL.Texture, ref SDL.TextureSize,
                    (IntPtr)pic->data[0], pic->linesize[0],
                    (IntPtr)pic->data[1], pic->linesize[1],
                    (IntPtr)pic->data[2], pic->linesize[2]);
            }
#if DEBUG
            // Not sure if this is needed but ffplay source does handle this case, all my tests had positive linesize
            else if (pic->linesize[0] < 0 && pic->linesize[1] < 0 && pic->linesize[2] < 0)
            {
                Console.WriteLine("Negative Linesize");
                SDL_UpdateYUVTexture(SDL.Texture, ref SDL.TextureSize,
                    (IntPtr)(pic->data[0] + pic->linesize[0] * (pic->height - 1)), -pic->linesize[0],
                    (IntPtr)(pic->data[1] + pic->linesize[1] * (av_ceil_rshift(pic->height, 1) - 1)), -pic->linesize[1],
                    (IntPtr)(pic->data[2] + pic->linesize[2] * (av_ceil_rshift(pic->height, 1) - 1)), -pic->linesize[2]);
            }
#endif
            // While this doesn't seem to be handled in ffplay but the texture can be non-planar with some decoders
            else if (pic->linesize[0] > 0 && pic->linesize[1] == 0)
            {
                SDL_UpdateTexture(SDL.Texture, ref SDL.TextureSize, (IntPtr)pic->data[0], pic->linesize[0]);
            }
            else Console.WriteLine($"Error: Non-positive planar linesizes are not supported, open an issue on Github. {pic->linesize[0]} {pic->linesize[1]} {pic->linesize[2]}");
        }

        bool converterFirstFrameCheck = false;
        unsafe bool DecodeNextFrame()
        {
            int ret = 0;

            lock (Decoder.CodecLock)
                ret = avcodec_receive_frame(Decoder.CodecCtx, Decoder.ReceiveFrame);

            if (ret == AVERROR(EAGAIN))
            {
                // Try again for the next SDL frame
                return false;
            }
            else if (ret != 0)
            {
                // Should not happen
                Console.WriteLine($"avcodec_receive_frame {ret}");
                return false;
            }
            else
            {
                // On the first frame we get check if we need to use a converter
                if (!converterFirstFrameCheck && Decoder.CodecCtx->pix_fmt != AVPixelFormat.AV_PIX_FMT_NONE)
                {
#if DEBUG
                    Console.WriteLine($"Using pixel format {Decoder.CodecCtx->pix_fmt}");
#endif
                    converterFirstFrameCheck = true;
                    if (Decoder.CodecCtx->pix_fmt != AVPixelFormat.AV_PIX_FMT_YUV420P)
                    {
                        Converter = InitializeConverter(Decoder.CodecCtx);
                        // Render to the converted frame
                        Decoder.RenderFrame = Converter.Frame;
                    }
                }

                if (Converter.Converter != null)
                {
                    var source = Decoder.ReceiveFrame;
                    var target = Decoder.RenderFrame;
                    sws_scale(Converter.Converter, source->data, source->linesize, 0, source->height, target->data, target->linesize);
                }
                else
                {
                    // Swap the frames so we can render source
                    var toRender = Decoder.ReceiveFrame;
                    var receiveNext = Decoder.RenderFrame;

                    Decoder.ReceiveFrame = receiveNext;
                    Decoder.RenderFrame = toRender;
                }

                return true;
            }
        }
    }
}
