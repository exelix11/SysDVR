using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using System;
using System.IO;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Targets.FileOutput
{
    unsafe class Mp4Output : IDisposable
    {
        private string Filename;
        bool Running = false;

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

            StreamsSyncObject sync = new();

            if (VideoTarget is not null)
            {
                VStream = avformat_new_stream(OutCtx, avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264));
                if (VStream == null) throw new Exception("Couldn't allocate video stream");
                
                VideoTarget.OpenEncoder(OutCtx, sync, 0);
            }

            if (AudioTarget is not null)
            {
                AStream = avformat_new_stream(OutCtx, null);
                if (AStream == null) throw new Exception("Couldn't allocate audio stream");
                AStream->id = VideoTarget == null ? 0 : 1;

                AudioTarget.OpenEncoder(OutCtx, sync, AStream->id);
            }

            avio_open(&OutCtx->pb, Filename, AVIO_FLAG_WRITE).AssertZero();
            avformat_write_header(OutCtx, null).AssertZero();

            Running = true;
        }

        void DisposeNativeResources()
        {
            if (OutCtx != null)
            {
                if (OutCtx->pb != null)
                    avio_close(OutCtx->pb);
                
                avformat_free_context(OutCtx);
                OutCtx = null;
                AStream = null;
                VStream = null;
            }
        }

        public unsafe void Stop()
        {
            if (Running)
            {
                AudioTarget?.Stop();
                VideoTarget?.Stop();

                Program.DebugLog("Finalizing recording file...");
                av_write_trailer(OutCtx);

                Running = false;
                AudioTarget?.Dispose();
            }

            DisposeNativeResources();
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    Stop();
                else
                    DisposeNativeResources();

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
