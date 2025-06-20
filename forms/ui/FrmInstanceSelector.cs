using CloudLauncher.utils;
using System.Drawing;
using System.Windows.Forms;

namespace CloudLauncher.forms.ui
{
    public partial class FrmInstanceSelector : Form
    {
        private readonly InstancesManager _instancesManager;
        private List<InstanceConfig> _instances;
        public InstanceConfig SelectedInstance { get; private set; }
        public bool IsLoggingOut { get; private set; }

        public FrmInstanceSelector(bool showRememberOption = true)
        {
            InitializeComponent();
            UIStyler.ApplyStyles(this, true);
            _instancesManager = new InstancesManager();

            // Only show remember option if requested
            cbRememberInstance.Visible = showRememberOption;
            cbRememberInstance.Checked = RegistryConfig.GetUserPreference("RememberLastInstance", true);
        }

        private void FrmInstanceSelector_Load(object sender, EventArgs e)
        {
            LoadInstances();
            SelectLastUsedInstance();
        }

        private void SelectLastUsedInstance()
        {
            try
            {
                string lastInstanceDir = RegistryConfig.GetUserPreference<string>("LastInstanceDirectory");
                if (!string.IsNullOrEmpty(lastInstanceDir))
                {
                    foreach (ListViewItem item in listViewInstances.Items)
                    {
                        var instance = (InstanceConfig)item.Tag;
                        string instanceDir = Path.GetDirectoryName(instance.ConfigPath);
                        if (instanceDir.Equals(lastInstanceDir, StringComparison.OrdinalIgnoreCase))
                        {
                            item.Selected = true;
                            listViewInstances.EnsureVisible(item.Index);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to select last used instance: {ex.Message}");
            }
        }

        private void LoadInstances()
        {
            try
            {
                _instances = _instancesManager.GetInstances();
                imageList.Images.Clear();
                listViewInstances.Items.Clear();

                foreach (var instance in _instances)
                {
                    string name = instance.GetValue("Instance", "Name", "Unnamed Instance");
                    string logoPath = instance.GetValue("Instance", "Logo", string.Empty);
                    string directory = Path.GetFileName(Path.GetDirectoryName(instance.ConfigPath));

                    Image logo;
                    if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                    {
                        logo = Image.FromFile(logoPath);
                    }
                    else
                    {
                        // Use default logo
                        logo = Image.FromFile(Path.Combine(Application.StartupPath, "assets", "logo.png"));
                    }

                    imageList.Images.Add(logo);
                    
                    ListViewItem item = new ListViewItem(new[] { name, directory });
                    item.ImageIndex = imageList.Images.Count - 1;
                    item.Tag = instance;
                    listViewInstances.Items.Add(item);
                }

                if (listViewInstances.Items.Count > 0 && listViewInstances.SelectedItems.Count == 0)
                {
                    listViewInstances.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load instances: {ex.Message}");
                Alert.Error("Failed to load Minecraft instances. Please check the logs for details.");
            }
        }

        private void ListViewInstances_DoubleClick(object sender, EventArgs e)
        {
            SelectInstance();
        }

        private void ListViewInstances_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = listViewInstances.SelectedItems.Count > 0;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            SelectInstance();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SelectInstance()
        {
            if (listViewInstances.SelectedItems.Count > 0)
            {
                SelectedInstance = (InstanceConfig)listViewInstances.SelectedItems[0].Tag;
                
                // Save the selected instance directory if remember is checked
                if (cbRememberInstance.Checked)
                {
                    string instanceDir = Path.GetDirectoryName(SelectedInstance.ConfigPath);
                    RegistryConfig.SaveUserPreference("LastInstanceDirectory", instanceDir);
                    RegistryConfig.SaveUserPreference("RememberLastInstance", true);
                    Logger.Info($"Saved last used instance directory: {instanceDir}");
                }
                else
                {
                    RegistryConfig.SaveUserPreference("RememberLastInstance", false);
                    RegistryConfig.DeleteValue("LastInstanceDirectory");
                    Logger.Info("Instance memory disabled");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
} 