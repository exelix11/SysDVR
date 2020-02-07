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
	public partial class RTSPStreamOptControl : UserControl, IStreamTargetControl
	{
		public RTSPStreamOptControl()
		{
			InitializeComponent();
		}

		public StreamKind TargetKind { get; set; }

		public string GetCommandLine()
		{
			StringBuilder res = new StringBuilder();

			res.Append("rtsp");

			if (TargetKind == StreamKind.Video)
				res.Append(" --no-audio");
			else if (TargetKind == StreamKind.Audio)
				res.Append(" --no-video");

			return res.ToString();
		}

		public string GetExtraCmd()
		{
			var mpv = textBox1.Text;

			if (!string.IsNullOrEmpty(mpv) && !File.Exists(mpv))
				throw new Exception($"{mpv} does not exist");

			if (!string.IsNullOrEmpty(mpv))
				return $"{mpv} rtsp://127.0.0.1:6666/";
			return "";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog()
			{
				FileName = "mpv.com",
				Filter = "mpv cli executable (mpv.com)|mpv.com"
			};
			if (opn.ShowDialog() == DialogResult.OK)
				textBox1.Text = opn.FileName;
		}
	}
}
