namespace CloudLauncher.forms.ui
{
    partial class FrmInstanceSelector
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Elipse = new Guna.UI2.WinForms.Guna2Elipse(components);
            userNavigationBar1 = new CloudLauncher.components.UserNavigationBar();
            listViewInstances = new ListView();
            columnName = new ColumnHeader();
            columnDirectory = new ColumnHeader();
            imageList = new ImageList(components);
            buttonPanel = new Panel();
            cbRememberInstance = new CloudLauncher.components.CheckBox(components);
            btnSelect = new CloudLauncher.components.Button(components);
            btnCancel = new CloudLauncher.components.Button(components);
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // Elipse
            // 
            Elipse.BorderRadius = 10;
            Elipse.TargetControl = this;
            // 
            // userNavigationBar1
            // 
            userNavigationBar1.BackColor = Color.FromArgb(24, 25, 28);
            userNavigationBar1.Dock = DockStyle.Top;
            userNavigationBar1.Location = new Point(0, 0);
            userNavigationBar1.Name = "userNavigationBar1";
            userNavigationBar1.Size = new Size(600, 38);
            userNavigationBar1.TabIndex = 0;
            // 
            // listViewInstances
            // 
            listViewInstances.BackColor = Color.FromArgb(32, 34, 37);
            listViewInstances.Columns.AddRange(new ColumnHeader[] { columnName, columnDirectory });
            listViewInstances.Dock = DockStyle.Fill;
            listViewInstances.ForeColor = Color.White;
            listViewInstances.FullRowSelect = true;
            listViewInstances.LargeImageList = imageList;
            listViewInstances.Location = new Point(0, 38);
            listViewInstances.MultiSelect = false;
            listViewInstances.Name = "listViewInstances";
            listViewInstances.Size = new Size(600, 322);
            listViewInstances.SmallImageList = imageList;
            listViewInstances.TabIndex = 1;
            listViewInstances.UseCompatibleStateImageBehavior = false;
            listViewInstances.View = View.Details;
            listViewInstances.SelectedIndexChanged += ListViewInstances_SelectedIndexChanged;
            listViewInstances.DoubleClick += ListViewInstances_DoubleClick;
            // 
            // columnName
            // 
            columnName.Text = "Name";
            columnName.Width = 200;
            // 
            // columnDirectory
            // 
            columnDirectory.Text = "Directory";
            columnDirectory.Width = 350;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(48, 48);
            imageList.TransparentColor = Color.Transparent;
            // 
            // buttonPanel
            // 
            buttonPanel.BackColor = Color.FromArgb(24, 25, 28);
            buttonPanel.Controls.Add(cbRememberInstance);
            buttonPanel.Controls.Add(btnSelect);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Location = new Point(0, 360);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(600, 40);
            buttonPanel.TabIndex = 2;
            // 
            // cbRememberInstance
            // 
            cbRememberInstance.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbRememberInstance.Animated = true;
            cbRememberInstance.AutoSize = true;
            cbRememberInstance.BackColor = Color.Transparent;
            cbRememberInstance.Checked = true;
            cbRememberInstance.CheckedState.BorderRadius = 0;
            cbRememberInstance.CheckedState.BorderThickness = 0;
            cbRememberInstance.CheckState = CheckState.Checked;
            cbRememberInstance.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            cbRememberInstance.ForeColor = Color.White;
            cbRememberInstance.Location = new Point(261, 7);
            cbRememberInstance.Name = "cbRememberInstance";
            cbRememberInstance.Size = new Size(169, 24);
            cbRememberInstance.TabIndex = 3;
            cbRememberInstance.Text = "Remember Instance";
            cbRememberInstance.UncheckedState.BorderRadius = 0;
            cbRememberInstance.UncheckedState.BorderThickness = 0;
            cbRememberInstance.UseVisualStyleBackColor = false;
            // 
            // btnSelect
            // 
            btnSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelect.Animated = true;
            btnSelect.BackColor = Color.Transparent;
            btnSelect.BorderRadius = 6;
            btnSelect.CustomizableEdges = customizableEdges1;
            btnSelect.Enabled = false;
            btnSelect.FillColor = Color.FromArgb(255, 87, 34);
            btnSelect.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            btnSelect.ForeColor = Color.White;
            btnSelect.IndicateFocus = true;
            btnSelect.Location = new Point(436, 7);
            btnSelect.Name = "btnSelect";
            btnSelect.PressedColor = Color.FromArgb(231, 80, 34);
            btnSelect.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnSelect.Size = new Size(157, 23);
            btnSelect.TabIndex = 0;
            btnSelect.TabStop = false;
            btnSelect.Text = "Select";
            btnSelect.Click += BtnSelect_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Animated = true;
            btnCancel.BackColor = Color.FromArgb(32, 34, 37);
            btnCancel.BorderRadius = 6;
            btnCancel.CustomizableEdges = customizableEdges3;
            btnCancel.FillColor = Color.FromArgb(255, 87, 34);
            btnCancel.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.IndicateFocus = true;
            btnCancel.Location = new Point(8, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.PressedColor = Color.FromArgb(231, 80, 34);
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnCancel.Size = new Size(129, 23);
            btnCancel.TabIndex = 1;
            btnCancel.TabStop = false;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;
            // 
            // FrmInstanceSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 34, 37);
            ClientSize = new Size(600, 400);
            Controls.Add(listViewInstances);
            Controls.Add(buttonPanel);
            Controls.Add(userNavigationBar1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmInstanceSelector";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Select Instance";
            Load += FrmInstanceSelector_Load;
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse Elipse;
        private CloudLauncher.components.UserNavigationBar userNavigationBar1;
        private ListView listViewInstances;
        private ImageList imageList;
        private Panel buttonPanel;
        private CloudLauncher.components.Button btnSelect;
        private CloudLauncher.components.Button btnCancel;
        private CloudLauncher.components.CheckBox cbRememberInstance;
        private ColumnHeader columnName;
        private ColumnHeader columnDirectory;
    }
} 