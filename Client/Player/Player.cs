﻿using System;
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
using LibUsbDotNet;

namespace SysDVR.Client.Player
{
	struct SDLContext
	{
		public volatile bool Initialized;

		public IntPtr Texture { get; init; }
		public IntPtr Renderer { get; init; }
		public IntPtr Window { get; init; }
		public SDL_Rect TextureSize;

		public object TextureLock { get; init; }
	}

	struct SDLAudioContext
	{
		public uint DeviceID { get; init; }
		public SDL_AudioCallback CallbackDelegate { get; init; }
		public GCHandle TargetHandle { get; init; }
	}

	unsafe class DecoderContext
	{
		public AVCodecContext* CodecCtx { get; init; }

		public AVFrame* RenderFrame;
		public AVFrame* ReceiveFrame;

		public AVFrame* Frame1 { get; init; }
		public AVFrame* Frame2 { get; init; }

		public object CodecLock { get; init; }

		public StreamSynchronizationHelper SyncHelper;
		public AutoResetEvent OnFrameEvent;
    }

	unsafe struct FormatConverterContext
	{
		public SwsContext* Converter { get; init; }
		public AVFrame* Frame { get; init; }	
	}

	class PlayerManager : BaseStreamManager, IDisposable
	{
		readonly Player player;

		bool disposedValue;

		public string WindowTitle { get; set; }
		public bool StartFullScreen { get; set; }

		public PlayerManager(bool HasVideo, bool HasAudio, bool hwAcc, string codecName, string quality) : base(
			HasVideo ? new H264StreamTarget() : null,
			HasAudio ? new AudioStreamTarget() : null)
		{
			player = new Player(
				VideoTarget as H264StreamTarget,
				AudioTarget as AudioStreamTarget,
				hwAcc, 
				codecName, 
				quality
			);
		}

		public override void Begin()
		{
			base.Begin();
			player.Start();
		}

		public override void Stop()
		{
			base.Stop();
			player.Stop();
		}

