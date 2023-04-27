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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbChannelsBoth = new System.Windows.Forms.RadioButton();
            this.rbChannelsAudio = new System.Windows.Forms.RadioButton();
            this.rbChannelsVideo = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTcpIP = new System.Windows.Forms.TextBox();
            this.rbSrcUsb = new System.Windows.Forms.RadioButton();
            this.rbSrcTcp = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.pAdvOptions = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbPlay = new System.Windows.Forms.RadioButton();
            this.rbSaveToFile = new System.Windows.Forms.RadioButton();
            this.rbStreamRtsp = new System.Windows.Forms.RadioButton();
            this.rbPlayMpv = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbIgnoreSync = new System.Windows.Forms.CheckBox();
            this.cbStats = new System.Windows.Forms.CheckBox();
            this.cbUsbLog = new System.Windows.Forms.CheckBox();
            this.cbUsbWarn = new System.Windows.Forms.CheckBox();
            this.StreamConfigPanel = new System.Windows.Forms.Panel();
            this.cbAdvOpt = new System.Windows.Forms.CheckBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.button4 = new System.Windows.Forms.Button();
            this.cbLogStatus = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pAdvOptions.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(551, 600);
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
            this.button2.Location = new System.Drawing.Point(416, 600);
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
            this.button3.Location = new System.Drawing.Point(530, 600);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(19, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "?";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.BatchInfo);
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
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbTcpIP);
            this.groupBox2.Controls.Add(this.rbSrcUsb);
            this.groupBox2.Controls.Add(this.rbSrcTcp);
            this.groupBox2.Location = new System.Drawing.Point(6, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(620, 102);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stream source";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(11, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(604, 33);
            this.label2.TabIndex = 0;
            this.label2.Text = "Remember to switch to the correct mode with SysDVR-Settings on your console befor" +
    "e beginning to stream. If you need help check the guide.";
            // 
            // tbTcpIP
            // 
            this.tbTcpIP.Location = new System.Drawing.Point(169, 75);
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
            this.rbSrcUsb.Location = new System.Drawing.Point(10, 53);
            this.rbSrcUsb.Name = "rbSrcUsb";
            this.rbSrcUsb.Size = new System.Drawing.Size(245, 17);
            this.rbSrcUsb.TabIndex = 0;
            this.rbSrcUsb.TabStop = true;
            this.rbSrcUsb.Tag = "";
            this.rbSrcUsb.Text = "USB (Will automatically install driver, if needed)";
            this.rbSrcUsb.UseVisualStyleBackColor = true;
            // 
            // rbSrcTcp
            // 
            this.rbSrcTcp.AutoSize = true;
            this.rbSrcTcp.Location = new System.Drawing.Point(10, 77);
            this.rbSrcTcp.Name = "rbSrcTcp";
            this.rbSrcTcp.Size = new System.Drawing.Size(155, 17);
            this.rbSrcTcp.TabIndex = 1;
            this.rbSrcTcp.TabStop = true;
            this.rbSrcTcp.Tag = "";
            this.rbSrcTcp.Text = "TCP Bridge (network mode)";
            this.rbSrcTcp.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(620, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = "This utility will configure the SysDVR-Client command line automatically.\r\nIf you" +
    "\'re not sure what to do here check out the guide on GitHub";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(6, 48);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(620, 20);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Open the guide";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // pAdvOptions
            // 
            this.pAdvOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pAdvOptions.Controls.Add(this.groupBox4);
            this.pAdvOptions.Controls.Add(this.groupBox3);
            this.pAdvOptions.Controls.Add(this.StreamConfigPanel);
            this.pAdvOptions.Location = new System.Drawing.Point(6, 242);
            this.pAdvOptions.Name = "pAdvOptions";
            this.pAdvOptions.Size = new System.Drawing.Size(620, 351);
            this.pAdvOptions.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbPlay);
            this.groupBox4.Controls.Add(this.rbSaveToFile);
            this.groupBox4.Controls.Add(this.rbStreamRtsp);
            this.groupBox4.Controls.Add(this.rbPlayMpv);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(614, 112);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Stream mode";
            // 
            // rbPlay
            // 
            this.rbPlay.AutoSize = true;
            this.rbPlay.Location = new System.Drawing.Point(7, 19);
            this.rbPlay.Name = "rbPlay";
            this.rbPlay.Size = new System.Drawing.Size(259, 17);
            this.rbPlay.TabIndex = 3;
            this.rbPlay.TabStop = true;
            this.rbPlay.Tag = "PLAY";
            this.rbPlay.Text = "Play with the built-in video player (Recommended)";
            this.rbPlay.UseVisualStyleBackColor = true;
            this.rbPlay.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
            // 
            // rbSaveToFile
            // 
            this.rbSaveToFile.AutoSize = true;
            this.rbSaveToFile.Location = new System.Drawing.Point(7, 88);
            this.rbSaveToFile.Name = "rbSaveToFile";
            this.rbSaveToFile.Size = new System.Drawing.Size(78, 17);
            this.rbSaveToFile.TabIndex = 2;
            this.rbSaveToFile.TabStop = true;
            this.rbSaveToFile.Tag = "File";
            this.rbSaveToFile.Text = "Save to file";
            this.rbSaveToFile.UseVisualStyleBackColor = true;
            this.rbSaveToFile.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
            // 
            // rbStreamRtsp
            // 
            this.rbStreamRtsp.AutoSize = true;
            this.rbStreamRtsp.Location = new System.Drawing.Point(7, 42);
            this.rbStreamRtsp.Name = "rbStreamRtsp";
            this.rbStreamRtsp.Size = new System.Drawing.Size(575, 17);
            this.rbStreamRtsp.TabIndex = 0;
            this.rbStreamRtsp.TabStop = true;
            this.rbStreamRtsp.Tag = "RTSP";
            this.rbStreamRtsp.Text = "Relay to a different video player via RTSP -- This is not the RTSP option in SysD" +
    "VR-settings, read the guide if unsure";
            this.rbStreamRtsp.UseVisualStyleBackColor = true;
            this.rbStreamRtsp.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
            // 
            // rbPlayMpv
            // 
            this.rbPlayMpv.AutoSize = true;
            this.rbPlayMpv.Location = new System.Drawing.Point(7, 65);
            this.rbPlayMpv.Name = "rbPlayMpv";
            this.rbPlayMpv.Size = new System.Drawing.Size(256, 17);
            this.rbPlayMpv.TabIndex = 1;
            this.rbPlayMpv.TabStop = true;
            this.rbPlayMpv.Tag = "Mpv";
            this.rbPlayMpv.Text = "Play in mpv with low latency (single channel only)";
            this.rbPlayMpv.UseVisualStyleBackColor = true;
            this.rbPlayMpv.CheckedChanged += new System.EventHandler(this.StreamTargetSelected);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cbLogStatus);
            this.groupBox3.Controls.Add(this.cbIgnoreSync);
            this.groupBox3.Controls.Add(this.cbStats);
            this.groupBox3.Controls.Add(this.cbUsbLog);
            this.groupBox3.Controls.Add(this.cbUsbWarn);
            this.groupBox3.Location = new System.Drawing.Point(7, 280);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(608, 68);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced/Debug options";
            // 
            // cbIgnoreSync
            // 
            this.cbIgnoreSync.AutoSize = true;
            this.cbIgnoreSync.Location = new System.Drawing.Point(140, 19);
            this.cbIgnoreSync.Name = "cbIgnoreSync";
            this.cbIgnoreSync.Size = new System.Drawing.Size(143, 17);
            this.cbIgnoreSync.TabIndex = 5;
            this.cbIgnoreSync.Text = "Ignore Audio/Video sync";
            this.cbIgnoreSync.UseVisualStyleBackColor = true;
            // 
            // cbStats
            // 
            this.cbStats.AutoSize = true;
            this.cbStats.Location = new System.Drawing.Point(7, 19);
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
            this.cbUsbLog.Size = new System.Drawing.Size(136, 17);
            this.cbUsbLog.TabIndex = 4;
            this.cbUsbLog.Text = "Print LibUsb debug info";
            this.cbUsbLog.UseVisualStyleBackColor = true;
            // 
            // cbUsbWarn
            // 
            this.cbUsbWarn.AutoSize = true;
            this.cbUsbWarn.Location = new System.Drawing.Point(7, 45);
            this.cbUsbWarn.Name = "cbUsbWarn";
            this.cbUsbWarn.Size = new System.Drawing.Size(128, 17);
            this.cbUsbWarn.TabIndex = 3;
            this.cbUsbWarn.Text = "Print LibUsb warnings";
            this.cbUsbWarn.UseVisualStyleBackColor = true;
            // 
            // StreamConfigPanel
            // 
            this.StreamConfigPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StreamConfigPanel.Location = new System.Drawing.Point(5, 120);
            this.StreamConfigPanel.Name = "StreamConfigPanel";
            this.StreamConfigPanel.Size = new System.Drawing.Size(610, 159);
            this.StreamConfigPanel.TabIndex = 1;
            // 
            // cbAdvOpt
            // 
            this.cbAdvOpt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbAdvOpt.AutoSize = true;
            this.cbAdvOpt.Checked = true;
            this.cbAdvOpt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAdvOpt.Location = new System.Drawing.Point(132, 603);
            this.cbAdvOpt.Name = "cbAdvOpt";
            this.cbAdvOpt.Size = new System.Drawing.Size(141, 17);
            this.cbAdvOpt.TabIndex = 5;
            this.cbAdvOpt.Text = "Show advanced options";
            this.cbAdvOpt.UseVisualStyleBackColor = true;
            this.cbAdvOpt.CheckedChanged += new System.EventHandler(this.cbAdvOpt_CheckedChanged);
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel2.Location = new System.Drawing.Point(1, 600);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(125, 20);
            this.linkLabel2.TabIndex = 4;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Common issues";
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(299, 600);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(114, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Reinstall USB driver";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cbLogStatus
            // 
            this.cbLogStatus.AutoSize = true;
            this.cbLogStatus.Location = new System.Drawing.Point(298, 19);
            this.cbLogStatus.Name = "cbLogStatus";
            this.cbLogStatus.Size = new System.Drawing.Size(125, 17);
            this.cbLogStatus.TabIndex = 6;
            this.cbLogStatus.Text = "Log status messages";
            this.cbLogStatus.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 627);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.cbAdvOpt);
            this.Controls.Add(this.pAdvOptions);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(646, 666);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(646, 309);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "<Set in code>";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pAdvOptions.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbChannelsBoth;
		private System.Windows.Forms.RadioButton rbChannelsAudio;
		private System.Windows.Forms.RadioButton rbChannelsVideo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.RadioButton rbSrcTcp;
		private System.Windows.Forms.RadioButton rbSrcUsb;
		private System.Windows.Forms.TextBox tbTcpIP;
		private System.Windows.Forms.Panel pAdvOptions;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton rbPlay;
		private System.Windows.Forms.RadioButton rbSaveToFile;
		private System.Windows.Forms.RadioButton rbStreamRtsp;
		private System.Windows.Forms.RadioButton rbPlayMpv;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox cbStats;
		private System.Windows.Forms.CheckBox cbUsbLog;
		private System.Windows.Forms.CheckBox cbUsbWarn;
		private System.Windows.Forms.Panel StreamConfigPanel;
		private System.Windows.Forms.CheckBox cbAdvOpt;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox cbIgnoreSync;
        private System.Windows.Forms.CheckBox cbLogStatus;
    }
}

