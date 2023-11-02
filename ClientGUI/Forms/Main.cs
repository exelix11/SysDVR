using Serilog;
using SysDVRClientGUI.DriverInstall;
using SysDVRClientGUI.Forms.DriverInstall;
using SysDVRClientGUI.Logic;
using SysDVRClientGUI.Models;
using SysDVRClientGUI.ModesUI;
using SysDVRClientGUI.ViewLogic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static SysDVRClientGUI.Logic.Constants;
using static SysDVRClientGUI.Logic.HelperFunctions;
using static SysDVRClientGUI.Resources.Resources;

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

        private bool firstRun = true;
        private readonly Dictionary<string, string> availableLanguages = new()
        {
            { "en-EN","English" },
            { "en-US","English" },
            { "de-DE","German" }
        };

        private readonly PictureboxSlideAnimation slideAnimation;
        private readonly PictureboxBounceAnimation bounceAnimation;

        public Main()
        {
            this.InitializeComponent();
            this.IPA_AddressBox.IpAddressChanged += (o, e) => Log.Debug($"Selected ipv4 address is now: \"{e.IPAddressValue}\"");

            this.Size = cbAdvOpt.Checked ? this.MaximumSize : this.MinimumSize;
            this.Text = $"{typeof(Main).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title} {GetVersionString()}";
            this.LoadUserSettings();

            this.CMB_Languages.DataSource = this.availableLanguages.DistinctBy(x => x.Value).ToArray();
            this.CMB_Languages.DisplayMember = "Value";
            if (string.IsNullOrEmpty(RuntimeStorage.Config.Configuration.UserLanguage))
            {
                this.CMB_Languages.SelectedItem = this.availableLanguages.FirstOrDefault(x => x.Key == Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
                Log.Verbose($"Language initialized with \"{Thread.CurrentThread.CurrentUICulture.IetfLanguageTag}\"");
            }
            else
            {
                this.CMB_Languages.SelectedItem = this.availableLanguages.FirstOrDefault(x => x.Key == RuntimeStorage.Config.Configuration.UserLanguage);
                Log.Verbose($"Language initialized with \"{RuntimeStorage.Config.Configuration.UserLanguage}\"");
            }

            this.ApplyLocalization();
            this.StreamTargetSelected(this.rbPlay, EventArgs.Empty);
            this.StreamKindSelected(this.rbChannelsBoth, EventArgs.Empty);

            if (Program.ApplicationIcon != null)
                this.Icon = Program.ApplicationIcon;

            if (File.Exists(SYSDVR_DLL))
                ClientDllPath = SYSDVR_DLL;

#if DEBUG
            // When in debug mode also search sysdvr's visual studio build folder
            else if (Directory.Exists(@$"..\..\..\..\Client\bin\Debug\net7.0") && Directory.GetFiles(@$"..\..\..\..\Client\bin\Debug\net7.0").Length > 0)
            {
                string currentPath = Path.GetDirectoryName(typeof(Main).Assembly.Location);
                foreach (string f in Directory.GetFiles(@$"..\..\..\..\Client\bin\Debug\net7.0"))
                {
                    File.Copy(f, Path.GetFileName(f), true);
                }
                foreach (string d in Directory.GetDirectories(@$"..\..\..\..\Client\bin\Debug\net7.0"))
                {
                    Directory.CreateDirectory(Path.GetFileName(d));
                    CopyFilesRecursively(d, Path.Combine(currentPath, Path.GetFileName(d)));
                }
                ClientDllPath = SYSDVR_DLL;
            }
#endif
            DotnetMajorVersion = FindDotnet(out DotnetPath, out DotnetIs32Bit);

            this.slideAnimation = new(this.PBX_Logo);
            this.bounceAnimation = new(this.PBX_Logo);
        }

        private void ApplyLocalization()
        {
            this.BTN_Exit.Text = EXIT;
            this.BTN_Launch.Text = LAUNCH;
            this.LBL_Infotext.Text = MAIN_INFOTEXT;
            this.LLBL_ProjectWiki.Text = MAIN_LINKLABEL_TEXT;
            this.rbChannelsBoth.Text = BOTH;
            this.rbChannelsAudio.Text = AUDIO;
            this.rbChannelsVideo.Text = VIDEO;
            this.cbAdvOpt.Text = MAIN_SHOW_ADVANCED_OPTIONS;
            this.BTN_CreateBatch.Text = MAIN_CREATE_QUICKLAUNCH;
            this.BTN_DriverInstall.Text = MAIN_REINSTALL_USB;
            this.GRP_StreamingChannels.Text = MAIN_GRP_STREAMCHANNELS;
            this.GRP_StreamingSource.Text = MAIN_GRP_STREAMINGSOURCE;
            this.GRP_StreamMode.Text = MAIN_GRP_STREAMINGMODE;
            this.GRP_AdvOptions.Text = MAIN_GRP_ADVANCEDOPTIONS;
            this.LBL_StreamingSourceInfo.Text = MAIN_GRP_STREAMINGSOURCE_INFO;
            this.rbSrcUsb.Text = MAIN_STREAMING_USB_INFO;
            this.rbSrcTcp.Text = MAIN_STREAMING_TCP_INFO;
            this.cbUsbWarn.Text = MAIN_OPTIONS_PRINT_LIBUSB_WARNINGS;
            this.cbUsbLog.Text = MAIN_OPTIONS_PRINT_LIBUSB_DEBUGINFO;
            this.cbStats.Text = MAIN_OPTIONS_LOG_STATUS;
            this.cbIgnoreSync.Text = MAIN_OPTIONS_IGNORE_AUDIOVIDEO_SYNC;
            this.cbLogStatus.Text = MAIN_OPTIONS_LOG_TRANSFER;
            this.rbPlay.Text = MAIN_STREAMOPTIONS_PLAY;
            this.rbPlayMpv.Text = MAIN_STREAMOPTIONS_PLAY_MVP;
            this.rbSaveToFile.Text = MAIN_STREAMOPTIONS_FILE;
            this.rbStreamRtsp.Text = MAIN_STREAMOPTIONS_PLAY_RTSP;
        }

        private void LoadUserSettings()
        {
            this.cbAdvOpt.Checked = RuntimeStorage.Config.Configuration.ShowAdvancedOptions;

            switch (RuntimeStorage.Config.Configuration.ChannelsToStream)
            {
                case StreamKind.Video:
                    this.rbChannelsVideo.Checked = true;
                    break;
                case StreamKind.Audio:
                    this.rbChannelsAudio.Checked = true;
                    break;
                case StreamKind.Both:
                    this.rbChannelsBoth.Checked = true;
                    break;
            }

            switch (RuntimeStorage.Config.Configuration.StreamSource)
            {
                case StreamSource.Usb:
                    this.rbSrcUsb.Checked = true;
                    break;
                case StreamSource.Tcp:
                    this.rbSrcTcp.Checked = true;
                    break;
            }

            if (RuntimeStorage.Config.Configuration.IpAddress != default)
            {
                IPA_AddressBox.SetIpAddress(RuntimeStorage.Config.Configuration.IpAddress);
            }

            switch (RuntimeStorage.Config.Configuration.StreamMode)
            {
                case StreamMode.Play:
                    this.rbPlay.Checked = true;
                    break;
                case StreamMode.PlayMpv:
                    this.rbPlayMpv.Checked = true;
                    break;
                case StreamMode.Rtsp:
                    this.rbStreamRtsp.Checked = true;
                    break;
                case StreamMode.SaveToFile:
                    this.rbSaveToFile.Checked = true;
                    break;
            }

            cbStats.Checked = RuntimeStorage.Config.Configuration.AdvancedOptions.LogTransferInfo;
            cbIgnoreSync.Checked = RuntimeStorage.Config.Configuration.AdvancedOptions.IngoreAudioVideoSync;
            cbLogStatus.Checked = RuntimeStorage.Config.Configuration.AdvancedOptions.LogStatusMsgs;
            cbUsbWarn.Checked = RuntimeStorage.Config.Configuration.AdvancedOptions.PrintLibUsbWarnings;
            cbUsbLog.Checked = RuntimeStorage.Config.Configuration.AdvancedOptions.PrintLibUsbDebug;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClientDllPath))
            {
                MessageBox.Show($"{SYSDVR_DLL} {MAIN_DLL_NOT_FOUND}");
                this.Close();
            }

            if (DotnetMajorVersion == 0)
            {
                if (MessageBox.Show(MAIN_NET_NOT_FOUND, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start(DOTNET_DOWNLOAD_URL);
                this.Close();
            }
            else if (Environment.Is64BitOperatingSystem && DotnetIs32Bit && MessageBox.Show(MAIN_NET_WRONG_BIT, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(DOTNET_64BIT_DOWNLOAD_URL);
                this.Close();
            }

            if (DotnetMajorVersion < REQUIRED_DOTNET_MAJOR)
            {
                if (MessageBox.Show(MAIN_NET_OLD, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start(DOTNET_DOWNLOAD_URL);
                else
                    MessageBox.Show(MAIN_NET_WARNING);
            }

            this.slideAnimation.Start();
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
        public static bool CheckUSBDriver()
        {
            var state = DriverHelper.GetDriverInfo();
            if (state == DriverStatus.NotInstalled)
            {
                if (MessageBox.Show(MAIN_DRIVER_NOT_FOUND, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    new DriverInstallForm(false).ShowDialog();
                    return true;
                }
                else MessageBox.Show(MAIN_DRIVER_NOT_FOUND_WARNING);
            }
            else if (state == DriverStatus.Unknown)
            {
                DialogResult res = MessageBox.Show(MAIN_DRIVER_WARNING, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (res == DialogResult.Yes)
                    return CheckUSBDriver();
                else if (res == DialogResult.Cancel)
                    return true;
            }
            return false;
        }

        private string GetExtraArgs()
        {
            StringBuilder str = new();

            void Append(string s) { str.Append(' '); str.Append(s); }

            if (cbStats.Checked || cbIgnoreSync.Checked || cbLogStatus.Checked)
            {
                List<string> opt = new()
                {
                    "log"
                };

                if (cbStats.Checked)
                    opt.Add("stats");

                if (cbIgnoreSync.Checked)
                    opt.Add("nosync");

                Append("--debug " + string.Join(",", opt));
            }
            if (cbUsbLog.Checked) Append("--usb-debug");
            if (cbUsbWarn.Checked) Append("--usb-warn");
            return str.ToString();
        }

        private LaunchCommand GetClientCommandLine()
        {
            StringBuilder args = new();

            args.Append($"\"{Path.GetFullPath(ClientDllPath)}\" ");

            if (rbSrcUsb.Checked)
                args.Append("usb ");
            else if (rbSrcTcp.Checked)
                args.AppendFormat("bridge {0} ", IPA_AddressBox.IPAddressValue.ToString());

            if (CurKind == StreamKind.Audio)
                args.Append("--no-video ");
            else if (CurKind == StreamKind.Video)
                args.Append("--no-audio ");

            args.Append(CurrentControl.GetClientCommandLine());
            if (args[^1] != ' ')
                args.Append(' ');

            args.Append(this.GetExtraArgs());

            return new LaunchCommand
            {
                Executable = DotnetPath,
                Arguments = args.ToString().Trim(),
                FileDependencies = new string[] { Path.GetFullPath(ClientDllPath) }
            };
        }

        private IEnumerable<LaunchCommand> GetFinalCommand()
        {
            if (rbSrcUsb.Checked && CheckUSBDriver())
                return Array.Empty<LaunchCommand>();

            return new LaunchCommand[]
            {
                this.GetClientCommandLine(),
                CurrentControl.GetExtraCmd()
            }
            .Where(x => x != null);
        }

        private void Launch(object sender, EventArgs e)
        {
            IEnumerable<LaunchCommand> cmds = this.GetFinalCommand();
            if (cmds == null || !cmds.Any())
                return;

            // Launch SysDVR-Client with /K so any error is shown to the user
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = "cmd",
                // Cursed quote escaping https://superuser.com/questions/1213094/how-to-escape-in-cmd-exe-c-parameters
                Arguments = $"/K \"{cmds.First()}\""
            });

            if (cmds.Count() > 1)
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
            IEnumerable<LaunchCommand> cmds = this.GetFinalCommand();
            if (cmds == null || !cmds.Any())
                return;

            SaveFileDialog sav = new() { Filter = "batch file|*.bat", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory, RestoreDirectory = false, FileName = "SysDVR Launcher.bat" };
            if (sav.ShowDialog() != DialogResult.OK)
                return;

            CreateBatch b = new(this.ClientDllPath, cmds);
            b.Create();
            b.SaveToFile(sav.FileName);

            if (MessageBox.Show("Done, launch SysDVR-Client now ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.Launch(sender, e);
        }

        private void LLBL_ProjectWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start(GITHUB_PROJECT_URL_WIKI);

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
            if (MessageBox.Show($"{EXIT} {typeof(Main).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title}?", MAIN_EXIT_APPLICATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void WriteUserSettings()
        {
            RuntimeStorage.Config.Configuration.ShowAdvancedOptions = this.cbAdvOpt.Checked;

            if (this.rbChannelsBoth.Checked)
            {
                RuntimeStorage.Config.Configuration.ChannelsToStream = StreamKind.Both;
            }
            if (this.rbChannelsAudio.Checked)
            {
                RuntimeStorage.Config.Configuration.ChannelsToStream = StreamKind.Audio;
            }
            if (this.rbChannelsVideo.Checked)
            {
                RuntimeStorage.Config.Configuration.ChannelsToStream = StreamKind.Video;
            }

            RuntimeStorage.Config.Configuration.StreamSource = this.rbSrcUsb.Checked ? StreamSource.Usb : StreamSource.Tcp;

            RuntimeStorage.Config.Configuration.IpAddress = IPA_AddressBox.IPAddressValue;

            if (this.rbPlay.Checked)
            {
                RuntimeStorage.Config.Configuration.StreamMode = StreamMode.Play;
            }
            if (this.rbPlayMpv.Checked)
            {
                RuntimeStorage.Config.Configuration.StreamMode = StreamMode.PlayMpv;
            }
            if (this.rbStreamRtsp.Checked)
            {
                RuntimeStorage.Config.Configuration.StreamMode = StreamMode.Rtsp;
            }
            if (this.rbSaveToFile.Checked)
            {
                RuntimeStorage.Config.Configuration.StreamMode = StreamMode.SaveToFile;
            }

            RuntimeStorage.Config.Configuration.AdvancedOptions.LogTransferInfo = cbStats.Checked;
            RuntimeStorage.Config.Configuration.AdvancedOptions.IngoreAudioVideoSync = cbIgnoreSync.Checked;
            RuntimeStorage.Config.Configuration.AdvancedOptions.LogStatusMsgs = cbLogStatus.Checked;
            RuntimeStorage.Config.Configuration.AdvancedOptions.PrintLibUsbWarnings = cbUsbWarn.Checked;
            RuntimeStorage.Config.Configuration.AdvancedOptions.PrintLibUsbDebug = cbUsbLog.Checked;

            RuntimeStorage.Config.Save();

            Log.Verbose($"User-Configuration saved");
        }

        private void rbSrcTcp_CheckedChanged(object sender, EventArgs e)
        {
            IPA_AddressBox.Enabled = ((RadioButton)sender).Checked;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WriteUserSettings();
            Log.Information($"Closing application");
            Log.CloseAndFlush();
        }

        private void CMB_Languages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstRun)
            {
                this.firstRun = false;
                return;
            }

            string langIdentifier = this.availableLanguages.FirstOrDefault(x => x.Key == ((KeyValuePair<string, string>)this.CMB_Languages.SelectedItem).Key).Key;

            RuntimeStorage.Config.Configuration.UserLanguage = langIdentifier;
            RuntimeStorage.Config.Save();

            Log.Information($"Language switched to \"{langIdentifier}\"");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(langIdentifier);
            this.ApplyLocalization();
            if (this.StreamConfigPanel.Controls.Count >= 1 && this.StreamConfigPanel.Controls[0] is IStreamTargetControl streamControl)
            {
                streamControl.ApplyLocalization();
            }
        }

        private void PBX_Logo_Click(object sender, EventArgs e)
        {
            this.bounceAnimation.Start();
        }
    }
}
