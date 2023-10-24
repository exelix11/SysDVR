namespace SysDVRClientGUI.ModesUI
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
            this.CHK_HwAcc = new System.Windows.Forms.CheckBox();
            this.CHK_BestScaling = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CHK_HwAcc
            // 
            this.CHK_HwAcc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.CHK_HwAcc.Location = new System.Drawing.Point(6, 6);
            this.CHK_HwAcc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CHK_HwAcc.Name = "CHK_HwAcc";
            this.CHK_HwAcc.Size = new System.Drawing.Size(500, 47);
            this.CHK_HwAcc.TabIndex = 5;
            this.CHK_HwAcc.Text = "Use hardware acceleration for decoding (not recommended as auto detection may fail, it's better to use the --decoder option from cmd)";
            this.CHK_HwAcc.UseVisualStyleBackColor = true;
            this.CHK_HwAcc.CheckedChanged += this.CHK_HwAcc_CheckedChanged;
            // 
            // CHK_BestScaling
            // 
            this.CHK_BestScaling.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.CHK_BestScaling.Location = new System.Drawing.Point(6, 51);
            this.CHK_BestScaling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CHK_BestScaling.Name = "CHK_BestScaling";
            this.CHK_BestScaling.Size = new System.Drawing.Size(500, 40);
            this.CHK_BestScaling.TabIndex = 6;
            this.CHK_BestScaling.Text = "Use anisotropic filtering for scaling (direct3d renderer only)";
            this.CHK_BestScaling.UseVisualStyleBackColor = true;
            this.CHK_BestScaling.CheckedChanged += this.CHK_BestScaling_CheckedChanged;
            // 
            // PlayStreamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CHK_BestScaling);
            this.Controls.Add(this.CHK_HwAcc);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PlayStreamControl";
            this.Size = new System.Drawing.Size(510, 104);
            this.ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.CheckBox CHK_HwAcc;
        private System.Windows.Forms.CheckBox CHK_BestScaling;
    }
}
