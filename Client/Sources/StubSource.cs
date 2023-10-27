using System;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;

namespace SysDVR.Client.Sources
{
    // Stub source for testing SDL/Ffmpeg UI without a real console 
    class StubSource : StreamingSource
	{
		CancellationToken Cancellation;

		// Check for bugs
		bool connected;

		public StubSource(bool hasVideo, bool hasAudio)
		{
            SourceKind = (hasVideo, hasAudio) switch
			{
				(true, true) => StreamKind.Both,
				(true, false) => StreamKind.Video,
				(false, true) => StreamKind.Audio,
				_ => throw new NotSupportedException()
			};
		}

        public override void Flush() { }

		public override bool ReadHeader(byte[] buffer)
		{
            if (!connected)
                throw new Exception("Stub not connected");

			while (!Cancellation.IsCancellationRequested)
				Thread.Sleep(1000);

			return false;
        }

		public override bool ReadPayload(byte[] buffer, int length)
		{
			if (!connected)
				throw new Exception("Stub not connected");

            while (!Cancellation.IsCancellationRequested)
                Thread.Sleep(1000);

            return false;
        }

		public override void StopStreaming() { }

        public override async Task ConnectAsync(CancellationToken token)
        {
            Cancellation = token;	
			ReportMessage("Connecting stub...");
			await Task.Delay(20, token).ConfigureAwait(false);
            ReportMessage("Stub connected");
			connected = true;
        }

        public override bool ReadRaw(byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public override bool WriteData(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}