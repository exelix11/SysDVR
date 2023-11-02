namespace SysDVRClientGUI.ModesUI
{
    partial class MpvStreamControl
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
            this.LBL_Info = new System.Windows.Forms.Label();
            this.LLBL_Download = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // BTN_Browse
            // 
            this.BTN_Browse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_Browse.Location = new System.Drawing.Point(514, 35);
            this.BTN_Browse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_Browse.Name = "BTN_Browse";
            this.BTN_Browse.Size = new System.Drawing.Size(48, 27);
            this.BTN_Browse.TabIndex = 1;
            this.BTN_Browse.Text = "...";
            this.BTN_Browse.UseVisualStyleBackColor = true;
            this.BTN_Browse.Click += this.BTN_Browse_Click;
            // 
            // LBL_Path
            // 
            this.LBL_Path.AutoSize = true;
            this.LBL_Path.Location = new System.Drawing.Point(4, 40);
            this.LBL_Path.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Path.Name = "LBL_Path";
            this.LBL_Path.Size = new System.Drawing.Size(61, 15);
            this.LBL_Path.TabIndex = 1;
            this.LBL_Path.Text = "Mpv path:";
            // 
            // TXT_MpvPath
            // 
            this.TXT_MpvPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.TXT_MpvPath.Location = new System.Drawing.Point(75, 37);
            this.TXT_MpvPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TXT_MpvPath.Name = "TXT_MpvPath";
            this.TXT_MpvPath.Size = new System.Drawing.Size(432, 23);
            this.TXT_MpvPath.TabIndex = 0;
            this.TXT_MpvPath.TextChanged += this.TXT_MpvPath_TextChanged;
            // 
            // LBL_Info
            // 
            this.LBL_Info.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LBL_Info.Location = new System.Drawing.Point(4, 5);
            this.LBL_Info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Info.Name = "LBL_Info";
            this.LBL_Info.Size = new System.Drawing.Size(559, 27);
            this.LBL_Info.TabIndex = 3;
            this.LBL_Info.Text = "To stream via mpv you must provide the path of its main executable (called mpv.com for windows)";
            // 
            // LLBL_Download
            // 
            this.LLBL_Download.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LLBL_Download.Location = new System.Drawing.Point(4, 63);
            this.LLBL_Download.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LLBL_Download.Name = "LLBL_Download";
            this.LLBL_Download.Size = new System.Drawing.Size(559, 20);
            this.LLBL_Download.TabIndex = 2;
            this.LLBL_Download.TabStop = true;
            this.LLBL_Download.Text = "Download mpv from the official site";
            this.LLBL_Download.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LLBL_Download.LinkClicked += this.LinkLabel1_LinkClicked;
            // 
            // MpvStreamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LLBL_Download);
            this.Controls.Add(this.LBL_Info);
            this.Controls.Add(this.TXT_MpvPath);
            this.Controls.Add(this.LBL_Path);
            this.Controls.Add(this.BTN_Browse);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MpvStreamControl";
            this.Size = new System.Drawing.Size(566, 103);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Browse;
        private System.Windows.Forms.Label LBL_Path;
        private System.Windows.Forms.TextBox TXT_MpvPath;
        private System.Windows.Forms.Label LBL_Info;
        private System.Windows.Forms.LinkLabel LLBL_Download;
    }
}
