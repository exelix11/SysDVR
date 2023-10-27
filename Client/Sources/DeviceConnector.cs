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

        readonly bool HasVideo;
        readonly bool HasAudio;

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
            
            HasVideo = opt.Kind is StreamKind.Video or StreamKind.Both;
            HasAudio = opt.Kind is StreamKind.Audio or StreamKind.Both;
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
            var src = new StubSource(Options);
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
            if (Info.ConnectionHandle is not DvrUsbDevice dev)
                throw new Exception("Wrong connection handle");

            var source = new UsbStreamingSource(dev, Options);
            source.OnMessage += MessageReceived;

            try
            {
                await source.ConnectAsync(Token.Token).ConfigureAwait(false);
            }
            finally
            {
                source.OnMessage -= MessageReceived;
            }

            var mng = GetManager();
            mng.AddSource(source);
            return mng;
        }

        async Task<BaseStreamManager> BeginNet() 
        {
            // Tcp bridge is single channel, needs two instances.
            TCPBridgeSource? vTcp = HasVideo ? new TCPBridgeSource(Info, Options, StreamKind.Video) : null;
            TCPBridgeSource? aTcp = HasAudio ? new TCPBridgeSource(Info, Options, StreamKind.Audio) : null;

            if (vTcp is not null) vTcp.OnMessage += MessageReceived;
            if (aTcp is not null) aTcp.OnMessage += MessageReceived;

            Task conn = Options.Kind == StreamKind.Both ?
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
