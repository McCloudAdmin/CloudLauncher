using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.components
{
    public partial class TrackBar : Guna.UI2.WinForms.Guna2TrackBar
    {
        public TrackBar()
        {
            InitializeComponent();
        }

        public TrackBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
