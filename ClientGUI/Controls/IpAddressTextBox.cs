using System;
using System.Net;
using System.Windows.Forms;

namespace SysDVRClientGUI.Controls
{
    public partial class IpAddressTextBox : UserControl
    {
        public IPAddress IPAddressValue { get; private set; }
        public bool IsValid { get; private set; }

        public event EventHandler<IpAddressChangedEventArgs> IpAddressChanged;

        public IpAddressTextBox()
        {
            this.InitializeComponent();
        }

        private void KeyPressValidation(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;

            if ((Char.IsControl(e.KeyChar)) || (char.IsDigit(e.KeyChar) && short.TryParse($"{t.Text}{e.KeyChar}", out short res) && res <= 255 && res >= 0))
            {
                e.Handled = false;

                if (!Char.IsControl(e.KeyChar) && $"{t.Text}{e.KeyChar}".Length >= 3)
                {
                    this.SelectNextControl((Control)sender, true, true, true, true);
                }

                return;
            }

            e.Handled = true;
        }

        private void IpAddress_TextChanged(object sender, EventArgs e)
        {
            if (IPAddress.TryParse($"{this.TXT_Octet1.Text}.{this.TXT_Octet2.Text}.{this.TXT_Octet3.Text}.{this.TXT_Octet4.Text}", out IPAddress ipa))
            {
                this.IPAddressValue = ipa;
                this.IsValid = true;
                this.IpAddressChanged?.Invoke(this, new(ipa));
                return;
            }

            this.IsValid = false;
            this.IpAddressChanged?.Invoke(this, new(null));
            this.IPAddressValue = null;
        }

        public void SetIpAddress(IPAddress iPAddress)
        {
            if (iPAddress == null)
            {
                return;
            }

            this.TXT_Octet1.Text = iPAddress.GetAddressBytes()[0].ToString();
            this.TXT_Octet2.Text = iPAddress.GetAddressBytes()[1].ToString();
            this.TXT_Octet3.Text = iPAddress.GetAddressBytes()[2].ToString();
            this.TXT_Octet4.Text = iPAddress.GetAddressBytes()[3].ToString();
        }
    }

    public sealed class IpAddressChangedEventArgs : EventArgs
    {
        public IPAddress IPAddressValue { get; init; }
        public IpAddressChangedEventArgs(IPAddress iPAddressValue)
        {
            this.IPAddressValue = iPAddressValue;
        }
    }
}
