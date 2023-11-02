using SysDVRClientGUI.Models;
using System.Windows.Forms;

namespace SysDVRClientGUI
{
    interface IStreamTargetControl : IContainerControl
    {
        string GetClientCommandLine();
        LaunchCommand GetExtraCmd();
        public void ApplyLocalization();
    }
}
