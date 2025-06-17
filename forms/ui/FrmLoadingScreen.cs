using CloudLauncher.Components;
using CloudLauncher.forms.auth;
using CloudLauncher.utils;

namespace CloudLauncher
{
    public partial class FrmLoadingScreen : Form
    {
        public FrmLoadingScreen()
        {
            InitializeComponent();
            UIStyler.ApplyStyles(this, true);
            Logger.Info("Worker got a new job to render form: " + Name);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.Info("Form rendered: " + Name);
            panel2.Width = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel2.Width += 5;

            if (panel2.Width >= panel1.Width)
            {
                timer1.Stop();

                FrmLogin loginForm = new FrmLogin();
                loginForm.Show();
                Alert.Warning("You are using a development version!");
                this.Hide();
                Logger.Info("Loading complete, proceeding to main application...");
            }
        }

        private void lblAppName_Click(object sender, EventArgs e)
        {

        }
    }
}
