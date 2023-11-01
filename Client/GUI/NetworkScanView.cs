﻿using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    internal class NetworkScanView : View
    {
        readonly StreamingOptions options;
        readonly NetworkScan scanner = new();
        readonly List<DeviceInfo> devices = new List<DeviceInfo>();
        readonly byte[] IpAddressTextBuf = new byte[256];
        
        string autoConnect;

        Gui.Popup ipEnterPopup = new("Enter console IP address");
        Gui.Popup incompatiblePopup = new("Error");
        Gui.CenterGroup manualIpCenter = new();
        Gui.CenterGroup popupBtnCenter = new();
        Gui.CenterGroup popupTbCenter = new();
        string? lastError;

        public NetworkScanView(StreamingOptions opt, string? autoConnect = null)
        {
            Popups.Add(ipEnterPopup);
            Popups.Add(incompatiblePopup);

            this.options = opt;
            this.autoConnect = autoConnect;

            scanner.OnDeviceFound += OnDeviceFound;
            scanner.OnFailure += OnFailure;
        }

        private void OnDeviceFound(DeviceInfo info)
        {
            lock (this)
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
            lock (this)
            {
                lastError = obj;
            }
        }

        public override void EnterForeground()
        {
            scanner.StartScanning();
            
            //if (Debugger.IsAttached)
                devices.Add(DeviceInfo.Stub());
            
            base.EnterForeground();
        }

        public override void LeaveForeground()
        {
            scanner.StopScannning();

            lock (this)
                devices.Clear();

            base.LeaveForeground();
        }

        public override void BackPressed()
        {
            if (ipEnterPopup.HandleBackButton())
                return;

            base.BackPressed();
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
            Program.Instance.PushView(new ConnectingView(info, options));
        }

        public override void Draw()
        {
            var portrait = Program.Instance.IsPortrait;

            Gui.BeginWindow("Network scanner");
            var win = ImGui.GetWindowSize();

            Gui.CenterText("Searching for network devices...");
            
            if (autoConnect is not null)
            {
                ImGui.Spacing();

                if (autoConnect == "")
                    Gui.CenterText("SysDVR will connect automatically to the first device that appears");
                else
                    Gui.CenterText("SysDVR will connect automatically to the console with serial containing: " + autoConnect);

                if (Gui.CenterButton("Cancel auto conect", new(win.X * 5 / 6, 0)))
                {
                    autoConnect = null;
                }

                ImGui.Spacing();
            }
            else ImGui.NewLine();

            var sz = win;
            sz.Y *= portrait ? .5f : .4f;
            sz.X *= portrait ? .92f : .82f;
            
            lock (this)
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

            if (portrait)
            {
                Gui.CenterText("Can't find your device ?");
                if (Gui.CenterButton("Use IP address"))
                    ButtonEnterIp();
            }
            else
            {
                manualIpCenter.StartHere();
                ImGui.TextWrapped("Can't find your device ?   ");
                ImGui.SameLine();

                if (ImGui.Button("Use IP address"))
                    ButtonEnterIp();

                manualIpCenter.EndHere();
            }

            if (lastError is not null)
            {
                ImGui.TextWrapped(lastError);
            }

            sz.Y = Gui.ButtonHeight();
            Gui.CursorFromBottom(sz.Y);
            if (Gui.CenterButton("Go back", sz))
            {
                Program.Instance.PopView();
            }

            DrawIpEnterPopup();

            if (incompatiblePopup.Begin())
            {
                ImGui.TextWrapped("The selected device is not compatible with this version of the client.");
                ImGui.TextWrapped("Make sure you're using the same version of SysDVR on both the console and this device.");

                if (Gui.CenterButton("Go back"))
                    incompatiblePopup.RequestClose();

                ImGui.EndPopup();
            }

            Gui.EndWindow();
        }

        void DrawIpEnterPopup() 
        {
            if (ipEnterPopup.Begin())
            {
                popupTbCenter.StartHere();
                ImGui.InputText("##ip", IpAddressTextBuf, (uint)IpAddressTextBuf.Length);
                popupTbCenter.EndHere();
                ImGui.Spacing();
                popupBtnCenter.StartHere();
                if (ImGui.Button("   Connect   "))
                {
                    ipEnterPopup.RequestClose();
                    var ip = Encoding.UTF8.GetString(IpAddressTextBuf, 0, Array.IndexOf<byte>(IpAddressTextBuf, 0));
                    ConnectToDevice(DeviceInfo.ForIp(ip));
                }

                ImGui.SameLine();
                if (ImGui.Button("    Cancel    "))
                    ipEnterPopup.RequestClose();

                popupBtnCenter.EndHere();
                ImGui.NewLine();

                ImGui.EndPopup();
            }
        }
    }
}
