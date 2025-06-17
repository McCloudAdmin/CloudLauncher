using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.components
{
    public partial class DropDown : Guna.UI2.WinForms.Guna2ComboBox
    {
        public DropDown()
        {
            InitializeComponent();
        }

        public DropDown(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
