using System.Windows.Forms;

namespace SysDVRClientGUI
{
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
