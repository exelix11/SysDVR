namespace SysDVRClientGUI.ModesUI
{
    partial class RTSPStreamOptControl
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.BTN_Browse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TXT_MpvPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbMpvUntimed = new System.Windows.Forms.CheckBox();
            this.cbMpvLowLat = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BTN_Browse
            // 
            this.BTN_Browse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_Browse.Location = new System.Drawing.Point(514, 67);
            this.BTN_Browse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_Browse.Name = "BTN_Browse";
            this.BTN_Browse.Size = new System.Drawing.Size(48, 27);
            this.BTN_Browse.TabIndex = 4;
            this.BTN_Browse.Text = "...";
            this.BTN_Browse.UseVisualStyleBackColor = true;
            this.BTN_Browse.Click += this.BTN_Browse_Click;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 73);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mpv path:";
            // 
            // TXT_MpvPath
            // 
            this.TXT_MpvPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.TXT_MpvPath.Location = new System.Drawing.Point(75, 69);
            this.TXT_MpvPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TXT_MpvPath.Name = "TXT_MpvPath";
            this.TXT_MpvPath.Size = new System.Drawing.Size(432, 23);
            this.TXT_MpvPath.TabIndex = 3;
            this.TXT_MpvPath.TextChanged += this.TXT_MpvPath_TextChanged;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label2.Location = new System.Drawing.Point(4, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(559, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "Launch the stream automatically ? Select Mpv's main executable (Optional)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.linkLabel1.Location = new System.Drawing.Point(4, 96);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(559, 20);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Download mpv from the official site";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += this.LLBL_LinkClicked;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label3.Location = new System.Drawing.Point(4, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(559, 37);
            this.label3.TabIndex = 0;
            this.label3.Text = "To connect to the RTSP stream open a video player (mpv is recommended) and connect to rtsp://127.0.0.1:6666/ (this is the fixed address of your pc, you don't need to change it)";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupBox1.Controls.Add(this.cbMpvUntimed);
            this.groupBox1.Controls.Add(this.cbMpvLowLat);
            this.groupBox1.Location = new System.Drawing.Point(6, 123);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(555, 55);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mpv options";
            // 
            // cbMpvUntimed
            // 
            this.cbMpvUntimed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.cbMpvUntimed.AutoSize = true;
            this.cbMpvUntimed.Location = new System.Drawing.Point(290, 22);
            this.cbMpvUntimed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbMpvUntimed.Name = "cbMpvUntimed";
            this.cbMpvUntimed.Size = new System.Drawing.Size(258, 19);
            this.cbMpvUntimed.TabIndex = 1;
            this.cbMpvUntimed.Text = "Do not wait for synchronization (--untimed)";
            this.cbMpvUntimed.UseVisualStyleBackColor = true;
            this.cbMpvUntimed.CheckedChanged += this.CHK_MpvUntimed_CheckedChanged;
            // 
            // cbMpvLowLat
            // 
            this.cbMpvLowLat.AutoSize = true;
            this.cbMpvLowLat.Location = new System.Drawing.Point(7, 22);
            this.cbMpvLowLat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbMpvLowLat.Name = "cbMpvLowLat";
            this.cbMpvLowLat.Size = new System.Drawing.Size(150, 19);
            this.cbMpvLowLat.TabIndex = 0;
            this.cbMpvLowLat.Text = "RTSP low-latency mode";
            this.cbMpvLowLat.UseVisualStyleBackColor = true;
            this.cbMpvLowLat.CheckedChanged += this.CHK_MpvLowLat_CheckedChanged;
            // 
            // RTSPStreamOptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.TXT_MpvPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTN_Browse);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "RTSPStreamOptControl";
            this.Size = new System.Drawing.Size(566, 182);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TXT_MpvPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbMpvUntimed;
        private System.Windows.Forms.CheckBox cbMpvLowLat;
    }
}
