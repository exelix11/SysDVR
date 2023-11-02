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
            this.LBL_Path = new System.Windows.Forms.Label();
            this.TXT_MpvPath = new System.Windows.Forms.TextBox();
            this.LBL_InfoLaunch = new System.Windows.Forms.Label();
            this.LLBL_Download = new System.Windows.Forms.LinkLabel();
            this.LBL_Info = new System.Windows.Forms.Label();
            this.GRP_Options = new System.Windows.Forms.GroupBox();
            this.cbMpvUntimed = new System.Windows.Forms.CheckBox();
            this.cbMpvLowLat = new System.Windows.Forms.CheckBox();
            this.GRP_Options.SuspendLayout();
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
            // LBL_Path
            // 
            this.LBL_Path.AutoSize = true;
            this.LBL_Path.Location = new System.Drawing.Point(4, 73);
            this.LBL_Path.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Path.Name = "LBL_Path";
            this.LBL_Path.Size = new System.Drawing.Size(61, 15);
            this.LBL_Path.TabIndex = 2;
            this.LBL_Path.Text = "Mpv path:";
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
            // LBL_InfoLaunch
            // 
            this.LBL_InfoLaunch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LBL_InfoLaunch.Location = new System.Drawing.Point(4, 44);
            this.LBL_InfoLaunch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_InfoLaunch.Name = "LBL_InfoLaunch";
            this.LBL_InfoLaunch.Size = new System.Drawing.Size(559, 27);
            this.LBL_InfoLaunch.TabIndex = 1;
            this.LBL_InfoLaunch.Text = "Launch the stream automatically ? Select Mpv's main executable (Optional)";
            // 
            // LLBL_Download
            // 
            this.LLBL_Download.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LLBL_Download.Location = new System.Drawing.Point(4, 96);
            this.LLBL_Download.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LLBL_Download.Name = "LLBL_Download";
            this.LLBL_Download.Size = new System.Drawing.Size(559, 20);
            this.LLBL_Download.TabIndex = 5;
            this.LLBL_Download.TabStop = true;
            this.LLBL_Download.Text = "Download mpv from the official site";
            this.LLBL_Download.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LLBL_Download.LinkClicked += this.LLBL_LinkClicked;
            // 
            // LBL_Info
            // 
            this.LBL_Info.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LBL_Info.Location = new System.Drawing.Point(4, 0);
            this.LBL_Info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Info.Name = "LBL_Info";
            this.LBL_Info.Size = new System.Drawing.Size(559, 37);
            this.LBL_Info.TabIndex = 0;
            this.LBL_Info.Text = "To connect to the RTSP stream open a video player (mpv is recommended) and connect to rtsp://127.0.0.1:6666/ (this is the fixed address of your pc, you don't need to change it)";
            // 
            // GRP_Options
            // 
            this.GRP_Options.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.GRP_Options.Controls.Add(this.cbMpvUntimed);
            this.GRP_Options.Controls.Add(this.cbMpvLowLat);
            this.GRP_Options.Location = new System.Drawing.Point(6, 123);
            this.GRP_Options.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GRP_Options.Name = "GRP_Options";
            this.GRP_Options.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GRP_Options.Size = new System.Drawing.Size(555, 55);
            this.GRP_Options.TabIndex = 6;
            this.GRP_Options.TabStop = false;
            this.GRP_Options.Text = "Mpv options";
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
            this.Controls.Add(this.GRP_Options);
            this.Controls.Add(this.LBL_Info);
            this.Controls.Add(this.LLBL_Download);
            this.Controls.Add(this.TXT_MpvPath);
            this.Controls.Add(this.LBL_Path);
            this.Controls.Add(this.BTN_Browse);
            this.Controls.Add(this.LBL_InfoLaunch);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "RTSPStreamOptControl";
            this.Size = new System.Drawing.Size(566, 182);
            this.GRP_Options.ResumeLayout(false);
            this.GRP_Options.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Browse;
        private System.Windows.Forms.Label LBL_Path;
        private System.Windows.Forms.TextBox TXT_MpvPath;
        private System.Windows.Forms.Label LBL_InfoLaunch;
        private System.Windows.Forms.LinkLabel LLBL_Download;
        private System.Windows.Forms.Label LBL_Info;
        private System.Windows.Forms.GroupBox GRP_Options;
        private System.Windows.Forms.CheckBox cbMpvUntimed;
        private System.Windows.Forms.CheckBox cbMpvLowLat;
    }
}
