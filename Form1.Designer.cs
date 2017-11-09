namespace BK2BT
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bKBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadBKAlphaBin = new System.Windows.Forms.ToolStripMenuItem();
            this.bTBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bTAlphaBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveBinMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveBinAs = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAlphaBinAs = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new OpenTK.GLControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightGray;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(698, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBinToolStripMenuItem,
            this.SaveBinMenu});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // openBinToolStripMenuItem
            // 
            this.openBinToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bKBinToolStripMenuItem,
            this.LoadBKAlphaBin,
            this.bTBinToolStripMenuItem,
            this.bTAlphaBinToolStripMenuItem});
            this.openBinToolStripMenuItem.Name = "openBinToolStripMenuItem";
            this.openBinToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openBinToolStripMenuItem.Text = "Open Bin";
            this.openBinToolStripMenuItem.Click += new System.EventHandler(this.openBinToolStripMenuItem_Click);
            // 
            // bKBinToolStripMenuItem
            // 
            this.bKBinToolStripMenuItem.Name = "bKBinToolStripMenuItem";
            this.bKBinToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.bKBinToolStripMenuItem.Text = "BK Bin";
            this.bKBinToolStripMenuItem.Click += new System.EventHandler(this.bKBinToolStripMenuItem_Click);
            // 
            // LoadBKAlphaBin
            // 
            this.LoadBKAlphaBin.Enabled = false;
            this.LoadBKAlphaBin.Name = "LoadBKAlphaBin";
            this.LoadBKAlphaBin.Size = new System.Drawing.Size(142, 22);
            this.LoadBKAlphaBin.Text = "BK Alpha Bin";
            this.LoadBKAlphaBin.Click += new System.EventHandler(this.bKAlphaBinToolStripMenuItem_Click);
            // 
            // bTBinToolStripMenuItem
            // 
            this.bTBinToolStripMenuItem.Enabled = false;
            this.bTBinToolStripMenuItem.Name = "bTBinToolStripMenuItem";
            this.bTBinToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.bTBinToolStripMenuItem.Text = "BT Bin";
            // 
            // bTAlphaBinToolStripMenuItem
            // 
            this.bTAlphaBinToolStripMenuItem.Enabled = false;
            this.bTAlphaBinToolStripMenuItem.Name = "bTAlphaBinToolStripMenuItem";
            this.bTAlphaBinToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.bTAlphaBinToolStripMenuItem.Text = "BT Alpha Bin";
            // 
            // SaveBinMenu
            // 
            this.SaveBinMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveBinAs,
            this.SaveAlphaBinAs});
            this.SaveBinMenu.Enabled = false;
            this.SaveBinMenu.Name = "SaveBinMenu";
            this.SaveBinMenu.Size = new System.Drawing.Size(152, 22);
            this.SaveBinMenu.Text = "Save Bin...";
            this.SaveBinMenu.Click += new System.EventHandler(this.saveBinForBKToolStripMenuItem_Click);
            // 
            // SaveBinAs
            // 
            this.SaveBinAs.Name = "SaveBinAs";
            this.SaveBinAs.Size = new System.Drawing.Size(177, 22);
            this.SaveBinAs.Text = "Save Bin As...";
            this.SaveBinAs.Click += new System.EventHandler(this.bKBinToolStripMenuItem1_Click);
            // 
            // SaveAlphaBinAs
            // 
            this.SaveAlphaBinAs.Enabled = false;
            this.SaveAlphaBinAs.Name = "SaveAlphaBinAs";
            this.SaveAlphaBinAs.Size = new System.Drawing.Size(177, 22);
            this.SaveAlphaBinAs.Text = "Save Alpha Bin As...";
            this.SaveAlphaBinAs.Click += new System.EventHandler(this.SaveAlphaBinAs_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.controlsToolStripMenuItem.Text = "Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.controlsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(119, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 411);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(698, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusLabel.Text = "Ready";
            this.StatusLabel.Click += new System.EventHandler(this.StatusLabel_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.MaximumSize = new System.Drawing.Size(3840, 2160);
            this.panel1.MinimumSize = new System.Drawing.Size(698, 390);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(698, 390);
            this.panel1.TabIndex = 4;
            this.panel1.VSync = true;
            this.panel1.Load += new System.EventHandler(this.panel1_Load);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 433);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "BK to BT Bin";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveBinMenu;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private OpenTK.GLControl panel1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bKBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadBKAlphaBin;
        private System.Windows.Forms.ToolStripMenuItem bTBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bTAlphaBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveBinAs;
        private System.Windows.Forms.ToolStripMenuItem SaveAlphaBinAs;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
    }
}

