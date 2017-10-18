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
            this.saveBinForBKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBinForBTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Preview = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightGray;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(614, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBinToolStripMenuItem,
            this.saveBinForBKToolStripMenuItem,
            this.saveBinForBTToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // openBinToolStripMenuItem
            // 
            this.openBinToolStripMenuItem.Name = "openBinToolStripMenuItem";
            this.openBinToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openBinToolStripMenuItem.Text = "Open Bin";
            this.openBinToolStripMenuItem.Click += new System.EventHandler(this.openBinToolStripMenuItem_Click);
            // 
            // saveBinForBKToolStripMenuItem
            // 
            this.saveBinForBKToolStripMenuItem.Enabled = false;
            this.saveBinForBKToolStripMenuItem.Name = "saveBinForBKToolStripMenuItem";
            this.saveBinForBKToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBinForBKToolStripMenuItem.Text = "Save Bin for Kazooie";
            // 
            // saveBinForBTToolStripMenuItem
            // 
            this.saveBinForBTToolStripMenuItem.Enabled = false;
            this.saveBinForBTToolStripMenuItem.Name = "saveBinForBTToolStripMenuItem";
            this.saveBinForBTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBinForBTToolStripMenuItem.Text = "Save Bin for Tooie";
            this.saveBinForBTToolStripMenuItem.Click += new System.EventHandler(this.saveBinForBTToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 369);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(614, 22);
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
            // Preview
            // 
            this.Preview.AutoSize = true;
            this.Preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 70F);
            this.Preview.Location = new System.Drawing.Point(122, 33);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(379, 214);
            this.Preview.TabIndex = 2;
            this.Preview.Text = "Preview\r\nWIP";
            this.Preview.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 291);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(577, 46);
            this.label1.TabIndex = 3;
            this.label1.Text = "(Todo after conversion stability)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 391);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Preview);
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
        private System.Windows.Forms.ToolStripMenuItem saveBinForBKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBinForBTToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label Preview;
        private System.Windows.Forms.Label label1;
    }
}

