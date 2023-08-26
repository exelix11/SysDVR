using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    internal class MainView : View
    {
        byte[] text = new byte[1024];
        
        readonly string UsbModeWarn;
        readonly bool IsWindows;

        public MainView()
        {
            IsWindows = OperatingSystem.IsWindows();
            if (OperatingSystem.IsWindows())
                UsbModeWarn = "Requires USB drivers to be installed.";
            else if (OperatingSystem.IsLinux())
                UsbModeWarn = "Requires udev rules to be configured correctly.";
        }

        //void LaunchTcp() 
        //{
        //    var StreamManager = new PlayerManager(true, true, false, null, null)
        //    {
        //        WindowTitle = "",
        //        StartFullScreen = false
        //    };

        //    var len = Array.IndexOf<byte>(text, 0);
        //    var ip = Encoding.ASCII.GetString(text, 0, len).Trim();
        //    StreamManager.AddSource(new TCPBridgeSource(ip, StreamKind.Video));
        //    StreamManager.AddSource(new TCPBridgeSource(ip, StreamKind.Audio));

        //    StreamManager.Begin();
        //    Program.PlayerInstance = StreamManager.player;
        //}

        //void LaunchStub() 
        //{
        //    var StreamManager = new PlayerManager(true, true, false, null, null)
        //    {
        //        WindowTitle = "",
        //        StartFullScreen = false
        //    };

        //    StreamManager.AddSource(new StubSource(true, true));

        //    StreamManager.Begin();
        //    Program.PlayerInstance = StreamManager.player;
        //}

        bool ModeButton2(Image image, string title, int width, int height)
        {
            const int InnerPadding = 15;

            // This is what we're looking for:
            // TITLE TITLE
            //   <image> 

            Gui.H2();
            var titleLen = ImGui.CalcTextSize(title);
            ImGui.PopFont();

            Vector2 imageFrame = new(
                width - InnerPadding * 2,
                image.ScaleHeight(width - InnerPadding * 2));

            var imageSpaceY = height - InnerPadding * 3 - titleLen.Y;

            if (imageFrame.Y > imageSpaceY)
                imageFrame = new(image.ScaleWidth((int)imageSpaceY), imageSpaceY);

            var x = ImGui.GetCursorPosX();
            var y = ImGui.GetCursorPosY();

            ImRect bodySize = new ImRect();
            bodySize.Min = new(x, y);
            bodySize.Max = new(x + width, y + height);

            var id = ImGui.GetID(title);

            if (!ImGui.ItemAdd(bodySize, id, IntPtr.Zero, ImGuiItemFlags.None))
                return false;

            var btn = ImGui.ButtonBehavior(bodySize, id, out var hovered, out var held, ImGuiButtonFlags.MouseButtonDefault);
            var col = ImGui.GetColorU32((held && hovered) ? ImGuiCol.ButtonActive : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);

            ImGui.RenderNavHighlight(bodySize, id, 0);
            ImGui.RenderFrame(bodySize.Min, bodySize.Max, col, true, 0);

            y += InnerPadding;

            // Title
            ImGui.SetCursorPos(new(x + width / 2 - titleLen.X / 2, y));
            Gui.H2();
            ImGui.Text(title);
            ImGui.PopFont();

            y += InnerPadding + titleLen.Y;

            ImGui.SetCursorPos(new Vector2(x + width / 2 - imageFrame.X / 2, y));
            ImGui.Image(image.Texture, imageFrame);

            return btn;
        }

        public override void Draw()
        {
            Gui.BeginWindow("Main");
            Gui.CenterImage(Resources.Logo, 120);
            Gui.H1();
            Gui.CenterText("SysDVR-Client");
            ImGui.PopFont();

            bool wifi = false, usb = false;

            int ModeButtonWidth =  (int)(Program.Instance.UiScale * 350);
            int ModeButtonHeight = (int)(Program.Instance.UiScale * 200);
            var w = ImGui.GetContentRegionAvail().X;
            var y = ImGui.GetCursorPosY() + 20;

            if (w < ModeButtonWidth * 2.2)
            {
                var center = w / 2 - ModeButtonWidth / 2;

                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton2(Resources.WifiIcon, "Network mode", ModeButtonWidth, ModeButtonHeight);

                ImGui.SetCursorPos(new(center, y + 20 + ModeButtonHeight));
                usb = ModeButton2(Resources.UsbIcon, "USB mode", ModeButtonWidth, ModeButtonHeight);
            }
            else 
            {
                var center = w / 2 - (ModeButtonWidth + ModeButtonWidth + 20) / 2;
                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton2(Resources.WifiIcon, "Network mode", ModeButtonWidth, ModeButtonHeight);

                ImGui.SetCursorPos(new(center + ModeButtonWidth + 20, y));
                usb = ModeButton2(Resources.UsbIcon, "USB mode", ModeButtonWidth, ModeButtonHeight);
            }

            ImGui.Button("Open the guide");
            if (IsWindows)
            {
                ImGui.SameLine();
                ImGui.Button("Install USB driver");
            }

            ImGui.End();
        }
    }
}
