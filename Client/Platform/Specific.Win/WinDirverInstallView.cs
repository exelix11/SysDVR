﻿using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Platform.Specific.Win
{
	internal class WinDirverInstallView : View
	{
		readonly StringTable.UsbDriverTable Strings = Program.Strings.UsbDriver;

		string DriverStatus;

		bool IsInstalling;
		string? InstallationState;

		void CheckStatus()
		{
			try
			{
				WinDriverInstall.CheckInstalled(out DriverStatus);
			}
			catch (Exception ex) 
			{
				DriverStatus = $"{Strings.DetectionFailed} : {ex}";
				Console.WriteLine(ex.ToString());
			}
		}

		public override void BackPressed()
		{
			if (IsInstalling)
				return;

			base.BackPressed();
		}

		public override void Created()
		{
			base.Created();
			CheckStatus();
		}

		async void InstallDriver() 
		{
			IsInstalling = true;
			try
			{
				await WinDriverInstall.Install((s) => InstallationState = s + "\n");
				DriverStatus = Strings.InstallSucceeded;
			}
			catch (Exception ex) 
			{
				DriverStatus = $"{Strings.InstallFailed} : {ex}";
			}

			IsInstalling = false;
			InstallationState = "";
		}

		public override void Draw()
		{
			if (!Gui.BeginWindow("Driver install"))
				return;

			Gui.CenterText(Strings.Title);
			ImGui.NewLine();

			var portrait = Program.Instance.IsPortrait;
			var win = ImGui.GetWindowSize();
			var sz = win;

			sz.Y = Gui.ButtonHeight();
			sz.X *= portrait ? .92f : .82f;

			ImGui.TextWrapped(Strings.Description);
			ImGui.NewLine();

			if (IsInstalling)
			{
				ImGui.TextWrapped(Strings.Installing);
				ImGui.TextWrapped(InstallationState);
			}
			else
			{
				ImGui.TextWrapped(DriverStatus);
				ImGui.NewLine();

				if (Gui.CenterButton(Strings.InstallButton, sz))
					InstallDriver();

				if (Gui.CenterButton(Strings.CheckAgainButton, sz))
					CheckStatus();

				ImGui.NewLine();
				ImGui.TextWrapped(Strings.InstallInfo);
				ImGui.TextWrapped(WinDriverInstall.DriverUrl);
				ImGui.TextWrapped($"{Strings.FileHashInfo} {WinDriverInstall.DriverHash}");
				ImGui.NewLine();

				Gui.CursorFromBottom(sz.Y);
				if (Gui.CenterButton(GeneralStrings.BackButton, sz))
					BackPressed();
			}

			Gui.EndWindow();
		}
	}
}
