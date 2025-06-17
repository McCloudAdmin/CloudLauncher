namespace CloudLauncher.forms.auth
{
    partial class FrmLogin
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            Elipse = new Guna.UI2.WinForms.Guna2Elipse(components);
            userNavigationBar1 = new CloudLauncher.components.UserNavigationBar();
            btnLogin = new CloudLauncher.components.Button(components);
            btnRemove = new CloudLauncher.components.Button(components);
            btnAddAccount = new CloudLauncher.components.Button(components);
            txtOfflineName = new CloudLauncher.components.TextBox(components);
            dropAccounts = new CloudLauncher.components.DropDown(components);
            lblSelectAccount = new CloudLauncher.components.Label(components);
            lblOfflineUsername = new CloudLauncher.components.Label(components);
            SuspendLayout();
            // 
            // Elipse
            // 
            Elipse.BorderRadius = 24;
            Elipse.TargetControl = this;
            // 
            // userNavigationBar1
            // 
            userNavigationBar1.BackColor = Color.FromArgb(25, 25, 25);
            userNavigationBar1.Dock = DockStyle.Top;
            userNavigationBar1.Location = new Point(0, 0);
            userNavigationBar1.Name = "userNavigationBar1";
            userNavigationBar1.Size = new Size(342, 37);
            userNavigationBar1.TabIndex = 6;
            // 
            // btnLogin
            // 
            btnLogin.Animated = true;
            btnLogin.BackColor = Color.Transparent;
            btnLogin.BorderRadius = 6;
            btnLogin.CustomizableEdges = customizableEdges7;
            btnLogin.DisabledState.BorderColor = Color.DarkGray;
            btnLogin.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLogin.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLogin.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLogin.FillColor = Color.FromArgb(255, 87, 34);
            btnLogin.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.HoverState.FillColor = Color.FromArgb(231, 80, 34);
            btnLogin.IndicateFocus = true;
            btnLogin.Location = new Point(12, 190);
            btnLogin.Name = "btnLogin";
            btnLogin.PressedColor = Color.FromArgb(231, 80, 34);
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnLogin.Size = new Size(188, 45);
            btnLogin.TabIndex = 7;
            btnLogin.TabStop = false;
            btnLogin.Text = "Login";
            btnLogin.Click += btnLogin_Click;
            // 
            // btnRemove
            // 
            btnRemove.Animated = true;
            btnRemove.BackColor = Color.Transparent;
            btnRemove.BorderRadius = 6;
            btnRemove.CustomizableEdges = customizableEdges5;
            btnRemove.DisabledState.BorderColor = Color.DarkGray;
            btnRemove.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRemove.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRemove.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRemove.FillColor = Color.FromArgb(255, 87, 34);
            btnRemove.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            btnRemove.ForeColor = Color.White;
            btnRemove.HoverState.FillColor = Color.FromArgb(231, 80, 34);
            btnRemove.IndicateFocus = true;
            btnRemove.Location = new Point(206, 190);
            btnRemove.Name = "btnRemove";
            btnRemove.PressedColor = Color.FromArgb(231, 80, 34);
            btnRemove.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnRemove.Size = new Size(127, 45);
            btnRemove.TabIndex = 8;
            btnRemove.TabStop = false;
            btnRemove.Text = "Remove";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnAddAccount
            // 
            btnAddAccount.Animated = true;
            btnAddAccount.BackColor = Color.Transparent;
            btnAddAccount.BorderRadius = 6;
            btnAddAccount.CustomizableEdges = customizableEdges3;
            btnAddAccount.DisabledState.BorderColor = Color.DarkGray;
            btnAddAccount.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddAccount.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddAccount.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddAccount.FillColor = Color.FromArgb(255, 87, 34);
            btnAddAccount.Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            btnAddAccount.ForeColor = Color.White;
            btnAddAccount.HoverState.FillColor = Color.FromArgb(231, 80, 34);
            btnAddAccount.IndicateFocus = true;
            btnAddAccount.Location = new Point(12, 241);
            btnAddAccount.Name = "btnAddAccount";
            btnAddAccount.PressedColor = Color.FromArgb(231, 80, 34);
            btnAddAccount.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnAddAccount.Size = new Size(322, 45);
            btnAddAccount.TabIndex = 9;
            btnAddAccount.TabStop = false;
            btnAddAccount.Text = "Add Premium Account";
            btnAddAccount.Click += btnAddAccount_Click;
            // 
            // txtOfflineName
            // 
            txtOfflineName.Animated = true;
            txtOfflineName.BackColor = Color.Transparent;
            txtOfflineName.BorderColor = Color.Transparent;
            txtOfflineName.BorderRadius = 6;
            txtOfflineName.CustomizableEdges = customizableEdges2;
            txtOfflineName.DefaultText = "";
            txtOfflineName.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtOfflineName.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtOfflineName.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtOfflineName.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtOfflineName.FillColor = Color.FromArgb(25, 25, 25);
            txtOfflineName.FocusedState.BorderColor = Color.FromArgb(255, 87, 34);
            txtOfflineName.Font = new Font("Trebuchet MS", 9F);
            txtOfflineName.ForeColor = Color.White;
            txtOfflineName.HoverState.BorderColor = Color.FromArgb(255, 87, 34);
            txtOfflineName.Location = new Point(12, 137);
            txtOfflineName.Name = "txtOfflineName";
            txtOfflineName.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtOfflineName.PlaceholderText = "";
            txtOfflineName.SelectedText = "";
            txtOfflineName.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtOfflineName.Size = new Size(321, 38);
            txtOfflineName.TabIndex = 10;
            // 
            // dropAccounts
            // 
            dropAccounts.BackColor = Color.Transparent;
            dropAccounts.BorderColor = Color.Transparent;
            dropAccounts.BorderRadius = 6;
            dropAccounts.BorderThickness = 0;
            dropAccounts.CustomizableEdges = customizableEdges1;
            dropAccounts.DrawMode = DrawMode.OwnerDrawFixed;
            dropAccounts.DropDownStyle = ComboBoxStyle.DropDownList;
            dropAccounts.FillColor = Color.FromArgb(25, 25, 25);
            dropAccounts.FocusedColor = Color.FromArgb(231, 80, 34);
            dropAccounts.FocusedState.BorderColor = Color.FromArgb(231, 80, 34);
            dropAccounts.Font = new Font("Trebuchet MS", 9F);
            dropAccounts.ForeColor = Color.White;
            dropAccounts.ItemHeight = 32;
            dropAccounts.Location = new Point(8, 67);
            dropAccounts.Margin = new Padding(4, 3, 4, 3);
            dropAccounts.MaxDropDownItems = 12;
            dropAccounts.Name = "dropAccounts";
            dropAccounts.RightToLeft = RightToLeft.No;
            dropAccounts.ShadowDecoration.CustomizableEdges = customizableEdges1;
            dropAccounts.Size = new Size(325, 38);
            dropAccounts.TabIndex = 0;
            dropAccounts.TabStop = false;
            dropAccounts.SelectedIndexChanged += dropAccounts_SelectedIndexChanged;
            // 
            // lblSelectAccount
            // 
            lblSelectAccount.AutoSize = false;
            lblSelectAccount.BackColor = Color.Transparent;
            lblSelectAccount.Font = new Font("Trebuchet MS", 10F);
            lblSelectAccount.ForeColor = Color.White;
            lblSelectAccount.Location = new Point(12, 43);
            lblSelectAccount.Name = "lblSelectAccount";
            lblSelectAccount.Size = new Size(321, 20);
            lblSelectAccount.TabIndex = 11;
            lblSelectAccount.Text = "Select Account";
            // 
            // lblOfflineUsername
            // 
            lblOfflineUsername.AutoSize = false;
            lblOfflineUsername.BackColor = Color.Transparent;
            lblOfflineUsername.Font = new Font("Trebuchet MS", 10F);
            lblOfflineUsername.ForeColor = Color.White;
            lblOfflineUsername.Location = new Point(12, 111);
            lblOfflineUsername.Name = "lblOfflineUsername";
            lblOfflineUsername.Size = new Size(321, 20);
            lblOfflineUsername.TabIndex = 12;
            lblOfflineUsername.Text = "Offline Username";
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 15, 15);
            ClientSize = new Size(342, 299);
            Controls.Add(lblOfflineUsername);
            Controls.Add(lblSelectAccount);
            Controls.Add(dropAccounts);
            Controls.Add(txtOfflineName);
            Controls.Add(btnAddAccount);
            Controls.Add(btnRemove);
            Controls.Add(btnLogin);
            Controls.Add(userNavigationBar1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CloudLauncher";
            ResumeLayout(false);

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Elipse Elipse;
        private components.UserNavigationBar userNavigationBar1;
        private components.Button btnAddAccount;
        private components.Button btnRemove;
        private components.Button btnLogin;
        private components.TextBox txtOfflineName;
        private components.DropDown dropAccounts;
        private components.Label lblOfflineUsername;
        private components.Label lblSelectAccount;
    }
}