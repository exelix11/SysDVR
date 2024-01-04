using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SysDVR.Client.Core;
using System.Linq;
using SDL2;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace SysDVR.Client.Targets.Player
{
    // On some devices (mainly mac os) audio doesn't work with our AudioStreamTarget implementation
    // It's unclear why but SDL_QueueAudio seems to always work
    // However we lose control over A/V syncrhonization so this is really just a workaround for some platforms
    class QueuedStreamAudioTarget : OutStream
    {
        // This is set by the SDL audio manager during initialization
        internal uint DeviceID;

        protected unsafe override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            fixed (byte* ptr = block.Span)
                SDL.SDL_QueueAudio(DeviceID, ptr, (uint)block.Span.Length);

            block.Free();
        }
    }

    static class AudioStreamTargetNative
    {
        public static unsafe void SDLCallback(IntPtr userdata, IntPtr buf, int len) =>
            ((AudioStreamTarget)GCHandle.FromIntPtr(userdata).Target).SDLCallback(new Span<byte>(buf.ToPointer(), len));
    }

    class AudioStreamTarget : OutStream
    {
        // Only as debug info
        public int Pending;
        readonly bool log = Program.Options.Debug.Log;        

        public StreamSynchronizationHelper SyncHelper;

        // Sample queue to be submitted to SDL
        readonly BlockingCollection<(PoolBuffer, ulong)> samples = new BlockingCollection<(PoolBuffer, ulong)>(20);

        // The block we are now submitting (already removed from the queue)
        PoolBuffer? currentBlock;

        // The offset in currentBlock. -1 means we need to get a new block
        int currentOffset = -1;

        public void SDLCallback(Span<byte> buffer)
        {
            // This function is tricky because we need to handle feeding SDL exactly the amount of data it needs
            // this means we might need to dynamically dequeue blocks if the current one is not enough

            // Where there is data to push
            while (buffer.Length != 0 && !Cancel.IsCancellationRequested)
            {
                // If the current block is not empty
                if (currentOffset != -1)
                {
                    // Do not overflow either buffer
                    int toCopy = Math.Min(currentBlock.Length - currentOffset, buffer.Length);
                    
                    // Perform the copy
                    currentBlock.RawBuffer.AsSpan().Slice(currentOffset, toCopy).CopyTo(buffer);

                    // Cut the SDL buffer to the remainig free space
                    buffer = buffer.Slice(toCopy);

                    // Advance the offset in the current block
                    currentOffset += toCopy;
                }

#if DEBUG
                // This should never happen...
                if (currentBlock is not null && currentOffset > currentBlock.Length)
                    Debugger.Break();
#endif

                // If the block is exhausted get rid of it
                if (currentBlock is not null && currentOffset >= currentBlock.Length)
                {
                    currentOffset = -1;
                    currentBlock.Free();
                    currentBlock = null;
				}

                // If there is no current block and we still need to push more data
                if (currentOffset == -1 && buffer.Length != 0)
                {
                    try
                    {
                    again:
                        // Get the next block
                        var (block, ts) = samples.Take(Cancel);
                        
                        // Make sure we can use it (note when SyncHelper is disabled this is always true)
                        if (!SyncHelper.CheckTimestamp(false, ts))
                        {
                            if (log)
                                Console.WriteLine($"Dropping audio packet with ts {ts}");

                            block.Free();
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

        protected override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            try
            {
                samples.Add((block, ts), Cancel);
				Pending = samples.Count;
				// Free is called by the consumer thread...
			}
			catch 
            {
                block.Free();
            }
        }

        protected override void DisposeImpl()
        {
			// Free any remaining elements
			currentOffset = -1;
			currentBlock?.Free();
			samples.ToList().ForEach(x => x.Item1.Free());
            
            base.Dispose();
        }
    }

    class H264StreamTarget : OutStream
    {
        Task VideoConsumerTask = null!;
        
        readonly Channel<(PoolBuffer, ulong)> videoBuffer = Channel.CreateBounded<(PoolBuffer, ulong)>(
            new BoundedChannelOptions(50)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true
            },
            (x) => {
                x.Item1.Free();
            }
        );

        public int Pending;

        DecoderContext ctx;
        int timebase_den;
        unsafe AVPacket* packet;
        StreamSynchronizationHelper sync;
        bool log;
        AutoResetEvent onFrame;

        // The video player consumer may block but we don't want that when streaming over USB
        // since the same thread handles both audio and video, we use a buffer to avoid blocking
        async Task ConsumeVideoAsync()
        {
            var reader = videoBuffer.Reader;
            while (!Cancel.IsCancellationRequested)
            {
                var (buf, ts) = await reader.ReadAsync(Cancel).ConfigureAwait(false);
                bool success = false;
                bool tsFailed = false;

                Interlocked.Decrement(ref Pending);

                // Try at most 5 times ( = 5 frames since we lock on the frame event)
                for (int i = 0; i < 4; i++)
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
                        // Adaptive rendering causes a lot of stuttering, for now avoid it in the video player
                        //Program.Instance.KickRendering(false);

                        // Normally this only happens if the UI thread is not pulling video frames like when the window is being dragged
                        // However it also seems to happen for some specific games so simply dropping packets will cause visual artifacts
                        // Wait for the next frame and submit again
                        // Since we don't check this every frame we may wake up too early but only for a frame so it's fine
                        onFrame.WaitOne(500);

                        if (Cancel.IsCancellationRequested)
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
                        // Tell the UI thread to start rendering again
                        Program.Instance.KickRendering(false);
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

        unsafe public H264StreamTarget()
        {
            packet = av_packet_alloc();
            log = Program.Options.Debug.Log;
        }

        unsafe ~H264StreamTarget()
        {
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

        protected override void UseCancellationTokenImpl(CancellationToken tok)
        {
            base.UseCancellationTokenImpl(tok);
            // Start the consumer only after the token is set
            VideoConsumerTask = Task.Run(ConsumeVideoAsync);
        }

        protected override async void DisposeImpl()
        {
            if (!Cancel.IsCancellationRequested)
                throw new Exception("Disposing without cancelling the token first");

            videoBuffer.Writer.Complete();

            try
            {
                // If the cancellation token is not set, the consumer thread has probably already terminated
                await VideoConsumerTask;
            }
            catch { /* ignore */ }

            // Free any remaining buffers
            videoBuffer.Reader.ReadAllAsync()
                .ToBlockingEnumerable()
                .ToList()
                .ForEach(x => x.Item1.Free());

            base.Dispose();
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

        protected override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            try
            {
				// Free is called by the consumer thread...
				_ = videoBuffer.Writer.WriteAsync((block, ts), Cancel);
                Interlocked.Increment(ref Pending);
            }
            catch 
            {
                block.Free();
			}
        }
    }
}
