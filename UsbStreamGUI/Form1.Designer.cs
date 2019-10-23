namespace UsbStreamGUI
{
	partial class Form1
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

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.StreamConfigPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.radioButton5 = new System.Windows.Forms.RadioButton();
			this.radioButton6 = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.cbStats = new System.Windows.Forms.CheckBox();
			this.cbUsbLog = new System.Windows.Forms.CheckBox();
			this.cbUsbWarn = new System.Windows.Forms.CheckBox();
			this.cbDesync = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.button4 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(551, 375);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Launch";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Launch);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(403, 375);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(114, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Export batch file";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.ExportBatch);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(518, 375);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(19, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "?";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.BatchInfo);
			// 
			// StreamConfigPanel
			// 
			this.StreamConfigPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.StreamConfigPanel.Location = new System.Drawing.Point(6, 178);
			this.StreamConfigPanel.Name = "StreamConfigPanel";
			this.StreamConfigPanel.Size = new System.Drawing.Size(620, 108);
			this.StreamConfigPanel.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioButton3);
			this.groupBox1.Controls.Add(this.radioButton2);
			this.groupBox1.Controls.Add(this.radioButton1);
			this.groupBox1.Location = new System.Drawing.Point(6, 76);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(620, 45);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Channels to stream";
			// 
			// radioButton3
			// 
			this.radioButton3.AutoSize = true;
			this.radioButton3.Location = new System.Drawing.Point(223, 19);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(47, 17);
			this.radioButton3.TabIndex = 2;
			this.radioButton3.TabStop = true;
			this.radioButton3.Text = "Both";
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(113, 19);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(52, 17);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "Audio";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(6, 19);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(52, 17);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Video";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.radioButton4);
			this.groupBox2.Controls.Add(this.radioButton5);
			this.groupBox2.Controls.Add(this.radioButton6);
			this.groupBox2.Location = new System.Drawing.Point(6, 127);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(620, 45);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Stream method";
			// 
			// radioButton4
			// 
			this.radioButton4.AutoSize = true;
			this.radioButton4.Location = new System.Drawing.Point(361, 19);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(78, 17);
			this.radioButton4.TabIndex = 2;
			this.radioButton4.TabStop = true;
			this.radioButton4.Tag = "File";
			this.radioButton4.Text = "Save to file";
			this.radioButton4.UseVisualStyleBackColor = true;
			this.radioButton4.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// radioButton5
			// 
			this.radioButton5.AutoSize = true;
			this.radioButton5.Location = new System.Drawing.Point(156, 19);
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.Size = new System.Drawing.Size(140, 17);
			this.radioButton5.TabIndex = 1;
			this.radioButton5.TabStop = true;
			this.radioButton5.Tag = "Tcp";
			this.radioButton5.Text = "Relay via network (TCP)";
			this.radioButton5.UseVisualStyleBackColor = true;
			this.radioButton5.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// radioButton6
			// 
			this.radioButton6.AutoSize = true;
			this.radioButton6.Location = new System.Drawing.Point(6, 19);
			this.radioButton6.Name = "radioButton6";
			this.radioButton6.Size = new System.Drawing.Size(79, 17);
			this.radioButton6.TabIndex = 0;
			this.radioButton6.TabStop = true;
			this.radioButton6.Tag = "Mpv";
			this.radioButton6.Text = "Play in mpv";
			this.radioButton6.UseVisualStyleBackColor = true;
			this.radioButton6.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.cbStats);
			this.groupBox3.Controls.Add(this.cbUsbLog);
			this.groupBox3.Controls.Add(this.cbUsbWarn);
			this.groupBox3.Controls.Add(this.cbDesync);
			this.groupBox3.Location = new System.Drawing.Point(6, 292);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(620, 71);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Experimental/Debug options";
			// 
			// cbStats
			// 
			this.cbStats.AutoSize = true;
			this.cbStats.Location = new System.Drawing.Point(140, 19);
			this.cbStats.Name = "cbStats";
			this.cbStats.Size = new System.Drawing.Size(114, 17);
			this.cbStats.TabIndex = 3;
			this.cbStats.Text = "Log transfer speed";
			this.cbStats.UseVisualStyleBackColor = true;
			// 
			// cbUsbLog
			// 
			this.cbUsbLog.AutoSize = true;
			this.cbUsbLog.Location = new System.Drawing.Point(140, 44);
			this.cbUsbLog.Name = "cbUsbLog";
			this.cbUsbLog.Size = new System.Drawing.Size(292, 17);
			this.cbUsbLog.TabIndex = 2;
			this.cbUsbLog.Text = "Usb log verbose (not recommended for actual streaming)";
			this.cbUsbLog.UseVisualStyleBackColor = true;
			// 
			// cbUsbWarn
			// 
			this.cbUsbWarn.AutoSize = true;
			this.cbUsbWarn.Location = new System.Drawing.Point(7, 45);
			this.cbUsbWarn.Name = "cbUsbWarn";
			this.cbUsbWarn.Size = new System.Drawing.Size(107, 17);
			this.cbUsbWarn.TabIndex = 1;
			this.cbUsbWarn.Text = "Usb log warnings";
			this.cbUsbWarn.UseVisualStyleBackColor = true;
			// 
			// cbDesync
			// 
			this.cbDesync.AutoSize = true;
			this.cbDesync.Location = new System.Drawing.Point(7, 21);
			this.cbDesync.Name = "cbDesync";
			this.cbDesync.Size = new System.Drawing.Size(112, 17);
			this.cbDesync.TabIndex = 0;
			this.cbDesync.Text = "Attempt desync fix";
			this.cbDesync.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(620, 43);
			this.label1.TabIndex = 7;
			this.label1.Text = "This utility will configure the UsbStream command line automatically, remember th" +
    "at you still need to setup the USB drivers as explained on the guide";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.Location = new System.Drawing.Point(6, 50);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(620, 20);
			this.linkLabel1.TabIndex = 8;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Open the guide";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button4.Location = new System.Drawing.Point(8, 374);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(117, 23);
			this.button4.TabIndex = 9;
			this.button4.Text = "Common issues";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(630, 402);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.StreamConfigPanel);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(646, 441);
			this.Name = "Form1";
			this.Text = "UsbStreamGUI";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Panel StreamConfigPanel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton5;
		private System.Windows.Forms.RadioButton radioButton6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox cbUsbLog;
		private System.Windows.Forms.CheckBox cbUsbWarn;
		private System.Windows.Forms.CheckBox cbDesync;
		private System.Windows.Forms.CheckBox cbStats;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button button4;
	}
}

