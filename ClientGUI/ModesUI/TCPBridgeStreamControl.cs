using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SysDVRClientGUI
{
	public partial class TCPBridgeStreamControl : UserControl, IStreamTargetControl
	{
		public TCPBridgeStreamControl()
		{
			InitializeComponent();
		}

		public StreamKind TargetKind { get; set; }

		public string GetCommandLine()
		{
			StringBuilder res = new StringBuilder();

			res.Append("bridge " + textBox1.Text);

			if (TargetKind == StreamKind.Video)
				res.Append(" --no-audio");
			else if (TargetKind == StreamKind.Audio)
				res.Append(" --no-video");

			return res.ToString();
		}

		public string GetExtraCmd()
		{
			return rtspStreamOptControl1.GetExtraCmd();
		}

		private void TCPBridgeStreamControl_Load(object sender, EventArgs e)
		{

		}
	}
}
