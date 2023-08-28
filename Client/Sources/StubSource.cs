using System;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;

namespace SysDVR.Client.Sources
{
    // Stub source for testing SDL/Ffmpeg UI without a real console 
    class StubSource : IStreamingSource
	{
        public event Action<string> OnMessage;

        readonly StreamKind kind;
		public StreamKind SourceKind => kind;
		CancellationToken Cancellation;

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

        public void Flush() { }

		public bool ReadHeader(byte[] buffer)
		{
            Task.Delay(-1, Cancellation).GetAwaiter().GetResult();
			return false;
        }

		public bool ReadPayload(byte[] buffer, int length)
		{
			Task.Delay(-1, Cancellation).GetAwaiter().GetResult();
            return false;
        }

		public void StopStreaming() { }

		public void UseCancellationToken(CancellationToken tok)
		{
			Cancellation = tok;
		}

        public Task ConnectAsync(CancellationToken token)
        {
            Cancellation = token;
			return Task.Delay(1000);
        }
    }
}