using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL2;
using static SDL2.SDL;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Diagnostics;

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

	unsafe struct SDLAudioContext
	{
		public SDL_AudioSpec AudioSpec { get; init; }
		public int ChannelCount { get; init; }
		public int SampleSize { get; init; }
		public int SampleRate { get; init; }
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
	}

	unsafe struct FormatConverterContext
	{
		public SwsContext* Converter { get; init; }
		public AVFrame* Frame { get; init; }	
	}

	class PlayerManager : BaseStreamManager, IDisposable
	{
		readonly Player player;
		private bool disposedValue;

		public PlayerManager(bool HasVideo, bool HasAudio, bool hwAcc, string codecName, string quality) : base(
			HasVideo ? new H264StreamTarget() : null,
			HasAudio ? new AudioStreamTarget() : null)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
			{
				Console.WriteLine(
					"WARNING: You seem to be running the 32-bit version of .NET, on Windows this is NOT supported due to ffmpeg not providing official 32-bit versions of their libs.\r\n" +
					"If you're running a 64-bit install of Windows (check your pc info) uninstall .NET and install the x64 version from Microsoft's website.\r\n" +
					"If you're running a 32-bit install of Windows you should upgrade your PC. Alternatively you need to find a 32-bit build of ffmpeg libs and copy them in the SysDVR-Client folder after that you can ignore this warning.\r\n" +
					"Continuing without the 32-bit libs WILL crash with a missing libraries/wrong library format error."
				);
			}

			player = new Player(this, hwAcc, codecName, quality);
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
				player.UiThreadMain();
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

		protected bool ShouldQuit = true;

		bool Running = false;
		protected Thread ReceiveThread;
		volatile bool TextureConsumed = true;

		protected DecoderContext Decoder; // Need to swap the target frames
		protected SDLContext SDL; // This must be initialized by the UI thread
		protected FormatConverterContext Converter; // Initialized only if needed
		protected readonly SDLAudioContext SDLAudio;

		static SDLContext InitSDLVideo(string scaleQuality)
		{
			SDL_InitSubSystem(SDL_INIT_VIDEO).Assert(SDL_GetError);

			var win = SDL_CreateWindow($"SysDVR-Client [PID {Process.GetCurrentProcess().Id}]", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, StreamInfo.VideoWidth, 
				StreamInfo.VideoHeight,	SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE).Assert(SDL_GetError);

			if (scaleQuality != null)
			{
				if (SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, scaleQuality) == SDL_bool.SDL_FALSE)
					Console.WriteLine($"Coudln't set the requested scale quality {scaleQuality}: {SDL_GetError()}");
			}
			else SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "linear");

			var render = SDL_CreateRenderer(win, -1,
				SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
				SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC |
				SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE)
				.Assert(SDL_GetError);

			SDL_GetRendererInfo(render, out var info);
			Console.WriteLine($"Initialized SDL with {Marshal.PtrToStringAnsi(info.name)} renderer");

			var tex = SDL_CreateTexture(render, SDL_PIXELFORMAT_IYUV,
				(int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
				StreamInfo.VideoWidth, StreamInfo.VideoHeight).Assert(SDL_GetError);

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
			SDL_InitSubSystem(SDL_INIT_AUDIO).Assert(SDL_GetError);

			var handle = GCHandle.Alloc(target);

			SDL_AudioSpec wantedSpec = new SDL_AudioSpec()
			{
				silence = 0,
				channels = StreamInfo.AudioChannels,
				format = AUDIO_S16LSB,
				freq = StreamInfo.AudioSampleRate,
				size = StreamInfo.AudioPayloadSize * StreamInfo.AudioBatching,
				callback = AudioStreamTargetNative.SDLCallback,
				userdata = GCHandle.ToIntPtr(handle)
			};

			SDL_OpenAudio(ref wantedSpec, IntPtr.Zero).Assert(SDL_GetError);

			return new SDLAudioContext
			{
				AudioSpec = wantedSpec,
				ChannelCount = wantedSpec.channels,
				SampleRate = wantedSpec.freq,
				SampleSize = wantedSpec.format switch
				{
					AUDIO_S16 => 2,
					AUDIO_S32 => 4,
					AUDIO_F32 => 4,
					_ => throw new NotImplementedException(),
				},
				TargetHandle = handle
			};
		}

		static unsafe DecoderContext InitDecoder(string forceDecoder)
		{				
			var codec = avcodec_find_decoder_by_name(forceDecoder);

			if (codec == null)
			{
				Console.WriteLine($"ERROR: The codec {forceDecoder} Couldn't be initialized, falling back to default decoder.");
				return InitDecoder(false);
			}
			else
			{
				Console.WriteLine("You manually set a video decoder with the --decoder option, in case of issues remove it to use the default one.");
				return InitDecoder(codec);
			}
		}

		static unsafe DecoderContext InitDecoder(bool hwAcc)
		{
			AVCodec* codec = null;

			if (hwAcc)
			{
				var name = CodecUtils.GetH264Decoders().Where(x => x.Name != "h264").FirstOrDefault();

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

			return InitDecoder(codec);
		}

		static unsafe DecoderContext InitDecoder(AVCodec* codec)
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

			// TODO: in theory we should set SPS and PPS in extradata here but it seems to not be working due to format differencies

			avcodec_open2(codectx, codec, null).Assert("Couldn't open the codec.");

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
				CodecLock = new object()
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

			av_frame_get_buffer(dstframe, 32).Assert("Couldn't allocate the buffer for the converted frame");
			
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

		unsafe public void UiThreadMain()
		{
			SDL = InitSDLVideo(ScaleQuality);

			SDL_Rect DisplayRect = new SDL_Rect { x = 0, y = 0 };
			bool fullscreen = false;

			void CalculateDisplayRect()
			{
				const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;
				SDL_GetWindowSize(SDL.Window, out int w, out int h);

				// Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
				SDL_GetRendererOutputSize(SDL.Renderer, out int pixelWidth, out int pixelHeight);
				float scaleX = pixelWidth / (float)w;
				float scaleY = pixelHeight / (float)h;
				SDL_RenderSetScale(SDL.Renderer, scaleX, scaleY);				
				
				if (w > h)
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

			CalculateDisplayRect();

			while (!ShouldQuit)
			{
				if (!TextureConsumed)
				{
					// TODO: this call is needed only with opengl on linux (and not on every linux install i tested) where TextureUpdate must be called by the main thread,
					// Check if are there any performance improvements by moving this to the decoder thread on other OSes
					UpdateSDLTexture(Decoder.RenderFrame);
					SDL_RenderCopy(SDL.Renderer, SDL.Texture, in SDL.TextureSize, in DisplayRect);
					SDL_RenderPresent(SDL.Renderer);
					TextureConsumed = true;
				}
				else
				{
					for (int i = 0; i < 50 && TextureConsumed; i++)
						Thread.Sleep(2); // If no new frame wait up to 100ms before polling events again
					if (!TextureConsumed)
						continue;
				}

				int res;
				do
				{
					res = SDL_PollEvent(out var evt);
					if (evt.type == SDL_EventType.SDL_QUIT
						|| (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
						|| (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE))
						ShouldQuit = true;
					else if (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
						CalculateDisplayRect();
					else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_F11) 
					{
						SDL_SetWindowFullscreen(SDL.Window, fullscreen ? 0 : (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
						fullscreen = !fullscreen;
						CalculateDisplayRect();
					}
				} while (res > 0);
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
				SDL_UpdateYUVTexture(SDL.Texture, in SDL.TextureSize,
					(IntPtr)pic->data[0], pic->linesize[0],
					(IntPtr)pic->data[1], pic->linesize[1],
					(IntPtr)pic->data[2], pic->linesize[2]);
			}
#if DEBUG
			// Not sure if this is needed but ffplay source does handle this case, all my tests had positive linesize
			else if (pic->linesize[0] < 0 && pic->linesize[1] < 0 && pic->linesize[2] < 0)
			{
				Console.WriteLine("Negative Linesize");
				SDL_UpdateYUVTexture(SDL.Texture, in SDL.TextureSize,
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

		unsafe void DecodeReceiverThreadMain()
		{
			while (!SDL.Initialized && !ShouldQuit)
				Thread.Sleep(100);

			bool converterCheck = false;

			while (!ShouldQuit)
			{
				int ret = 0;
				lock (Decoder.CodecLock)
					ret = avcodec_receive_frame(Decoder.CodecCtx, Decoder.ReceiveFrame);

				if (ret == AVERROR(EAGAIN))
					Thread.Sleep(2);
				else if (ret != 0)
					Console.WriteLine($"avcodec_receive_frame {ret}");
				else
				{
					// On the first frame we get check if we need to use a converter
					if (!converterCheck && Decoder.CodecCtx->pix_fmt != AVPixelFormat.AV_PIX_FMT_NONE)
					{
#if DEBUG
						Console.WriteLine($"Using pixel format {Decoder.CodecCtx->pix_fmt}");
#endif
						converterCheck = true;
						if (Decoder.CodecCtx->pix_fmt != AVPixelFormat.AV_PIX_FMT_YUV420P)
						{
							Converter = InitializeConverter(Decoder.CodecCtx);
							// Render to the converted frame
							Decoder.RenderFrame = Converter.Frame;
						}
					}

					while (!TextureConsumed && !ShouldQuit)
						Thread.Sleep(2);

					if (ShouldQuit)
						break;

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

					TextureConsumed = false;

					// TODO: figure out synchronization. 
					// The current implementation shows video as fast as it arrives, it seems to work fine but not sure if it's correct.
				}
			}
		}

		public Player(PlayerManager owner, bool hwAcc, string codecName, string scaleQuality)
		{
			HasAudio = owner.HasAudio;
			HasVideo = owner.HasVideo;
			ScaleQuality = scaleQuality;

			if (!HasAudio && !HasVideo)
				throw new Exception("Can't start a player with no streams");

			SDL_Init(0).Assert(SDL_GetError);

			if (HasVideo)
			{
				Decoder = codecName == null ? InitDecoder(hwAcc) : InitDecoder(codecName);
				((H264StreamTarget)owner.VideoTarget).UseContext(Decoder);
			}

			if (HasAudio)
			{
				SDLAudio = InitSDLAudio((AudioStreamTarget)owner.AudioTarget);
			}
		}

		public void Start() 
		{
			if (Running)
				throw new Exception("Already started");

			Running = true;
			ShouldQuit = false;

			if (HasAudio)
				SDL_PauseAudio(0);

			if (HasVideo)
			{
				ReceiveThread = new Thread(DecodeReceiverThreadMain);
				ReceiveThread.Start();
			}
		}

		public void Stop() 
		{
			ShouldQuit = true;

			//if (HasAudio)
			//	SDL_PauseAudio(1); this seems to hang sometimes for no apparent reason

			if (HasVideo)
			{
				ReceiveThread.Join();
				ReceiveThread = null;
			}
			
			Running = false;
		}

		private bool disposedValue;
		protected unsafe virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (Running)
					Stop();

				// Dispose of unmanaged resources
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
				throw new Exception($"Asserition failed {code} != {expectedValue} : {(MessageFun?.Invoke() ?? "Unknown error:")}");
		}

		public static void Assert(this int code, Func<string> MessageFun = null)
		{
			if (code != 0)
				throw new Exception($"Asserition failed: {code} {(MessageFun?.Invoke() ?? "Unknown error")}");
		}

		public static void Assert(this int code, string Message)
		{
			if (code != 0)
				throw new Exception($"Asserition failed: {code} {Message}");
		}

		public static IntPtr Assert(this IntPtr val, Func<string> MessageFun = null)
		{
			if (val == IntPtr.Zero)
				throw new Exception($"Asserition failed: pointer is null {(MessageFun?.Invoke() ?? "Unknown error")}");
			return val;
		}
	}
}
