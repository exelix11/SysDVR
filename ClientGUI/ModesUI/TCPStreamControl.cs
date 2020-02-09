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
	public partial class TCPStreamControl : UserControl, IStreamTargetControl
	{
		public TCPStreamControl()
		{
			InitializeComponent();
		}

		StreamKind _Target;
		public StreamKind TargetKind
		{
			get => _Target;
			set
			{
				_Target = value;
				tbVideoPort.Enabled = (_Target & StreamKind.Video) != 0;
				tbAudioPort.Enabled = (_Target & StreamKind.Audio) != 0;
			}
		}

		public string GetCommandLine()
		{
			StringBuilder res = new StringBuilder();

			var _vPort = short.TryParse(tbVideoPort.Text, out short vPort);
			var _aPort = short.TryParse(tbAudioPort.Text, out short aPort);

			if ((_Target & StreamKind.Video) != 0)
			{
				if (!_vPort) throw new Exception("Enter a valid port number for video stream");
				res.AppendFormat("video tcp {0}", vPort);
			}

			if ((_Target & StreamKind.Audio) != 0)
			{
				if (!_aPort) throw new Exception("Enter a valid port number for audio stream");
				if (_vPort && aPort == vPort) throw new Exception("The video and audio ports must be different");
				if (res.Length != 0) res.Append(" ");
				res.AppendFormat("audio tcp {0}", aPort);
			}

			return res.ToString();
		}

		public string GetExtraCmd() => "";

		private void TCPStreamControl_Load(object sender, EventArgs e)
		{

		}
	}
}
