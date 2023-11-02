namespace SysDVRClientGUI.ModesUI
{
    partial class FileStreamControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileStreamControl));
            this.LBL_Info = new System.Windows.Forms.Label();
            this.tbVideoFile = new System.Windows.Forms.TextBox();
            this.btnVideo = new System.Windows.Forms.Button();
            this.LBL_Path = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LBL_Info
            // 
            this.LBL_Info.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LBL_Info.Location = new System.Drawing.Point(4, 5);
            this.LBL_Info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Info.Name = "LBL_Info";
            this.LBL_Info.Size = new System.Drawing.Size(597, 54);
            this.LBL_Info.TabIndex = 4;
            this.LBL_Info.Text = resources.GetString("LBL_Info.Text");
            // 
            // tbVideoFile
            // 
            this.tbVideoFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tbVideoFile.Location = new System.Drawing.Point(48, 66);
            this.tbVideoFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbVideoFile.Name = "tbVideoFile";
            this.tbVideoFile.Size = new System.Drawing.Size(509, 23);
            this.tbVideoFile.TabIndex = 0;
            this.tbVideoFile.TextChanged += this.tbVideoFile_TextChanged;
            // 
            // btnVideo
            // 
            this.btnVideo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnVideo.Location = new System.Drawing.Point(565, 65);
            this.btnVideo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(36, 27);
            this.btnVideo.TabIndex = 1;
            this.btnVideo.Text = "...";
            this.btnVideo.UseVisualStyleBackColor = true;
            this.btnVideo.Click += this.BTN_Video_Click;
            // 
            // LBL_Path
            // 
            this.LBL_Path.AutoSize = true;
            this.LBL_Path.Location = new System.Drawing.Point(4, 69);
            this.LBL_Path.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LBL_Path.Name = "LBL_Path";
            this.LBL_Path.Size = new System.Drawing.Size(34, 15);
            this.LBL_Path.TabIndex = 9;
            this.LBL_Path.Text = "Path:";
            // 
            // FileStreamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LBL_Path);
            this.Controls.Add(this.btnVideo);
            this.Controls.Add(this.tbVideoFile);
            this.Controls.Add(this.LBL_Info);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FileStreamControl";
            this.Size = new System.Drawing.Size(604, 111);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label LBL_Info;
        private System.Windows.Forms.TextBox tbVideoFile;
        private System.Windows.Forms.Button btnVideo;
        private System.Windows.Forms.Label LBL_Path;
    }
}
