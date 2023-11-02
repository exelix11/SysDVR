using SysDVRClientGUI.Logic;
using SysDVRClientGUI.Models;
using System;
using System.Windows.Forms;
using static SysDVRClientGUI.Resources.Resources;

namespace SysDVRClientGUI.ModesUI
{
    public partial class FileStreamControl : UserControl, IStreamTargetControl
    {
        public FileStreamControl()
        {
            this.InitializeComponent();
            this.ApplyLocalization();
            this.tbVideoFile.Text = RuntimeStorage.Config.Configuration.FileStreamControlOptions.LastUsedPath;
        }

        public void ApplyLocalization()
        {
            this.LBL_Info.Text = FILESTREAM_CONTROL_INFO;
            this.LBL_Path.Text = $"{PATH}:";
        }

        public LaunchCommand GetExtraCmd() => null;

        public string GetClientCommandLine()
        {
            if (string.IsNullOrWhiteSpace(tbVideoFile.Text))
                throw new Exception("Select a valid path to save the video data first");

            return $"--file \"{tbVideoFile.Text}\"";
        }

        private void BTN_Video_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new() { Filter = "mp4 file|*.mp4" };
            if (sav.ShowDialog() == DialogResult.OK)
                tbVideoFile.Text = sav.FileName;
        }

        private void tbVideoFile_TextChanged(object sender, EventArgs e)
        {
            RuntimeStorage.Config.Configuration.FileStreamControlOptions.LastUsedPath = ((TextBox)sender).Text;
        }
    }
}
