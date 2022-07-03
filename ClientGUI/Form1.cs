using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI
{
	public partial class Form1 : Form
	{
		StreamKind CurKind = StreamKind.Audio;
		IStreamTargetControl CurrentControl = null;

		static string VersionString()
		{
			var Version = typeof(Program).Assembly.GetName().Version;
			if (Version == null) return "<unknown version>";
			StringBuilder str = new StringBuilder();
			str.Append(Version.Major);
			str.Append(".");
			str.Append(Version.Minor);

			if (Version.Revision != 0)
			{
				str.Append(".");
				str.Append(Version.Revision);
			}

			return str.ToString();
		}

		public Form1()
		{
			InitializeComponent();
		}

		void SetDefaultText() => this.Text = "SysDVR-Client GUI " + VersionString();

		private void Form1_Load(object sender, EventArgs e)
		{
			SetDefaultText();

#if RELEASE
			if (!File.Exists("SysDVR-Client.dll"))
			{
				MessageBox.Show("SysDVR-Client.dll not found, did you extract all the files in the same folder ?");
				this.Close();
			}
#endif

			if (Utils.FindExecutableInPath("dotnet.exe") == null)
			{
				if (MessageBox.Show(".NET 6 doesn't seem to be installed on this pc but it's needed for SysDVR-Client, do you want to open the download page ?\r\n\r\nYou need to download .NET 6 desktop x64 runime or a more recent version", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
					Process.Start("https://dotnet.microsoft.com/download");
				this.Close();
			}
			else if (!Utils.IsDotnet6Installed())
			{
				if (MessageBox.Show("It seems you're running an outdated version of .NET. Since SysDVR 5.0 the client app requires the .NET 6 runtime. Do you want to open the download page ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
					Process.Start("https://dotnet.microsoft.com/download");
				else 
					MessageBox.Show("If you don't upgrade the installed version SysDVR may not work.") ;
			}

			rbStreamRtsp.Checked = true;
			rbChannelsBoth.Checked = true;
			rbPlay.Checked = true;
			cbAdvOpt.Checked = false;
		}

		private void StreamTargetSelected(object sender, EventArgs e)
		{
			if (!((RadioButton)sender).Checked)
				return;

			Dictionary<object, IStreamTargetControl> StreamControls = new Dictionary<object, IStreamTargetControl>
			{	
				{ rbStreamRtsp, new RTSPStreamOptControl() { Dock = DockStyle.Fill} },
				{ rbPlayMpv, new MpvStreamControl() { Dock = DockStyle.Fill} },
				{ rbSaveToFile , new FileStreamControl() { Dock = DockStyle.Fill} },
				{ rbPlay, new PlayStreamControl() { Dock = DockStyle.Fill} }
			};

			CurrentControl = StreamControls[sender];
			StreamConfigPanel.Controls.Clear();
			StreamConfigPanel.Controls.Add((Control)CurrentControl);
		}

		private void StreamKindSelected(object sender, EventArgs e)
		{
			if (!((RadioButton)sender).Checked)
				return;

			var cbToChannel = new Dictionary<object, StreamKind>
			{
				{ rbChannelsBoth, StreamKind.Both},
				{ rbChannelsVideo, StreamKind.Video},
				{ rbChannelsAudio, StreamKind.Audio},
			};

			CurKind = cbToChannel[sender];

			if (CurKind == StreamKind.Both && rbPlayMpv.Checked)
			{
				rbPlayMpv.Checked = false;
				rbStreamRtsp.Checked = true;
			}

			rbPlayMpv.Enabled = CurKind != StreamKind.Both;
		}

		void CheckUSBDriver() 
		{
			// Note if the state is unknown no message is shown, it usually means sysdvr has never been plugged in before
			if (DriverInstall.DriverHelper.GetDriverInfo() == DriverInstall.DriverStatus.NotInstalled)
			{
				if (MessageBox.Show("You selected USB streaming but it seems that the SysDVR driver is not installed, do you want to install it now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					DriverInstall.DriverHelper.InstallDriver();
				}
				else MessageBox.Show("Without installing the driver USB streaming may not work");
			}
		}

		string GetExtraArgs() 
		{
			StringBuilder str = new StringBuilder();

			void append(string s) { str.Append(" "); str.Append(s); }

			if (cbStats.Checked) append("--print-stats");
			if (cbUsbLog.Checked) append("--usb-debug");
			if (cbUsbWarn.Checked) append("--usb-warn");
			if (cbForceLibusb.Checked) append("--no-winusb");
			return str.ToString();
		}

		string GetFinalCommand() 
		{
			if (rbSrcUsb.Checked)
				CheckUSBDriver();

			try
			{
				if (CurrentControl == null)
					throw new Exception("Select all the options first");

				string extra = CurrentControl.GetExtraCmd();
				string commandLine = CurrentControl.GetCommandLine();

				StringBuilder str = new StringBuilder();

				if (!string.IsNullOrWhiteSpace(extra))
					str.Append("start ");

				str.Append("dotnet SysDVR-Client.dll ");

				if (rbSrcUsb.Checked)
					str.Append("usb ");
				else if (rbSrcTcp.Checked)
					str.AppendFormat("bridge {0} ", tbTcpIP.Text);
				else 
					throw new Exception("Invalid source");

				if (CurKind == StreamKind.Audio)
					str.Append("--no-video ");
				else if (CurKind == StreamKind.Video)
					str.Append("--no-audio ");

				str.Append(commandLine);

				str.Append(GetExtraArgs());

				if (!string.IsNullOrWhiteSpace(extra))
				{
					str.Append("\ntimeout 2 > NUL && ");
					str.Append(extra);
				}

				return str.ToString();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
			return null;
		}

		private void Launch(object sender, EventArgs e)
		{
			var cmds = GetFinalCommand()?.Split('\n');
			if (cmds != null)
			{
				string cmdArg = cmds.Length > 1 ? "/C" : "/K";
				foreach (var cmd in cmds)
					System.Diagnostics.Process.Start("cmd", $"{cmdArg} {cmd}");
				this.Close();
			}
		}

		private void ExportBatch(object sender, EventArgs e)
		{
			string cmd = GetFinalCommand();
			if (cmd == null) return;

			SaveFileDialog sav = new SaveFileDialog() { Filter = "batch file|*.bat", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory, RestoreDirectory = false, FileName = "SysDVR Launcher.bat" };
			if (sav.ShowDialog() != DialogResult.OK)
				return;

			if (!File.Exists(Path.Combine(Directory.GetParent(sav.FileName).FullName, "SysDVR-Client.dll")))
				if (MessageBox.Show("You're saving the bat file in a different path than the one containing SysDVR-client, the bat script won't work unless you place it there !\r\n\r\nDo you want to continue anyway ?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
					return;

			File.WriteAllText(sav.FileName, cmd);
			
			if (MessageBox.Show("Done, launch SysDVR-Client now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
				Launch(sender,e);
		}

		private void BatchInfo(object sender, EventArgs e) =>
			MessageBox.Show("This will create a bat file to launch SysDVR-Client with the selected options you will just need to double click it. The file name depends on the configuration, you can rename it later.\r\n");

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
			Process.Start("https://github.com/exelix11/SysDVR/wiki/Troubleshooting");

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => 
			Process.Start("https://github.com/exelix11/SysDVR/wiki/");

		private void tbTcpIP_Enter(object sender, EventArgs e)
		{
			if (tbTcpIP.Text == "IP address")
				tbTcpIP.Text = "";
		}

		private void tbTcpIP_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tbTcpIP.Text))
				tbTcpIP.Text = "IP address";
		}

		private void tbTcpIP_TextChanged(object sender, EventArgs e)
		{
			if (tbTcpIP.Text != "IP address" && tbTcpIP.Text != "" && !rbSrcTcp.Checked)
				rbSrcTcp.Checked = true;
		}

		private void cbAdvOpt_CheckedChanged(object sender, EventArgs e)
		{
			pAdvOptions.Visible = cbAdvOpt.Checked;
			this.Size = cbAdvOpt.Checked ? this.MaximumSize : this.MinimumSize;
		}

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("In case of issues with USB streaming you can try to reinstall the driver. Do you want to do it now ? (SysDVR will be closed)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
				if (DriverInstall.DriverHelper.InstallDriver())
					Environment.Exit(0);
			}
        }
    }

    static class Utils 
	{
		public static string FindExecutableInPath(string fileName) =>
			Environment.GetEnvironmentVariable("PATH")
				.Split(Path.PathSeparator)
				.Select(x => Path.Combine(x, fileName))
				.FirstOrDefault(x => File.Exists(x));

		public static bool IsDotnet6Installed()
		{
			Process proc = new Process();
			proc.StartInfo = new ProcessStartInfo()
			{
				FileName = "dotnet",
				Arguments = "--info",
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			proc.Start();
			proc.WaitForExit();
			var s = proc.StandardOutput.ReadToEnd();

			// Find dotnet 6
			foreach (Match match in new Regex(@"NETCore\.App (\d+)\.").Matches(s))
			{
				if (match.Captures.Count == 0) continue;

				string val = match.Value.Substring("NETCore.App ".Length).TrimEnd('.');

				if (int.TryParse(val, out int num) && num >= 6)
					return true;
			}

			return false;
		}
	}
}
