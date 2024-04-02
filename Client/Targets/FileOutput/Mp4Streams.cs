using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Targets.FileOutput
{
    static class FirstTimestamp
    {
        static object sync = new object();
        static long value = -1;

        public static long GetOrSet(ulong ts)
        {
            lock (sync)
            {
                if (value == -1)
                {
                    value = (long)ts;
                }
            }
            return value;
        }
    }

    unsafe class Mp4AudioTarget : OutStream
    {
        AVFormatContext* outCtx;
        object ctxSync;
        AVCodecContext* codecCtx;

        AVFrame* frame;
        AVPacket* packet;

        int channelId;
        int timebase_den;

        bool running = false;
        public void Stop()
        {
            if (!running)
                return;

            lock (this)
            {
                running = false;
                // Flush the encoder
                SendFrame(null);
                outCtx = null;
            }
        }

        public void StartWithContext(AVFormatContext* ctx, object sync, int id)
        {
            outCtx = ctx;
            ctxSync = sync;
            channelId = id;

            timebase_den = outCtx->streams[id]->time_base.den;

            var encoder = avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP2);
            if (encoder == null) 
                throw new Exception("Couldn't find MP2 encoder");

            codecCtx = avcodec_alloc_context3(encoder);
            if (codecCtx == null) 
                throw new Exception("Couldn't allocate MP2 encoder");

            codecCtx->sample_rate = StreamInfo.AudioSampleRate;
            av_channel_layout_default(&codecCtx->ch_layout, StreamInfo.AudioChannels);
            codecCtx->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
            codecCtx->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_S16;
            codecCtx->bit_rate = 128000;
            codecCtx->time_base = new AVRational { num = 1, den = timebase_den };

            avcodec_open2(codecCtx, encoder, null).AssertNotNeg();

            frame = av_frame_alloc();
            if (frame == null) throw new Exception("Couldn't allocate AVFrame");
            frame->nb_samples = Math.Max(StreamInfo.MinAudioSamplesPerPayload, codecCtx->frame_size);
            frame->format = (int)AVSampleFormat.AV_SAMPLE_FMT_S16;
            av_channel_layout_default(&frame->ch_layout, StreamInfo.AudioChannels);
            frame->sample_rate = StreamInfo.AudioSampleRate;

            av_frame_get_buffer(frame, 0);

            packet = av_packet_alloc();
            if (packet == null) throw new Exception("Couldn't allocate AVPacket");

            running = true;
        }

        long firstTs = -1;
        // need to fill the encoder frame before sending it, it can also happen across SendData calls
        int frameFreeSpace = 0;
        private void SendData(Span<byte> data, ulong ts)
        {
            if (!running)
                return;

            double diffTs = 0;

            lock (this)
            {
                if (firstTs == -1)
                    firstTs = FirstTimestamp.GetOrSet(ts);

                // Should look into endianness for this
                while (data.Length > 0 && running)
                {
                    bool newframe = frameFreeSpace == 0;

                    if (newframe)
                    {
                        av_frame_make_writable(frame).AssertNotNeg();
                        frameFreeSpace = frame->linesize[0];

                        // Account for the case where the user opens the home menu
                        var diffWithFirst = ((double)ts - firstTs) / 1E+6;
                        // Set the timestamp at the start of a new packet
                        frame->pkt_dts = frame->pts = (long)((diffTs + diffWithFirst) * timebase_den);
                        // Account for the timestamp increments caused by sending multiple frames in a single SendData() call
                        diffTs += frameFreeSpace / (StreamInfo.AudioChannels * StreamInfo.AudioSampleSize) / (double)StreamInfo.AudioSampleRate;
                    }

                    Span<byte> target = new Span<byte>(frame->data[0] + frame->linesize[0] - frameFreeSpace, frameFreeSpace);
                    int copyBytes = Math.Min(data.Length, frameFreeSpace);

                    data.Slice(0, copyBytes).CopyTo(target);
					data = data.Slice(copyBytes);
                    frameFreeSpace -= copyBytes;

                    if (frameFreeSpace == 0)
                        SendFrame(frame);
                }
            }
        }

        private unsafe void SendFrame(AVFrame* frame)
        {
            var result = avcodec_send_frame(codecCtx, frame);

            // Seems that flush frames now cause an error but the documentation still claims that's the correct way to flush with this API
            if (frame != null)
                result.AssertZero();

            while (avcodec_receive_packet(codecCtx, packet) == 0)
            {
                packet->stream_index = channelId;
                lock (ctxSync)
                    av_interleaved_write_frame(outCtx, packet).AssertNotNeg();
                av_packet_unref(packet);
            }
        }

        void FreeNativeResource() 
        {
            if (this.frame == null)
                return;
            
            AVFrame* frame = this.frame;
            av_frame_free(&frame);
            this.frame = null;

            AVPacket* packet = this.packet;
            av_packet_free(&packet);
            this.packet = null;

            AVCodecContext* c = codecCtx;
            avcodec_free_context(&c);
            this.codecCtx = null;
        }

        ~Mp4AudioTarget()
        {
            FreeNativeResource();
        }

        protected override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            SendData(block.Span, ts);
            block.Free();
        }

        protected override void DisposeImpl()
        {
            Stop();
            FreeNativeResource();
            base.Dispose();
        }
    }

    unsafe class Mp4VideoTarget : OutStream
    {
        AVFormatContext* outCtx;
        object ctxSync;

        int timebase_den;

        bool running = true;
        public void Stop()
        {
            lock (this)
            {
                running = false;
                outCtx = null;
            }
        }

        public void StartWithContext(AVFormatContext* ctx, object sync)
        {
            outCtx = ctx;
            ctxSync = sync;
            timebase_den = outCtx->streams[0]->time_base.den;
            running = true;
        }

        long firstTs = -1;
        private void SendData(Span<byte> data, ulong ts)
        {
            if (!running)
                return;

            lock (this)
            {
                // Must add SPS and PPS to the first frame manually to keep ffmpeg happy
                if (firstTs == -1)
                {
                    byte[] next = new byte[data.Length + StreamInfo.SPS.Length + StreamInfo.PPS.Length];
                    Buffer.BlockCopy(StreamInfo.SPS, 0, next, 0, StreamInfo.SPS.Length);
                    Buffer.BlockCopy(StreamInfo.PPS, 0, next, StreamInfo.SPS.Length, StreamInfo.PPS.Length);
                    data.CopyTo(next.AsSpan(StreamInfo.SPS.Length + StreamInfo.PPS.Length, data.Length));

                    data = next;

                    firstTs = FirstTimestamp.GetOrSet(ts);
                }

                fixed (byte* nal_data = data)
                {
                    AVPacket* pkt = av_packet_alloc();

                    pkt->data = nal_data;
                    pkt->size = data.Length;
                    pkt->dts = pkt->pts = ((long)ts - firstTs) * timebase_den / (long)1E+6;
                    pkt->stream_index = 0;

                    lock (ctxSync)
                        av_interleaved_write_frame(outCtx, pkt).AssertNotNeg();

                    av_packet_free(&pkt);
                }
            }
        }

        protected override void SendDataImpl(PoolBuffer block, ulong ts)
        {
            SendData(block.Span, ts);
            block.Free();
        }
    }
}
