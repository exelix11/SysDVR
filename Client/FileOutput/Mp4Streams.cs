using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FFmpeg.AutoGen;
using SysDVR.Client.Player;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.FileOutput
{
	unsafe class Mp4AudioTarget : IOutStream, IDisposable
	{
		AVFormatContext* OutCtx;
		AVCodecContext* CodecCtx;

		AVFrame* frame;
		AVPacket* packet;

		int channelId;

		bool running = true;
		public void Stop()
		{
			running = false;
		}

		public void UseContext(AVFormatContext* ctx, int id)
		{
			OutCtx = ctx;
			channelId = id;

			if (OutCtx->streams[id]->time_base.den != StreamInfo.AudioSampleRate)
				Console.WriteLine("Warning: time_base doesn't match the sample rate");

			var encoder = avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP3);

			CodecCtx = avcodec_alloc_context3(encoder);
			if (CodecCtx == null) throw new Exception("Couldn't allocate MP3 encoder");

			CodecCtx->sample_rate = StreamInfo.AudioSampleRate;
			CodecCtx->channels = StreamInfo.AudioChannels;
			CodecCtx->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
			CodecCtx->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
			CodecCtx->max_samples = StreamInfo.AudioSamplesPerPayload * StreamInfo.AudioChannels;
			CodecCtx->frame_size = StreamInfo.AudioSamplesPerPayload;
			CodecCtx->channel_layout = AV_CH_LAYOUT_STEREO;
			CodecCtx->bit_rate = 128000;
			CodecCtx->time_base = new AVRational { num = 1, den = StreamInfo.AudioSampleRate };

			avcodec_open2(CodecCtx, encoder, null).AssertNotNeg();

			frame = av_frame_alloc();
			if (frame == null) throw new Exception("Couldn't allocate AVFrame");
			frame->nb_samples = Math.Min(StreamInfo.AudioSamplesPerPayload, CodecCtx->frame_size);
			frame->format = (int)AVSampleFormat.AV_SAMPLE_FMT_FLTP;
			frame->channel_layout = AV_CH_LAYOUT_STEREO;
			frame->sample_rate = StreamInfo.AudioSampleRate;

			av_frame_get_buffer(frame, 0);

			packet = av_packet_alloc();
			if (packet == null) throw new Exception("Couldn't allocate AVPacket");
		}

		public void Dispose()
		{
			AVFrame* frame = this.frame;
			av_frame_free(&frame);

			AVPacket* packet = this.packet;
			av_packet_free(&packet);
		}

		long firstTs = -1;
		public void SendData(byte[] data, int offset, int size, ulong ts)
		{
			if (!running)
				return;

			if (firstTs == -1)
				firstTs = (long)ts;

			// Will break on big endian
			Span<short> d = MemoryMarshal.Cast<byte, short>(new Span<byte>(data, offset, size));
			long samplesSinceTs = 0;
			while (d.Length > 0)
			{
				int SamplesInBlock = Math.Min(
					d.Length / 2, // Two channels array of shorts
					frame->linesize[0] / 4 // Single channel array of bytes, 4 byte per sample
				);
				
				Span<float> l = new Span<float>(frame->data[0], frame->linesize[0] / sizeof(float));
				Span<float> r = new Span<float>(frame->data[1], frame->linesize[0] / sizeof(float));
				
				for (int i = 0; i < SamplesInBlock; i++)
				{
					l[i] = d[i * 2] / -(float)Int16.MinValue;
					r[i] = d[i * 2 + 1] / -(float)Int16.MinValue;
				}
				
				d = d.Slice(SamplesInBlock * 2);

				// We use the sample rate as time base so we can just add samples to the pts
				frame->pts = ((long)ts - firstTs) * StreamInfo.AudioSampleRate / (long)1E+6 + samplesSinceTs;
				samplesSinceTs += SamplesInBlock;

				avcodec_send_frame(CodecCtx, frame);
				//Console.Write("" + avcodec_send_frame(CodecCtx, frame) + " ");
				if (avcodec_receive_packet(CodecCtx, packet) < 0)
					continue;

				packet->stream_index = channelId;
				av_interleaved_write_frame(OutCtx, packet).AssertNotNeg();
			}
		}

		public void UseCancellationToken(CancellationToken tok)
		{

		}
	}

	unsafe class Mp4VideoTarget : IOutStream
	{
		AVFormatContext* OutCtx;
		int timebase_den;

		bool running = true;
		public void Stop() 
		{
			running = false;
		}

		public void UseContext(AVFormatContext* ctx)
		{
			OutCtx = ctx;
			timebase_den = OutCtx->streams[0]->time_base.den;
		}

		long firstTs = -1;
		long dts = 0;
		public void SendData(byte[] data, int offset, int size, ulong ts)
		{
			if (!running)
				return;

			// Must add SPS and PPS to the first frame manually to keep ffmpeg happy
			if (firstTs == -1)
			{
				byte[] next = new byte[size + StreamInfo.SPS.Length + StreamInfo.PPS.Length];
				Buffer.BlockCopy(StreamInfo.SPS, 0, next, 0, StreamInfo.SPS.Length);
				Buffer.BlockCopy(StreamInfo.PPS, 0, next, StreamInfo.SPS.Length, StreamInfo.PPS.Length);
				Buffer.BlockCopy(data, offset, next, StreamInfo.SPS.Length + StreamInfo.PPS.Length, size);

				data = next;
				size = next.Length;

				firstTs = (long)ts;				
			}

			fixed (byte* nal_data = data)
			{
				AVPacket pkt;
				av_init_packet(&pkt);

				pkt.data = nal_data + offset;
				pkt.size = size;
				pkt.dts = dts++;
				pkt.pts = ((long)ts - firstTs) * timebase_den / (long)1E+6;
				pkt.stream_index = 0;

				//Console.WriteLine($"{size:x5} {ts} {pkt.dts}");

				av_write_frame(OutCtx, &pkt).AssertNotNeg();
			}
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			//throw new NotImplementedException();
		}
	}
}
