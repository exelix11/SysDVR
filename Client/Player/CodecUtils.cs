using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace SysDVR.Client.Player
{
	public static class CodecUtils
	{
		public static unsafe void PrintAllCodecs() 
		{
			void* i = null;
			AVCodec* c = null;

			Console.WriteLine("Available codecs: ");
			while ((c = av_codec_iterate(&i)) != null)
			{
				if (av_codec_is_decoder(c) != 0)
				{
					var desc = Marshal.PtrToStringAnsi((IntPtr)c->long_name);
					var name = Marshal.PtrToStringAnsi((IntPtr)c->name);

					if (!desc.Contains("264") && !name.Contains("264"))
						continue;

					Console.WriteLine($"Name: {name} Description: {desc}");
				}
			}
		}
		
	}
}
