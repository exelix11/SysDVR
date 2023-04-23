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

	class AudioStreamTarget : IOutStream
	{
		readonly BlockingCollection<(PoolBuffer, ulong)> samples = new BlockingCollection<(PoolBuffer, ulong)>(20);
		readonly bool log = DebugOptions.Current.Log;

		CancellationToken tok;
		StreamSynchronizationHelper sync;

		public void UseSynchronizationHeloper(StreamSynchronizationHelper sync)
		{
			this.sync = sync;
		}

        PoolBuffer currentBlock;
		int currentOffset = -1;
		public void SDLCallback(Span<byte> buffer) 
		{
			while (buffer.Length != 0 && !tok.IsCancellationRequested)
			{
				if (currentOffset != -1)
				{
					int toCopy = Math.Min(currentBlock.Length - currentOffset, buffer.Length);
					currentBlock.RawBuffer.AsSpan().Slice(currentOffset, toCopy).CopyTo(buffer);

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
						again:
                        var (block, ts) = samples.Take(tok);
						if (!sync.CheckTimestamp(false, ts))
						{
							if (log)
								Console.WriteLine($"Dropping audio packet with ts {ts}");
                            goto again;
						}

                        currentBlock = block;
						currentOffset = 0;
					}
					catch (OperationCanceledException)
					{
						return;
					}
				}
			}
		}

		public unsafe void SendData(PoolBuffer block, UInt64 ts)
		{
			samples.Add((block, ts), tok);
		}

		public void UseCancellationToken(CancellationToken tok)
		{
			this.tok = tok;
		}
	}

	unsafe class H264StreamTarget : IOutStream
	{
		DecoderContext ctx;
		CancellationToken tok;
		int timebase_den;
		AVPacket* packet;
		StreamSynchronizationHelper sync;
		bool log;

        unsafe public H264StreamTarget() {
			packet = av_packet_alloc();
			log = DebugOptions.Current.Log;
        }

		unsafe ~H264StreamTarget() {
			fixed (AVPacket** p = &packet)
				av_packet_free(p);
		}

		unsafe public void UseContext(DecoderContext ctx)
		{
			this.ctx = ctx;
			timebase_den = ctx.CodecCtx->time_base.den;
			sync = ctx.SyncHelper;
        }

		public void UseCancellationToken(CancellationToken tok)
		{
			this.tok = tok;
		}

		long firstTs = -1;
		public unsafe void SendData(PoolBuffer data, ulong ts)
		{
			byte[] buffer = data.RawBuffer;
			int size = data.Length;

            if (firstTs == -1)
				firstTs = (long)ts;

			if (sync.CheckTimestamp(true, ts))
			{
				fixed (byte* nal_data = buffer)
				{
					var pkt = packet;
					pkt->data = nal_data;
					pkt->size = size;
					pkt->pts = pkt->dts = (long)(((long)ts - firstTs) / 1E+6 * timebase_den);

					int res = 0;
					int resendCount = 0;

				send_again:
					lock (ctx.CodecLock)
						res = avcodec_send_packet(ctx.CodecCtx, pkt);

					if (res == AVERROR(EAGAIN))
					{
						// Normally this only happens if the UI thread is not pulling video frames like when the window is being dragged
						// Since this is not threaded anymore if we block here the device thread also blocks, possibly causing desync, so just discard the packet after a few attempts
						if (resendCount < 60 && !tok.IsCancellationRequested)
						{
							Thread.Sleep(1);
							resendCount++;
							goto send_again;
						}
					}
					else if (res != 0)
					{
						Console.WriteLine($"avcodec_send_packet {res}");
					}
				}
			}
			else if (log)
				Console.WriteLine($"Dropping video packet with ts {ts}");

            data.Free();
		}
	}
}
