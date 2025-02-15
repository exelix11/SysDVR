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
        readonly StringTable.ConnectionTable Strings = Program.Strings.Connection;

        readonly DeviceInfo info;
        readonly DeviceConnector conn;

        CancellationTokenSource? src = new();
        
        bool connected;
        bool isError;
        string? errorLine;

        public ConnectingView(DeviceInfo info, StreamingOptions opt)
        {
            this.info = info;
         
            conn = new DeviceConnector(info, src, opt);
            conn.OnMessage += Conn_OnMessage;
        }

        private void Conn_OnMessage(string obj)
        {
            errorLine ??= "";
            errorLine += obj + "\n";
            Program.Instance.KickRendering(true);
        }

        public override void BackPressed()
        {
            // If we're connected, the view will be replaced in a few instants
            if (src is null && !isError)
                return;

            base.BackPressed();
        }

        public override async void Created()
        {
            StreamManager manager;
            try
            {
                manager = await conn.ConnectForPlayer();
            }
            catch (Exception e)
            {
                if (src!.IsCancellationRequested)
                    return;

                Console.WriteLine($"Player connection failed: {e}");
                Conn_OnMessage(e.ToString());
                isError = true;
                return;
            }
            finally 
            {
                // We don't need the token anymore.
                // if the connection failed it's not needed anymore
                // otherwise it's now owned by the player
                src = null;
                conn.OnMessage -= Conn_OnMessage;
            }

            // This must execute on the main thread
            Program.Instance.PostAction(() =>
            {
                try
                {
                    Program.Instance.ReplaceView(new PlayerView((PlayerManager)manager));
                    connected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Player creation failed");
                    Conn_OnMessage(e.ToString());
                    isError = true;
                }
            });
        }

        public override void Destroy()
        {
            if (!connected)
            {
                conn.OnMessage -= Conn_OnMessage;
                // If we are cdonnected the cancellation token is passed to the player and we don't own it anymore
                src?.Cancel();
            }
            
            base.Destroy();
        }

        public override void Draw()
        {
            if (!Gui.BeginWindow("Connecting"))
                return;

            ImGui.NewLine();

            Gui.H2();

            var title = isError ? Strings.Error : Strings.Title;
			Gui.CenterText(title);
			
            Gui.PopFont();

            Gui.CenterText(info.ToString());
            
            ImGui.NewLine();

            var btnSize = new Vector2(ImGui.GetWindowSize().X * 5 / 6, Gui.ButtonHeight());
            if (errorLine != null)
                ImGui.TextWrapped(errorLine);

            Gui.CursorFromBottom(btnSize.Y);

            var text = isError ? GeneralStrings.BackButton : GeneralStrings.CancelButton;
			if (Gui.CenterButton(text, btnSize))
                BackPressed();

            Gui.EndWindow();
        }
    }
}
