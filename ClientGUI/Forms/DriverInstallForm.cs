using SysDVRClientGUI.DriverInstall;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI.Forms.DriverInstall
{
    public partial class DriverInstallForm : Form
    {
        readonly bool fromCommandLine;

        public DriverInstallForm(bool fromCommandLine)
        {
            InitializeComponent();
            this.fromCommandLine = fromCommandLine;
        }

        private void DriverInstallResultForm_Load(object sender, EventArgs e)
        {
            var info = DriverHelper.GetDriverInfo();

            if (info == DriverStatus.Installed) 
                label1.Text += "It seems the driver is already installed, unless you face issues, you DON'T need to install it again.";
            else if (info == DriverStatus.NotInstalled)
                label1.Text += "It seems the driver is not installed, you need to install it to use SysDVR.";
            else
                label1.Text += "It seems Windows has never detected the SysDVR device ID, enable USB mode in SysDVR-Settings and connect your console. Note that USB-C to C cables may not work.";
        
            if (fromCommandLine && info == DriverStatus.Installed)
            {
                MessageBox.Show("The correct driver seems to be already installed, you don't need to install it again.");
            }
        }

        void ShowError(string message) 
        {
            button1.Text = "Error";
            MessageBox.Show(message + "\r\n\r\nRestart this application and try again, if it keeps on happening open an issue on GitHub.\r\nYou can also try following the manual driver installation guide on GitHub", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.ControlBox = false;
            button1.Enabled = false;
            progressBar1.Visible = true;
            bool error = false;

            button1.Text = "Downloading...";
            try 
            {
                await DriverHelper.DownloadDriver();
            }
            catch (Exception ex)
            {
                ShowError("Driver download and extraction failed: " + ex.Message);
                error = true;
            }

            if (!error)
            {
                button1.Text = "Installing...";
                try
                {
                    DriverHelper.InstallDriver();
                }
                catch (Exception ex)
                {
                    ShowError("Driver installation failed: " + ex.Message);
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
                button1.Text = "Done";
            }

            MessageBox.Show("SysDVR-Client GUI will now close");
            Environment.Exit(0);
        }
    }
}
