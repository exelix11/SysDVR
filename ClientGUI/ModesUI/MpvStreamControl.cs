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
	public partial class MpvStreamControl : UserControl, IStreamTargetControl
	{
		public MpvStreamControl()
		{
			InitializeComponent();
		}

		public StreamKind TargetKind { get; set; }

		public string GetClientCommandLine()
		{
			if (!File.Exists(textBox1.Text))
				throw new Exception($"{textBox1.Text} does not exist");

			return $"--mpv \"{textBox1.Text}\"";
		}

		public LaunchCommand GetExtraCmd() => null;

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

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
			System.Diagnostics.Process.Start("https://mpv.io/installation/");

		private void MpvStreamControl_Load(object sender, EventArgs e)
		{

		}
	}
}
