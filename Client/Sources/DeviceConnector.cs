using SysDVR.Client.Core;
using SysDVR.Client.Targets;
using SysDVR.Client.Targets.Player;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    internal class DeviceConnector
    {
        public event Action<string>? OnMessage;

        readonly DeviceInfo Info;
        readonly CancellationTokenSource Token;
        readonly StreamingOptions Options;

        void MessageReceived(string msg) 
        {
            Console.WriteLine(msg);
            OnMessage?.Invoke(msg);
        }

        public DeviceConnector(DeviceInfo info, CancellationTokenSource token, StreamingOptions opt)
        {
            Info = info;
            Token = token;
            Options = opt;
        }

        async Task<StreamingSource> ConnectInternal() 
        {
			StreamingSource source;

            if (Info.Source == ConnectionType.Net)
                source = new TCPBridgeSource(Info, Token.Token, Options);
            else if (Info.Source == ConnectionType.Usb)
                source = new UsbStreamingSource(
                    Info.ConnectionHandle as DvrUsbDevice ?? throw new Exception("Invalid device handle"),
                    Options, Token.Token);
            else if (Info.Source == ConnectionType.Stub)
                source = new StubSource(Options, Token.Token, 10000);
            else
                throw new Exception($"Connection source not implemented: {Info.Source}");

			source.OnMessage += MessageReceived;
			try
			{
				await source.Connect().ConfigureAwait(false);
			}
			catch
			{
				source.OnMessage -= MessageReceived;
				source.Dispose();
				throw;
			}

			source.OnMessage -= MessageReceived;
            return source;
		}

        public async Task<StreamManager> ConnectStub()
        {
            return StubStreamManager.Create(await ConnectInternal(), Token);
        }

		public async Task<PlayerManager> ConnectForPlayer()
        {
            try
            {
                var source = await ConnectInternal().ConfigureAwait(false);
				return new PlayerManager(source, Token);
            }
            catch
            {
                Info?.Dispose();
                throw;
            }
        }
    }
}