		public override void MainThread()
		{
			if (HasVideo)
			{
				Console.WriteLine("Starting stream, close the player window to stop.");
				Console.WriteLine("Press F11 for full screen, esc to quit.");
				Console.WriteLine();
				player.UiThreadMain(StartFullScreen, WindowTitle);
			}
			else base.MainThread();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposedValue)
			{
				if (disposing)
				{
					player.Dispose();
				}

				disposedValue = true;
			}
		}
	}

	class Player : IDisposable
	{
		readonly bool HasAudio;
		readonly bool HasVideo;
		readonly string ScaleQuality;
		readonly StreamSynchronizationHelper SyncHelper;
		readonly AutoResetEvent? OnNewFrame;

        private bool Running = false;
		
		protected DecoderContext Decoder; 
		protected SDLContext SDL; // This must be initialized by the UI thread
		protected FormatConverterContext Converter; // Initialized only when the decoder output format doesn't match the SDL texture format
		protected readonly SDLAudioContext SDLAudio;
		public string BaseWindowTitle;

		static SDLContext InitSDLVideo(string scaleQuality, string windowTitle)
		{
			SDL_InitSubSystem(SDL_INIT_VIDEO).AssertZero(SDL_GetError);

			SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");

			var win = SDL_CreateWindow(
                windowTitle,
				SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, StreamInfo.VideoWidth, 
				StreamInfo.VideoHeight,	SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE)
				.AssertNotNull(SDL_GetError);

			if (scaleQuality != null)
			{
				if (SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, scaleQuality) == SDL_bool.SDL_FALSE)
					Console.WriteLine($"Coudln't set the requested scale quality {scaleQuality}: {SDL_GetError()}");
			}
			else SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "linear");

			var render = SDL_CreateRenderer(win, -1,
				SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
				SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC )
				.AssertNotNull(SDL_GetError);

			SDL_GetRendererInfo(render, out var info);
			Console.WriteLine($"Initialized SDL with {Marshal.PtrToStringAnsi(info.name)} renderer");

			var tex = SDL_CreateTexture(render, SDL_PIXELFORMAT_IYUV,
				(int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
				StreamInfo.VideoWidth, StreamInfo.VideoHeight).AssertNotNull(SDL_GetError);

			return new SDLContext()
			{
				Initialized = true,
				TextureSize = new SDL_Rect() { x = 0, y = 0, w = StreamInfo.VideoWidth, h = StreamInfo.VideoHeight },
				Renderer = render,
				Texture = tex,
				Window = win,
				TextureLock = new object()
			};
		}
		
		static unsafe SDLAudioContext InitSDLAudio(AudioStreamTarget target)
		{
			SDL_InitSubSystem(SDL_INIT_AUDIO).AssertZero(SDL_GetError);

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

            SDL_PauseAudioDevice(deviceId, 0);

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

		static unsafe DecoderContext InitDecoderRequestDecoderName(string forceDecoder)
		{				
			var codec = avcodec_find_decoder_by_name(forceDecoder);

			if (codec == null)
			{
				Console.WriteLine($"ERROR: The codec {forceDecoder} Couldn't be initialized, falling back to default decoder.");
				return InitDecoderAuto(false);
			}
			else
			{
				Console.WriteLine("You manually set a video decoder with the --decoder option, in case of issues remove it to use the default one.");
				return CreateDecoderContext(codec);
			}
		}

		unsafe static DecoderContext InitDecoderAuto(bool hwAcc)
		{
			AVCodec* codec = null;

			if (hwAcc)
			{
				var name = LibavUtils.GetH264Decoders().Where(x => x.Name != "h264").FirstOrDefault();

				if (name != null)
				{
					codec = avcodec_find_decoder_by_name(name.Name);
					if (codec != null)
						Console.WriteLine("Attempting to initialize the video player with an hardware accelerated video decoder, in case of issues try removing the --hw-acc option do disable this.");
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

		private void SetFullScreen(bool enableFullScreen)
		{
			SDL_SetWindowFullscreen(SDL.Window, enableFullScreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
			SDL_ShowCursor(enableFullScreen ? SDL_DISABLE : SDL_ENABLE);
		}

		private unsafe void InitializeLoadingTexture() 
		{
			var currentDllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var loadingTexturePath = Path.Combine(currentDllPath, "runtimes", "loading.yuv");
			byte[] data = null;

			if (File.Exists(loadingTexturePath))
				data = File.ReadAllBytes(loadingTexturePath);
			else 
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

		unsafe public void UiThreadMain(bool startFullScreen, string windowTitle)
		{
            BaseWindowTitle = $"SysDVR-Client - {windowTitle ?? ($"PID {Process.GetCurrentProcess().Id}")}"; 
            SDL = InitSDLVideo(ScaleQuality, BaseWindowTitle);
            InitializeLoadingTexture();

            SDL_Rect DisplayRect = new SDL_Rect { x = 0, y = 0 };
			bool fullscreen = startFullScreen;

			void CalculateDisplayRect()
			{
				const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;
				SDL_GetWindowSize(SDL.Window, out int w, out int h);

				// Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
				SDL_GetRendererOutputSize(SDL.Renderer, out int pixelWidth, out int pixelHeight);
				float scaleX = pixelWidth / (float)w;
				float scaleY = pixelHeight / (float)h;
				SDL_RenderSetScale(SDL.Renderer, scaleX, scaleY);				
				
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

				SDL_RenderClear(SDL.Renderer);
			}

			if (fullscreen)
				SetFullScreen(true);

			CalculateDisplayRect();

            var countFps = DebugOptions.Current.Fps;
            FramerateCounter fpsCounter = new();
            if (countFps)
                fpsCounter.Start();

            while (Running)
			{
                if (DecodeNextFrame())
                {
                    // TODO: this call is needed only with opengl on linux (and not on every linux install i tested) where TextureUpdate must be called by the main thread,
                    // Check if are there any performance improvements by moving this to the decoder thread on other OSes
                    UpdateSDLTexture(Decoder.RenderFrame);
                    fpsCounter.OnFrame();
                }

                // SDL_RenderClear seems to cause a memory leak when the window is minimized on Windows.
				// Guess we don't really need to call it anyway since we're overwriting the whole screen anyway
                SDL_RenderCopy(SDL.Renderer, SDL.Texture, ref SDL.TextureSize, ref DisplayRect);
				SDL_RenderPresent(SDL.Renderer);

                // Signal we're presenting something to SDL to kick the decding thread
                // We don't care if we didn't actually decoded anything we just do it here
                // to do this on every vsync to avoid arbitrary sleeps on the other side
                OnNewFrame?.Set();

                if (countFps && fpsCounter.GetFps(out var fps))
				{
					SDL_SetWindowTitle(SDL.Window, $"{BaseWindowTitle} - FPS: {fps}");
				}

				int res = SDL_PollEvent(out var evt);
				while (res > 0)
				{
					if (evt.type == SDL_EventType.SDL_QUIT
						|| (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
						|| (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE))
						Stop();
					else if (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
						CalculateDisplayRect();
					else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_F11)
					{
						fullscreen = !fullscreen;
						SetFullScreen(fullscreen);
						CalculateDisplayRect();
					}

					res = SDL_PollEvent(out evt);
				};
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int av_ceil_rshift(int a, int b) =>
			-((-(a)) >> (b));

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

		public Player(H264StreamTarget? videoTarget, AudioStreamTarget? audioTarget, bool hwAcc, string codecName, string scaleQuality)
		{
			HasVideo = videoTarget is not null;
			HasAudio = audioTarget is not null;
			ScaleQuality = scaleQuality;

			if (!HasAudio && !HasVideo)
				throw new Exception("Can't start a player with no streams");

			SDL_Init(0).AssertZero(SDL_GetError);

			// SyncHelper is disabled if there is only a single stream
			// Note that it can also be disabled via a --debug flag and this is handled by the constructor
			SyncHelper = new(HasAudio && HasVideo);

            if (HasVideo)
			{
				Decoder = codecName == null ? InitDecoderAuto(hwAcc) : InitDecoderRequestDecoderName(codecName);
				Decoder.SyncHelper = SyncHelper;
                videoTarget.UseContext(Decoder);
				OnNewFrame = Decoder.OnFrameEvent;
            }

			if (HasAudio)
			{
				SDLAudio = InitSDLAudio(audioTarget);
				audioTarget.UseSynchronizationHeloper(SyncHelper);
			}
		}

		public void Start() 
		{
			if (Running)
				throw new Exception("Already started");

			Running = true;

			if (HasAudio)
				SDL_PauseAudio(0);
		}

		public void Stop() 
		{
			Running = false;
		}

		private bool disposedValue;
		protected unsafe virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (Running)
						Stop();
				}

				// Dispose of unmanaged resources
				if (HasAudio)
					SDLAudio.TargetHandle.Free();
				
				SDL_Quit();

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
				}

				OnNewFrame?.Dispose();

                disposedValue = true;
			}
		}

		~Player()
		{
		    Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	static class Exten
	{
		public static void AssertEqual(this int code, int expectedValue, Func<string> MessageFun = null)
		{
			if (code != expectedValue)
				throw new Exception($"Assertion failed {code} != {expectedValue} : {(MessageFun?.Invoke() ?? "Unknown error:")}");
		}

        public static void AssertNotZero(this uint code, Func<string> MessageFun = null)
        {
            if (code == 0)
                throw new Exception($"Assertion failed: {code} {(MessageFun?.Invoke() ?? "Unknown error")}");
        }

        public static void AssertZero(this int code, Func<string> MessageFun = null)
		{
			if (code != 0)
				throw new Exception($"Assertion failed: {code} {(MessageFun?.Invoke() ?? "Unknown error")}");
		}

		public static void AssertNotNeg(this int code, Func<string> MessageFun = null)
		{
			if (code < 0)
				throw new Exception($"Assertion failed: {code} {(MessageFun?.Invoke() ?? "Unknown error")}");
		}

		public static void AssertZero(this int code, string Message)
		{
			if (code != 0)
				throw new Exception($"Assertion failed: {code} {Message}");
		}

		public static IntPtr AssertNotNull(this IntPtr val, Func<string> MessageFun = null)
		{
			if (val == IntPtr.Zero)
				throw new Exception($"Assertion failed: pointer is null {(MessageFun?.Invoke() ?? "Unknown error")}");
			return val;
		}
	}
}
