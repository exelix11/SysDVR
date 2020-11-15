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

namespace SysDVR.Client.Player
{
	struct SDLContext
	{
		public volatile bool Initialized;

		public IntPtr Texture;
		public IntPtr Renderer;
		public IntPtr Window;
		public SDL_Rect TextureSize;

		public object TextureLock;
	}

	unsafe struct SDLAudioContext
	{
		public SwrContext* ConverterCtx;
		public SDL_AudioSpec AudioSpec;
		public int ChannelCount, SampleSize, SampleRate;
		public GCHandle TargetHandle;
	}

	unsafe struct DecoderContext
	{
		public AVCodecContext* CodecCtx;
		public AVFrame* OutFrame;

		public SwsContext* ConverterContext;
		public AVFrame* ConvertedFrame;

		public object CodecLock;
	}

	class PlayerManager : BaseStreamManager, IDisposable
	{
		readonly Player player;
		private bool disposedValue;

		public PlayerManager(bool HasVideo, bool HasAudio, bool hwAcc, string codecName) : base(
			HasVideo ? new H264StreamTarget() : null,
			HasAudio ? new AudioStreamTarget() : null)
		{
			player = new Player(this, hwAcc, codecName);
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

		protected bool ShouldQuit = true;

		bool Running = false;
		protected Thread ReceiveThread;
		volatile bool TextureConsumed = true;

		protected SDLContext SDL; // This must be initialized by the UI thread
		protected readonly SDLAudioContext SDLAudio;
		protected readonly DecoderContext Decoder;

		SDLContext InitSDLVideo()
		{
			SDL_InitSubSystem(SDL_INIT_VIDEO).Assert(SDL_GetError);

			var win = SDL_CreateWindow("SysDVR-Client", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, StreamInfo.VideoWidth, 
				StreamInfo.VideoHeight,	SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE).Assert(SDL_GetError);

			var render = SDL_CreateRenderer(win, -1,
				SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
				SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC |
				SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE)
				.Assert(SDL_GetError);

			var tex = SDL_CreateTexture(render, SDL_PIXELFORMAT_YV12,
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
		
		unsafe SDLAudioContext InitSDLAudio(AudioStreamTarget target)
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

			SDL_OpenAudio(ref wantedSpec, out SDL_AudioSpec spec).Assert(SDL_GetError);

			SwrContext* swrctx = null;

			if (spec.channels != wantedSpec.channels ||
				spec.format != wantedSpec.format ||
				spec.freq != wantedSpec.freq)
			{
				swrctx = swr_alloc();
				if (swrctx == null)
					throw new Exception("Couldn't allocate the audio converter context");

				av_opt_set_int(swrctx, "in_channel_layout", AV_CH_LAYOUT_STEREO, 0).Assert("av_opt_set_int in_channel_layout");
				av_opt_set_int(swrctx, "in_sample_rate", StreamInfo.AudioSampleRate, 0).Assert("av_opt_set_int in_sample_rate");
				av_opt_set_sample_fmt(swrctx, "in_sample_fmt", AVSampleFormat.AV_SAMPLE_FMT_S16, 0).Assert("av_opt_set_sample_fmt in_sample_fmt");

				if (spec.channels != 1 && spec.channels != 2)
					throw new Exception("Invalid channel count");

				av_opt_set_int(swrctx, "out_channel_layout",
					spec.channels == 1 ? AV_CH_LAYOUT_MONO : AV_CH_LAYOUT_STEREO, 0).Assert("av_opt_set_int out_channel_layout");

				av_opt_set_int(swrctx, "out_sample_rate", spec.freq, 0).Assert("av_opt_set_int out_sample_rate");

				av_opt_set_sample_fmt(swrctx, "out_sample_fmt", spec.format switch
				{
					AUDIO_S16 => AVSampleFormat.AV_SAMPLE_FMT_S16,
					AUDIO_S32 => AVSampleFormat.AV_SAMPLE_FMT_S32,
					AUDIO_F32 => AVSampleFormat.AV_SAMPLE_FMT_FLT,
					_ => throw new NotImplementedException(),
				}, 0).Assert("av_opt_set_sample_fmt out_sample_fmt");

				swr_init(swrctx).Assert("Couldn't initialize the audio converter");
			}

			return new SDLAudioContext
			{
				AudioSpec = spec,
				ChannelCount = spec.channels,
				ConverterCtx = swrctx,
				SampleRate = spec.freq,
				SampleSize = spec.format switch
				{
					AUDIO_S16 => 2,
					AUDIO_S32 => 4,
					AUDIO_F32 => 4,
					_ => throw new NotImplementedException(),
				},
				TargetHandle = handle
			};
		}

		unsafe DecoderContext InitDecoder(string forceDecoder)
		{				
			var codec = avcodec_find_decoder_by_name(forceDecoder);

			if (codec == null)
			{
				Console.WriteLine($"ERROR: The codec {forceDecoder} Couldn't be initialized, falling back to default decoder.");
				return InitDecoder(true);
			}
			else
			{
				Console.WriteLine("You manually set a video decoder with the --decoder option, in case of issues remove it to use the default one.");
				return InitDecoder(codec);
			}
		}

		unsafe DecoderContext InitDecoder(bool hwAcc)
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

		// Some codecs such as h265 and h264_v4l2m2m don't expose the format they support as pix_fmts so look them up manually
		// Todo: add more codecs that cause issues
		private static AVPixelFormat GetPixFormat(string codecName) => codecName switch 
		{
			"h264_v4l2m2m" => AVPixelFormat.AV_PIX_FMT_NV12,
			"h264" => AVPixelFormat.AV_PIX_FMT_YUV420P,
			_ => AVPixelFormat.AV_PIX_FMT_NV12
		};

		unsafe DecoderContext InitDecoder(AVCodec* codec)
		{
			if (codec == null)
				throw new Exception("Codec can't be null");

			string codecName = Marshal.PtrToStringAnsi((IntPtr)codec->name);

			Console.WriteLine($"Initializing video player with {codecName} codec.");

			var codectx = avcodec_alloc_context3(codec);
			if (codectx == null)
				throw new Exception("Couldn't allocate a codec context");

			codectx->width = StreamInfo.VideoWidth;
			codectx->height = StreamInfo.VideoHeight;

			// Actually no clue about this part, what's the right format to set when an encoder doesn't provide any ?
			codectx->pix_fmt = 
				codec->pix_fmts == null || *codec->pix_fmts == AVPixelFormat.AV_PIX_FMT_NONE ? // If the codec doesn't provide any format
				GetPixFormat(codecName) : // Use ours
				*codec->pix_fmts; // Otherwise use the codec's

			avcodec_open2(codectx, codec, null).Assert("Couldn't open the codec.");

#if DEBUG
			Console.WriteLine($"Using pixel format {codectx->pix_fmt}");
#endif

			var pic = av_frame_alloc();
			if (pic == null)
				throw new Exception("Couldn't allocate the decoding frame");

			AVFrame* dstframe = null;
			SwsContext* swsContext = null;
			// If we're using a format we can't display initialize the converter
			if (codectx->pix_fmt != AVPixelFormat.AV_PIX_FMT_YUV420P)
			{
				dstframe = av_frame_alloc();

				if (dstframe == null)
					throw new Exception("Couldn't allocate the the converted frame");

				dstframe->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;
				dstframe->width = StreamInfo.VideoWidth;
				dstframe->height = StreamInfo.VideoHeight;

				av_frame_get_buffer(dstframe, 32).Assert("Couldn't allocate the buffer for the converted frame");

				swsContext = sws_getContext(codectx->width, codectx->height, codectx->pix_fmt,
											dstframe->width, dstframe->height, AVPixelFormat.AV_PIX_FMT_YUV420P,
											SWS_BILINEAR, null, null, null);

				if (swsContext == null)
					throw new Exception("Couldn't initialize the converter");
			}

			return new DecoderContext()
			{
				CodecCtx = codectx,
				OutFrame = pic,
				ConverterContext = swsContext,
				ConvertedFrame = dstframe,
				CodecLock = new object()
			};
		}

		public void UiThreadMain()
		{
			SDL = InitSDLVideo();

			SDL_Rect DisplayRect = new SDL_Rect { x = 0, y = 0 };

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
					lock (SDL.TextureLock)
					{
						SDL_RenderCopy(SDL.Renderer, SDL.Texture, in SDL.TextureSize, in DisplayRect);
						SDL_RenderPresent(SDL.Renderer);
						TextureConsumed = true;
					}
				else
				{
					for (int i = 0; i < 20 && TextureConsumed; i++)
						Thread.Sleep(5); // If no new frame wait up to 100ms before polling events again
					if (!TextureConsumed)
						continue;
				}

				int res;
				do
				{
					res = SDL_PollEvent(out var evt);
					if (evt.type == SDL_EventType.SDL_QUIT ||
						evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
						ShouldQuit = true;
					else if (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
						CalculateDisplayRect();
				} while (res > 0);
			}
		}

		unsafe void DecodeReceiverThreadMain()
		{
			while (!SDL.Initialized && !ShouldQuit)
				Thread.Sleep(100);

			// Copy state to locals for faster access
			var sdl = SDL;
			var dec = Decoder;

			while (!ShouldQuit)
			{
				int ret = 0;
				lock (dec.CodecLock)
					ret = avcodec_receive_frame(dec.CodecCtx, dec.OutFrame);

				if (ret == AVERROR(EAGAIN))
					Thread.Sleep(2);
				else if (ret != 0)
					Console.WriteLine($"avcodec_receive_frame {ret}");
				else
				{
					while (!TextureConsumed && !ShouldQuit)
						Thread.Sleep(2);

					if (ShouldQuit)
						break;

					var pic = dec.OutFrame;

					if (dec.ConverterContext != null)
					{
						sws_scale(dec.ConverterContext, pic->data, pic->linesize, 0, pic->height, dec.ConvertedFrame->data, dec.ConvertedFrame->linesize);
						pic = dec.ConvertedFrame;
					}

					lock (sdl.TextureLock)
					{
						SDL_UpdateYUVTexture(sdl.Texture, in sdl.TextureSize,
							(IntPtr)pic->data[0], pic->linesize[0],
							(IntPtr)pic->data[1], pic->linesize[1],
							(IntPtr)pic->data[2], pic->linesize[2]);

						TextureConsumed = false;
					}

					// TODO: figure out synchronization. 
					// The current implementation shows video as fast as it arrives, it seems to work fine but not sure if it's correct.
				}
			}
		}

		public Player(PlayerManager owner, bool hwAcc, string codecName)
		{
			HasAudio = owner.HasAudio;
			HasVideo = owner.HasVideo;

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
				((AudioStreamTarget)owner.AudioTarget).UseContext(SDLAudio);
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

				if (HasAudio && SDLAudio.ConverterCtx != null)
				{
					var ctx = SDLAudio.ConverterCtx;
					swr_free(&ctx);
				}

				if (HasVideo)
				{
					var ptr = Decoder.OutFrame;
					av_frame_free(&ptr);
				
					var ptr2 = Decoder.CodecCtx;
					avcodec_free_context(&ptr2);
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
