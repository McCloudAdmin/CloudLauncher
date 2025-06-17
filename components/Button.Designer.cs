namespace CloudLauncher.components
{
    partial class Button
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
            // Button
            // 
            Animated = true;
            BackColor = Color.Transparent;
            BorderRadius = 6;
            DialogResult = DialogResult.None;
            DisabledState.BorderColor = Color.DarkGray;
            DisabledState.CustomBorderColor = Color.DarkGray;
            DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            FillColor = Color.FromArgb(255, 87, 34);
            Font = new Font("Trebuchet MS", 11F, FontStyle.Bold);
            HoverState.FillColor = Color.FromArgb(231, 80, 34);
            IndicateFocus = true;
            PressedColor = Color.FromArgb(231, 80, 34);
            ShadowDecoration.CustomizableEdges = customizableEdges1;
            TabStop = false;
            Text = "MyButton";
            PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
