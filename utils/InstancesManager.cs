namespace CloudLauncher.utils
{
    public class InstancesManager
    {
        private readonly string _instancesPath;
        private List<InstanceConfig> _instances;

        public InstancesManager()
        {
            _instancesPath = Path.Combine(Program.appWorkDir, "instances");
            if (!Directory.Exists(_instancesPath))
            {
                Directory.CreateDirectory(_instancesPath);
                Logger.Warn($"Created instances directory at: {_instancesPath}");
            }
            else
            {
                Logger.Info($"Instances directory already exists at: {_instancesPath}");
                LoadInstances();
            }
        }

        private void LoadInstances()
        {
            _instances = new List<InstanceConfig>();
            string[] dirs = GetDirs();
            foreach (string dir in dirs)
            {
                Logger.Info($"Instance directory found: {dir}");
                string settingsPath = Path.Combine(dir, "settings.ini");
                if (File.Exists(settingsPath))
                {
                    Logger.Info($"Instance settings file found: {settingsPath}");
                    InstanceConfig instanceConfig = new InstanceConfig(settingsPath);
                    _instances.Add(instanceConfig);

                    Logger.Info($"Instance name: {instanceConfig.GetValue("Instance", "Name")}");
                    Logger.Info($"Instance logo: {instanceConfig.GetValue("Instance", "Logo")}");
                }
                else
                {
                    Logger.Warn($"Instance settings file not found: {settingsPath}");
                }
            }
            Logger.Info($"Found {dirs.Length} instance directories");
        }

        private string[] GetDirs()
        {
            return Directory.GetDirectories(_instancesPath);
        }

        public List<InstanceConfig> GetInstances()
        {
            if (_instances == null)
            {
                LoadInstances();
            }
            return _instances;
        }

        public void CreateInstance(string name, string logoPath = null)
        {
            string instanceDir = Path.Combine(_instancesPath, name.ToLower().Replace(" ", "_"));
            Directory.CreateDirectory(instanceDir);

            string settingsPath = Path.Combine(instanceDir, "settings.ini");
            var config = new InstanceConfig(settingsPath);
            
            config.SetValue("Instance", "Name", name);
            if (!string.IsNullOrEmpty(logoPath))
            {
                config.SetValue("Instance", "Logo", logoPath);
            }

            LoadInstances(); // Reload instances list
        }

        public void DeleteInstance(string instanceDir)
        {
            if (Directory.Exists(instanceDir))
            {
                Directory.Delete(instanceDir, true);
                LoadInstances(); // Reload instances list
            }
        }

        public InstanceConfig GetInstance(string path)
        {
            if (File.Exists(Path.Combine(path, "settings.ini")))
            {
                return new InstanceConfig(Path.Combine(path, "settings.ini"));
            }
            return null;
        }
    }
}