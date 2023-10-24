using Microsoft.Win32;
using SysDVRClientGUI.DriverInstall;
using SysDVRClientGUI.Forms.DriverInstall;
using SysDVRClientGUI.ModesUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using static SysDVRClientGUI.Logic.Constants;
using static SysDVRClientGUI.Logic.HelperFunctions;

namespace SysDVRClientGUI.Forms
{
    public partial class Main : Form
    {
        StreamKind CurKind = StreamKind.Audio;
        IStreamTargetControl CurrentControl = null;

        readonly string ClientDllPath;
        readonly string DotnetPath;
        readonly int DotnetMajorVersion;
        readonly bool DotnetIs32Bit;

        const string BatchLauncherFileCheckTemplate =
@":: Ensure {1} file exists
if not exist ""{0}"" (
    echo.
    echo Could not find {1}, create a new launcher from the GUI.
    pause
    exit /b 1
)

";

        public Main()
        {
            this.InitializeComponent();
            this.Size = cbAdvOpt.Checked ? this.MaximumSize : this.MinimumSize;
            this.Text = $"{typeof(Main).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title} {GetVersionString()}";
            this.StreamTargetSelected(this.rbPlay, EventArgs.Empty);
            this.StreamKindSelected(this.rbChannelsBoth, EventArgs.Empty);

            if (Program.ApplicationIcon != null)
                this.Icon = Program.ApplicationIcon;

            if (File.Exists(SYSDVR_DLL))
                ClientDllPath = SYSDVR_DLL;
            // When in debug mode also search sysdvr's visual studio build folder
#if DEBUG
            else if (File.Exists(@$"..\..\..\..\Client\bin\Debug\net7.0\{SYSDVR_DLL}"))
                ClientDllPath = Path.GetFullPath(@$"..\..\..\..\Client\bin\Debug\net7.0\{SYSDVR_DLL}");
#endif
            DotnetMajorVersion = FindDotnet(out DotnetPath, out DotnetIs32Bit);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClientDllPath))
            {
                MessageBox.Show($"{SYSDVR_DLL} not found, did you extract all the files in the same folder ?");
                this.Close();
            }

