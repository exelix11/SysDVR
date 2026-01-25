using ImGuiNET;
using SysDVR.Client.App;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace SysDVR.Client.GUI
{
    internal class NetworkScanView : View
    {
		readonly StringTable.NetworkScanTable Strings = Program.Strings.NetworkScan;

		readonly StreamingOptions options;
        readonly NetworkScan scanner = new();
        readonly List<DeviceInfo> devices = new List<DeviceInfo>();
        readonly byte[] IpAddressTextBuf = new byte[256];

        readonly object syncLock = new();

        string? autoConnect;
        string autoConnectGuiText;

        Gui.Popup ipEnterPopup = new(Program.Strings.NetworkScan.IpConnectDialogTitle);
        Gui.Popup incompatiblePopup = new(Program.Strings.General.PopupErrorTitle);
        Gui.CenterGroup popupBtnCenter = new();
        Gui.CenterGroup popupTbCenter = new();
        string? lastError;            

        public NetworkScanView(ClientApp owner, StreamingOptions opt, string? autoConnect = null) : base(owner)
        {
            Popups.Add(ipEnterPopup);
            Popups.Add(incompatiblePopup);

            this.options = opt;
            this.autoConnect = autoConnect;

            scanner.OnDeviceFound += OnDeviceFound;
            scanner.OnFailure += OnFailure;

            if (autoConnect is not null)
            {
                if (autoConnect == "" || Program.Options.HideSerials)
                    autoConnectGuiText = Program.Strings.Connection.AutoConnect;
                // Fromat this text once so it's not done every frame
                else
					autoConnectGuiText = string.Format(Program.Strings.Connection.AutoConnectWithSerial, autoConnect);
			}

            if (!string.IsNullOrWhiteSpace(Program.Options.LastNetworkModeIP))
            {
                var data = Encoding.UTF8.GetBytes(Program.Options.LastNetworkModeIP);
                if (data.Length < IpAddressTextBuf.Length - 1)
                    data.AsSpan().CopyTo(IpAddressTextBuf);
            }
        }

        private void OnDeviceFound(DeviceInfo info)
        {
            lock (syncLock)
            {
                devices.Add(info);
            }

            if (autoConnect != null) 
            {
                if (autoConnect == "" || info.Serial.EndsWith(autoConnect, StringComparison.InvariantCultureIgnoreCase))
                    ConnectToDevice(info);
            }

            SignalEvent();
        }

        private void OnFailure(string obj)
        {
            lock (syncLock)
            {
                lastError = obj;
            }
        }

        public override void EnterForeground()
        {
            scanner.StartScanning();
            
            if (Program.Options.Debug.Log || Debugger.IsAttached)
                devices.Add(DeviceInfo.Stub());
            
            base.EnterForeground();
        }

        public override void LeaveForeground()
        {
            scanner.StopScannning();

            lock (syncLock)
                devices.Clear();

            base.LeaveForeground();
        }

        void ButtonEnterIp()
        {
            Popups.Open(ipEnterPopup);
        }

        void ConnectToDevice(DeviceInfo info)
        {
            if (!info.IsProtocolSupported)
            {
                Popups.Open(incompatiblePopup);
                return;
            }

            autoConnect = null;
            Owner.PushView(new ConnectingView(Owner, info, options));
        }

        public override void Draw()
        {
            var portrait = Owner.IsPortrait;

            if (!Gui.BeginWindow("Network scanner"))
                return;

            var win = ImGui.GetWindowSize();

            Gui.CenterText(Strings.Title);
            
            if (autoConnect is not null)
            {
                ImGui.Spacing();
                
                Gui.CenterText(autoConnectGuiText);

                if (Gui.CenterButton(Program.Strings.Connection.AutoConnectCancelButton, new(win.X * 5 / 6, 0)))
                {
                    autoConnect = null;
                }

                ImGui.Spacing();
            }
            
            ImGui.NewLine();

            var sz = win;
            sz.Y *= portrait ? .5f : .4f;
            sz.X *= portrait ? .92f : .82f;
            
            lock (syncLock)
            {
                ImGui.SetCursorPosX(win.X / 2 - sz.X / 2);
                ImGui.BeginChildFrame(ImGui.GetID("##DevList"), sz, ImGuiWindowFlags.NavFlattened);
                var btn = new Vector2(sz.X, 0);
                foreach (var dev in devices)
                {
                    if (ImGui.Button(dev.ToString(), btn))
                        ConnectToDevice(dev);
                }
                ImGui.EndChildFrame();
                ImGui.NewLine();
            }

            Gui.CenterText(Strings.ManualConnectLabel);
            
            if (Gui.CenterButton(Strings.ManualConnectButton))
                ButtonEnterIp();

			if (!Program.IsAndroid)
				ImGui.TextWrapped(Strings.FirewallLabel);

			if (lastError is not null)
            {
                ImGui.TextWrapped(lastError);
            }

            sz.Y = Gui.ButtonHeight();
            Gui.CursorFromBottom(sz.Y);
            if (Gui.CenterButton(GeneralStrings.BackButton, sz))
            {
                Owner.PopView();
            }

            DrawIpEnterPopup();

            if (incompatiblePopup.Begin())
            {
                ImGui.TextWrapped(Program.Strings.Connection.DeviceNotCompatible);

                if (Gui.CenterButton(GeneralStrings.BackButton))
                    incompatiblePopup.RequestClose();

                ImGui.EndPopup();
            }

            Gui.EndWindow();
        }

        void SaveIpPreference(string ipAddress)
        {
            if (Program.Options.LastNetworkModeIP != ipAddress)
            {
                Program.Options.LastNetworkModeIP = ipAddress;

                try
                {
                    SystemUtil.StoreSettingsString(Program.Options.SerializeToJson());
                }
                catch (Exception e)
                {
                    Program.DebugLog("Failed to store settings: " + e);
                }
            }
        }

        void DrawIpEnterPopup() 
        {
            if (ipEnterPopup.Begin())
            {
                ImGui.TextWrapped(Strings.ConnectByIPLabel);

                popupTbCenter.StartHere();
                ImGui.InputText("##ip", IpAddressTextBuf, (uint)IpAddressTextBuf.Length);
                popupTbCenter.EndHere();

                ImGui.Spacing();
                popupBtnCenter.StartHere();
                if (ImGui.Button(Strings.ConnectByIPButton))
                {
                    ipEnterPopup.RequestClose();
                    var ip = Encoding.UTF8.GetString(IpAddressTextBuf, 0, Array.IndexOf<byte>(IpAddressTextBuf, 0));
                    SaveIpPreference(ip);
                    ConnectToDevice(DeviceInfo.ForIp(ip));
                }

                ImGui.SameLine();
                if (ImGui.Button(GeneralStrings.CancelButton))
                    ipEnterPopup.RequestClose();

                popupBtnCenter.EndHere();
                ImGui.NewLine();

                ImGui.EndPopup();
            }
        }
    }
}
