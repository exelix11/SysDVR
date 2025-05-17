using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using ImGuiNET;
using SDL2;
using SysDVR.Client.Sources;
using SysDVR.Client.ThirdParty.Openh264;
using SysDVR.Client.Targets.Decoding;

namespace SysDVR.Client.Targets.Player
{
    //unsafe class DecoderContext
    //{
    //    public AVCodecContext* CodecCtx { get; init; }

    //    public AVFrame* RenderFrame;
    //    public AVFrame* ReceiveFrame;

    //    public AVFrame* Frame1 { get; init; }
    //    public AVFrame* Frame2 { get; init; }

    //    public object CodecLock { get; init; }

    //    public StreamSynchronizationHelper SyncHelper;
    //    public AutoResetEvent OnFrameEvent;
    //}

    unsafe struct FormatConverterContext
    {
        public SwsContext* Converter { get; init; }
        public AVFrame* Frame { get; init; }
    }

    // Guarantees that the audio and video stream are compatible with the player
    class PlayerManager : StreamManager
    {
        internal new OpenH264PlayerTarget VideoTarget;
        internal new AudioOutStream AudioTarget;

        readonly public bool IsCompatibleAudioStream;

        static OutStream? MakeAudioStream(bool hasAudio) 
        {
            if (!hasAudio)
                return null;

            var useCompat = Program.Options.AudioPlayerMode switch
            {
                SDLAudioMode.Compatible => true,
                SDLAudioMode.Default => false,
                // Currently only mac os seems to need the compatible mode by default
                _ => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            };

            if (useCompat)
                return new QueuedStreamAudioTarget();
            else
                return new AudioStreamTarget(); 
        }

        static OutStream? MakeVideoStream(bool hasVideo)
        {
            if (!hasVideo)
                return null;

            return new OpenH264PlayerTarget();
        }

        public void UseSyncManager(StreamSynchronizationHelper manager)
        {
            if (AudioTarget is AudioStreamTarget)
            {
                ((AudioStreamTarget)AudioTarget).SyncHelper = manager;
            }
        }

        public PlayerManager(StreamingSource source, CancellationTokenSource cancel) :
            base(source, MakeVideoStream(source.Options.HasVideo), MakeAudioStream(source.Options.HasAudio), cancel)
        {
            VideoTarget = base.VideoTarget as OpenH264PlayerTarget;
            AudioTarget = base.AudioTarget as AudioOutStream;
            IsCompatibleAudioStream = AudioTarget is QueuedStreamAudioTarget;
        }
    }

    class AudioPlayer : IDisposable
    {
        readonly uint DeviceID;
        
        // Are we using the MacOS strategy ?
        readonly bool IsCompatiblePlayer;
        
        // Keep a reference to the callback to prevent GC from collecting it
        SDL_AudioCallback? CallbackDelegate;

        // Manually pin the Target object so it can be used as opaque pointer for the native code
        GCHandle TargetHandle;

        public const ushort AudioFormat = AUDIO_S16LSB;

        public AudioPlayer(AudioOutStream target) 
        {
            Program.SdlCtx.BugCheckThreadId();

            IsCompatiblePlayer = target is QueuedStreamAudioTarget;

            SDL_AudioSpec wantedSpec = new SDL_AudioSpec()
            {
                channels = StreamInfo.AudioChannels,
                format = AudioFormat,
                freq = StreamInfo.AudioSampleRate,
                // StreamInfo.MinAudioSamplesPerPayload * 2 was the default until sysdvr 5.4
                // however SDL will pick its preferred buffer size since we pass SDL_AUDIO_ALLOW_SAMPLES_CHANGE,
                // this is fine since we have our own buffering.
                samples = StreamInfo.MinAudioSamplesPerPayload,
            };

            if (!IsCompatiblePlayer)
            {
                TargetHandle = GCHandle.Alloc(target, GCHandleType.Normal);
                CallbackDelegate = AudioStreamTargetNative.SDLCallback;
                wantedSpec.callback = CallbackDelegate;
                wantedSpec.userdata = GCHandle.ToIntPtr(TargetHandle);
            }

            DeviceID = SDL_OpenAudioDevice(IntPtr.Zero, 0, ref wantedSpec, out var obtained, (int)SDL_AUDIO_ALLOW_SAMPLES_CHANGE);

            DeviceID.AssertNotZero(SDL_GetError);

			Program.DebugLog($"SDL_Audio: requested samples per callback={wantedSpec.samples} obtained={obtained.samples}");

            if (IsCompatiblePlayer)
            {
                ((QueuedStreamAudioTarget)(target)).DeviceID = DeviceID;
            }
        }

        public void Pause() 
        {
            SDL_PauseAudioDevice(DeviceID, 1);
        }

