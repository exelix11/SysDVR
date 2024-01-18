﻿using ImGuiNET;
using LibUsbDotNet;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
	internal class UsbDevicesView : View
	{
		readonly StringTable.UsbPageTable Strings = Program.Strings.Usb;

		readonly StreamingOptions options;
		readonly DvrUsbContext? context;
		readonly Gui.Popup incompatiblePopup = new(Program.Strings.General.PopupErrorTitle);

		DisposableCollection<DvrUsbDevice> devices;

		CancellationTokenSource? autoConnectCancel;
		string? autoConnect;
		string? autoConnectGuiText;
		string? lastError;

		public UsbDevicesView(StreamingOptions options, string? autoConnect = null)
		{
			this.options = options;
			this.autoConnect = autoConnect;

			Popups.Add(incompatiblePopup);

			try
			{
				context = new DvrUsbContext(Program.Options.UsbLogging);
			}
			catch (Exception ex)
			{
				lastError = ex.ToString();
			}

			if (autoConnect is not null)
			{
				autoConnectCancel = new CancellationTokenSource();
				_ = AutoConnectTask();

				if (autoConnect == "" || Program.Options.HideSerials)
					autoConnectGuiText = Program.Strings.Connection.AutoConnect;
				else
					autoConnectGuiText = string.Format(Program.Strings.Connection.AutoConnectWithSerial, autoConnect);
			}
		}

		public override void EnterForeground()
		{
			base.EnterForeground();
			SearchDevices();
		}

		public override void LeaveForeground()
		{
			StopAutoConnect();
			base.LeaveForeground();
		}

		public override void Destroy()
		{
			StopAutoConnect();
			devices?.Dispose();
			base.Destroy();
		}

		void StopAutoConnect()
		{
			lock (this)
			{
				autoConnect = null;
				autoConnectCancel?.Cancel();
			}
		}

		async Task AutoConnectTask()
		{
			try
			{
				while (true)
				{
					await Task.Delay(5000, autoConnectCancel.Token);

					if (autoConnect is null || autoConnectCancel.IsCancellationRequested)
						break;

					SearchDevices();
				}
			}
			catch (TaskCanceledException) { }
			catch (Exception ex)
			{
				Console.WriteLine($"AutoConnectTask failed {ex}");
			}
		}

		void SearchDevices()
		{
			lock (this)
			{
				if (context is null)
					return;

				lastError = null;

				try
				{
					devices = context.FindSysdvrDevices();

					if (autoConnect != null)
					{
						foreach (var dev in devices)
						{
							if (autoConnect == "" || dev.Info.Serial.EndsWith(autoConnect, StringComparison.InvariantCultureIgnoreCase))
							{
								ConnectToDevice(dev);
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					lastError = ex.ToString();
				}
			}
		}

		void ConnectToDevice(DvrUsbDevice info)
		{
			StopAutoConnect();

			if (!info.Info.IsProtocolSupported)
			{
				Popups.Open(incompatiblePopup);
				return;
			}

			if (devices is not null)
			{
				devices.ExcludeFromDispose(info);
				devices.Dispose();
				devices = null;
			}

			Program.Instance.PushView(new ConnectingView(info.Info, options));
		}

		public override void Draw()
		{
			var portrait = Program.Instance.IsPortrait;

			if (!Gui.BeginWindow("USB Devices list"))
				return;

			var win = ImGui.GetWindowSize();

			Gui.CenterText(Strings.Title);

			if (autoConnect is not null)
			{
				ImGui.Spacing();

				Gui.CenterText(autoConnectGuiText);

				if (Gui.CenterButton(Program.Strings.Connection.AutoConnectCancelButton, new(win.X * 5 / 6, 0)))
					StopAutoConnect();

				ImGui.Spacing();
			}

			ImGui.NewLine();

			var sz = win;
			sz.Y *= portrait ? .5f : .4f;
			sz.X *= portrait ? .92f : .82f;

			if (devices is null || devices.Count == 0)
			{
				Gui.CenterText(Strings.NoDevicesLabel);
				ImGui.NewLine();

				ImGui.TextWrapped(Strings.NoDevicesHelp);

				if (Program.IsWindows)
					ImGui.TextWrapped(Strings.NoDevicesHelpWindows);
				else if (Program.IsLinux)
					ImGui.TextWrapped(Strings.NoDevicesHelpLinux);
				else if (Program.IsAndroid)
					ImGui.TextWrapped(Strings.NoDevicesHelpAndroid);
			}
			else
			{
				ImGui.SetCursorPosX(win.X / 2 - sz.X / 2);
				ImGui.BeginChildFrame(ImGui.GetID("##DevList"), sz, ImGuiWindowFlags.NavFlattened);
				var btn = new Vector2(sz.X, 0);
				foreach (var dev in devices)
				{
					if (ImGui.Button(dev.Info.ToString(), btn))
						ConnectToDevice(dev);
				}
				ImGui.EndChildFrame();
			}
			ImGui.NewLine();

			sz.Y = Gui.ButtonHeight();
			if (Gui.CenterButton(Strings.RefreshButton, sz))
			{
				SearchDevices();
			}

			if (lastError is not null)
			{
				Gui.CenterText(GeneralStrings.ErrorMessage);
				ImGui.TextWrapped(lastError);
			}

			Gui.CursorFromBottom(sz.Y);
			if (Gui.CenterButton(GeneralStrings.BackButton, sz))
			{
				Program.Instance.PopView();
			}

			if (incompatiblePopup.Begin())
			{
				ImGui.TextWrapped(Program.Strings.Connection.DeviceNotCompatible);

				if (Gui.CenterButton(GeneralStrings.BackButton))
					incompatiblePopup.RequestClose();

				ImGui.EndPopup();
			}

			Gui.EndWindow();
		}
	}
}
