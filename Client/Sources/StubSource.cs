using System;
using System.Buffers;
using System.Runtime.InteropServices;
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

        public ushort SimulateVersion = ProtocolUtil.ProtocolVersion3;

        public StubSource(StreamingOptions options, CancellationToken cancellation, int sendPackets) : 
            base(options, cancellation) 
        {
            SendPackets = sendPackets;
        }

        public override async Task Connect()
        {
            await Task.Delay(2000, Cancellation).ConfigureAwait(false);
            await DoHandshake(Options.Kind).ConfigureAwait(false);
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
			return Task.FromResult(Encoding.ASCII.GetBytes($"SysDVR|{ProtocolUtil.VersionCodeToString(SimulateVersion)}\0"));
		}

		protected override Task<byte[]> SendHandshakePacket(ProtoHandshakeRequest req, int size)
        {
            if (SimulateVersion == ProtocolUtil.ProtocolVersion2)
            {
                ProtoHandshakeResponse02 res = new();
                res.Result = ProtoHandshakeRequest.HandshakeOKCode;
                var data = new byte[ProtoHandshakeResponse02.Size];
                MemoryMarshal.Write(data, res);

                return Task.FromResult(data);
            }
            else if (SimulateVersion == ProtocolUtil.ProtocolVersion3)
            {
                ProtoHandshakeResponse03 res = new();
                res.Result = ProtoHandshakeRequest.HandshakeOKCode;
                var data = new byte[ProtoHandshakeResponse03.Size];
                MemoryMarshal.Write(data, res);

                return Task.FromResult(data);
            }
            else throw new NotImplementedException();
        }

		public override void Dispose() { }
	}
}