using SysDVRClientGUI.Logic;
using SysDVRClientGUI.Models;
using System;
using System.IO;
using System.Windows.Forms;
using static SysDVRClientGUI.Logic.Constants;
using static SysDVRClientGUI.Resources.Resources;

namespace SysDVRClientGUI.ModesUI
{
    public partial class MpvStreamControl : UserControl, IStreamTargetControl
    {
        public MpvStreamControl()
        {
            this.InitializeComponent();
            this.ApplyLocalization();
            this.TXT_MpvPath.Text = RuntimeStorage.Config.Configuration.PlayMpvStreamControlOptions.MpvPath;
        }

        public StreamKind TargetKind { get; set; }

        public void ApplyLocalization()
        {
            this.LBL_Info.Text = MPVSTREAM_INFO;
            this.LBL_Path.Text = $"Mpv {PATH}:";
            this.LLBL_Download.Text = MPVSTREAM_DOWNLOAD;
        }

        public string GetClientCommandLine()
        {
            if (!File.Exists(TXT_MpvPath.Text))
                throw new Exception($"{TXT_MpvPath.Text} does not exist");

            return $"--mpv \"{TXT_MpvPath.Text}\"";
        }

        public LaunchCommand GetExtraCmd() => null;

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

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start(MPV_INSTALL_URL);

        private void TXT_MpvPath_TextChanged(object sender, EventArgs e)
        {
            RuntimeStorage.Config.Configuration.PlayMpvStreamControlOptions.MpvPath = ((TextBox)sender).Text;
        }
    }
}
