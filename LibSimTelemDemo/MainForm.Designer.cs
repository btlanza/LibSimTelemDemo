namespace LibSimTelemDemo
{
    partial class MainForm
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
            this.cbxSelectGame = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.chkAutoDetect = new System.Windows.Forms.CheckBox();
            this.grpGetGame = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.grpGetGame.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxSelectGame
            // 
            this.cbxSelectGame.FormattingEnabled = true;
            this.cbxSelectGame.Location = new System.Drawing.Point(6, 19);
            this.cbxSelectGame.Name = "cbxSelectGame";
            this.cbxSelectGame.Size = new System.Drawing.Size(273, 21);
            this.cbxSelectGame.TabIndex = 0;
            this.cbxSelectGame.SelectedIndexChanged += new System.EventHandler(this.cbxSelectGame_SelectedIndexChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(285, 19);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(69, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // chkAutoDetect
            // 
            this.chkAutoDetect.AutoSize = true;
            this.chkAutoDetect.Checked = true;
            this.chkAutoDetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoDetect.Location = new System.Drawing.Point(6, 46);
            this.chkAutoDetect.Name = "chkAutoDetect";
            this.chkAutoDetect.Size = new System.Drawing.Size(157, 17);
            this.chkAutoDetect.TabIndex = 2;
            this.chkAutoDetect.Text = "Detect game automatically?";
            this.chkAutoDetect.UseVisualStyleBackColor = true;
            this.chkAutoDetect.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);
            // 
            // grpGetGame
            // 
            this.grpGetGame.Controls.Add(this.cbxSelectGame);
            this.grpGetGame.Controls.Add(this.chkAutoDetect);
            this.grpGetGame.Controls.Add(this.btnStart);
            this.grpGetGame.Location = new System.Drawing.Point(12, 12);
            this.grpGetGame.Name = "grpGetGame";
            this.grpGetGame.Size = new System.Drawing.Size(360, 68);
            this.grpGetGame.TabIndex = 3;
            this.grpGetGame.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(297, 226);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.grpGetGame);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "LibSimTelem Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpGetGame.ResumeLayout(false);
            this.grpGetGame.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxSelectGame;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox chkAutoDetect;
        private System.Windows.Forms.GroupBox grpGetGame;
        private System.Windows.Forms.Button btnStop;
        private InterpolatedPictureBox picSteeringWheel;
        private InterpolatedPictureBox picPedals;
        private InterpolatedPictureBox picSpeedRPM;
    }
}

