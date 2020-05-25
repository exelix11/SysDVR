namespace SysDVRClientGUI
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
			this.label2 = new System.Windows.Forms.Label();
			this.tbVideoFile = new System.Windows.Forms.TextBox();
			this.btnVideo = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(3, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(512, 32);
			this.label2.TabIndex = 4;
			this.label2.Text = "This will save the raw data to a pair of files, video data is raw h264 NAL units," +
    " you can convert it to mp4 via ffmpeg, audio data is uncompressed 16-bit little-" +
    "endian stero samples at 48kHz";
			// 
			// tbVideoFile
			// 
			this.tbVideoFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbVideoFile.Location = new System.Drawing.Point(87, 46);
			this.tbVideoFile.Name = "tbVideoFile";
			this.tbVideoFile.Size = new System.Drawing.Size(391, 20);
			this.tbVideoFile.TabIndex = 0;
			// 
			// btnVideo
			// 
			this.btnVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnVideo.Location = new System.Drawing.Point(484, 45);
			this.btnVideo.Name = "btnVideo";
			this.btnVideo.Size = new System.Drawing.Size(31, 23);
			this.btnVideo.TabIndex = 1;
			this.btnVideo.Text = "...";
			this.btnVideo.UseVisualStyleBackColor = true;
			this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Save directory:";
			// 
			// FileStreamControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnVideo);
			this.Controls.Add(this.tbVideoFile);
			this.Controls.Add(this.label2);
			this.Name = "FileStreamControl";
			this.Size = new System.Drawing.Size(518, 96);
			this.Load += new System.EventHandler(this.FileStreamControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbVideoFile;
		private System.Windows.Forms.Button btnVideo;
		private System.Windows.Forms.Label label1;
	}
}
