namespace SysDVRClientGUI.Controls
{
    partial class IpAddressTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LBL_Dot0 = new System.Windows.Forms.Label();
            this.TXT_Octet1 = new System.Windows.Forms.TextBox();
            this.TXT_Octet2 = new System.Windows.Forms.TextBox();
            this.LBL_Dot1 = new System.Windows.Forms.Label();
            this.TXT_Octet3 = new System.Windows.Forms.TextBox();
            this.LBL_Dot2 = new System.Windows.Forms.Label();
            this.TXT_Octet4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LBL_Dot0
            // 
            this.LBL_Dot0.AutoSize = true;
            this.LBL_Dot0.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LBL_Dot0.Location = new System.Drawing.Point(29, 9);
            this.LBL_Dot0.Name = "LBL_Dot0";
            this.LBL_Dot0.Size = new System.Drawing.Size(10, 15);
            this.LBL_Dot0.TabIndex = 0;
            this.LBL_Dot0.Text = ".";
            // 
            // TXT_Octett1
            // 
            this.TXT_Octet1.Location = new System.Drawing.Point(3, 3);
            this.TXT_Octet1.MaxLength = 3;
            this.TXT_Octet1.Name = "TXT_Octett1";
            this.TXT_Octet1.PlaceholderText = "192";
            this.TXT_Octet1.Size = new System.Drawing.Size(24, 23);
            this.TXT_Octet1.TabIndex = 1;
            this.TXT_Octet1.TextChanged += this.IpAddress_TextChanged;
            this.TXT_Octet1.KeyPress += this.KeyPressValidation;
            // 
            // TXT_Octett2
            // 
            this.TXT_Octet2.Location = new System.Drawing.Point(40, 3);
            this.TXT_Octet2.MaxLength = 3;
            this.TXT_Octet2.Name = "TXT_Octett2";
            this.TXT_Octet2.PlaceholderText = "168";
            this.TXT_Octet2.Size = new System.Drawing.Size(24, 23);
            this.TXT_Octet2.TabIndex = 2;
            this.TXT_Octet2.TextChanged += this.IpAddress_TextChanged;
            this.TXT_Octet2.KeyPress += this.KeyPressValidation;
            // 
            // LBL_Dot1
            // 
            this.LBL_Dot1.AutoSize = true;
            this.LBL_Dot1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LBL_Dot1.Location = new System.Drawing.Point(66, 9);
            this.LBL_Dot1.Name = "LBL_Dot1";
            this.LBL_Dot1.Size = new System.Drawing.Size(10, 15);
            this.LBL_Dot1.TabIndex = 2;
            this.LBL_Dot1.Text = ".";
            // 
            // TXT_Octett3
            // 
            this.TXT_Octet3.Location = new System.Drawing.Point(78, 3);
            this.TXT_Octet3.MaxLength = 3;
            this.TXT_Octet3.Name = "TXT_Octett3";
            this.TXT_Octet3.PlaceholderText = "0";
            this.TXT_Octet3.Size = new System.Drawing.Size(24, 23);
            this.TXT_Octet3.TabIndex = 3;
            this.TXT_Octet3.TextChanged += this.IpAddress_TextChanged;
            this.TXT_Octet3.KeyPress += this.KeyPressValidation;
            // 
            // LBL_Dot2
            // 
            this.LBL_Dot2.AutoSize = true;
            this.LBL_Dot2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LBL_Dot2.Location = new System.Drawing.Point(104, 9);
            this.LBL_Dot2.Name = "LBL_Dot2";
            this.LBL_Dot2.Size = new System.Drawing.Size(10, 15);
            this.LBL_Dot2.TabIndex = 4;
            this.LBL_Dot2.Text = ".";
            // 
            // TXT_Octett4
            // 
            this.TXT_Octet4.Location = new System.Drawing.Point(116, 3);
            this.TXT_Octet4.MaxLength = 3;
            this.TXT_Octet4.Name = "TXT_Octett4";
            this.TXT_Octet4.PlaceholderText = "1";
            this.TXT_Octet4.Size = new System.Drawing.Size(24, 23);
            this.TXT_Octet4.TabIndex = 4;
            this.TXT_Octet4.TextChanged += this.IpAddress_TextChanged;
            this.TXT_Octet4.KeyPress += this.KeyPressValidation;
            // 
            // IpAddressTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TXT_Octet4);
            this.Controls.Add(this.TXT_Octet3);
            this.Controls.Add(this.LBL_Dot2);
            this.Controls.Add(this.TXT_Octet2);
            this.Controls.Add(this.LBL_Dot1);
            this.Controls.Add(this.TXT_Octet1);
            this.Controls.Add(this.LBL_Dot0);
            this.Name = "IpAddressTextBox";
            this.Size = new System.Drawing.Size(148, 31);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label LBL_Dot0;
        private System.Windows.Forms.TextBox TXT_Octet1;
        private System.Windows.Forms.TextBox TXT_Octet2;
        private System.Windows.Forms.Label LBL_Dot1;
        private System.Windows.Forms.TextBox TXT_Octet3;
        private System.Windows.Forms.Label LBL_Dot2;
        private System.Windows.Forms.TextBox TXT_Octet4;
    }
}
