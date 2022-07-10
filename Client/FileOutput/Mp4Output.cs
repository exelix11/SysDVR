using FFmpeg.AutoGen;
using SysDVR.Client.Player;
using System;
using static FFmpeg.AutoGen.ffmpeg;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace SysDVR.Client.FileOutput
{
	class Mp4OutputManager : BaseStreamManager, IDisposable
	{
		private bool disposedValue;
		readonly Mp4Output output;

		public Mp4OutputManager(string filename, bool HasVideo, bool HasAudio) : base(
			HasVideo ? new Mp4VideoTarget() : null,
			HasAudio ? new Mp4AudioTarget() : null)
		{
			output = new Mp4Output(filename, this);
		}

		public override void Begin()
		{
			// Open output handles before launching threads
			output.Start();
			base.Begin();
		}

		public override void Stop()
		{
			// Close the output first because sometimes the other threads can get stuck (especially with USB) and prevent the recorder from finalizing the file.
			output.Stop();
			base.Stop();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposedValue)
			{
				if (disposing)
				{
					output.Dispose();
				}

				disposedValue = true;
			}
		}
	}

	unsafe class Mp4Output : IDisposable
	{
		bool Running = false;
		string Filename;
		
		AVFormatContext* OutCtx;
		AVStream* VStream, AStream;

		Mp4VideoTarget Vid;
		Mp4AudioTarget Aud;

		public Mp4Output(string filename, Mp4OutputManager manager)
		{
			Vid = manager.VideoTarget as Mp4VideoTarget;
			Aud = manager.AudioTarget as Mp4AudioTarget;
			Filename = filename;
		}

		public void Start() 
		{
			var OutFmt = av_guess_format(null, Filename, null);
			if (OutFmt == null) throw new Exception("Couldn't find output format");

			AVFormatContext* ctx = null;
			avformat_alloc_output_context2(&ctx, OutFmt, null, null).AssertNotNeg();
			OutCtx = ctx != null ? ctx : throw new Exception("Couldn't allocate output context");

			if (Vid != null)
			{
				VStream = avformat_new_stream(OutCtx, avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264));
				if (VStream == null) throw new Exception("Couldn't allocate video stream");

				VStream->codecpar->codec_id = AVCodecID.AV_CODEC_ID_H264;
				VStream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
				VStream->codecpar->width = StreamInfo.VideoWidth;
				VStream->codecpar->height = StreamInfo.VideoHeight;
				VStream->codecpar->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;

				/* 
				 * TODO: This is needed for MKV files but doesn't seem to be quite right: 
				 * ffmpeg shows several errors and seeking in mpv doesn't work. Adding this to mp4 files breaks video in the windows 10 video player.
				*/
				//var (ptr, sz) = LibavUtils.AllocateH264Extradata();;
				//VStream->codecpar->extradata = (byte*)ptr.ToPointer();
				//VStream->codecpar->extradata_size = sz;
			}

			if (Aud != null)
			{
				AStream = avformat_new_stream(OutCtx, avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP2));
				if (AStream == null) throw new Exception("Couldn't allocate audio stream");
			
				AStream->id = Vid == null ? 0 : 1;
				AStream->codecpar->codec_id = AVCodecID.AV_CODEC_ID_MP2;
				AStream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
				AStream->codecpar->sample_rate = StreamInfo.AudioSampleRate;
				AStream->codecpar->channels = StreamInfo.AudioChannels;
				AStream->codecpar->format = (int)AVSampleFormat.AV_SAMPLE_FMT_S16;
				AStream->codecpar->channel_layout = AV_CH_LAYOUT_STEREO;
				AStream->codecpar->frame_size = StreamInfo.AudioSamplesPerPayload;
				AStream->codecpar->bit_rate = 128000;
			}

			avio_open(&OutCtx->pb, Filename, AVIO_FLAG_WRITE).Assert();
			avformat_write_header(OutCtx, null).Assert();

			object sync = new object();
			Vid?.StartWithContext(OutCtx, sync);
			Aud?.StartWithContext(OutCtx, sync, AStream->id);

			var defColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("if you close SysDVR-Client via the X button the output video may become corrupted.");
			Console.ForegroundColor = defColor;

			Running = true;
		}

		public unsafe void Stop() 
		{
			Aud?.Stop();
			Vid?.Stop();

			Console.WriteLine("Finalizing file...");

			av_write_trailer(OutCtx);
			avio_close(OutCtx->pb);
			avformat_free_context(OutCtx);	
			OutCtx = null;

			Running = false;
			
			Aud?.Dispose();
		}

		private bool disposedValue;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (Running)
					Stop();

				disposedValue = true;
			}
		}

		~Mp4Output()
		{
		    Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
