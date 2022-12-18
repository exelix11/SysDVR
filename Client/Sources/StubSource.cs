using System;
using System.Threading;

namespace SysDVR.Client.Sources
{
    // Stub source for testing SDL/Ffmpeg UI without a real console 
    class StubSource : IStreamingSource
	{
		public bool Logging { get; set; }
		CancellationToken Cancellation;

		public void Flush() { }
        
		public bool ReadHeader(byte[] buffer) =>
			throw new NotImplementedException();

		public bool ReadPayload(byte[] buffer, int length) =>
			throw new NotImplementedException();

		public void StopStreaming() { }

		public void UseCancellationToken(CancellationToken tok)
		{
			Cancellation = tok;
		}

		public void WaitForConnection()
		{
			while (!Cancellation.IsCancellationRequested)
				Thread.Sleep(1000);
		}
	}
}