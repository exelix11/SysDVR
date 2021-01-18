using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SysDVR.Client
{
	class MpvStdinManager : BaseStreamManager
	{
		private const string BaseArgs = "--profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --untimed --cache-pause=no --no-correct-pts";

		private static IOutStream GetVTarget(StreamKind kind, string path) =>
			kind == StreamKind.Video ? new StdInTarget(path, "- --fps=30 " + BaseArgs) : null;

		private static IOutStream GetATarget(StreamKind kind, string path) =>
			kind == StreamKind.Audio ? new StdInTarget(path, "- --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000 " + BaseArgs) : null;

		// This can only handle one stream
		public MpvStdinManager(StreamKind kind, string path) :
			base(GetVTarget(kind, path), GetATarget(kind, path))
		{

		}
	}

	class StdOutManager : BaseStreamManager
	{
		private static IOutStream GetVTarget(StreamKind kind) =>
			kind == StreamKind.Video ? new StdOutTarget() : null;

		private static IOutStream GetATarget(StreamKind kind) =>
			kind == StreamKind.Audio ? new StdOutTarget() : null;

		// This can only handle one stream
		public StdOutManager(StreamKind kind) :
			base(GetVTarget(kind), GetATarget(kind))
		{

		}
	}

	class StdInTarget : IOutStream
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
			proc.StandardInput.BaseStream.Flush();
		}

		public void UseCancellationToken(CancellationToken tok) { }
	}

	class StdOutTarget : IOutStream
	{
		Stream stdout;

		public StdOutTarget()
		{
			stdout = Console.OpenStandardOutput();
		}

		private bool FirstTime = true;
		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			if (FirstTime)
			{
				stdout.Write(StreamInfo.SPS);
				stdout.Write(StreamInfo.PPS);
				FirstTime = false;
			}

			stdout.Write(data, offset, size);
			stdout.Flush();
		}

		public void UseCancellationToken(CancellationToken tok) { }
	}
}
