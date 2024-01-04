using SysDVR.Client.Core;
using SysDVR.Client.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

	internal class StubStreamManager : BaseStreamManager
	{
		public StubStreamManager(StreamingSource source, CancellationTokenSource cancel) : 
			base(source, new StubTarget("Video"), new StubTarget("Audio"), cancel)
		{
		}
	}
}
