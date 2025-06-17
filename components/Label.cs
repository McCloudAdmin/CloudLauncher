using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.components
{
    public partial class Label : Guna.UI2.WinForms.Guna2HtmlLabel
    {
        public Label()
        {
            InitializeComponent();
        }

        public Label(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
