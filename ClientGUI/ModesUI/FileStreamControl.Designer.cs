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
			this.tbAudioFile = new System.Windows.Forms.TextBox();
			this.tbVideoFile = new System.Windows.Forms.TextBox();
			this.btnVideo = new System.Windows.Forms.Button();
			this.btnAudio = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
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
			this.label2.Text = "This will save the raw data to a file, video data is h264 NAL units, you can conv" +
    "ert it to mp4 via ffmpeg, audio data is composed of uncompressed 16-bit little-e" +
    "ndian stero samples at 48kHz";
			// 
			// tbAudioFile
			// 
			this.tbAudioFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAudioFile.Location = new System.Drawing.Point(87, 73);
			this.tbAudioFile.Name = "tbAudioFile";
			this.tbAudioFile.Size = new System.Drawing.Size(391, 20);
			this.tbAudioFile.TabIndex = 5;
			// 
			// tbVideoFile
			// 
			this.tbVideoFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbVideoFile.Location = new System.Drawing.Point(87, 46);
			this.tbVideoFile.Name = "tbVideoFile";
			this.tbVideoFile.Size = new System.Drawing.Size(391, 20);
			this.tbVideoFile.TabIndex = 6;
			// 
			// btnVideo
			// 
			this.btnVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnVideo.Location = new System.Drawing.Point(484, 45);
			this.btnVideo.Name = "btnVideo";
			this.btnVideo.Size = new System.Drawing.Size(31, 23);
			this.btnVideo.TabIndex = 7;
			this.btnVideo.Text = "...";
			this.btnVideo.UseVisualStyleBackColor = true;
			this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
			// 
			// btnAudio
			// 
			this.btnAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAudio.Location = new System.Drawing.Point(484, 71);
			this.btnAudio.Name = "btnAudio";
			this.btnAudio.Size = new System.Drawing.Size(31, 23);
			this.btnAudio.TabIndex = 8;
			this.btnAudio.Text = "...";
			this.btnAudio.UseVisualStyleBackColor = true;
			this.btnAudio.Click += new System.EventHandler(this.btnAudio_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Save video as:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Save audio as:";
			// 
			// FileStreamControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAudio);
			this.Controls.Add(this.btnVideo);
			this.Controls.Add(this.tbVideoFile);
			this.Controls.Add(this.tbAudioFile);
			this.Controls.Add(this.label2);
			this.Name = "FileStreamControl";
			this.Size = new System.Drawing.Size(518, 105);
			this.Load += new System.EventHandler(this.FileStreamControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbAudioFile;
		private System.Windows.Forms.TextBox tbVideoFile;
		private System.Windows.Forms.Button btnVideo;
		private System.Windows.Forms.Button btnAudio;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
	}
}
