using ImGuiNET;
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
				DriverStatus = "Error while detecting the driver state: " + ex.Message;
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
				DriverStatus = "The installation was succesful";
			}
			catch (Exception ex) 
			{
				DriverStatus = "Installation failed: " + ex.ToString();
			}

			IsInstalling = false;
			InstallationState = "";
		}

		public override void Draw()
		{
			if (!Gui.BeginWindow("Driver install"))
				return;

			Gui.CenterText("Driver install");
			ImGui.NewLine();

			var portrait = Program.Instance.IsPortrait;
			var win = ImGui.GetWindowSize();
			var sz = win;

			sz.Y = Gui.ButtonHeight();
			sz.X *= portrait ? .92f : .82f;

			ImGui.TextWrapped("SysDVR now uses standard Android ADB drivers");
			ImGui.TextWrapped("The driver is signed by Google, so it is safe to install and doesn't require any complex install steps.");
			ImGui.NewLine();

			if (IsInstalling)
			{
				ImGui.TextWrapped("Installation in progress...");
				ImGui.TextWrapped(InstallationState);
			}
			else
			{
				ImGui.TextWrapped(DriverStatus);
				ImGui.NewLine();

				if (Gui.CenterButton("Install", sz))
					InstallDriver();

				if (Gui.CenterButton("Check status again", sz))
					CheckStatus();

				ImGui.NewLine();
				ImGui.TextWrapped("If you choose to install the driver it will be downloaded from");
				ImGui.TextWrapped(WinDriverInstall.DriverUrl);
				ImGui.TextWrapped("The expected SHA256 hash of the zip file is " + WinDriverInstall.DriverHash);
				ImGui.NewLine();

				Gui.CursorFromBottom(sz.Y);
				if (Gui.CenterButton("Back", sz))
					BackPressed();
			}

			Gui.EndWindow();
		}
	}
}
