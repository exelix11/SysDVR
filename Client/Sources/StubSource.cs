using System;
using System.Threading;
using SysDVR.Client.Core;

namespace SysDVR.Client.Sources
{
    // Stub source for testing SDL/Ffmpeg UI without a real console 
    class StubSource : IStreamingSource
	{
		public DebugOptions Logging { get; set; }

		readonly StreamKind kind;
		public StreamKind SourceKind => kind;

		public StubSource(bool hasVideo, bool hasAudio)
		{
			kind = (hasVideo, hasAudio) switch
			{
				(true, true) => StreamKind.Both,
				(true, false) => StreamKind.Video,
				(false, true) => StreamKind.Audio,
				_ => throw new NotSupportedException()
			};
		}

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