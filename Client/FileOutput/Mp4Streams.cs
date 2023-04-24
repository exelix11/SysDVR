using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            codecCtx = avcodec_alloc_context3(encoder);
            if (codecCtx == null) throw new Exception("Couldn't allocate MP2 encoder");

            codecCtx->sample_rate = StreamInfo.AudioSampleRate;
            av_channel_layout_default(&codecCtx->ch_layout, StreamInfo.AudioChannels);
            codecCtx->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
            codecCtx->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_S16;
            codecCtx->bit_rate = 128000;
            codecCtx->time_base = new AVRational { num = 1, den = timebase_den };

            avcodec_open2(codecCtx, encoder, null).AssertNotNeg();

            frame = av_frame_alloc();
            if (frame == null) throw new Exception("Couldn't allocate AVFrame");
            frame->nb_samples = Math.Min(StreamInfo.MinAudioSamplesPerPayload, codecCtx->frame_size);
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
        ulong framePrevTs = 0;
        private void SendData(byte[] data, int size, ulong ts)
        {
            if (!running)
                return;

            lock (this)
            {
                if (firstTs == -1)
                    firstTs = (long)ts;

                // Should look into endianness for this
                Span<byte> d = new Span<byte>(data, 0, size);
                while (d.Length > 0 && running)
                {
                    bool newframe = frameFreeSpace == 0;

                    if (newframe)
                    {
                        av_frame_make_writable(frame).AssertNotNeg();
                        frameFreeSpace = frame->linesize[0];
                        framePrevTs = ts;
                    }

                    Span<byte> target = new Span<byte>(frame->data[0] + frame->linesize[0] - frameFreeSpace, frameFreeSpace);
                    int copyBytes = Math.Min(d.Length, frameFreeSpace);

                    d.Slice(0, copyBytes).CopyTo(target);
                    d = d.Slice(copyBytes);
                    frameFreeSpace -= copyBytes;

                    frame->pkt_dts = frame->pts = (long)(((long)ts - firstTs) * timebase_den / 1E+6);
                    ts += (ulong)(copyBytes / 4 * 1E+6 / StreamInfo.AudioSampleRate);

                    if (frameFreeSpace == 0)
                        SendFrame(frame);
                }
            }
        }

        private unsafe void SendFrame(AVFrame* frame)
        {
            avcodec_send_frame(codecCtx, frame).Assert();
            while (avcodec_receive_packet(codecCtx, packet) == 0)
            {
                packet->stream_index = channelId;
                lock (ctxSync)
                    av_interleaved_write_frame(outCtx, packet).AssertNotNeg();
                av_packet_unref(packet);
            }
        }

        public void SendData(PoolBuffer block, ulong ts)
        {
            SendData(block.RawBuffer, block.Length, ts);
            block.Free();
        }

        public void UseCancellationToken(CancellationToken tok)
        {

        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                AVFrame* frame = this.frame;
                av_frame_free(&frame);

                AVPacket* packet = this.packet;
                av_packet_free(&packet);

                AVCodecContext* c = this.codecCtx;
                avcodec_free_context(&c);

                disposedValue = true;
            }
        }

        ~Mp4AudioTarget()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    unsafe class Mp4VideoTarget : IOutStream
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
        private void SendData(byte[] data, int size, ulong ts)
        {
            if (!running)
                return;

            lock (this)
            {
                // Must add SPS and PPS to the first frame manually to keep ffmpeg happy
                if (firstTs == -1)
                {
                    byte[] next = new byte[size + StreamInfo.SPS.Length + StreamInfo.PPS.Length];
                    Buffer.BlockCopy(StreamInfo.SPS, 0, next, 0, StreamInfo.SPS.Length);
                    Buffer.BlockCopy(StreamInfo.PPS, 0, next, StreamInfo.SPS.Length, StreamInfo.PPS.Length);
                    Buffer.BlockCopy(data, 0, next, StreamInfo.SPS.Length + StreamInfo.PPS.Length, size);

                    data = next;
                    size = next.Length;

                    firstTs = (long)ts;
                }

                fixed (byte* nal_data = data)
                {
                    AVPacket* pkt = av_packet_alloc();

                    pkt->data = nal_data;
                    pkt->size = size;
                    pkt->dts = pkt->pts = ((long)ts - firstTs) * timebase_den / (long)1E+6;
                    pkt->stream_index = 0;

                    lock (ctxSync)
                        av_interleaved_write_frame(outCtx, pkt).AssertNotNeg();

                    av_packet_free(&pkt);
                }
            }
        }

        public void SendData(PoolBuffer block, ulong ts)
        {
            SendData(block.RawBuffer, block.Length, ts);
            block.Free();
        }

        public void UseCancellationToken(CancellationToken tok)
        {

        }
    }
}
