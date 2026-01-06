using SysDVR.Client.Core;
using SysDVR.Client.Sources;
using System;
using System.Threading;

namespace SysDVR.Client.Targets
{
	internal class StubTarget : OutStream
	{
		public string Tag;

		public StubTarget(string tag)
		{
			Tag = tag;
		}

		protected override void SendDataImpl(PoolBuffer block, ulong ts)
		{
			Console.WriteLine($"{Tag} packet {ts} {block.Length}");
			block.Free();
		}
	}

	internal static class StubStreamManager 
	{
		public static StreamManager Create(StreamingSource source, CancellationTokenSource cancel)
		{
            return new StreamManager(source, 
				source.Options.HasVideo ? new StubTarget("Video") : null,
				source.Options.HasAudio ? new StubTarget("Audio") : null, 
				cancel);
        }
	}
}
