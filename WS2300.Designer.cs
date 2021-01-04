namespace WS2300
{
    partial class WS2300
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WS2300));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnInTmp = new System.Windows.Forms.Button();
            this.lblIntTemp = new System.Windows.Forms.Label();
            this.btnRead = new System.Windows.Forms.Button();
            this.textBoxReceive = new System.Windows.Forms.TextBox();
            this.btnMinMax = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnHistorySave = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnReadTh = new System.Windows.Forms.Button();
            this.lblHistory = new System.Windows.Forms.Label();
            this.btnHistoryRead = new System.Windows.Forms.Button();
            this.btnGRAPH = new System.Windows.Forms.Button();
            this.btnMeans = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.checkBoxValues = new System.Windows.Forms.CheckBox();
            this.comboBoxCom = new System.Windows.Forms.ComboBox();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 352);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(394, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(112, 17);
            this.toolStripStatusLabel.Text = "toolStripStatusLabel";
            // 
            // btnInTmp
            // 
            this.btnInTmp.Location = new System.Drawing.Point(12, 12);
            this.btnInTmp.Name = "btnInTmp";
            this.btnInTmp.Size = new System.Drawing.Size(90, 23);
            this.btnInTmp.TabIndex = 1;
            this.btnInTmp.Text = "Internal Temp";
            this.btnInTmp.UseVisualStyleBackColor = true;
            this.btnInTmp.Click += new System.EventHandler(this.BtnInTmp_Click);
            // 
            // lblIntTemp
            // 
            this.lblIntTemp.AutoSize = true;
            this.lblIntTemp.Location = new System.Drawing.Point(133, 17);
            this.lblIntTemp.Name = "lblIntTemp";
            this.lblIntTemp.Size = new System.Drawing.Size(56, 13);
            this.lblIntTemp.TabIndex = 2;
            this.lblIntTemp.Text = "lblIntTemp";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(13, 131);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(90, 23);
            this.btnRead.TabIndex = 5;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.BtnRead_Click);
            // 
            // textBoxReceive
            // 
            this.textBoxReceive.Location = new System.Drawing.Point(131, 41);
            this.textBoxReceive.Multiline = true;
            this.textBoxReceive.Name = "textBoxReceive";
            this.textBoxReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxReceive.Size = new System.Drawing.Size(251, 231);
            this.textBoxReceive.TabIndex = 15;
            // 
            // btnMinMax
            // 
            this.btnMinMax.Location = new System.Drawing.Point(12, 160);
            this.btnMinMax.Name = "btnMinMax";
            this.btnMinMax.Size = new System.Drawing.Size(90, 23);
            this.btnMinMax.TabIndex = 6;
            this.btnMinMax.Text = "Read min/Max";
            this.btnMinMax.UseVisualStyleBackColor = true;
            this.btnMinMax.Click += new System.EventHandler(this.BtnMinMax_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 189);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(90, 23);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset mi/Mx";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnHistorySave
            // 
            this.btnHistorySave.Location = new System.Drawing.Point(13, 288);
            this.btnHistorySave.Name = "btnHistorySave";
            this.btnHistorySave.Size = new System.Drawing.Size(118, 23);
            this.btnHistorySave.TabIndex = 9;
            this.btnHistorySave.Text = "Save History from";
            this.btnHistorySave.UseVisualStyleBackColor = true;
            this.btnHistorySave.Click += new System.EventHandler(this.BtnHistorySave_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(137, 289);
            this.dateTimePicker1.MinDate = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(115, 20);
            this.dateTimePicker1.TabIndex = 10;
            // 
            // btnReadTh
            // 
            this.btnReadTh.Location = new System.Drawing.Point(13, 218);
            this.btnReadTh.Name = "btnReadTh";
            this.btnReadTh.Size = new System.Drawing.Size(90, 23);
            this.btnReadTh.TabIndex = 8;
            this.btnReadTh.Text = "READ All";
            this.btnReadTh.UseVisualStyleBackColor = true;
            this.btnReadTh.Click += new System.EventHandler(this.BtnReadTh_Click);
            // 
            // lblHistory
            // 
            this.lblHistory.AutoSize = true;
            this.lblHistory.Location = new System.Drawing.Point(258, 293);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(70, 13);
            this.lblHistory.TabIndex = 10;
            this.lblHistory.Text = " to file History";
            // 
            // btnHistoryRead
            // 
            this.btnHistoryRead.Location = new System.Drawing.Point(137, 315);
            this.btnHistoryRead.Name = "btnHistoryRead";
            this.btnHistoryRead.Size = new System.Drawing.Size(115, 23);
            this.btnHistoryRead.TabIndex = 13;
            this.btnHistoryRead.Text = "Open History";
            this.btnHistoryRead.UseVisualStyleBackColor = true;
            this.btnHistoryRead.Click += new System.EventHandler(this.BtnViewLog_Click);
            // 
            // btnGRAPH
            // 
            this.btnGRAPH.Image = ((System.Drawing.Image)(resources.GetObject("btnGRAPH.Image")));
            this.btnGRAPH.Location = new System.Drawing.Point(13, 249);
            this.btnGRAPH.Name = "btnGRAPH";
            this.btnGRAPH.Size = new System.Drawing.Size(90, 23);
            this.btnGRAPH.TabIndex = 14;
            this.btnGRAPH.Text = "GRAPH";
            this.btnGRAPH.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGRAPH.UseVisualStyleBackColor = true;
            this.btnGRAPH.Click += new System.EventHandler(this.GRAPH_Click);
            // 
            // btnMeans
            // 
            this.btnMeans.Location = new System.Drawing.Point(53, 316);
            this.btnMeans.Name = "btnMeans";
            this.btnMeans.Size = new System.Drawing.Size(75, 23);
            this.btnMeans.TabIndex = 12;
            this.btnMeans.Text = "MEANS";
            this.btnMeans.UseVisualStyleBackColor = true;
            this.btnMeans.Click += new System.EventHandler(this.BtnMean_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.SystemColors.Info;
            this.btnOpen.Location = new System.Drawing.Point(12, 70);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(90, 23);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "OPEN COM";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Info;
            this.btnClose.Location = new System.Drawing.Point(12, 99);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "CLOSE COM";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // checkBoxValues
            // 
            this.checkBoxValues.AutoSize = true;
            this.checkBoxValues.Location = new System.Drawing.Point(32, 320);
            this.checkBoxValues.Name = "checkBoxValues";
            this.checkBoxValues.Size = new System.Drawing.Size(15, 14);
            this.checkBoxValues.TabIndex = 11;
            this.checkBoxValues.UseVisualStyleBackColor = true;
            // 
            // comboBoxCom
            // 
            this.comboBoxCom.FormattingEnabled = true;
            this.comboBoxCom.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8"});
            this.comboBoxCom.Location = new System.Drawing.Point(13, 43);
            this.comboBoxCom.Name = "comboBoxCom";
            this.comboBoxCom.Size = new System.Drawing.Size(90, 21);
            this.comboBoxCom.TabIndex = 2;
            this.comboBoxCom.Text = "COM1";
            // 
            // WS2300
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 374);
            this.Controls.Add(this.comboBoxCom);
            this.Controls.Add(this.checkBoxValues);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnMeans);
            this.Controls.Add(this.btnGRAPH);
            this.Controls.Add(this.btnHistoryRead);
            this.Controls.Add(this.lblHistory);
            this.Controls.Add(this.btnReadTh);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnHistorySave);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnMinMax);
            this.Controls.Add(this.textBoxReceive);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.lblIntTemp);
            this.Controls.Add(this.btnInTmp);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "WS2300";
            this.Text = "Form1";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button btnInTmp;
        private System.Windows.Forms.Label lblIntTemp;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.TextBox textBoxReceive;
        private System.Windows.Forms.Button btnMinMax;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnHistorySave;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnReadTh;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.Button btnHistoryRead;
        private System.Windows.Forms.Button btnGRAPH;
        private System.Windows.Forms.Button btnMeans;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox checkBoxValues;
        private System.Windows.Forms.ComboBox comboBoxCom;
    }
}

