using SysDVRClientGUI.Logic;
using SysDVRClientGUI.Models;
using System;
using System.IO;
using System.Windows.Forms;
using static SysDVRClientGUI.Logic.Constants;

namespace SysDVRClientGUI.ModesUI
{
    public partial class RTSPStreamOptControl : UserControl, IStreamTargetControl
    {
        public StreamKind TargetKind { get; set; }

        public RTSPStreamOptControl()
        {
            this.InitializeComponent();

            this.TXT_MpvPath.Text = RuntimeStorage.Config.Configuration.RtspStreamControlOptions.MpvPath;
            this.cbMpvLowLat.Checked = RuntimeStorage.Config.Configuration.RtspStreamControlOptions.LowLatency;
            this.cbMpvUntimed.Checked = RuntimeStorage.Config.Configuration.RtspStreamControlOptions.Untimed;
        }

        public string GetClientCommandLine() => "--rtsp";

        public LaunchCommand GetExtraCmd()
        {
            var mpv = TXT_MpvPath.Text;

            if (!string.IsNullOrEmpty(mpv) && !File.Exists(mpv))
                throw new Exception($"{mpv} does not exist");

            var args = "rtsp://127.0.0.1:6666/";

            if (cbMpvLowLat.Checked)
                args += " --profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --cache-pause=no";

            if (cbMpvUntimed.Checked)
                args += " --untimed";

            return new LaunchCommand
            {
                Executable = mpv,
                Arguments = args
            };
        }

        private void BTN_Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog opn = new()
            {
                FileName = "mpv.com",
                Filter = "mpv cli executable (mpv.com)|mpv.com"
            };

            if (opn.ShowDialog() == DialogResult.OK)
                TXT_MpvPath.Text = opn.FileName;
        }

        private void LLBL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start(MPV_INSTALL_URL);

        private void TXT_MpvPath_TextChanged(object sender, EventArgs e)
        {
            RuntimeStorage.Config.Configuration.RtspStreamControlOptions.MpvPath = ((TextBox)sender).Text;
        }

        private void CHK_MpvLowLat_CheckedChanged(object sender, EventArgs e)
        {
            RuntimeStorage.Config.Configuration.RtspStreamControlOptions.LowLatency = ((CheckBox)sender).Checked;
        }

        private void CHK_MpvUntimed_CheckedChanged(object sender, EventArgs e)
        {
            RuntimeStorage.Config.Configuration.RtspStreamControlOptions.Untimed = ((CheckBox)sender).Checked;
        }
    }
}
