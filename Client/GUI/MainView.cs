using ImGuiNET;
using LibUsbDotNet;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

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

        enum StreamChannels
        {
            Audio,
            Video,
            Both
        }

        StreamChannels channels = StreamChannels.Both;

        Gui.CenterGroup centerRadios;
        Gui.CenterGroup centerOptions;
        
        float uiScale;
        bool portrait;
        int ModeButtonWidth;
        int ModeButtonHeight;

        public override void ResolutionChanged()
        {        
            centerRadios.Reset();
            centerOptions.Reset();

            portrait = Program.Instance.IsPortrait;
            uiScale = Program.Instance.UiScale;
            ModeButtonWidth = (int)(350 * uiScale);
            ModeButtonHeight = (int)(200 * uiScale);

            base.ResolutionChanged();
        }

        public override void Draw()
        {
            Gui.BeginWindow("Main");
            Gui.CenterImage(Resources.Logo, 120);
            Gui.H1();
            Gui.CenterText("SysDVR-Client " + Program.Version);
            ImGui.PopFont();

            bool wifi = false, usb = false;

            var w = ImGui.GetContentRegionAvail().X;
            var y = ImGui.GetCursorPosY() + 40 * uiScale;

            if (w < ModeButtonWidth * 2.2)
            {
                var center = w / 2 - ModeButtonWidth / 2;

                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton2(Resources.WifiIcon, "Network mode", ModeButtonWidth, ModeButtonHeight);

                y += 20 * uiScale + ModeButtonHeight;

                ImGui.SetCursorPos(new(center, y));
                usb = ModeButton2(Resources.UsbIcon, "USB mode", ModeButtonWidth, ModeButtonHeight);

                y += ModeButtonHeight;
            }
            else 
            {
                var center = w / 2 - (ModeButtonWidth + ModeButtonWidth + 20) / 2;
                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton2(Resources.WifiIcon, "Network mode", ModeButtonWidth, ModeButtonHeight);

                ImGui.SetCursorPos(new(center + ModeButtonWidth + 20, y));
                usb = ModeButton2(Resources.UsbIcon, "USB mode", ModeButtonWidth, ModeButtonHeight);

                y += ModeButtonHeight;
            }

            ImGui.SetCursorPos(new(0, y + 30 * uiScale));

            Gui.CenterText("Select the streaming mode");

            centerRadios.StartHere();
            ChannelRadio("Video only", StreamChannels.Video);
            ImGui.SameLine();
            ChannelRadio("Audio only", StreamChannels.Audio);
            ImGui.SameLine();
            ChannelRadio("Stream Both", StreamChannels.Both);
            centerRadios.EndHere();

            ImGui.NewLine();

            if (ImGui.Button("Stub player"))
            {
                LaunchStub();
            }

            ImGui.PushFont(Program.Instance.FontText);

            centerOptions.StartHere();
            ImGui.Button("Open the guide");
            if (IsWindows)
            {
                ImGui.SameLine();
                ImGui.Button("Install USB driver");
            }
            ImGui.SameLine();
            ImGui.Button("Settings");
            centerOptions.EndHere();

            ImGui.PopFont();

            ImGui.ImageButton("asdsa", Resources.UsbIcon.Texture, new(60, 20));

            Gui.EndWindow();
        }

        void ChannelRadio(string name, StreamChannels target) 
        {
            if (ImGui.RadioButton(name, channels == target))
                channels = target;
        }

        void LaunchStub() 
        {
           //var StreamManager = new PlayerManager(true, false);
           //
           //StreamManager.AddSource(new StubSource(true, false));
           //
           //var view = new PlayerView(StreamManager.VideoTarget, StreamManager.AudioTarget);
           //Program.Instance.PushView(view);
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
            float InnerPadding = 15 * uiScale;

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

            var scroll = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

            // Apply scroll only to the bounding box since it uses lower level APIs
            ImRect bodySize = new ImRect();
            bodySize.Min = new Vector2(x, y) - scroll;
            bodySize.Max = new Vector2(x + width, y + height) - scroll;

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
    }
}
