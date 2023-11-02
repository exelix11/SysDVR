using SysDVRClientGUI.DriverInstall;
using System;
using System.Reflection;
using System.Windows.Forms;
using static SysDVRClientGUI.Resources.Resources;

namespace SysDVRClientGUI.Forms.DriverInstall
{
    public partial class DriverInstallForm : Form
    {
        readonly bool fromCommandLine;

        public DriverInstallForm(bool fromCommandLine)
        {
            this.InitializeComponent();
            this.fromCommandLine = fromCommandLine;
            this.BTN_Install.Text = INSTALL;
            this.Text = $"{DRIVER} {INSTALL}";
            this.LBL_Infotext.Text = $"{DRIVERINSTALL_FORM_INFOTEXT}\r\n\r\n";
        }

        private void DriverInstallResultForm_Load(object sender, EventArgs e)
        {
            var info = DriverHelper.GetDriverInfo();

            if (info == DriverStatus.Installed)
                LBL_Infotext.Text += DRIVERINSTALL_FORM_INSTALLED_ALREADY_INFO;
            else if (info == DriverStatus.NotInstalled)
                LBL_Infotext.Text += DRIVERINSTALL_FORM_NOT_INSTALLED;
            else
                LBL_Infotext.Text += DRIVERINSTALL_FORM_NOT_DETECTED;

            if (fromCommandLine && info == DriverStatus.Installed)
            {
                MessageBox.Show(DRIVERINSTALL_FORM_INSTALLED_ALREADY);
            }
        }

        void ShowError(string message)
        {
            BTN_Install.Text = ERROR;
            MessageBox.Show(message + DRIVERINSTALL_FORM_ERRORMESSAGE, ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void BTN_Install_Click(object sender, EventArgs e)
        {
            this.ControlBox = false;
            BTN_Install.Enabled = false;
            progressBar1.Visible = true;
            bool error = false;

            BTN_Install.Text = $"{DOWNLOADING}...";
            try
            {
                await DriverHelper.DownloadDriver();
            }
            catch (Exception ex)
            {
                this.ShowError($"{DRIVERINSTALL_EXTRACT_FAILED}: " + ex.Message);
                error = true;
            }

            if (!error)
            {
                BTN_Install.Text = $"{INSTALLING}...";
                try
                {
                    DriverHelper.InstallDriver();
                }
                catch (Exception ex)
                {
                    this.ShowError($"{DRIVERINSTALL_FAILED}: " + ex.Message);
                    error = true;
                }
            }

            if (!error)
            {
                try
                {
                    DriverHelper.DeleteTempDir();
                }
                catch
                {
                    // No need to catch this
                }
            }

            if (!error)
            {
                BTN_Install.Text = DONE;
            }

            MessageBox.Show($"{typeof(DriverInstallForm).Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title} {WILL_CLOSE_NOW}");
            Application.Exit();
        }
    }
}
