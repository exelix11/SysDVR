#if DEBUG
using System;
using System.Threading;

namespace SysDVR.Client.Sources
{
	class StubSource : IStreamingSource
	{
		public bool Logging { get; set; }

		public void Flush()
		{

		}

		public bool ReadHeader(byte[] buffer) =>
			throw new NotImplementedException();

		public bool ReadPayload(byte[] buffer, int length) =>
			throw new NotImplementedException();

		public void StopStreaming()
		{

		}

		public void UseCancellationToken(CancellationToken tok)
		{

		}

		public void WaitForConnection()
		{
			while (true)
				Thread.Sleep(1000);
		}
	}
}
#endif
