namespace SysDVRClientGUI
{
	partial class PlayStreamControl
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
			this.cbHwAcc = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// cbHwAcc
			// 
			this.cbHwAcc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbHwAcc.Location = new System.Drawing.Point(5, 5);
			this.cbHwAcc.Name = "cbHwAcc";
			this.cbHwAcc.Size = new System.Drawing.Size(429, 43);
			this.cbHwAcc.TabIndex = 5;
			this.cbHwAcc.Text = "Use hardware acceleration for decoding (not recommended as auto detection may fai" +
    "l, it\'s better to use the --decoder option from cmd)";
			this.cbHwAcc.UseVisualStyleBackColor = true;
			// 
			// PlayStreamControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbHwAcc);
			this.Name = "PlayStreamControl";
			this.Size = new System.Drawing.Size(437, 90);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox cbHwAcc;
	}
}
