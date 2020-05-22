namespace SysDVRClientGUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.StreamConfigPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbChannelsBoth = new System.Windows.Forms.RadioButton();
			this.rbChannelsAudio = new System.Windows.Forms.RadioButton();
			this.rbChannelsVideo = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tbTcpIP = new System.Windows.Forms.TextBox();
			this.rbSrcUsb = new System.Windows.Forms.RadioButton();
			this.rbSrcTcp = new System.Windows.Forms.RadioButton();
			this.rbStreamRtsp = new System.Windows.Forms.RadioButton();
			this.rbSaveToFile = new System.Windows.Forms.RadioButton();
			this.rbPlayMpv = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.cbForceLibusb = new System.Windows.Forms.CheckBox();
			this.cbStats = new System.Windows.Forms.CheckBox();
			this.cbUsbLog = new System.Windows.Forms.CheckBox();
			this.cbUsbWarn = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.button4 = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(551, 427);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 9;
			this.button1.Text = "Launch";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Launch);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(416, 427);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(114, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Export batch file";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.ExportBatch);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(530, 427);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(19, 23);
			this.button3.TabIndex = 8;
			this.button3.Text = "?";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.BatchInfo);
			// 
			// StreamConfigPanel
			// 
			this.StreamConfigPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.StreamConfigPanel.Location = new System.Drawing.Point(6, 238);
			this.StreamConfigPanel.Name = "StreamConfigPanel";
			this.StreamConfigPanel.Size = new System.Drawing.Size(620, 105);
			this.StreamConfigPanel.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.rbChannelsBoth);
			this.groupBox1.Controls.Add(this.rbChannelsAudio);
			this.groupBox1.Controls.Add(this.rbChannelsVideo);
			this.groupBox1.Location = new System.Drawing.Point(6, 83);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(620, 45);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Channels to stream";
			// 
			// rbChannelsBoth
			// 
			this.rbChannelsBoth.AutoSize = true;
			this.rbChannelsBoth.Location = new System.Drawing.Point(223, 19);
			this.rbChannelsBoth.Name = "rbChannelsBoth";
			this.rbChannelsBoth.Size = new System.Drawing.Size(47, 17);
			this.rbChannelsBoth.TabIndex = 2;
			this.rbChannelsBoth.TabStop = true;
			this.rbChannelsBoth.Text = "Both";
			this.rbChannelsBoth.UseVisualStyleBackColor = true;
			this.rbChannelsBoth.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// rbChannelsAudio
			// 
			this.rbChannelsAudio.AutoSize = true;
			this.rbChannelsAudio.Location = new System.Drawing.Point(114, 19);
			this.rbChannelsAudio.Name = "rbChannelsAudio";
			this.rbChannelsAudio.Size = new System.Drawing.Size(52, 17);
			this.rbChannelsAudio.TabIndex = 1;
			this.rbChannelsAudio.TabStop = true;
			this.rbChannelsAudio.Text = "Audio";
			this.rbChannelsAudio.UseVisualStyleBackColor = true;
			this.rbChannelsAudio.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// rbChannelsVideo
			// 
			this.rbChannelsVideo.AutoSize = true;
			this.rbChannelsVideo.Location = new System.Drawing.Point(6, 19);
			this.rbChannelsVideo.Name = "rbChannelsVideo";
			this.rbChannelsVideo.Size = new System.Drawing.Size(52, 17);
			this.rbChannelsVideo.TabIndex = 0;
			this.rbChannelsVideo.TabStop = true;
			this.rbChannelsVideo.Text = "Video";
			this.rbChannelsVideo.UseVisualStyleBackColor = true;
			this.rbChannelsVideo.CheckedChanged += new System.EventHandler(this.StreamKindSelected);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.tbTcpIP);
			this.groupBox2.Controls.Add(this.rbSrcUsb);
			this.groupBox2.Controls.Add(this.rbSrcTcp);
			this.groupBox2.Location = new System.Drawing.Point(6, 134);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(620, 48);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Stream source";
			// 
			// tbTcpIP
			// 
			this.tbTcpIP.Location = new System.Drawing.Point(380, 18);
			this.tbTcpIP.MaxLength = 15;
			this.tbTcpIP.Name = "tbTcpIP";
			this.tbTcpIP.Size = new System.Drawing.Size(100, 20);
			this.tbTcpIP.TabIndex = 2;
			this.tbTcpIP.Text = "IP address";
			this.tbTcpIP.TextChanged += new System.EventHandler(this.tbTcpIP_TextChanged);
			this.tbTcpIP.Enter += new System.EventHandler(this.tbTcpIP_Enter);
			this.tbTcpIP.Leave += new System.EventHandler(this.tbTcpIP_Leave);
			// 
			// rbSrcUsb
			// 
			this.rbSrcUsb.AutoSize = true;
			this.rbSrcUsb.Location = new System.Drawing.Point(7, 19);
			this.rbSrcUsb.Name = "rbSrcUsb";
			this.rbSrcUsb.Size = new System.Drawing.Size(201, 17);
			this.rbSrcUsb.TabIndex = 0;
			this.rbSrcUsb.TabStop = true;
			this.rbSrcUsb.Tag = "";
			this.rbSrcUsb.Text = "USB (requires setting up USB drivers)";
			this.rbSrcUsb.UseVisualStyleBackColor = true;
			// 
			// rbSrcTcp
			// 
			this.rbSrcTcp.AutoSize = true;
			this.rbSrcTcp.Location = new System.Drawing.Point(228, 19);
			this.rbSrcTcp.Name = "rbSrcTcp";
			this.rbSrcTcp.Size = new System.Drawing.Size(155, 17);
			this.rbSrcTcp.TabIndex = 1;
			this.rbSrcTcp.TabStop = true;
			this.rbSrcTcp.Tag = "";
			this.rbSrcTcp.Text = "TCP Bridge (network mode)";
			this.rbSrcTcp.UseVisualStyleBackColor = true;
			// 
			// rbStreamRtsp
			// 
			this.rbStreamRtsp.AutoSize = true;
			this.rbStreamRtsp.Location = new System.Drawing.Point(6, 19);
			this.rbStreamRtsp.Name = "rbStreamRtsp";
			this.rbStreamRtsp.Size = new System.Drawing.Size(188, 17);
			this.rbStreamRtsp.TabIndex = 0;
			this.rbStreamRtsp.TabStop = true;
			this.rbStreamRtsp.Tag = "RTSP";
			this.rbStreamRtsp.Text = "Stream via RTSP (Recommended)";
			this.rbStreamRtsp.UseVisualStyleBackColor = true;
			this.rbStreamRtsp.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// rbSaveToFile
			// 
			this.rbSaveToFile.AutoSize = true;
			this.rbSaveToFile.Location = new System.Drawing.Point(503, 19);
			this.rbSaveToFile.Name = "rbSaveToFile";
			this.rbSaveToFile.Size = new System.Drawing.Size(78, 17);
			this.rbSaveToFile.TabIndex = 2;
			this.rbSaveToFile.TabStop = true;
			this.rbSaveToFile.Tag = "File";
			this.rbSaveToFile.Text = "Save to file";
			this.rbSaveToFile.UseVisualStyleBackColor = true;
			this.rbSaveToFile.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// rbPlayMpv
			// 
			this.rbPlayMpv.AutoSize = true;
			this.rbPlayMpv.Location = new System.Drawing.Point(225, 19);
			this.rbPlayMpv.Name = "rbPlayMpv";
			this.rbPlayMpv.Size = new System.Drawing.Size(237, 17);
			this.rbPlayMpv.TabIndex = 1;
			this.rbPlayMpv.TabStop = true;
			this.rbPlayMpv.Tag = "Mpv";
			this.rbPlayMpv.Text = "Play in mpv (low latency, single channel only)";
			this.rbPlayMpv.UseVisualStyleBackColor = true;
			this.rbPlayMpv.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.cbForceLibusb);
			this.groupBox3.Controls.Add(this.cbStats);
			this.groupBox3.Controls.Add(this.cbUsbLog);
			this.groupBox3.Controls.Add(this.cbUsbWarn);
			this.groupBox3.Location = new System.Drawing.Point(6, 349);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(620, 71);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Experimental/Debug options";
			// 
			// cbForceLibusb
			// 
			this.cbForceLibusb.AutoSize = true;
			this.cbForceLibusb.Location = new System.Drawing.Point(140, 21);
			this.cbForceLibusb.Name = "cbForceLibusb";
			this.cbForceLibusb.Size = new System.Drawing.Size(134, 17);
			this.cbForceLibusb.TabIndex = 1;
			this.cbForceLibusb.Text = "Force LibUsb backend";
			this.cbForceLibusb.UseVisualStyleBackColor = true;
			// 
			// cbStats
			// 
			this.cbStats.AutoSize = true;
			this.cbStats.Location = new System.Drawing.Point(6, 21);
			this.cbStats.Name = "cbStats";
			this.cbStats.Size = new System.Drawing.Size(102, 17);
			this.cbStats.TabIndex = 0;
			this.cbStats.Text = "Log transfer info";
			this.cbStats.UseVisualStyleBackColor = true;
			// 
			// cbUsbLog
			// 
			this.cbUsbLog.AutoSize = true;
			this.cbUsbLog.Location = new System.Drawing.Point(140, 44);
			this.cbUsbLog.Name = "cbUsbLog";
			this.cbUsbLog.Size = new System.Drawing.Size(325, 17);
			this.cbUsbLog.TabIndex = 3;
			this.cbUsbLog.Text = "Print LibUsb debug info (not recommended for actual streaming)";
			this.cbUsbLog.UseVisualStyleBackColor = true;
			// 
			// cbUsbWarn
			// 
			this.cbUsbWarn.AutoSize = true;
			this.cbUsbWarn.Location = new System.Drawing.Point(7, 45);
			this.cbUsbWarn.Name = "cbUsbWarn";
			this.cbUsbWarn.Size = new System.Drawing.Size(128, 17);
			this.cbUsbWarn.TabIndex = 2;
			this.cbUsbWarn.Text = "Print LibUsb warnings";
			this.cbUsbWarn.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(620, 43);
			this.label1.TabIndex = 2;
			this.label1.Text = "This utility will configure the SysDVR-Client command line automatically, remembe" +
    "r that for the first time you still need to setup the USB drivers as explained o" +
    "n the guide";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.Location = new System.Drawing.Point(6, 51);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(620, 20);
			this.linkLabel1.TabIndex = 0;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Open the guide";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button4.Location = new System.Drawing.Point(6, 426);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(117, 23);
			this.button4.TabIndex = 6;
			this.button4.Text = "Common issues";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.rbSaveToFile);
			this.groupBox4.Controls.Add(this.rbStreamRtsp);
			this.groupBox4.Controls.Add(this.rbPlayMpv);
			this.groupBox4.Location = new System.Drawing.Point(9, 188);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(617, 44);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Stream mode";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(630, 454);
			this.Controls.Add(this.groupBox4);
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
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(646, 493);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(646, 493);
			this.Name = "Form1";
			this.Text = "<Set in code>";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Panel StreamConfigPanel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbChannelsBoth;
		private System.Windows.Forms.RadioButton rbChannelsAudio;
		private System.Windows.Forms.RadioButton rbChannelsVideo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rbSaveToFile;
		private System.Windows.Forms.RadioButton rbPlayMpv;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox cbUsbLog;
		private System.Windows.Forms.CheckBox cbUsbWarn;
		private System.Windows.Forms.CheckBox cbStats;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.RadioButton rbStreamRtsp;
		private System.Windows.Forms.RadioButton rbSrcTcp;
		private System.Windows.Forms.RadioButton rbSrcUsb;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox tbTcpIP;
		private System.Windows.Forms.CheckBox cbForceLibusb;
	}
}