        public void Resume() 
        {
            SDL_PauseAudioDevice(DeviceID, 0);
        }

        public void Dispose()
        {
            Pause();
            SDL_CloseAudioDevice(DeviceID);
            TargetHandle.Free();
        }
    }

    class VideoPlayer : IDisposable
    {
        public const AVPixelFormat TargetDecodingFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
        public readonly uint TargetTextureFormat = SDL_PIXELFORMAT_IYUV;

        //public DecoderContext Decoder { get; private set; }
        FormatConverterContext Converter; // Initialized only when the decoder output format doesn't match the SDL texture format

        public string DecoderName { get; private set; }
        public bool AcceleratedDecotr { get; private set; }

        public object TextureLock;
        public IntPtr TargetTexture;
        public SDL_Rect TargetTextureSize;

        PlanarYUVFrame Render;
        readonly OpenH264PlayerTarget Target;

        public VideoPlayer(string? preferredDecoderName, bool hwAccel, OpenH264PlayerTarget target)
        {
            this.Target = target;

            Render = target.AcquireFrame();

            InitVideoDecoder(preferredDecoderName, hwAccel);
            InitSDLRenderTexture();
        }

        void InitSDLRenderTexture()
        {
            Program.SdlCtx.BugCheckThreadId();

            var tex = SDL_CreateTexture(Program.SdlCtx.RendererHandle, TargetTextureFormat,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                StreamInfo.VideoWidth, StreamInfo.VideoHeight).AssertNotNull(SDL_GetError);

            if (Program.Options.Debug.Log)
            {
                var pixfmt = SDL_QueryTexture(tex, out var format, out var a, out var w, out var h);
                Console.WriteLine($"SDL texture info: f = {SDL_GetPixelFormatName(format)} a = {a} w = {w} h = {h}");

                SDL_RendererInfo info;
                SDL_GetRendererInfo(Program.SdlCtx.RendererHandle, out info);
                for (int i = 0; i < info.num_texture_formats; i++) unsafe
                    {
                        Console.WriteLine($"Renderer supports pixel format {SDL_GetPixelFormatName(info.texture_formats[i])}");
                    }
            }

            TargetTextureSize = new SDL_Rect() { x = 0, y = 0, w = StreamInfo.VideoWidth, h = StreamInfo.VideoHeight };
            TargetTexture = tex;
            TextureLock = new object();
        }

        unsafe void InitVideoDecoder(string? name, bool useHwAcc)
        {
            //AVCodec* codec = null;

            //if (name is not null)
            //{
            //    codec = avcodec_find_decoder_by_name(name);
            //}
            
            //if (codec == null && useHwAcc)
            //{
            //    name = LibavUtils.GetH264Decoders().Where(x => x.Name != "h264").FirstOrDefault()?.Name;

            //    if (name != null)
            //    {
            //        codec = avcodec_find_decoder_by_name(name);
            //        if (codec != null)
            //        {
            //            AcceleratedDecotr = true;
            //        }
            //    }
            //}

            //if (codec == null)
            //    codec = avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);

            //if (codec == null)
            //    throw new Exception("Couldn't find any compatible video codecs");

            //Decoder = CreateDecoderContext(codec);
            //DecoderName = Marshal.PtrToStringAnsi((IntPtr)codec->name);
        }

        //static unsafe DecoderContext CreateDecoderContext(AVCodec* codec)
        //{
        //    if (codec == null)
        //        throw new Exception("Codec can't be null");

        //    string codecName = Marshal.PtrToStringAnsi((IntPtr)codec->name);

        //    Console.WriteLine(string.Format(Program.Strings.Player.PlayerInitializationMessage, codecName));

        //    var codectx = avcodec_alloc_context3(codec);
        //    if (codectx == null)
        //        throw new Exception("Couldn't allocate a codec context");

        //    // These are set in ffplay
        //    codectx->codec_id = codec->id;
        //    codectx->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
        //    codectx->bit_rate = 0;

        //    // Some decoders break without this
        //    codectx->width = StreamInfo.VideoWidth;
        //    codectx->height = StreamInfo.VideoHeight;

        //    var (ex, sz) = LibavUtils.AllocateH264Extradata();
        //    codectx->extradata_size = sz;
        //    codectx->extradata = (byte*)ex.ToPointer();

        //    avcodec_open2(codectx, codec, null).AssertZero("Couldn't open the codec.");

        //    var pic = av_frame_alloc();
        //    if (pic == null)
        //        throw new Exception("Couldn't allocate the decoding frame");

        //    var pic2 = av_frame_alloc();
        //    if (pic2 == null)
        //        throw new Exception("Couldn't allocate the decoding frame");

