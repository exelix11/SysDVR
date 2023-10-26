using SysDVRClientGUI.Logic;
using SysDVRClientGUI.Models;
using System.Text;
using System.Windows.Forms;

namespace SysDVRClientGUI.ModesUI
{
    public partial class PlayStreamControl : UserControl, IStreamTargetControl
    {
        public PlayStreamControl()
        {
            this.InitializeComponent();
            this.CHK_BestScaling.Checked = RuntimeStorage.Config.Configuration.PlayStreamControlOptions.BestScaling;
            this.CHK_HwAcc.Checked = RuntimeStorage.Config.Configuration.PlayStreamControlOptions.HardwareAcceleration;
        }

        public string GetClientCommandLine()
        {
            StringBuilder sb = new();

            if (CHK_HwAcc.Checked)
                sb.Append("--hw-acc ");

            if (CHK_BestScaling.Checked)
                sb.Append("--scale best");

            return sb.ToString().Trim();
        }

        public LaunchCommand GetExtraCmd() => null;

        private void CHK_BestScaling_CheckedChanged(object sender, System.EventArgs e)
        {
            RuntimeStorage.Config.Configuration.PlayStreamControlOptions.BestScaling = ((CheckBox)sender).Checked;
        }

        private void CHK_HwAcc_CheckedChanged(object sender, System.EventArgs e)
        {
            RuntimeStorage.Config.Configuration.PlayStreamControlOptions.HardwareAcceleration = ((CheckBox)sender).Checked;
        }
    }
}
