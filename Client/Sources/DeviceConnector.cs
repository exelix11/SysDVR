using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    internal class DeviceConnector
    {
        public event Action<string> OnError;
        public event Action<string> OnMessage;
        public event Action<BaseStreamManager> OnConnected;

        readonly DeviceInfo Info;
        readonly CancellationTokenSource Token;
        readonly bool IsForPlayer;
        readonly StreamKind Kind;

        readonly bool HasVideo;
        readonly bool HasAudio;

        public DeviceConnector(DeviceInfo info, CancellationTokenSource token, StreamKind kind)
        {
            Info = info;
            Token = token;
            Kind = kind;

            // TODO
            IsForPlayer = true;
            
            HasVideo = Kind is StreamKind.Video or StreamKind.Both;
            HasAudio = Kind is StreamKind.Audio or StreamKind.Both;
        }

        BaseStreamManager GetManager()
        {
            if (IsForPlayer)
                return new PlayerManager(HasVideo, HasAudio, Token);
            else throw new Exception();
        }

        public async void Connect() 
        {
            try 
            {
                if (Info.Source == ConnectionType.Net)
                    await BeginNet();
                else if (Info.Source == ConnectionType.Usb)
                    await BeginUsb();
            }
            catch (Exception ex)
            {
                if (Token.IsCancellationRequested)
                    return;

                OnError?.Invoke(ex.Message);
            }
        }

        async Task BeginUsb() 
        {
            throw new NotImplementedException();
        }

        async Task BeginNet() 
        {
            // Tcp bridge is single channel, needs two instances.
            TCPBridgeSource? vTcp = Kind is StreamKind.Video or StreamKind.Both ? new TCPBridgeSource(Info, StreamKind.Video) : null;
            TCPBridgeSource? aTcp = Kind is StreamKind.Audio or StreamKind.Both ? new TCPBridgeSource(Info, StreamKind.Audio) : null;

            if (vTcp is not null) vTcp.OnError += OnMessage;
            if (aTcp is not null) aTcp.OnError += OnMessage;

            Task conn = Kind == StreamKind.Both ?
                Task.WhenAll(vTcp!.ConnectAsync(Token.Token), aTcp!.ConnectAsync(Token.Token)) :
                vTcp?.ConnectAsync(Token.Token) ?? aTcp!.ConnectAsync(Token.Token);

            try
            {
                await conn;
            }
            finally 
            {
                if (vTcp is not null) vTcp.OnError -= OnMessage;
                if (aTcp is not null) aTcp.OnError -= OnMessage;
            }

            var mng = GetManager();
            if (vTcp is not null) mng.AddSource(vTcp);
            if (aTcp is not null) mng.AddSource(aTcp);

            OnConnected?.Invoke(mng);
        }
    }
}
