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

		// Check for bugs
		bool connected;

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
            if (!connected)
                throw new Exception("Stub not connected");

			while (!Cancellation.IsCancellationRequested)
				Thread.Sleep(1000);

			return false;
        }

		public bool ReadPayload(byte[] buffer, int length)
		{
			if (!connected)
				throw new Exception("Stub not connected");

            while (!Cancellation.IsCancellationRequested)
                Thread.Sleep(1000);

            return false;
        }

		public void StopStreaming() { }

		public void UseCancellationToken(CancellationToken tok)
		{
			Cancellation = tok;
		}

        public async Task ConnectAsync(CancellationToken token)
        {
            Cancellation = token;	
			OnMessage?.Invoke("Connecting stub...");
			await Task.Delay(2000, token).ConfigureAwait(false);
            OnMessage?.Invoke("Stub connected");
			connected = true;
        }
    }
}