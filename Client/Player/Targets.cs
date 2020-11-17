using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using SysDVR.Client;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SysDVR.Client.Player
{
	static class AudioStreamTargetNative
	{
		public static unsafe void SDLCallback(IntPtr userdata, IntPtr buf, int len) =>
			((AudioStreamTarget)GCHandle.FromIntPtr(userdata).Target).SDLCallback(new Span<byte>(buf.ToPointer(), len));
	}

	class AudioStreamTarget : IOutTarget
	{
		readonly BlockingCollection<PoolBuffer> samples = new BlockingCollection<PoolBuffer>(20);
		CancellationToken tok;

		PoolBuffer currentBlock;
		int currentOffset = -1;
		public void SDLCallback(Span<byte> buffer) 
		{
			while (buffer.Length != 0 && !tok.IsCancellationRequested)
			{
				if (currentOffset != -1)
				{
					int toCopy = Math.Min(currentBlock.Length - currentOffset, buffer.Length);
					currentBlock.Buffer.AsSpan().Slice(currentOffset, toCopy).CopyTo(buffer);

					buffer = buffer.Slice(toCopy);
					currentOffset += toCopy;
				}

				if (currentOffset >= currentBlock.Length)
				{
					currentOffset = -1;
					currentBlock.Free();
				}

				if (currentOffset == -1 && buffer.Length != 0)
				{
					try
					{
						currentBlock = samples.Take(tok);
						currentOffset = 0;
					}
					catch (OperationCanceledException)
					{
						return;
					}
				}
			}
		}

		private SDLAudioContext AudioCtx = new SDLAudioContext { };
		public unsafe void SendData(PoolBuffer block, UInt64 ts)
		{
			if (AudioCtx.ConverterCtx != null) { 
				int inSamples = block.Length / StreamInfo.AudioSampleSize / StreamInfo.AudioChannels;

				var outSamples = (int)av_rescale_rnd(
					swr_get_delay(AudioCtx.ConverterCtx, StreamInfo.AudioSampleRate) + inSamples,
					StreamInfo.AudioSampleRate, AudioCtx.SampleRate, AVRounding.AV_ROUND_UP);

				if (outSamples <= 0)
					throw new Exception($"ERROR: Calculated sample size is {outSamples}");

				var outData = PoolBuffer.Rent(outSamples * AudioCtx.ChannelCount * AudioCtx.SampleSize);

				fixed (byte* outbuf = outData.Buffer)
				fixed (byte* inbuf = block.Buffer)
					swr_convert(AudioCtx.ConverterCtx, &outbuf, outSamples, &inbuf, inSamples).AssertEqual(outSamples);

				block.Free();
				samples.Add(outData, tok);
			}
			else samples.Add(block, tok);
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			this.tok = tok;
		}

		public void SendData(byte[] data, int offset, int size, ulong ts)
		{
			throw new NotImplementedException("For efficiency only the PoolBuffer interface can be used with this class");
		}

		internal void UseContext(SDLAudioContext ctx)
		{
			AudioCtx = ctx;
		}
	}

	class H264StreamTarget : IOutTarget
	{
		DecoderContext ctx;
		CancellationToken tok;

		public void UseContext(DecoderContext ctx)
		{
			this.ctx = ctx;
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			this.tok = tok;
		}

		bool FirstPacket = true;
		public unsafe void SendData(byte[] data, int offset, int size, ulong ts)
		{
			// Must add SPS and PPS to the first frame manually to keep ffmpeg happy
			if (FirstPacket)
			{
				byte[] next = new byte[size + StreamInfo.SPS.Length + StreamInfo.PPS.Length];
				Buffer.BlockCopy(StreamInfo.SPS, 0, next, 0, StreamInfo.SPS.Length);
				Buffer.BlockCopy(StreamInfo.PPS, 0, next, StreamInfo.SPS.Length, StreamInfo.PPS.Length);
				Buffer.BlockCopy(data, offset, next, StreamInfo.SPS.Length + StreamInfo.PPS.Length, size);

				data = next;
				size = next.Length;

				FirstPacket = false;
			}

			fixed (byte* nal_data = data)
			{
				AVPacket pkt;
				av_init_packet(&pkt);

				pkt.data = nal_data + offset;
				pkt.size = size;
				pkt.pts = (long)ts;

				int res = 0;

			send_again:
				lock (ctx.CodecLock)
					res = avcodec_send_packet(ctx.CodecCtx, &pkt);
				if (res == AVERROR(EAGAIN))
				{
					Thread.Sleep(2);
					if (!tok.IsCancellationRequested)
						goto send_again;
				}
				else if (res != 0)
				{
					Console.WriteLine($"avcodec_send_packet {res}");
				}
			}
		}
	}
}
