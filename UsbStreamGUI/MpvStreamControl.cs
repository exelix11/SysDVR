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

namespace UsbStreamGUI
{
	public partial class MpvStreamControl : UserControl, IStreamTargetControl
	{
		public MpvStreamControl()
		{
			InitializeComponent();
		}

		public StreamKind TargetKind { get; set; }

		public string GetCommandLine()
		{
			if (!File.Exists(textBox1.Text))
				throw new Exception($"{textBox1.Text} does not exist");

			StringBuilder res = new StringBuilder();

			string mpv = textBox1.Text;
			if (mpv.EndsWith(".com"))
				mpv = mpv.Substring(0, mpv.Length - 4);

			if ((TargetKind & StreamKind.Video) != 0)
			{
				res.AppendFormat("video mpv \"{0}\"", mpv);
			}

			if ((TargetKind & StreamKind.Audio) != 0)
			{
				if (res.Length != 0) res.Append(" ");
				res.AppendFormat("audio mpv \"{0}\"", mpv);
			}

			return res.ToString();
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

		private void label2_Click(object sender, EventArgs e)
		{

		}
	}
}
