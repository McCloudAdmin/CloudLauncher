namespace CloudLauncher.components
{
    partial class CheckBox
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
            SuspendLayout();
            // 
            // CheckBox
            // 
            Animated = true;
            BackColor = Color.Transparent;
            CheckedState.BorderColor = Color.FromArgb(231, 80, 34);
            CheckedState.BorderRadius = 0;
            CheckedState.BorderThickness = 0;
            CheckedState.FillColor = Color.FromArgb(231, 80, 34);
            Font = new Font("Trebuchet MS", 9F, FontStyle.Bold);
            ForeColor = Color.White;
            UncheckedState.BorderColor = Color.FromArgb(38, 38, 38);
            UncheckedState.BorderRadius = 0;
            UncheckedState.BorderThickness = 0;
            UncheckedState.FillColor = Color.FromArgb(38, 38, 38);
            UseVisualStyleBackColor = false;
            ResumeLayout(false);
        }

        #endregion
    }
}
