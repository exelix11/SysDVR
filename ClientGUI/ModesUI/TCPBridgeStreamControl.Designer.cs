namespace SysDVRClientGUI
{
	partial class TCPBridgeStreamControl
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.rtspStreamOptControl1 = new SysDVRClientGUI.RTSPStreamOptControl();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Switch IP address: ";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(107, 25);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(112, 20);
			this.textBox1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(4, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(479, 23);
			this.label2.TabIndex = 4;
			this.label2.Text = "This will realy the video data received from SysDVR over network as an RTSP serve" +
    "r on your pc";
			// 
			// rtspStreamOptControl1
			// 
			this.rtspStreamOptControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtspStreamOptControl1.Location = new System.Drawing.Point(0, 49);
			this.rtspStreamOptControl1.Name = "rtspStreamOptControl1";
			this.rtspStreamOptControl1.Size = new System.Drawing.Size(485, 113);
			this.rtspStreamOptControl1.TabIndex = 0;
			// 
			// TCPBridgeStreamControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rtspStreamOptControl1);
			this.Name = "TCPBridgeStreamControl";
			this.Size = new System.Drawing.Size(485, 165);
			this.Load += new System.EventHandler(this.TCPBridgeStreamControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private RTSPStreamOptControl rtspStreamOptControl1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
	}
}
