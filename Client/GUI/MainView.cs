using FFmpeg.AutoGen;
using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Platform;
using System;
using System.Numerics;

namespace SysDVR.Client.GUI
{
    internal class MainView : View
    {
        readonly string Heading;
        readonly string SecondLine;

        bool HasDiskPermission;
        bool CanRequesDiskPermission;

        StreamKind StreamMode = StreamKind.Both;

        Gui.CenterGroup centerRadios;
        Gui.CenterGroup centerOptions;

        Gui.Popup initErrorPopup = new Gui.Popup(Program.Strings.MainGUI_InitializationErrorTitle);
        string? initError;

        float uiScale;
        int ModeButtonWidth;
        int ModeButtonHeight;

        public MainView()
        {
            Popups.Add(initErrorPopup);

            Heading = "SysDVR-Client " + Program.Version;
            SecondLine = $"build id {Program.BuildID}";

            UpdateDiskPermissionStatus();

			initError = Program.GetInitializationError();
			if (!string.IsNullOrWhiteSpace(initError))
                Popups.Open(initErrorPopup);
        }

		public override void EnterForeground()
		{
			base.EnterForeground();
			StreamMode = Program.Options.Streaming.Kind;
		}

		void UpdateDiskPermissionStatus()
        {
            HasDiskPermission = Resources.HasDiskAccessPermission();
            if (!HasDiskPermission)
                CanRequesDiskPermission = Resources.CanRequestDiskAccessPermission();
        }

        public override void ResolutionChanged()
        {
            centerRadios.Reset();
            centerOptions.Reset();

            uiScale = Program.Instance.UiScale;
            ModeButtonWidth = (int)(350 * uiScale);
            ModeButtonHeight = (int)(200 * uiScale);

            base.ResolutionChanged();
        }

        public override void Draw()
        {
            if (!Gui.BeginWindow("Main"))
                return;

            Gui.CenterImage(Resources.Logo, 120);
            Gui.H1();
            Gui.CenterText(Heading);
			Gui.PopFont();
            Gui.CenterText(SecondLine);

            bool wifi = false, usb = false;

            var w = ImGui.GetContentRegionAvail().X;
            var y = ImGui.GetCursorPosY() + 40 * uiScale;

            if (w < ModeButtonWidth * 2.2)
            {
                var center = w / 2 - ModeButtonWidth / 2;

                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton(Resources.WifiIcon, Program.Strings.MainGUI_NetworkButton, ModeButtonWidth, ModeButtonHeight);

                y += 20 * uiScale + ModeButtonHeight;

                ImGui.SetCursorPos(new(center, y));
                usb = ModeButton(Resources.UsbIcon, Program.Strings.MainGUI_USBButton, ModeButtonWidth, ModeButtonHeight);

                y += ModeButtonHeight;
            }
            else
            {
                var center = w / 2 - (ModeButtonWidth + ModeButtonWidth + 20) / 2;
                ImGui.SetCursorPos(new(center, y));
                wifi = ModeButton(Resources.WifiIcon, Program.Strings.MainGUI_NetworkButton, ModeButtonWidth, ModeButtonHeight);

                ImGui.SetCursorPos(new(center + ModeButtonWidth + 20, y));
                usb = ModeButton(Resources.UsbIcon, Program.Strings.MainGUI_USBButton, ModeButtonWidth, ModeButtonHeight);

                y += ModeButtonHeight;
            }

            if (usb)
                Program.Instance.PushView(new UsbDevicesView(BuildOptions()));
            else if (wifi)
                Program.Instance.PushView(new NetworkScanView(BuildOptions()));

            ImGui.SetCursorPos(new(0, y + 30 * uiScale));

            Gui.CenterText(Program.Strings.MainGUI_ChannelLabel);

            centerRadios.StartHere();
            ChannelRadio(Program.Strings.MainGUI_ChannelVideo, StreamKind.Video);
            ImGui.SameLine();
            ChannelRadio(Program.Strings.MainGUI_ChannelAudio, StreamKind.Audio);
            ImGui.SameLine();
            ChannelRadio(Program.Strings.MainGUI_ChannelBoth, StreamKind.Both);
            centerRadios.EndHere();

            ImGui.NewLine();

            if (!HasDiskPermission)
            {
                ImGui.TextWrapped(Program.Strings.MainGUI_FileAccess);
                if (CanRequesDiskPermission)
                {
                    if (Gui.CenterButton(Program.Strings.MainGUI_FileAccessRequestButton))
                    {
                        Resources.RequestDiskAccessPermission();
                        UpdateDiskPermissionStatus();
                    }
                }
                ImGui.NewLine();
            }

            centerOptions.StartHere();
			if (ImGui.Button(Program.Strings.MainGUI_GithubButton))
				SystemUtil.OpenURL("https://github.com/exelix11/SysDVR/");

			ImGui.SameLine();
			if (ImGui.Button(Program.Strings.MainGUI_GuideButton))
				SystemUtil.OpenURL("https://github.com/exelix11/SysDVR/wiki");

			if (Program.IsWindows)
            {
                ImGui.SameLine();
                if (ImGui.Button(Program.Strings.MainGUI_DriverInstallButton))
                    Program.Instance.PushView(new Platform.Specific.Win.WinDirverInstallView());
            }

			ImGui.SameLine();
            if (ImGui.Button(Program.Strings.MainGUI_SettingsButton))
            {
                Program.Instance.PushView(new OptionsView());
            }
            centerOptions.EndHere();

            DrawInitErroPopup();

            Gui.EndWindow();
        }

        void DrawInitErroPopup()
        {
            if (initErrorPopup.Begin())
            {
                ImGui.TextWrapped(initError);

                ImGui.NewLine();

                if (ImGui.Button(Program.Strings.MainGUI_PopupCloseButton))
                    initErrorPopup.RequestClose();

                ImGui.EndPopup();
            }
        }

        void ChannelRadio(string name, StreamKind target)
        {
            if (ImGui.RadioButton(name, StreamMode == target))
                StreamMode = target;
        }

        StreamingOptions BuildOptions() 
        {
            var opt = Program.Options.Streaming.Clone();
            opt.Kind = StreamMode;
            return opt;
        }

		bool ModeButton(Image image, string title, int width, int height)
        {
            float InnerPadding = 15 * uiScale;

            // This is what we're looking for:
            // TITLE TITLE
            //   <image> 

            Gui.H2();
            var titleLen = ImGui.CalcTextSize(title);
			Gui.PopFont();

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
			Gui.PopFont();

            y += InnerPadding + titleLen.Y;

            ImGui.SetCursorPos(new Vector2(x + width / 2 - imageFrame.X / 2, y));
            ImGui.Image(image.Texture, imageFrame);

            return btn;
        }
    }
}
