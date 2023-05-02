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

	unsafe class H264StreamTarget : IOutStream, IDisposable
	{
        readonly Thread VideoConsumerThread;
        readonly BlockingCollection<(PoolBuffer, ulong)> videoBUffer = new BlockingCollection<(PoolBuffer, ulong)>(50);

        DecoderContext ctx;
		CancellationToken tok;
		int timebase_den;
		AVPacket* packet;
		StreamSynchronizationHelper sync;
		bool log;
		AutoResetEvent onFrame;

        // The video player consumer may block but we don't want that when streaming over USB
		// since the same thread handles both audio and video, we use a buffer to avoid blocking
        void VideoConsumerMain()
        {
            try
            {
                // We should not run out of capacity because the target will start dropping packets once their timestamp is too old
                foreach (var (buf, ts) in videoBUffer.GetConsumingEnumerable(tok))
                {
					bool success = false;
					bool tsFailed = false;

					// Try at most 5 times ( = 5 frames since we lock on the frame event)
					for (int i = 0; i < 20; i++)
					{
						// Drop the packet if while we were trying to send it audio went out of sync
						if (!sync.CheckTimestamp(true, ts))
						{
							tsFailed = true;
                            break;
                        }

                        var res = DecodePacket(buf, ts);

						if (res == AVERROR(EAGAIN))
						{
							// Normally this only happens if the UI thread is not pulling video frames like when the window is being dragged
							// However it also seems to happen for some specific games so simply dropping packets will cause visual artifacts
							// Wait for the next frame and submit again
							// Since we don't check this every frame we may wake up too early but only for a frame so it's fine
							onFrame.WaitOne(500);

                            if (tok.IsCancellationRequested)
								break;
						}
						else if (res != 0)
						{
							Console.WriteLine($"avcodec_send_packet {res}");
							break;
						}
						else
						{
							success = true;
							break;
						}
                    }

					if (!success && log)
					{
						if (tsFailed)
							Console.WriteLine($"Dropping video packet with ts {ts} to resync audio");
                        else
							Console.WriteLine($"Dropping video packet because the UI thread is too late");
                    }

                    buf.Free();
                }
            }
            catch
            {
                // Exception is thrown when consumer finishes or token is cancelled
            }
        }

        unsafe public H264StreamTarget() {
			packet = av_packet_alloc();
			log = DebugOptions.Current.Log;
			VideoConsumerThread = new Thread(VideoConsumerMain);
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
			onFrame = ctx.OnFrameEvent;
        }

		public void UseCancellationToken(CancellationToken tok)
		{
			this.tok = tok;
			// Start the consumer only after the token is set
			VideoConsumerThread.Start();
        }

		// Queue the packet so it's not blocking
        public void SendData(PoolBuffer data, ulong ts)
        {
            videoBUffer.Add((data, ts), tok);
        }

        public void Dispose()
        {
			// If the cancellation token is not set, the consumer thread has probably already terminated
			VideoConsumerThread.Join();
        }

        long firstTs = -1;
		public unsafe int DecodePacket(PoolBuffer data, ulong ts)
		{
			byte[] buffer = data.RawBuffer;
			int size = data.Length;

            if (firstTs == -1)
				firstTs = (long)ts;

            fixed (byte* nal_data = buffer)
            {
                var pkt = packet;
                pkt->data = nal_data;
                pkt->size = size;
                pkt->pts = pkt->dts = (long)(((long)ts - firstTs) / 1E+6 * timebase_den);

                lock (ctx.CodecLock)
                    return avcodec_send_packet(ctx.CodecCtx, pkt);
            }
		}
    }
}
