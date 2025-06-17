namespace CloudLauncher.components
{
    partial class DropDown
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            SuspendLayout();
            // 
            // DropDown
            // 
            FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            RightToLeft = RightToLeft.No;
            BackColor = Color.Transparent;
            BorderColor = Color.Transparent;
            BorderRadius = 6;
            BorderThickness = 0;
            CustomizableEdges = customizableEdges1;
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            FillColor = Color.FromArgb(25, 25, 25);
            FocusedColor = Color.FromArgb(231, 80, 34);
            FocusedState.BorderColor = Color.FromArgb(231, 80, 34);
            Font = new Font("Trebuchet MS", 9F);
            ForeColor = Color.White;
            ItemHeight = 32;
            Margin = new Padding(4, 3, 4, 3);
            MaxDropDownItems = 12;
            Size = new Size(321, 38);
            TabIndex = 0;
            TabStop = false;
            ShadowDecoration.CustomizableEdges = customizableEdges1;
            ResumeLayout(false);
        }

        #endregion
    }
}
