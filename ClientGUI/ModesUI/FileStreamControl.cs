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

		StreamKind _Target;
		public StreamKind TargetKind 
		{ 
			get => _Target;
			set {
				_Target = value;
				tbVideoFile.Enabled = btnVideo.Enabled = (_Target & StreamKind.Video) != 0;
				tbAudioFile.Enabled = btnAudio.Enabled = (_Target & StreamKind.Audio) != 0;
			}
		}

		public string GetExtraCmd() => "";
		public string GetCommandLine()
		{
			StringBuilder res = new StringBuilder();

			void CheckIfValid(string fname, string streamName)
			{
				if (string.IsNullOrWhiteSpace(fname)) throw new Exception($"Select a valid path for the {streamName} stream");
				if (File.Exists(fname)) throw new Exception($"The {fname} file already exists");
			}

			if ((_Target & StreamKind.Video) != 0)
			{
				CheckIfValid(tbVideoFile.Text, "video");
				res.AppendFormat("video file \"{0}\"", tbVideoFile.Text);
			}

			if ((_Target & StreamKind.Audio) != 0)
			{
				CheckIfValid(tbAudioFile.Text, "audio");
				if (res.Length != 0) res.Append(" ");
				res.AppendFormat("audio file \"{0}\"", tbAudioFile.Text);
			}

			return res.ToString();
		}

		private void FileStreamControl_Load(object sender, EventArgs e)
		{

		}

		private void btnVideo_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog()
			{
				Filter = "h264 file|*.264"
			};
			if (sav.ShowDialog() == DialogResult.OK)
				tbVideoFile.Text = sav.FileName;
		}

		private void btnAudio_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog()
			{
				Filter = "raw audio file|*.raw"
			};
			if (sav.ShowDialog() == DialogResult.OK)
				tbAudioFile.Text = sav.FileName;
		}
	}
}
