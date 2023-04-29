using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI.ModesUI
{
	public partial class PlayStreamControl : UserControl, IStreamTargetControl
	{
		public PlayStreamControl()
		{
			InitializeComponent();
		}

		public string GetClientCommandLine()
		{
			StringBuilder sb = new StringBuilder();

			if (cbHwAcc.Checked)
				sb.Append("--hw-acc ");

			if (cbBestScaling.Checked)
				sb.Append("--scale best");

			return sb.ToString().Trim();
		}

		public LaunchCommand GetExtraCmd() => null;
	}
}
