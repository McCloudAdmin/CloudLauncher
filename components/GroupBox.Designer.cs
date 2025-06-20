namespace CloudLauncher.components
{
    partial class GroupBox
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
            // GroupBox
            //
            BackColor = System.Drawing.Color.Transparent;
            BorderColor = System.Drawing.Color.Transparent;
            BorderRadius = 6;
            CustomBorderColor = System.Drawing.Color.FromArgb(231, 80, 34);
            CustomizableEdges = customizableEdges1;
            FillColor = System.Drawing.Color.FromArgb(10, 10, 10);
            Font = new System.Drawing.Font("Trebuchet MS", 11F, System.Drawing.FontStyle.Bold);
            ForeColor = System.Drawing.Color.White;
            ShadowDecoration.CustomizableEdges = customizableEdges1;
            Size = new System.Drawing.Size(300, 200);
            TabIndex = 3;
            Text = "GroupBox";
            UseTransparentBackground = true;
            ResumeLayout(false);
        }

        #endregion
    }
}
