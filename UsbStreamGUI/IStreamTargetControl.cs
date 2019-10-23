using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsbStreamGUI
{
	[Flags]
	public enum StreamKind : uint 
	{
		Video = 1,
		Audio = 1 << 1,
		Both = Video | Audio
	}

	public enum StreamTarget 
	{
		Mpv,
		File,
		Tcp
	}

	interface IStreamTargetControl : IContainerControl
	{
		StreamKind TargetKind { get; set; }
		string GetCommandLine();
	}
}
