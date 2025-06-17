namespace CloudLauncher.components
{
    partial class UserNavigationBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserNavigationBar));
            lblClose = new Guna.UI2.WinForms.Guna2HtmlLabel();
            Drag = new Guna.UI2.WinForms.Guna2DragControl(components);
            lblMinimize = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pictureBox1 = new PictureBox();
            lblAppName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lblClose
            // 
            lblClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblClose.BackColor = Color.Transparent;
            lblClose.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            lblClose.ForeColor = Color.White;
            lblClose.IsContextMenuEnabled = false;
            lblClose.IsSelectionEnabled = false;
            lblClose.Location = new Point(899, 4);
            lblClose.Name = "lblClose";
            lblClose.Size = new Size(14, 26);
            lblClose.TabIndex = 0;
            lblClose.TabStop = false;
            lblClose.Text = "X";
            lblClose.Click += lblClose_Click;
            // 
            // Drag
            // 
            Drag.DockIndicatorTransparencyValue = 0.6D;
            Drag.DragStartTransparencyValue = 0.8D;
            Drag.TargetControl = this;
            Drag.UseTransparentDrag = true;
            // 
            // lblMinimize
            // 
            lblMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMinimize.BackColor = Color.Transparent;
            lblMinimize.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            lblMinimize.ForeColor = Color.White;
            lblMinimize.IsContextMenuEnabled = false;
            lblMinimize.IsSelectionEnabled = false;
            lblMinimize.Location = new Point(871, 4);
            lblMinimize.Name = "lblMinimize";
            lblMinimize.Size = new Size(17, 26);
            lblMinimize.TabIndex = 1;
            lblMinimize.TabStop = false;
            lblMinimize.Text = "--";
            lblMinimize.Click += lblMinimize_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(5, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // lblAppName
            // 
            lblAppName.BackColor = Color.Transparent;
            lblAppName.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.IsContextMenuEnabled = false;
            lblAppName.IsSelectionEnabled = false;
            lblAppName.Location = new Point(43, 4);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(137, 26);
            lblAppName.TabIndex = 3;
            lblAppName.TabStop = false;
            lblAppName.Text = "CloudLauncher";
            // 
            // UserNavigationBar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 25);
            Controls.Add(lblAppName);
            Controls.Add(pictureBox1);
            Controls.Add(lblMinimize);
            Controls.Add(lblClose);
            Name = "UserNavigationBar";
            Size = new Size(923, 37);
            Load += UserNavigationBar_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblClose;
        private Guna.UI2.WinForms.Guna2DragControl Drag;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMinimize;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAppName;
        private PictureBox pictureBox1;
    }
}
