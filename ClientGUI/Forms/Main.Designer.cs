namespace SysDVRClientGUI.Forms
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.BTN_Launch = new System.Windows.Forms.Button();
            this.BTN_CreateBatch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbChannelsBoth = new System.Windows.Forms.RadioButton();
            this.rbChannelsAudio = new System.Windows.Forms.RadioButton();
            this.rbChannelsVideo = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TXT_TcpIp = new System.Windows.Forms.TextBox();
            this.rbSrcUsb = new System.Windows.Forms.RadioButton();
            this.rbSrcTcp = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.LLBL_ProjectWiki = new System.Windows.Forms.LinkLabel();
            this.pAdvOptions = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbPlay = new System.Windows.Forms.RadioButton();
            this.rbSaveToFile = new System.Windows.Forms.RadioButton();
            this.rbStreamRtsp = new System.Windows.Forms.RadioButton();
            this.rbPlayMpv = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLogStatus = new System.Windows.Forms.CheckBox();
            this.cbIgnoreSync = new System.Windows.Forms.CheckBox();
            this.cbStats = new System.Windows.Forms.CheckBox();
            this.cbUsbLog = new System.Windows.Forms.CheckBox();
            this.cbUsbWarn = new System.Windows.Forms.CheckBox();
            this.StreamConfigPanel = new System.Windows.Forms.Panel();
            this.cbAdvOpt = new System.Windows.Forms.CheckBox();
            this.BTN_DriverInstall = new System.Windows.Forms.Button();
            this.BTN_Exit = new System.Windows.Forms.Button();
            this.ToolTip_BatchInfo = new System.Windows.Forms.ToolTip(this.components);
            this.ERR_IpAddress = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pAdvOptions.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.ERR_IpAddress).BeginInit();
            this.SuspendLayout();
            // 
            // BTN_Launch
            // 
            this.BTN_Launch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_Launch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BTN_Launch.Location = new System.Drawing.Point(544, 693);
            this.BTN_Launch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_Launch.Name = "BTN_Launch";
            this.BTN_Launch.Size = new System.Drawing.Size(88, 27);
            this.BTN_Launch.TabIndex = 9;
            this.BTN_Launch.Text = "Launch";
            this.BTN_Launch.UseVisualStyleBackColor = true;
            this.BTN_Launch.Click += this.Launch;
            // 
            // BTN_CreateBatch
            // 
            this.BTN_CreateBatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_CreateBatch.Location = new System.Drawing.Point(308, 693);
            this.BTN_CreateBatch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_CreateBatch.Name = "BTN_CreateBatch";
            this.BTN_CreateBatch.Size = new System.Drawing.Size(211, 27);
            this.BTN_CreateBatch.TabIndex = 7;
            this.BTN_CreateBatch.Text = "Create quick launch shortcut";
            this.ToolTip_BatchInfo.SetToolTip(this.BTN_CreateBatch, "This will create a bat file to launch SysDVR-Client with the selected options you will just need to double click it.");
            this.BTN_CreateBatch.UseVisualStyleBackColor = true;
            this.BTN_CreateBatch.Click += this.ExportBatch;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupBox1.Controls.Add(this.rbChannelsBoth);
            this.groupBox1.Controls.Add(this.rbChannelsAudio);
            this.groupBox1.Controls.Add(this.rbChannelsVideo);
            this.groupBox1.Location = new System.Drawing.Point(7, 96);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(723, 52);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channels to stream";
            // 
            // rbChannelsBoth
            // 
            this.rbChannelsBoth.AutoSize = true;
            this.rbChannelsBoth.Checked = true;
            this.rbChannelsBoth.Location = new System.Drawing.Point(260, 22);
            this.rbChannelsBoth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbChannelsBoth.Name = "rbChannelsBoth";
            this.rbChannelsBoth.Size = new System.Drawing.Size(50, 19);
            this.rbChannelsBoth.TabIndex = 2;
            this.rbChannelsBoth.TabStop = true;
            this.rbChannelsBoth.Text = "Both";
            this.rbChannelsBoth.UseVisualStyleBackColor = true;
            this.rbChannelsBoth.CheckedChanged += this.StreamKindSelected;
            // 
            // rbChannelsAudio
            // 
            this.rbChannelsAudio.AutoSize = true;
            this.rbChannelsAudio.Location = new System.Drawing.Point(133, 22);
            this.rbChannelsAudio.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbChannelsAudio.Name = "rbChannelsAudio";
            this.rbChannelsAudio.Size = new System.Drawing.Size(57, 19);
            this.rbChannelsAudio.TabIndex = 1;
            this.rbChannelsAudio.Text = "Audio";
            this.rbChannelsAudio.UseVisualStyleBackColor = true;
            this.rbChannelsAudio.CheckedChanged += this.StreamKindSelected;
            // 
            // rbChannelsVideo
            // 
            this.rbChannelsVideo.AutoSize = true;
            this.rbChannelsVideo.Location = new System.Drawing.Point(7, 22);
            this.rbChannelsVideo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbChannelsVideo.Name = "rbChannelsVideo";
            this.rbChannelsVideo.Size = new System.Drawing.Size(55, 19);
            this.rbChannelsVideo.TabIndex = 0;
            this.rbChannelsVideo.Text = "Video";
            this.rbChannelsVideo.UseVisualStyleBackColor = true;
            this.rbChannelsVideo.CheckedChanged += this.StreamKindSelected;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.TXT_TcpIp);
            this.groupBox2.Controls.Add(this.rbSrcUsb);
            this.groupBox2.Controls.Add(this.rbSrcTcp);
            this.groupBox2.Location = new System.Drawing.Point(7, 155);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(723, 118);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stream source";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label2.Location = new System.Drawing.Point(13, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(705, 38);
            this.label2.TabIndex = 0;
            this.label2.Text = "Remember to switch to the correct mode with SysDVR-Settings on your console before beginning to stream. If you need help check the guide.";
            // 
            // TXT_TcpIp
            // 
            this.TXT_TcpIp.Enabled = false;
            this.TXT_TcpIp.Location = new System.Drawing.Point(197, 87);
            this.TXT_TcpIp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TXT_TcpIp.MaxLength = 15;
            this.TXT_TcpIp.Name = "TXT_TcpIp";
            this.TXT_TcpIp.Size = new System.Drawing.Size(116, 23);
            this.TXT_TcpIp.TabIndex = 2;
            this.TXT_TcpIp.Text = "IP address";
            this.TXT_TcpIp.TextChanged += this.TXT_TcpIp_TextChanged;
            this.TXT_TcpIp.Enter += this.tbTcpIP_Enter;
            this.TXT_TcpIp.Leave += this.tbTcpIP_Leave;
            // 
            // rbSrcUsb
            // 
            this.rbSrcUsb.AutoSize = true;
            this.rbSrcUsb.Checked = true;
            this.rbSrcUsb.Location = new System.Drawing.Point(12, 61);
            this.rbSrcUsb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbSrcUsb.Name = "rbSrcUsb";
            this.rbSrcUsb.Size = new System.Drawing.Size(274, 19);
            this.rbSrcUsb.TabIndex = 0;
            this.rbSrcUsb.TabStop = true;
            this.rbSrcUsb.Tag = "";
            this.rbSrcUsb.Text = "USB (Will automatically install driver, if needed)";
            this.rbSrcUsb.UseVisualStyleBackColor = true;
            // 
            // rbSrcTcp
            // 
            this.rbSrcTcp.AutoSize = true;
            this.rbSrcTcp.Location = new System.Drawing.Point(12, 89);
            this.rbSrcTcp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbSrcTcp.Name = "rbSrcTcp";
            this.rbSrcTcp.Size = new System.Drawing.Size(170, 19);
            this.rbSrcTcp.TabIndex = 1;
            this.rbSrcTcp.Tag = "";
            this.rbSrcTcp.Text = "TCP Bridge (network mode)";
            this.rbSrcTcp.UseVisualStyleBackColor = true;
            this.rbSrcTcp.CheckedChanged += this.rbSrcTcp_CheckedChanged;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(723, 81);
            this.label1.TabIndex = 0;
            this.label1.Text = "This utility will configure the SysDVR-Client command line automatically.\r\nIf you're not sure what to do here check out the guide on GitHub";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LLBL_ProjectWiki
            // 
            this.LLBL_ProjectWiki.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.LLBL_ProjectWiki.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LLBL_ProjectWiki.Location = new System.Drawing.Point(7, 55);
            this.LLBL_ProjectWiki.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LLBL_ProjectWiki.Name = "LLBL_ProjectWiki";
            this.LLBL_ProjectWiki.Size = new System.Drawing.Size(723, 23);
            this.LLBL_ProjectWiki.TabIndex = 0;
            this.LLBL_ProjectWiki.TabStop = true;
            this.LLBL_ProjectWiki.Text = "Open the guide";
            this.LLBL_ProjectWiki.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LLBL_ProjectWiki.LinkClicked += this.LLBL_ProjectWiki_LinkClicked;
            // 
            // pAdvOptions
            // 
            this.pAdvOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.pAdvOptions.Controls.Add(this.groupBox4);
            this.pAdvOptions.Controls.Add(this.groupBox3);
            this.pAdvOptions.Controls.Add(this.StreamConfigPanel);
            this.pAdvOptions.Location = new System.Drawing.Point(7, 279);
            this.pAdvOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pAdvOptions.Name = "pAdvOptions";
            this.pAdvOptions.Size = new System.Drawing.Size(723, 405);
            this.pAdvOptions.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbPlay);
            this.groupBox4.Controls.Add(this.rbSaveToFile);
            this.groupBox4.Controls.Add(this.rbStreamRtsp);
            this.groupBox4.Controls.Add(this.rbPlayMpv);
            this.groupBox4.Location = new System.Drawing.Point(4, 3);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Size = new System.Drawing.Size(716, 129);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Stream mode";
            // 
            // rbPlay
            // 
            this.rbPlay.AutoSize = true;
            this.rbPlay.Checked = true;
            this.rbPlay.Location = new System.Drawing.Point(8, 22);
            this.rbPlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbPlay.Name = "rbPlay";
            this.rbPlay.Size = new System.Drawing.Size(294, 19);
            this.rbPlay.TabIndex = 3;
            this.rbPlay.TabStop = true;
            this.rbPlay.Tag = "PLAY";
            this.rbPlay.Text = "Play with the built-in video player (Recommended)";
            this.rbPlay.UseVisualStyleBackColor = true;
            this.rbPlay.CheckedChanged += this.StreamTargetSelected;
            // 
            // rbSaveToFile
            // 
            this.rbSaveToFile.AutoSize = true;
            this.rbSaveToFile.Location = new System.Drawing.Point(8, 102);
            this.rbSaveToFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbSaveToFile.Name = "rbSaveToFile";
            this.rbSaveToFile.Size = new System.Drawing.Size(82, 19);
            this.rbSaveToFile.TabIndex = 2;
            this.rbSaveToFile.Tag = "File";
            this.rbSaveToFile.Text = "Save to file";
            this.rbSaveToFile.UseVisualStyleBackColor = true;
            this.rbSaveToFile.CheckedChanged += this.StreamTargetSelected;
            // 
            // rbStreamRtsp
            // 
            this.rbStreamRtsp.AutoSize = true;
            this.rbStreamRtsp.Location = new System.Drawing.Point(8, 48);
            this.rbStreamRtsp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbStreamRtsp.Name = "rbStreamRtsp";
            this.rbStreamRtsp.Size = new System.Drawing.Size(624, 19);
            this.rbStreamRtsp.TabIndex = 0;
            this.rbStreamRtsp.Tag = "RTSP";
            this.rbStreamRtsp.Text = "Relay to a different video player via RTSP -- This is not the RTSP option in SysDVR-settings, read the guide if unsure";
            this.rbStreamRtsp.UseVisualStyleBackColor = true;
            this.rbStreamRtsp.CheckedChanged += this.StreamTargetSelected;
            // 
            // rbPlayMpv
            // 
            this.rbPlayMpv.AutoSize = true;
            this.rbPlayMpv.Location = new System.Drawing.Point(8, 75);
            this.rbPlayMpv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rbPlayMpv.Name = "rbPlayMpv";
            this.rbPlayMpv.Size = new System.Drawing.Size(289, 19);
            this.rbPlayMpv.TabIndex = 1;
            this.rbPlayMpv.Tag = "Mpv";
            this.rbPlayMpv.Text = "Play in mpv with low latency (single channel only)";
            this.rbPlayMpv.UseVisualStyleBackColor = true;
            this.rbPlayMpv.CheckedChanged += this.StreamTargetSelected;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupBox3.Controls.Add(this.cbLogStatus);
            this.groupBox3.Controls.Add(this.cbIgnoreSync);
            this.groupBox3.Controls.Add(this.cbStats);
            this.groupBox3.Controls.Add(this.cbUsbLog);
            this.groupBox3.Controls.Add(this.cbUsbWarn);
            this.groupBox3.Location = new System.Drawing.Point(8, 323);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(709, 78);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced/Debug options";
            // 
            // cbLogStatus
            // 
            this.cbLogStatus.AutoSize = true;
            this.cbLogStatus.Location = new System.Drawing.Point(348, 22);
            this.cbLogStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbLogStatus.Name = "cbLogStatus";
            this.cbLogStatus.Size = new System.Drawing.Size(134, 19);
            this.cbLogStatus.TabIndex = 6;
            this.cbLogStatus.Text = "Log status messages";
            this.cbLogStatus.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreSync
            // 
            this.cbIgnoreSync.AutoSize = true;
            this.cbIgnoreSync.Location = new System.Drawing.Point(163, 22);
            this.cbIgnoreSync.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbIgnoreSync.Name = "cbIgnoreSync";
            this.cbIgnoreSync.Size = new System.Drawing.Size(157, 19);
            this.cbIgnoreSync.TabIndex = 5;
            this.cbIgnoreSync.Text = "Ignore Audio/Video sync";
            this.cbIgnoreSync.UseVisualStyleBackColor = true;
            // 
            // cbStats
            // 
            this.cbStats.AutoSize = true;
            this.cbStats.Location = new System.Drawing.Point(8, 22);
            this.cbStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbStats.Name = "cbStats";
            this.cbStats.Size = new System.Drawing.Size(113, 19);
            this.cbStats.TabIndex = 0;
            this.cbStats.Text = "Log transfer info";
            this.cbStats.UseVisualStyleBackColor = true;
            // 
            // cbUsbLog
            // 
            this.cbUsbLog.AutoSize = true;
            this.cbUsbLog.Location = new System.Drawing.Point(163, 51);
            this.cbUsbLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbUsbLog.Name = "cbUsbLog";
            this.cbUsbLog.Size = new System.Drawing.Size(151, 19);
            this.cbUsbLog.TabIndex = 4;
            this.cbUsbLog.Text = "Print LibUsb debug info";
            this.cbUsbLog.UseVisualStyleBackColor = true;
            // 
            // cbUsbWarn
            // 
            this.cbUsbWarn.AutoSize = true;
            this.cbUsbWarn.Location = new System.Drawing.Point(8, 52);
            this.cbUsbWarn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbUsbWarn.Name = "cbUsbWarn";
            this.cbUsbWarn.Size = new System.Drawing.Size(141, 19);
            this.cbUsbWarn.TabIndex = 3;
            this.cbUsbWarn.Text = "Print LibUsb warnings";
            this.cbUsbWarn.UseVisualStyleBackColor = true;
            // 
            // StreamConfigPanel
            // 
            this.StreamConfigPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.StreamConfigPanel.Location = new System.Drawing.Point(6, 138);
            this.StreamConfigPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StreamConfigPanel.Name = "StreamConfigPanel";
            this.StreamConfigPanel.Size = new System.Drawing.Size(712, 183);
            this.StreamConfigPanel.TabIndex = 1;
            // 
            // cbAdvOpt
            // 
            this.cbAdvOpt.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.cbAdvOpt.AutoSize = true;
            this.cbAdvOpt.Location = new System.Drawing.Point(7, 698);
            this.cbAdvOpt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbAdvOpt.Name = "cbAdvOpt";
            this.cbAdvOpt.Size = new System.Drawing.Size(152, 19);
            this.cbAdvOpt.TabIndex = 5;
            this.cbAdvOpt.Text = "Show advanced options";
            this.cbAdvOpt.UseVisualStyleBackColor = true;
            this.cbAdvOpt.CheckedChanged += this.cbAdvOpt_CheckedChanged;
            // 
            // BTN_DriverInstall
            // 
            this.BTN_DriverInstall.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_DriverInstall.Location = new System.Drawing.Point(168, 693);
            this.BTN_DriverInstall.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_DriverInstall.Name = "BTN_DriverInstall";
            this.BTN_DriverInstall.Size = new System.Drawing.Size(133, 27);
            this.BTN_DriverInstall.TabIndex = 6;
            this.BTN_DriverInstall.Text = "Reinstall USB driver";
            this.BTN_DriverInstall.UseVisualStyleBackColor = true;
            this.BTN_DriverInstall.Click += this.BTN_DriverInstall_Click;
            // 
            // BTN_Exit
            // 
            this.BTN_Exit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.BTN_Exit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BTN_Exit.Location = new System.Drawing.Point(637, 693);
            this.BTN_Exit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTN_Exit.Name = "BTN_Exit";
            this.BTN_Exit.Size = new System.Drawing.Size(88, 27);
            this.BTN_Exit.TabIndex = 10;
            this.BTN_Exit.Text = "E&xit";
            this.BTN_Exit.UseVisualStyleBackColor = true;
            this.BTN_Exit.Click += this.BTN_Exit_Click;
            // 
            // ToolTip_BatchInfo
            // 
            this.ToolTip_BatchInfo.ToolTipTitle = "Create Batch Info";
            // 
            // ERR_IpAddress
            // 
            this.ERR_IpAddress.ContainerControl = this;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 723);
            this.Controls.Add(this.BTN_Exit);
            this.Controls.Add(this.BTN_DriverInstall);
            this.Controls.Add(this.cbAdvOpt);
            this.Controls.Add(this.pAdvOptions);
            this.Controls.Add(this.LLBL_ProjectWiki);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BTN_CreateBatch);
            this.Controls.Add(this.BTN_Launch);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(751, 762);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(751, 351);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<Set in code>";
            this.FormClosing += this.Main_FormClosing;
            this.Load += this.Main_Load;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pAdvOptions.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.ERR_IpAddress).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Launch;
        private System.Windows.Forms.Button BTN_CreateBatch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbChannelsBoth;
        private System.Windows.Forms.RadioButton rbChannelsAudio;
        private System.Windows.Forms.RadioButton rbChannelsVideo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel LLBL_ProjectWiki;
        private System.Windows.Forms.RadioButton rbSrcTcp;
        private System.Windows.Forms.RadioButton rbSrcUsb;
        private System.Windows.Forms.TextBox TXT_TcpIp;
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
        private System.Windows.Forms.Button BTN_DriverInstall;
        private System.Windows.Forms.CheckBox cbIgnoreSync;
        private System.Windows.Forms.CheckBox cbLogStatus;
        private System.Windows.Forms.Button BTN_Exit;
        private System.Windows.Forms.ToolTip ToolTip_BatchInfo;
        private System.Windows.Forms.ErrorProvider ERR_IpAddress;
    }
}

