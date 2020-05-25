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
	public partial class FileStreamControl : UserControl, IStreamTargetControl
	{
		public FileStreamControl()
		{
			InitializeComponent();
		}

		public string GetExtraCmd() => "";

		public string GetCommandLine()
		{
			if (string.IsNullOrWhiteSpace(tbVideoFile.Text) || !Directory.Exists(tbVideoFile.Text))
				throw new Exception("Select a valid path to save the video data first");			

			return $"--file \"{tbVideoFile.Text}\"";
		}

		private void FileStreamControl_Load(object sender, EventArgs e)
		{

		}

		private void btnVideo_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dir = new FolderBrowserDialog();
			if (dir.ShowDialog() == DialogResult.OK)
			{
				if (File.Exists(Path.Combine(dir.SelectedPath, "video.h264")) || File.Exists(Path.Combine(dir.SelectedPath, "audio.raw")))
					if (MessageBox.Show("The folder you selected already contains a video.h264 or audio.raw file, these will be overwritten, do you want to continue ?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
						return;

				tbVideoFile.Text = dir.SelectedPath;
			}
		}
	}
}
