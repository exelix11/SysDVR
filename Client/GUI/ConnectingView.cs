using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    internal class ConnectingView : View
    {
        readonly DeviceInfo info;
        readonly CancellationTokenSource src = new();
        readonly DeviceConnector conn;

        bool connected;
        bool isError;
        string? errorLine;

        public ConnectingView(DeviceInfo info, StreamKind mode)
        {
            this.info = info;
         
            conn = new DeviceConnector(info, src, mode);
            conn.OnConnected += Conn_OnConnected;
            conn.OnError += Conn_OnError;
            conn.OnMessage += Conn_OnMessage;
        }

        private void Conn_OnMessage(string obj)
        {
            errorLine ??= "";
            errorLine += obj + "\n";
        }

        public override void Created()
        {
            conn.Connect();
        }

        private void Conn_OnError(string obj)
        {
            isError = true;
            errorLine = obj;
        }

        private void Conn_OnConnected(BaseStreamManager obj)
        {
            Console.WriteLine("Connected");

            conn.OnConnected -= Conn_OnConnected;
            conn.OnError -= Conn_OnError;
            conn.OnMessage -= Conn_OnMessage;
            connected = true;

            try
            {
                Program.Instance.ReplaceView(new PlayerView((PlayerManager)obj));
            } 
            catch (Exception e)
            {
                Conn_OnError(e.ToString());
            }
        }

        public override void Destroy()
        {
            if (!connected)
            {
                conn.OnConnected -= Conn_OnConnected;
                conn.OnError -= Conn_OnError;
                conn.OnMessage -= Conn_OnMessage;
                // If we are cdonnected the cancellation token is passed to the player 
                src.Cancel();
            }
            
            base.Destroy();
        }

        public override void Draw()
        {
            Gui.BeginWindow("Connecting");
            ImGui.NewLine();

            Gui.H2();
            Gui.CenterText(isError ? "Fatal error" : "Connecting, please wait");
            ImGui.PopFont();

            Gui.CenterText(info.ToString());
            
            ImGui.NewLine();

            var btnSize = new Vector2(ImGui.GetWindowSize().X * 5 / 6, Gui.ButtonHeight());
            if (errorLine != null)
                ImGui.TextWrapped(errorLine);

            Gui.CursorFromBottom(btnSize.Y);
            if (Gui.CenterButton(isError ? "Go back" : "Cancel", btnSize))
                Program.Instance.PopView();

            Gui.EndWindow();
        }
    }
}
