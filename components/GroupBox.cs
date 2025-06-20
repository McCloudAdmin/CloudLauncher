using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.components
{
    public partial class GroupBox : Guna.UI2.WinForms.Guna2GroupBox
    {
        public GroupBox()
        {
            InitializeComponent();
        }

        public GroupBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
