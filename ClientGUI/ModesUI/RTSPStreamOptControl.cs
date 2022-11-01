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

namespace SysDVRClientGUI.ModesUI
{
	public partial class RTSPStreamOptControl : UserControl, IStreamTargetControl
	{
		public RTSPStreamOptControl()
		{
			InitializeComponent();
		}

		public StreamKind TargetKind { get; set; }

		public string GetCommandLine() => "--rtsp";

		public string GetExtraCmd()
		{
			var mpv = textBox1.Text;

			if (!string.IsNullOrEmpty(mpv) && !File.Exists(mpv))
				throw new Exception($"{mpv} does not exist");

			if (!string.IsNullOrEmpty(mpv))
				mpv = $"\"{mpv}\" rtsp://127.0.0.1:6666/";
			else return "";

			if (cbMpvLowLat.Checked)
				mpv += " --profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --cache-pause=no";
			
			if (cbMpvUntimed.Checked) 
				mpv += " --untimed";

			return mpv;
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

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
			System.Diagnostics.Process.Start("https://mpv.io/installation/");
	}
}
