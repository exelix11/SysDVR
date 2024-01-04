using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;

namespace SysDVR.Client.Sources
{
    // Stub source for testing SDL/Ffmpeg UI without a real console 
    class StubSource : StreamingSource
    {
        // Check for bugs
        bool connected;

        public StubSource(StreamingOptions options, CancellationToken cancellation) : 
            base(options, cancellation) { }

        public override async Task Connect()
        {
            await Task.Delay(2000, Cancellation).ConfigureAwait(false);
            connected = true;
        }

		public override async Task Flush()
        {
            await Task.Delay(500, Cancellation).ConfigureAwait(false);
        }

        public override async Task<ReceivedPacket> ReadNextPacket()
        {
            if (!connected)
                throw new Exception("Not connected");

            await Task.Delay(-1, Cancellation).ConfigureAwait(false);
            throw new Exception();
        }

        public override Task StopStreaming()
        {
            return Task.CompletedTask;
        }

		protected override Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes)
		{
			return Task.FromResult(Encoding.ASCII.GetBytes($"SysDVR|{ProtoHandshakeRequest.CurrentProtocolString}\0"));
		}

		protected override Task<uint> SendHandshakePacket(ProtoHandshakeRequest req)
        {
            return Task.FromResult(ProtoHandshakeRequest.HandshakeOKCode);
        }

		public override void Dispose() { }
	}
}