namespace CloudLauncher.components
{
    partial class TextBox
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
            // TextBox
            // 
            Animated = true;
            BackColor = Color.Transparent;
            BorderColor = Color.Transparent;
            BorderRadius = 6;
            CustomizableEdges = customizableEdges1;
            DefaultText = "";
            DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            FillColor = Color.FromArgb(25, 25, 25);
            FocusedState.BorderColor = Color.FromArgb(255, 87, 34);
            Font = new Font("Trebuchet MS", 9F);
            ForeColor = Color.White;
            HoverState.BorderColor = Color.FromArgb(255, 87, 34);
            Name = "TextBox";
            PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            PlaceholderText = "";
            ShadowDecoration.CustomizableEdges = customizableEdges1;
            PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