        //    return new DecoderContext()
        //    {
        //        CodecCtx = codectx,
        //        Frame1 = pic,
        //        Frame2 = pic2,
        //        ReceiveFrame = pic,
        //        RenderFrame = pic2,
        //        CodecLock = new object(),
        //        OnFrameEvent = new AutoResetEvent(true)
        //    };
        //}

        public unsafe bool DecodeFrame() 
        {
            if (DecodeFrameInternal())
            {
                // TODO: this call is needed only with opengl on linux (and not on every linux install i tested) where TextureUpdate must be called by the main thread,
                // Check if are there any performance improvements by moving this to the decoder thread on other OSes
                UpdateSDLTexture(Render);

                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int av_ceil_rshift(int a, int b) =>
            -(-a >> b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void UpdateSDLTexture(PlanarYUVFrame pic)
        {
			SDL_UpdateYUVTexture(TargetTexture, ref TargetTextureSize,
				pic.YDecoded, pic.YLineSize,
				pic.UDecoded, pic.ULineSize,
				pic.VDecoded, pic.VLineSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void UpdateSDLTexture(AVFrame* pic)
        {
            if (pic->linesize[0] > 0 && pic->linesize[1] > 0 && pic->linesize[2] > 0)
            {
                SDL_UpdateYUVTexture(TargetTexture, ref TargetTextureSize,
                    (IntPtr)pic->data[0], pic->linesize[0],
                    (IntPtr)pic->data[1], pic->linesize[1],
                    (IntPtr)pic->data[2], pic->linesize[2]);
            }
#if DEBUG
            // Not sure if this is needed but ffplay source does handle this case, all my tests had positive linesize
            else if (pic->linesize[0] < 0 && pic->linesize[1] < 0 && pic->linesize[2] < 0)
            {
                Program.DebugLog("Negative Linesize");
                SDL_UpdateYUVTexture(TargetTexture, ref TargetTextureSize,
                    (IntPtr)(pic->data[0] + pic->linesize[0] * (pic->height - 1)), -pic->linesize[0],
                    (IntPtr)(pic->data[1] + pic->linesize[1] * (av_ceil_rshift(pic->height, 1) - 1)), -pic->linesize[1],
                    (IntPtr)(pic->data[2] + pic->linesize[2] * (av_ceil_rshift(pic->height, 1) - 1)), -pic->linesize[2]);
            }
#endif
            // While this doesn't seem to be handled in ffplay but the texture can be non-planar with some decoders
            else if (pic->linesize[0] > 0 && pic->linesize[1] == 0)
            {
                SDL_UpdateTexture(TargetTexture, ref TargetTextureSize, (nint)pic->data[0], pic->linesize[0]);
            }
            else Console.WriteLine($"Error: Non-positive planar linesizes are not supported, open an issue on Github. {pic->linesize[0]} {pic->linesize[1]} {pic->linesize[2]}");
        }

        bool converterFirstFrameCheck = false;
        unsafe bool DecodeFrameInternal()
        {
            return Target.SwapBuffers(ref Render);
		}

        unsafe static FormatConverterContext InitializeConverter(AVCodecContext* codecctx)
        {
            AVFrame* dstframe = null;
            SwsContext* swsContext = null;

            Program.DebugLog($"Initializing converter for {codecctx->pix_fmt}");

            dstframe = av_frame_alloc();

            if (dstframe == null)
                throw new Exception("Couldn't allocate the the converted frame");

            dstframe->format = (int)TargetDecodingFormat;
            dstframe->width = StreamInfo.VideoWidth;
            dstframe->height = StreamInfo.VideoHeight;

            av_frame_get_buffer(dstframe, 32).AssertZero("Couldn't allocate the buffer for the converted frame");

            swsContext = sws_getContext(codecctx->width, codecctx->height, codecctx->pix_fmt,
                                        dstframe->width, dstframe->height, (AVPixelFormat)dstframe->format,
                                        SWS_FAST_BILINEAR, null, null, null);

            if (swsContext == null)
                throw new Exception("Couldn't initialize the converter");

            return new FormatConverterContext()
            {
                Converter = swsContext,
                Frame = dstframe
            };
        }

        public unsafe void Dispose()
        {
            //var ptr = Decoder.Frame1;
            //av_frame_free(&ptr);

            //ptr = Decoder.Frame2;
            //av_frame_free(&ptr);

            //var ptr2 = Decoder.CodecCtx;
            //avcodec_free_context(&ptr2);

            //if (Converter.Converter != null)
            //{
            //    var ptr3 = Converter.Frame;
            //    av_frame_free(&ptr3);

            //    sws_freeContext(Converter.Converter);
            //}

            //if (TargetTexture != 0)
            //    SDL_DestroyTexture(TargetTexture);

            //Decoder.OnFrameEvent.Dispose();
        }
    }
}
