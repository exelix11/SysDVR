using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI
{
	[Flags]
	public enum StreamKind : uint 
	{
		Video = 1,
		Audio = 1 << 1,
		Both = Video | Audio
	}

    public class LaunchCommand
    {
        public string Executable { get; set; }
        public string Arguments { get; set; }

        public string[] FileDependencies { get; set; }

        public override string ToString()
        {
            return $"\"{Executable}\" {Arguments}";
        }
    }

    interface IStreamTargetControl : IContainerControl
	{
		string GetClientCommandLine();
        LaunchCommand GetExtraCmd();
	}
}
