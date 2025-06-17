using CloudLauncher.utils;
using System.ComponentModel;

namespace CloudLauncher.components
{
    public partial class Button : Guna.UI2.WinForms.Guna2Button
    {
        public Button()
        {
            InitializeComponent();
        }

        public Button(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}