            if (DotnetMajorVersion == 0)
            {
                if (MessageBox.Show(".NET doesn't seem to be installed on this pc but it's needed for SysDVR-Client, do you want to open the download page ?\r\n\r\nYou need to download .NET 6 desktop x64 runtime or a more recent version", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start("https://dotnet.microsoft.com/download");
                this.Close();
            }
            else if (Environment.Is64BitOperatingSystem && DotnetIs32Bit)
            {
                if (MessageBox.Show("It seems you installed 32-bit .NET instead of the 64-bit one, SysDVR-CLient will not work.\r\n\r\nYou can download the 64-bit version from: https://aka.ms/dotnet/6.0/windowsdesktop-runtime-win-x64.exe\r\n\r\nDo you want to open it now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start("https://aka.ms/dotnet/6.0/windowsdesktop-runtime-win-x64.exe");
                    this.Close();
                }
            }

            if (DotnetMajorVersion < REQUIRED_DOTNET_MAJOR)
            {
                if (MessageBox.Show("It seems you're running an outdated version of .NET. SysDVR-Client requires .NET 6 runtime or a more recent version. Do you want to open the download page ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start("https://dotnet.microsoft.com/download");
                else
                    MessageBox.Show("If you don't upgrade the installed version SysDVR may not work.");
            }
        }

        private void StreamTargetSelected(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;

            Dictionary<object, IStreamTargetControl> StreamControls = new()
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

        // Returns whether should cancel the operation
        bool CheckUSBDriver()
        {
        check_again:
            var state = DriverHelper.GetDriverInfo();
            if (state == DriverStatus.NotInstalled)
            {
                if (MessageBox.Show("You selected USB streaming but it seems that the SysDVR driver is not installed, do you want to install it now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    new DriverInstallForm(false).ShowDialog();
                    return true;
                }
                else MessageBox.Show("Without installing the driver USB streaming may not work");
            }
            else if (state == DriverStatus.Unknown)
            {
                var res = MessageBox.Show(
@"You selected USB streaming but Windows reports that it has never seen the SysDVR USB device, before continuing try the following:
1) Open SysDVR settings on your console and select USB mode then click apply.
   - If the settings app says SysDVR is not running, reboot your console.
2) Launch a compatible game.
3) Connect the console to the PC.

If you did everything correctly windows should play the device plugged in sound and possibly show a 'installing device' screen, if that happens click YES on this message box.
if you don't see anything it is possible that your USB C cable does not support data connections and you should try a different one. USB C to C cables are known to cause issues.

Do you want to try searching for the SysDVR USB device again ?
Pressing no will try to start streaming regardless but it will probably fail."
                , "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);


                if (res == DialogResult.Yes)
                    goto check_again;
                else if (res == DialogResult.Cancel)
                    return true;
            }
            return false;
        }

        string GetExtraArgs()
        {
            StringBuilder str = new StringBuilder();

            void append(string s) { str.Append(" "); str.Append(s); }

            if (cbStats.Checked || cbIgnoreSync.Checked || cbLogStatus.Checked)
            {
                List<string> opt = new List<string>();
                opt.Add("log");

                if (cbStats.Checked)
                    opt.Add("stats");

                if (cbIgnoreSync.Checked)
                    opt.Add("nosync");

                append("--debug " + string.Join(",", opt));
            }
            if (cbUsbLog.Checked) append("--usb-debug");
            if (cbUsbWarn.Checked) append("--usb-warn");
            return str.ToString();
        }

        LaunchCommand GetClientCommandLine()
        {
            StringBuilder args = new();

            args.Append($"\"{Path.GetFullPath(ClientDllPath)}\" ");

            if (rbSrcUsb.Checked)
                args.Append("usb ");
            else if (rbSrcTcp.Checked)
                args.AppendFormat("bridge {0} ", TXT_TcpIp.Text);
            else
                throw new Exception("Select a valid source.");

            if (CurKind == StreamKind.Audio)
                args.Append("--no-video ");
            else if (CurKind == StreamKind.Video)
                args.Append("--no-audio ");

            args.Append(CurrentControl.GetClientCommandLine());
            if (args[args.Length - 1] != ' ')
                args.Append(" ");

            args.Append(GetExtraArgs());

            return new LaunchCommand
            {
                Executable = DotnetPath,
                Arguments = args.ToString().Trim(),
                FileDependencies = new string[] { Path.GetFullPath(ClientDllPath) }
            };
        }

        LaunchCommand[] GetFinalCommand()
        {
            if (rbSrcUsb.Checked)
                if (CheckUSBDriver())
                    return null;

            try
            {
                if (CurrentControl == null)
                    throw new Exception("Select all the options first");

                var extra = CurrentControl.GetExtraCmd();

                return new LaunchCommand[]
                {
                    GetClientCommandLine(),
                    CurrentControl.GetExtraCmd()
                }
                .Where(x => x != null).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return null;
        }

        private void Launch(object sender, EventArgs e)
        {
            var cmds = GetFinalCommand();
            if (cmds == null)
                return;

            // Launch SysDVR-Client with /K so any error is shown to the user
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = "cmd",
                // Cursed quote escaping https://superuser.com/questions/1213094/how-to-escape-in-cmd-exe-c-parameters
                Arguments = $"/K \"{cmds[0]}\""
            });

            if (cmds.Length > 1)
            {
                // Give sysdvr time to start then launch any extra commands
                Thread.Sleep(2000);

                // Launch extra commands with /C so they close automatically
                foreach (var cmd in cmds.Skip(1))
                    Process.Start("cmd", $"/C \"{cmd}\"");
            }

            this.Close();
        }

        private void ExportBatch(object sender, EventArgs e)
        {
            LaunchCommand[] cmds = GetFinalCommand();
            if (cmds == null)
                return;

            SaveFileDialog sav = new() { Filter = "batch file|*.bat", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory, RestoreDirectory = false, FileName = "SysDVR Launcher.bat" };
            if (sav.ShowDialog() != DialogResult.OK)
                return;

            StringBuilder bat = new();

            bat.AppendLine("@echo off");
            bat.AppendLine("title SysDVR Launcher");
            bat.AppendLine("echo Launching SysDVR-Client...");

            // Check all dependencies first
            foreach (var cmd in cmds)
            {
                bat.AppendFormat(BatchLauncherFileCheckTemplate, cmd.Executable, Path.GetFileName(cmd.Executable));

                if (cmd.FileDependencies != null)
                    foreach (var dep in cmd.FileDependencies)
                        bat.AppendFormat(BatchLauncherFileCheckTemplate, dep, Path.GetFileName(dep));
            }

            // cd to sysdvr folder so that the dll can be found
            bat.AppendLine($"cd /D \"{Path.GetDirectoryName(Path.GetFullPath(ClientDllPath))}\"");

            // If there are multiple commands use start to launch them in parallel
            string prefix = cmds.Length > 1 ? "start " : "";

            // Launch the commands
            foreach (var cmd in cmds)
            {
                bat.AppendLine($"{prefix}{cmd}");
                // If there are multiple commands wait a bit before launching the next one

                if (!string.IsNullOrWhiteSpace(prefix))
                    bat.AppendLine("timeout 2 > NUL");
            }

            File.WriteAllText(sav.FileName, bat.ToString());

            if (MessageBox.Show("Done, launch SysDVR-Client now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Launch(sender, e);
        }

        private void LLBL_ProjectWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start(GITHUB_PROJECT_URL_WIKI);

        private void tbTcpIP_Enter(object sender, EventArgs e)
        {
            if (TXT_TcpIp.Text == "IP address")
                TXT_TcpIp.Text = "";
        }

        private void tbTcpIP_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXT_TcpIp.Text))
                TXT_TcpIp.Text = "IP address";
        }

        private void cbAdvOpt_CheckedChanged(object sender, EventArgs e)
        {
            pAdvOptions.Visible = cbAdvOpt.Checked;
            this.Size = cbAdvOpt.Checked ? this.MaximumSize : this.MinimumSize;
        }

        private void BTN_DriverInstall_Click(object sender, EventArgs e)
        {
            new DriverInstallForm(false).ShowDialog();
        }

        private void BTN_Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Exit {typeof(Main).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title}?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void rbSrcTcp_CheckedChanged(object sender, EventArgs e)
        {
            this.TXT_TcpIp.Enabled = ((RadioButton)sender).Checked;
        }

        private void TXT_TcpIp_TextChanged(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(this.TXT_TcpIp.Text, out _))
            {
                this.ERR_IpAddress.SetError((TextBox)sender, "Invalid IP address");
                return;
            }

            this.ERR_IpAddress.Clear();
        }
    }
}
