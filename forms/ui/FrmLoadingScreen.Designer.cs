namespace CloudLauncher
{
    partial class FrmLoadingScreen
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLoadingScreen));
            userNavigationBar1 = new CloudLauncher.components.UserNavigationBar();
            pictureBox1 = new PictureBox();
            Elipse = new Guna.UI2.WinForms.Guna2Elipse(components);
            panel1 = new Panel();
            panel2 = new Panel();
            timer1 = new System.Windows.Forms.Timer(components);
            lblAppName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // userNavigationBar1
            // 
            userNavigationBar1.BackColor = Color.FromArgb(25, 25, 25);
            userNavigationBar1.Dock = DockStyle.Top;
            userNavigationBar1.Location = new Point(0, 0);
            userNavigationBar1.Name = "userNavigationBar1";
            userNavigationBar1.Size = new Size(864, 35);
            userNavigationBar1.TabIndex = 0;
            userNavigationBar1.UseWaitCursor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(369, 51);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(126, 132);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.UseWaitCursor = true;
            // 
            // Elipse
            // 
            Elipse.BorderRadius = 24;
            Elipse.TargetControl = this;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 298);
            panel1.Name = "panel1";
            panel1.Size = new Size(864, 23);
            panel1.TabIndex = 2;
            panel1.UseWaitCursor = true;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(255, 87, 34);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(124, 23);
            panel2.TabIndex = 0;
            panel2.UseWaitCursor = true;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1;
            timer1.Tick += timer1_Tick;
            // 
            // lblAppName
            // 
            lblAppName.Anchor = AnchorStyles.Left;
            lblAppName.AutoSize = false;
            lblAppName.BackColor = Color.Transparent;
            lblAppName.Font = new Font("Trebuchet MS", 24F, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.IsContextMenuEnabled = false;
            lblAppName.IsSelectionEnabled = false;
            lblAppName.Location = new Point(283, 204);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(299, 66);
            lblAppName.TabIndex = 4;
            lblAppName.TabStop = false;
            lblAppName.Text = "CloudLauncher";
            lblAppName.TextAlignment = ContentAlignment.MiddleCenter;
            lblAppName.UseWaitCursor = true;
            lblAppName.Click += lblAppName_Click;
            // 
            // FrmLoadingScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 15, 15);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(864, 321);
            Controls.Add(lblAppName);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(userNavigationBar1);
            Cursor = Cursors.AppStarting;
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmLoadingScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CloudLauncher";
            UseWaitCursor = true;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private components.UserNavigationBar userNavigationBar1;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2Elipse Elipse;
        private Panel panel1;
        private Panel panel2;
        private System.Windows.Forms.Timer timer1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAppName;
    }
}
