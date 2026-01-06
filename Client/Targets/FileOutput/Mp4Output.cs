using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using System;
using System.IO;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Targets.FileOutput
{
    unsafe class Mp4Output : IDisposable
    {
        bool Running = false;
        string Filename;

        AVFormatContext* OutCtx;
        AVStream* VStream, AStream;

        internal Mp4VideoTarget? VideoTarget;
        internal Mp4AudioTarget? AudioTarget;

        public Mp4Output(string filename, Mp4VideoTarget? vTarget, Mp4AudioTarget? aTarget)
        {
            VideoTarget = vTarget;
            AudioTarget = aTarget;
            Filename = filename;
        }

        public void Start()
        {
            var name = Path.GetFileName(Filename);
            var OutFmt = av_guess_format(null, name, null);
            if (OutFmt == null) throw new Exception($"Couldn't find a valid output format for the provided file name: {name}");

            AVFormatContext* ctx = null;
            avformat_alloc_output_context2(&ctx, OutFmt, null, null).AssertNotNeg();
            OutCtx = ctx != null ? ctx : throw new Exception("Couldn't allocate output context");

            if (VideoTarget is not null)
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

            if (AudioTarget is not null)
            {
                AStream = avformat_new_stream(OutCtx, avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP2));
                if (AStream == null) throw new Exception("Couldn't allocate audio stream");

                AStream->id = VideoTarget == null ? 0 : 1;
                AStream->codecpar->codec_id = AVCodecID.AV_CODEC_ID_MP2;
                AStream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
                AStream->codecpar->sample_rate = StreamInfo.AudioSampleRate;
                av_channel_layout_default(&AStream->codecpar->ch_layout, StreamInfo.AudioChannels);
                AStream->codecpar->format = (int)AVSampleFormat.AV_SAMPLE_FMT_S16;
                AStream->codecpar->frame_size = StreamInfo.MinAudioSamplesPerPayload;
                AStream->codecpar->bit_rate = 128000;
            }

            avio_open(&OutCtx->pb, Filename, AVIO_FLAG_WRITE).AssertZero();
            avformat_write_header(OutCtx, null).AssertZero();

            StreamsSyncObject sync = new();
            VideoTarget?.StartWithContext(OutCtx, sync);
            AudioTarget?.StartWithContext(OutCtx, sync, AStream->id);

            Running = true;
        }

        public unsafe void Stop()
        {
            AudioTarget?.Stop();
            VideoTarget?.Stop();

            Program.DebugLog("Finalizing recording file...");

            av_write_trailer(OutCtx);
            avio_close(OutCtx->pb);
            avformat_free_context(OutCtx);
            OutCtx = null;

            Running = false;

            AudioTarget?.Dispose();
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
