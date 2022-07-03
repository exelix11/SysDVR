using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI.DriverInstall
{
    public partial class DriverInstallResultForm : Form
    {
        public DriverInstallResultForm(string log)
        {
            InitializeComponent();
            tbLog.Text = log;
        }

        private void DriverInstallResultForm_Load(object sender, EventArgs e)
        {

        }
    }
}
