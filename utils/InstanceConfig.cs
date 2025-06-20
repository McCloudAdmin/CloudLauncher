using Salaros.Configuration;
using System.IO;

namespace CloudLauncher.utils
{
    public class InstanceConfig
    {
        private ConfigParser _config;
        public string ConfigPath { get; private set; }
        private readonly object _lock = new object();

        public InstanceConfig(string instancePath)
        {
            ConfigPath = instancePath;
            lock (_lock)
            {
                if (_config == null)
                {
                    _config = new ConfigParser(instancePath);
                }
            }
        }

        // Generic methods to get/set values
        public string GetValue(string section, string key, string defaultValue = "")
        {
            return _config.GetValue(section, key, defaultValue);
        }

        public void SetValue(string section, string key, string value)
        {
            _config.SetValue(section, key, value);
            _config.Save();
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            return bool.TryParse(GetValue(section, key, defaultValue.ToString()), out bool result) ? result : defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            return int.TryParse(GetValue(section, key, defaultValue.ToString()), out int result) ? result : defaultValue;
        }

        public void SetBool(string section, string key, bool value)
        {
            SetValue(section, key, value.ToString().ToLower());
        }

        public void SetInt(string section, string key, int value)
        {
            SetValue(section, key, value.ToString());
        }
    }
}