using Salaros.Configuration;
using System.IO;

namespace CloudLauncher.utils
{
    public class LauncherConfig
    {
        private static ConfigParser _config;
        private static readonly string ConfigPath = Path.Combine(Program.appWorkDir, "launcher.conf");
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            lock (_lock)
            {
                if (_config == null)
                {
                    // Create default config if it doesn't exist
                    if (!File.Exists(ConfigPath))
                    {
                        CreateDefaultConfig();
                    }

                    _config = new ConfigParser(ConfigPath);
                }
            }
        }

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new ConfigParser();
            
            // Launcher Settings
            defaultConfig.SetValue("Launcher", "AutoUpdate", "true");
            defaultConfig.SetValue("Launcher", "ShowChangeLog", "true");
            defaultConfig.SetValue("Launcher", "DebugMode", "false");
            

            // Save the default configuration
            defaultConfig.Save(ConfigPath);
        }

        // Generic methods to get/set values
        public static string GetValue(string section, string key, string defaultValue = "")
        {
            Initialize();
            return _config.GetValue(section, key, defaultValue);
        }

        public static void SetValue(string section, string key, string value)
        {
            Initialize();
            _config.SetValue(section, key, value);
            _config.Save();
        }

        public static bool GetBool(string section, string key, bool defaultValue = false)
        {
            return bool.TryParse(GetValue(section, key, defaultValue.ToString()), out bool result) ? result : defaultValue;
        }

        public static int GetInt(string section, string key, int defaultValue = 0)
        {
            return int.TryParse(GetValue(section, key, defaultValue.ToString()), out int result) ? result : defaultValue;
        }

        public static void SetBool(string section, string key, bool value)
        {
            SetValue(section, key, value.ToString().ToLower());
        }

        public static void SetInt(string section, string key, int value)
        {
            SetValue(section, key, value.ToString());
        }
    }
}