using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI
{
	public partial class PlayStreamControl : UserControl, IStreamTargetControl
	{
		public PlayStreamControl()
		{
			InitializeComponent();
		}

		public string GetCommandLine()
		{
			StringBuilder sb = new StringBuilder();

			if (cbHwAcc.Checked)
				sb.Append("--hw-acc ");

			if (cbBestScaling.Checked)
				sb.Append("--scale best");

			return sb.ToString().Trim();
		}

		public string GetExtraCmd()
		{
			return "";
		}
	}
}
