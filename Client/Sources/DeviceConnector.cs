using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SysDVR.Client.Sources
{
    internal class DeviceConnector
    {
        public event Action<string> OnMessage;

        readonly DeviceInfo Info;
        readonly CancellationTokenSource Token;
        readonly bool IsForPlayer;
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

            // TODO
            IsForPlayer = true;
        }

        BaseStreamManager GetManager(StreamingSource source)
        {
            if (IsForPlayer)
                return new PlayerManager(source, Token);
            else throw new Exception();
        }

        public async Task<BaseStreamManager> Connect()
        {
            try
            {
                StreamingSource source = null;

                if (Info.Source == ConnectionType.Net)
                    source = new TCPBridgeSource(Info, Token.Token, Options);
                else if (Info.Source == ConnectionType.Usb)
                    source = new UsbStreamingSource(
                        Info.ConnectionHandle as DvrUsbDevice ?? throw new Exception("Invalid device handle"), 
                        Options, Token.Token);
                else if (Info.Source == ConnectionType.Stub)
                    source = new StubSource(Options, Token.Token);

                source.OnMessage += MessageReceived;
                try
                {
                    await source.Connect().ConfigureAwait(false);
                }
                finally
                {
                    source.OnMessage -= MessageReceived;
                }

                return GetManager(source);
            }
            catch
            {
                if (Token.IsCancellationRequested)
                    return null;

                throw;
            }
        }
    }
}
