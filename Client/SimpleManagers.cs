using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SysDVRClient
{
	class MpvStdinManager : BaseStreamManager
	{
		private const string BaseArgs = "--profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --untimed --cache-pause=no --no-correct-pts";

		private static IOutTarget GetVTarget(StreamKind kind, string path) =>
			kind == StreamKind.Video ? new StdInTarget(path, "- --fps=30 " + BaseArgs) : null;

		private static IOutTarget GetATarget(StreamKind kind, string path) =>
			kind == StreamKind.Audio ? new StdInTarget(path, "- --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000 " + BaseArgs) : null;

		// This can only handle one stream
		public MpvStdinManager(StreamKind kind, string path) :
			base(GetVTarget(kind, path), GetATarget(kind, path))
		{

		}
	}

	class SaveToDiskManager : BaseStreamManager
	{
		private static IOutTarget GetTarget(bool enable, string path, string name) =>
			enable ? new OutFileTarget(Path.Join(path, name)) : null;

		public SaveToDiskManager(bool video, bool audio, string path) :
			base(GetTarget(video, path, "video.h264"), GetTarget(audio, path, "audio.raw"))
		{

		}
	}

	class OutFileTarget : IOutTarget
	{
		FileStream Vfs;

		public OutFileTarget(string fname)
		{
			Vfs = File.Open(fname, FileMode.Create);
		}

		public void Dispose()
		{
			Vfs.Close();
			Vfs.Dispose();
		}

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			Vfs.Write(data, offset, size);
		}
	}

	class StdInTarget : IOutTarget
	{
		Process proc;

		public StdInTarget(string path, string args)
		{
			ProcessStartInfo p = new ProcessStartInfo()
			{
				Arguments = args,
				FileName = path,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			proc = Process.Start(p);
		}

		private bool FirstTime = true;
		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			if (FirstTime)
			{
				proc.StandardInput.BaseStream.Write(StreamInfo.SPS);
				proc.StandardInput.BaseStream.Write(StreamInfo.PPS);
				FirstTime = false;
			}

			proc.StandardInput.BaseStream.Write(data, offset, size);
		}

	}
}
