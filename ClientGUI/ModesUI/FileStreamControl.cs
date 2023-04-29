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
	public partial class FileStreamControl : UserControl, IStreamTargetControl
	{
		public FileStreamControl()
		{
			InitializeComponent();
		}

		public LaunchCommand GetExtraCmd() => null;

		public string GetClientCommandLine()
		{
			if (string.IsNullOrWhiteSpace(tbVideoFile.Text))
				throw new Exception("Select a valid path to save the video data first");			

			return $"--file \"{tbVideoFile.Text}\"";
		}

		private void FileStreamControl_Load(object sender, EventArgs e)
		{

		}

		private void btnVideo_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "mp4 file|*.mp4" };
			if (sav.ShowDialog() == DialogResult.OK)
			{
				tbVideoFile.Text = sav.FileName;
			}
		}
	}
}
