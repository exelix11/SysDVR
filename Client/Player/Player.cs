using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL2;
using static SDL2.SDL;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System.Threading.Tasks;
using System.Threading;

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

		public object CodecLock;
	}

	class PlayerManager : BaseStreamManager, IDisposable
	{
		readonly Player player;
		private bool disposedValue;

		public PlayerManager(bool HasVideo, bool HasAudio) : base(
			HasVideo ? new H264StreamTarget() : null,
			HasAudio ? new AudioStreamTarget() : null)
		{
			Helper.WindowsDllWorkadround();
			player = new Player(this);
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
		protected bool ShouldQuit = true;
		volatile bool TextureConsumed = true;

		protected SDLContext SDL; // This must be initialized by the UI thread
		protected readonly SDLAudioContext SDLAudio;
		protected readonly DecoderContext Decoder;

		SDLContext InitSDL()
		{
			SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO).Assert();

			var win = SDL_CreateWindow("SysDVR-Client", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, StreamInfo.VideoWidth, 
				StreamInfo.VideoHeight,	SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE).Assert();

			var render = SDL_CreateRenderer(win, -1,
				SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
				SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC |
				SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE)
				.Assert();

			var tex = SDL_CreateTexture(render, SDL_PIXELFORMAT_YV12,
				(int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
				StreamInfo.VideoWidth, StreamInfo.VideoHeight).Assert();

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

			SDL_OpenAudio(ref wantedSpec, out SDL_AudioSpec spec).Assert();

			SwrContext* swrctx = null;

			if (spec.channels != wantedSpec.channels ||
				spec.format != wantedSpec.format ||
				spec.freq != wantedSpec.freq)
			{
				swrctx = swr_alloc();
				if (swrctx == null)
					throw new Exception();

				av_opt_set_int(swrctx, "in_channel_layout", AV_CH_LAYOUT_STEREO, 0).Assert();
				av_opt_set_int(swrctx, "in_sample_rate", StreamInfo.AudioSampleRate, 0).Assert();
				av_opt_set_sample_fmt(swrctx, "in_sample_fmt", AVSampleFormat.AV_SAMPLE_FMT_S16, 0).Assert();

				if (spec.channels != 1 && spec.channels != 2)
					throw new Exception("Invalid channel count");

				av_opt_set_int(swrctx, "out_channel_layout",
					spec.channels == 1 ? AV_CH_LAYOUT_MONO : AV_CH_LAYOUT_STEREO, 0).Assert();

				av_opt_set_int(swrctx, "out_sample_rate", spec.freq, 0).Assert();

				av_opt_set_sample_fmt(swrctx, "out_sample_fmt", spec.format switch
				{
					AUDIO_S16 => AVSampleFormat.AV_SAMPLE_FMT_S16,
					AUDIO_S32 => AVSampleFormat.AV_SAMPLE_FMT_S32,
					AUDIO_F32 => AVSampleFormat.AV_SAMPLE_FMT_FLT,
					_ => throw new NotImplementedException(),
				}, 0).Assert();

				swr_init(swrctx).Assert();
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

		unsafe DecoderContext InitDecoder()
		{
			var codec = avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
			if (codec == null)
				throw new Exception();

			var codectx = avcodec_alloc_context3(codec);
			if (codectx == null)
				throw new Exception();

			codectx->width = StreamInfo.VideoWidth;
			codectx->height = StreamInfo.VideoHeight;
			codectx->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;

			avcodec_open2(codectx, codec, null).Assert();

			if (codectx->hwaccel != null)
				Console.WriteLine($"Using hardware acceleration: {Marshal.PtrToStringBSTR((IntPtr)codectx->hwaccel->name)}");
			else
				Console.WriteLine($"No HW accel");

			var pic = av_frame_alloc();
			if (pic == null)
				throw new Exception();

			return new DecoderContext()
			{
				CodecCtx = codectx,
				OutFrame = pic,
				CodecLock = new object()
			};
		}

		void RenderingThreadMain()
		{
			SDL = InitSDL();

			SDL_Rect DisplayRect = new SDL_Rect { x = 0, y = 0 };

			void CalculateDisplayRect()
			{
				SDL_GetWindowSize(SDL.Window, out int w, out int h);
				const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;

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
					for (int i = 0; i < 10 && TextureConsumed; i++)
						SDL_Delay(10);
					if (TextureConsumed)
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

			while (!ShouldQuit)
			{
				int ret = 0;
				lock (Decoder.CodecLock)
					ret = avcodec_receive_frame(Decoder.CodecCtx, Decoder.OutFrame);

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

					lock (SDL.TextureLock)
					{
						var pic = Decoder.OutFrame;

						SDL_UpdateYUVTexture(SDL.Texture, in SDL.TextureSize,
							(IntPtr)pic->data[0], pic->linesize[0],
							(IntPtr)pic->data[1], pic->linesize[1],
							(IntPtr)pic->data[2], pic->linesize[2]);

						TextureConsumed = false;
					}

					// TODO: This never seems to happen. figure out proper timings
					if (Decoder.OutFrame->pkt_duration != 0)
						Thread.Sleep((int)Decoder.OutFrame->pkt_duration);
				}
			}
		}

		readonly bool HasAudio;
		readonly bool HasVideo;
		public Player(PlayerManager owner)
		{
			HasAudio = owner.HasAudio;
			HasVideo = owner.HasVideo;

			if (!HasAudio && !HasVideo)
				throw new Exception("Can't start a player with no streams");

			if (HasVideo)
			{
				Decoder = InitDecoder();
				((H264StreamTarget)owner.VideoTarget).UseContext(Decoder);
			}

			if (HasAudio)
			{
				SDLAudio = InitSDLAudio((AudioStreamTarget)owner.AudioTarget);
				((AudioStreamTarget)owner.AudioTarget).UseContext(SDLAudio);
			}
		}

		public bool IsRunning => ReceiveThread != null;
		Thread ReceiveThread;
		Thread UIThread;
		public void Start() 
		{
			if (!ShouldQuit)
				throw new Exception("Already started");

			ShouldQuit = false;

			if (HasAudio)
				SDL_PauseAudio(0);

			if (HasVideo)
			{
				ReceiveThread = new Thread(DecodeReceiverThreadMain);
				UIThread = new Thread(RenderingThreadMain);
				ReceiveThread.Start();
				UIThread.Start();
			}
		}

		public void Stop() 
		{
			ShouldQuit = true;

			if (HasAudio)
				SDL_PauseAudio(0);

			if (HasVideo)
			{
				UIThread.Join();
				UIThread = null;
				ReceiveThread.Join();
				ReceiveThread = null;
			}
		}

		private bool disposedValue;
		protected unsafe virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (IsRunning)
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
		public static void Assert(this int code, int expectedValue)
		{
			if (code != expectedValue)
				throw new Exception($"Asserition failed {code} != {expectedValue}");
		}

		public static void Assert(this int code)
		{
			if (code != 0)
				throw new Exception("Asserition failed");
		}

		public static IntPtr Assert(this IntPtr val)
		{
			if (val == IntPtr.Zero)
				throw new Exception("Asserition failed");
			return val;
		}
	}
}
