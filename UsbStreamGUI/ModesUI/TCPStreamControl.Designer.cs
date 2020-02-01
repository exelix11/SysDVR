namespace UsbStreamGUI
{
	partial class TCPStreamControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbVideoPort = new System.Windows.Forms.TextBox();
			this.tbAudioPort = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Video port:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Audio port:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(1, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(518, 32);
			this.label3.TabIndex = 4;
			this.label3.Text = "This will stream via tcp on your pc (localhost), depending on the firewall config" +
    "uration you can use this to connect from another pc in the local network as well" +
    "";
			// 
			// tbVideoPort
			// 
			this.tbVideoPort.Location = new System.Drawing.Point(67, 41);
			this.tbVideoPort.Name = "tbVideoPort";
			this.tbVideoPort.Size = new System.Drawing.Size(62, 20);
			this.tbVideoPort.TabIndex = 5;
			this.tbVideoPort.Text = "6666";
			// 
			// tbAudioPort
			// 
			this.tbAudioPort.Location = new System.Drawing.Point(67, 65);
			this.tbAudioPort.Name = "tbAudioPort";
			this.tbAudioPort.Size = new System.Drawing.Size(62, 20);
			this.tbAudioPort.TabIndex = 6;
			this.tbAudioPort.Text = "6667";
			// 
			// TCPStreamControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbAudioPort);
			this.Controls.Add(this.tbVideoPort);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "TCPStreamControl";
			this.Size = new System.Drawing.Size(522, 102);
			this.Load += new System.EventHandler(this.TCPStreamControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbVideoPort;
		private System.Windows.Forms.TextBox tbAudioPort;
	}
}
