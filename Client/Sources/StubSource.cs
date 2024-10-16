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
        int SendPackets;
        byte replaySlot;

        public StubSource(StreamingOptions options, CancellationToken cancellation, int sendPackets) : 
            base(options, cancellation) 
        {
            SendPackets = sendPackets;
        }

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

            if (SendPackets == 0)
            {
                await Task.Delay(-1, Cancellation).ConfigureAwait(false);
                throw new Exception();
            }
            else
            {
                await Task.Delay(19, Cancellation).ConfigureAwait(false);

                --SendPackets;
                
                if (replaySlot > 10) 
                    replaySlot = 0;

				return new ReceivedPacket(
                    new PacketHeader() {
                        Magic = PacketHeader.MagicResponse,
                        DataSize = 1024,
                        Flags = (SendPackets % 2 == 0) ? PacketHeader.MetaIsVideo : PacketHeader.MetaIsAudio,
                        ReplaySlot = ++replaySlot,
                        Timestamp = 9999999
                    },
                    PoolBuffer.Rent(1024)
                );
            }
        }

        public override Task StopStreaming()
        {
            return Task.CompletedTask;
        }

		protected override Task<byte[]> ReadHandshakeHello(StreamKind stream, int maxBytes)
		{
			return Task.FromResult(Encoding.ASCII.GetBytes($"SysDVR|03\0"));
		}

		protected override Task<uint> SendHandshakePacket(ProtoHandshakeRequest req)
        {
            return Task.FromResult(ProtoHandshakeRequest.HandshakeOKCode);
        }

		public override void Dispose() { }
	}
}