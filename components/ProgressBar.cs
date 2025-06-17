using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.components
{
    public partial class ProgressBar : Guna.UI2.WinForms.Guna2ProgressBar
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        public ProgressBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
