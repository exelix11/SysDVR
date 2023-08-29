using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        readonly StreamKind Kind;

        readonly bool HasVideo;
        readonly bool HasAudio;

        void MessageReceived(string msg) 
        {
            Console.WriteLine(msg);
            OnMessage?.Invoke(msg);
        }

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

        public async Task<BaseStreamManager> Connect()
        {
            try
            {
                if (Info.Source == ConnectionType.Net)
                    return await BeginNet();
                else if (Info.Source == ConnectionType.Usb)
                    return await BeginUsb();
                else if (Info.Source == ConnectionType.Stub)
                    return await BeginStub();
            }
            catch
            {
                if (Token.IsCancellationRequested)
                    return null;

                throw;
            }

            return null;
        }

        async Task<BaseStreamManager> BeginStub()
        {
            Console.WriteLine("Stub");
            var src = new StubSource(HasVideo, HasAudio);
            src.OnMessage += MessageReceived;

            try
            {
                await src.ConnectAsync(Token.Token).ConfigureAwait(false);
            }
            finally 
            {
                src.OnMessage -= MessageReceived;
            }

            var mng = GetManager();
            mng.AddSource(src);
            return mng;
        }

        async Task<BaseStreamManager> BeginUsb() 
        {
            throw new NotImplementedException();
        }

        async Task<BaseStreamManager> BeginNet() 
        {
            // Tcp bridge is single channel, needs two instances.
            TCPBridgeSource? vTcp = HasVideo ? new TCPBridgeSource(Info, StreamKind.Video) : null;
            TCPBridgeSource? aTcp = HasAudio ? new TCPBridgeSource(Info, StreamKind.Audio) : null;

            if (vTcp is not null) vTcp.OnMessage += MessageReceived;
            if (aTcp is not null) aTcp.OnMessage += MessageReceived;

            Task conn = Kind == StreamKind.Both ?
                Task.WhenAll(vTcp!.ConnectAsync(Token.Token), aTcp!.ConnectAsync(Token.Token)) :
                vTcp?.ConnectAsync(Token.Token) ?? aTcp!.ConnectAsync(Token.Token);

            try
            {
                await conn.ConfigureAwait(false);
            }
            finally 
            {
                if (vTcp is not null) vTcp.OnMessage -= MessageReceived;
                if (aTcp is not null) aTcp.OnMessage -= MessageReceived;
            }

            var mng = GetManager();
            if (vTcp is not null) mng.AddSource(vTcp);
            if (aTcp is not null) mng.AddSource(aTcp);

            return mng;
        }
    }
}
