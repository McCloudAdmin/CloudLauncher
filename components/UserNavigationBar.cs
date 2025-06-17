using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CloudLauncher.utils;

namespace CloudLauncher.components
{
    public partial class UserNavigationBar : UserControl
    {
        public UserNavigationBar()
        {
            InitializeComponent();
            Logger.Info("Worker got a new job to render component: "+Name);
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Program.stop();
        }

        private void lblMinimize_Click(object sender, EventArgs e)
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.WindowState = FormWindowState.Minimized;
            }
        }

        private void UserNavigationBar_Load(object sender, EventArgs e)
        {
            Logger.Info("UI component rendered: "+Name);
        }
    }
}
