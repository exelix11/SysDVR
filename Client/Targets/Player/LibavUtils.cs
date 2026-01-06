using FFmpeg.AutoGen;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Targets.Player
{
    public static class LibavUtils
    {
        public record Codec(string Name, string Description, AVPixelFormat[] Formats, AVCodecID CodecID);

        public static unsafe Codec[] GetAllDecoders()
        {
            List<Codec> codecs = new List<Codec>();

            void* i = null;
            AVCodec* c = null;

            while ((c = av_codec_iterate(&i)) != null)
            {
                if (av_codec_is_decoder(c) != 0)
                {
                    var desc = Marshal.PtrToStringAnsi((IntPtr)c->long_name);
                    var name = Marshal.PtrToStringAnsi((IntPtr)c->name);

                    List<AVPixelFormat> formats = new List<AVPixelFormat>();
                    var fmt = c->pix_fmts;
                    while (fmt != null && *fmt != AVPixelFormat.AV_PIX_FMT_NONE)
                    {
                        formats.Add(*fmt);
                        fmt++;
                    }

                    codecs.Add(new Codec(name, desc, formats.ToArray(), c->id));
                }
            }

            return codecs.ToArray();
        }

        public static IEnumerable<Codec> GetH264Decoders() =>
            GetAllDecoders().Where(x => x.CodecID == AVCodecID.AV_CODEC_ID_H264);

        public static void PrintAllCodecs()
        {
            Console.WriteLine("List of all compatible video decoders: ");
            foreach (var c in GetH264Decoders())
                Console.WriteLine($"{c.Name} : {c.Description} - Formats: [{string.Join(',', c.Formats)}]");
        }

        public static unsafe (IntPtr, int) AllocateH264Extradata()
        {
            var mem = new System.IO.MemoryStream();
            var bin = new System.IO.BinaryWriter(mem);

            var sps = new Span<byte>(StreamInfo.SPS).Slice(4);
            var pps = new Span<byte>(StreamInfo.PPS).Slice(4);

            // This struct is called AVCDecoderConfigurationRecord
            bin.Write((byte)0x1);
            bin.Write(sps[1]);
            bin.Write(sps[2]);
            bin.Write(sps[3]);
            bin.Write((byte)(0xFC | 3));
            bin.Write((byte)(0xE0 | 1));
            bin.Write((byte)0);
            bin.Write((byte)(sps.Length & 0xFF));
            bin.Write(sps);
            bin.Write((byte)1);
            bin.Write((byte)0);
            bin.Write((byte)(pps.Length & 0xFF));
            bin.Write(pps);

            var data = mem.ToArray();
            // This pointer is freed by ffmpeg automatically, we must use av_malloc to prevent heap corruption 
            var ptr = av_malloc((ulong)data.Length);
            data.AsSpan().CopyTo(new Span<byte>(ptr, data.Length));
            return ((IntPtr)ptr, data.Length);
        }
    }
}
