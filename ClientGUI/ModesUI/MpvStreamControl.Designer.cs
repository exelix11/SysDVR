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
            this.label1 = new System.Windows.Forms.Label();
            this.TXT_MpvPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mpv path:";
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
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(559, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "To stream via mpv you must provide the path of its main executable (called mpv.com for windows)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.linkLabel1.Location = new System.Drawing.Point(4, 63);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(559, 20);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Download mpv from the official site";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += this.LinkLabel1_LinkClicked;
            // 
            // MpvStreamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TXT_MpvPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTN_Browse);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MpvStreamControl";
            this.Size = new System.Drawing.Size(566, 103);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TXT_MpvPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
