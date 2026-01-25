using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Targets.FileOutput
{
    class StreamsSyncObject
    {
        object sync = new();
        long firstTs = -1;

        public long GetOrSetFirstTimestamp(ulong ts)
        {
            lock (sync)
            {
                if (firstTs == -1)
                {
                    firstTs = (long)ts;
                }
            }
            return firstTs;
        }
    }

    interface IFileOutputCodecInfo 
    {
        unsafe void OpenEncoder(AVFormatContext* ctx, StreamsSyncObject sync, int id);
    }

    unsafe class Mp4AudioTarget : OutStream, IFileOutputCodecInfo
    {
        StreamsSyncObject Sync;

        AVFormatContext* outCtx;
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

        public void OpenEncoder(AVFormatContext* ctx, StreamsSyncObject sync, int id)
        {
            outCtx = ctx;
            Sync = sync;
            channelId = id;

            // Audio is raw samples and needs re-encoding, use an encoder and then copy the output parameters

            var encoder = avcodec_find_encoder(AVCodecID.AV_CODEC_ID_AAC);
            if (encoder == null) 
                throw new Exception("Couldn't find AAC encoder");

            codecCtx = avcodec_alloc_context3(encoder);
            if (codecCtx == null) 
                throw new Exception("Couldn't allocate AAC encoder");
            
            timebase_den = StreamInfo.AudioSampleRate;

            codecCtx->sample_rate = StreamInfo.AudioSampleRate;
            av_channel_layout_default(&codecCtx->ch_layout, StreamInfo.AudioChannels);
            codecCtx->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
            codecCtx->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
            codecCtx->bit_rate = 128000;
            codecCtx->time_base = new AVRational { num = 1, den = timebase_den };

            avcodec_open2(codecCtx, encoder, null).AssertNotNeg();
            avcodec_parameters_from_context(ctx->streams[id]->codecpar, codecCtx).AssertZero();

            frame = av_frame_alloc();
            if (frame == null) throw new Exception("Couldn't allocate AVFrame");
            frame->nb_samples = Math.Max(StreamInfo.MinAudioSamplesPerPayload, codecCtx->frame_size);
            frame->format = (int)AVSampleFormat.AV_SAMPLE_FMT_FLTP;
            av_channel_layout_default(&frame->ch_layout, StreamInfo.AudioChannels);
            frame->sample_rate = StreamInfo.AudioSampleRate;

            av_frame_get_buffer(frame, 0).AssertZero("Failed to call av_frame_get_buffer");

            packet = av_packet_alloc();
            if (packet == null) throw new Exception("Couldn't allocate AVPacket");

            running = true;
        }

        long firstTs = -1;
        // need to fill the encoder frame before sending it, it can also happen across SendData calls
        int frameFreeSpaceInSamples = 0;
        // We encode the output as floats
        const int EncodedSampleSize = sizeof(float);
        private void SendData(Span<byte> __data, ulong ts)
        {
            if (!running)
                return;

            if (__data.Length % StreamInfo.AudioSampleSize != 0)
                throw new ArgumentException("Audio data length must be a multiple of sample size");

            Span<short> source = MemoryMarshal.Cast<byte, short>(__data);

            double diffTs = 0;

            lock (this)
            {
                if (firstTs == -1)
                    firstTs = Sync.GetOrSetFirstTimestamp(ts);

                while (source.Length > 0 && running)
                {
                    bool newframe = frameFreeSpaceInSamples == 0;

                    if (newframe)
                    {
                        av_frame_make_writable(frame).AssertNotNeg();

                        if (frame->linesize[0] % EncodedSampleSize != 0)
                            throw new Exception("Unexpected frame free space alignment");

                        frameFreeSpaceInSamples = frame->linesize[0] / EncodedSampleSize;

                        // Account for the case where the user opens the home menu
                        var diffWithFirst = ((double)ts - firstTs) / 1E+6;
                        // Set the timestamp at the start of a new packet
                        frame->pkt_dts = frame->pts = (long)((diffTs + diffWithFirst) * timebase_den);
                        // Account for the timestamp increments caused by sending multiple frames in a single SendData() call
                        diffTs += (frameFreeSpaceInSamples / StreamInfo.AudioChannels) / (double)StreamInfo.AudioSampleRate;
                    }

                    var targetLeft = new Span<float>(frame->data[0] + frame->linesize[0] - frameFreeSpaceInSamples * EncodedSampleSize, frameFreeSpaceInSamples);

                    // frame->linesize[1] seems to always be 0. Use linesize[0] for both channels
                    var targetRight = new Span<float>(frame->data[1] + frame->linesize[0] - frameFreeSpaceInSamples * EncodedSampleSize, frameFreeSpaceInSamples);

                    // Source is non planar but target is, we will be consuming double the samples from source
                    int copySamples = Math.Min(source.Length * 2, targetLeft.Length);

                    // Manually resample from short to float
                    for (int i = 0; i < copySamples; i++)
                    {
                        var sampleL = (float)source[i * 2] / -(float)short.MinValue;
                        var sampleR = (float)source[i * 2 + 1] / -(float)short.MinValue;
                        targetLeft[i] = sampleL;
                        targetRight[i] = sampleR;
                    }

					source = source.Slice(copySamples * 2);
                    
                    frameFreeSpaceInSamples -= copySamples;

                    if (frameFreeSpaceInSamples == 0)
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
                lock (Sync)
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

    unsafe class Mp4VideoTarget : OutStream, IFileOutputCodecInfo
    {
        AVFormatContext* outCtx;
        StreamsSyncObject Sync;

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

        public void OpenEncoder(AVFormatContext* ctx, StreamsSyncObject sync, int id)
        {
            if (id != 0)
                throw new ArgumentException("The video stream should be the first stream in the file");

            outCtx = ctx;
            Sync = sync;

            // We copy the intput h264 as-is to the output, this means we don't need to allocate an encoder. Manually populate the stream codec parameters
            var stream = outCtx->streams[id];
            stream->codecpar->codec_id = AVCodecID.AV_CODEC_ID_H264;
            stream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
            stream->codecpar->width = StreamInfo.VideoWidth;
            stream->codecpar->height = StreamInfo.VideoHeight;
            stream->codecpar->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;

            timebase_den = stream->time_base.den = 90000; // Default timebase used by libavcodec for h264, empirically observed in ffmpeg

            /* 
             * TODO: This is needed for MKV files but doesn't seem to be quite right: 
             * ffmpeg shows several errors and seeking in mpv doesn't work. Adding this to mp4 files breaks video in the windows 10 video player.
            */
            //var (ptr, sz) = LibavUtils.AllocateH264Extradata();;
            //stream->codecpar->extradata = (byte*)ptr.ToPointer();
            //stream->codecpar->extradata_size = sz;

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

                    firstTs = Sync.GetOrSetFirstTimestamp(ts);
                }

                fixed (byte* nal_data = data)
                {
                    AVPacket* pkt = av_packet_alloc();

                    pkt->data = nal_data;
                    pkt->size = data.Length;
                    pkt->dts = pkt->pts = ((long)ts - firstTs) * timebase_den / (long)1E+6;
                    pkt->stream_index = 0;

                    lock (Sync)
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
