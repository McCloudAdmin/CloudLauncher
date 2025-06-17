using System;
using System.Drawing;
using Microsoft.Win32;

namespace CloudLauncher.utils
{
    public static class RegistryConfig
    {
        private const string REGISTRY_PATH = @"Software\MythicalSystems\CloudLauncher";
        private static readonly RegistryKey BaseKey = Registry.CurrentUser;

        static RegistryConfig()
        {
            try
            {
                // Ensure the registry path exists
                using (RegistryKey key = BaseKey.CreateSubKey(REGISTRY_PATH))
                {
                    if (key == null)
                    {
                        Logger.Error("Failed to create registry key path");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize registry: {ex.Message}");
            }
        }

        public static void SetValue(string key, object value)
        {
            try
            {
                using (RegistryKey regKey = BaseKey.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (regKey != null)
                    {
                        regKey.SetValue(key, value);
                        Logger.Debug($"Set registry value: {key} = {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to set registry value {key}: {ex.Message}");
            }
        }

        public static T GetValue<T>(string key, T defaultValue = default)
        {
            try
            {
                using (RegistryKey regKey = BaseKey.OpenSubKey(REGISTRY_PATH))
                {
                    if (regKey != null)
                    {
                        object value = regKey.GetValue(key);
                        if (value != null)
                        {
                            return (T)Convert.ChangeType(value, typeof(T));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get registry value {key}: {ex.Message}");
            }
            return defaultValue;
        }

        public static void DeleteValue(string key)
        {
            try
            {
                using (RegistryKey regKey = BaseKey.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (regKey != null)
                    {
                        regKey.DeleteValue(key, false);
                        Logger.Debug($"Deleted registry value: {key}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to delete registry value {key}: {ex.Message}");
            }
        }

        public static bool ValueExists(string key)
        {
            try
            {
                using (RegistryKey regKey = BaseKey.OpenSubKey(REGISTRY_PATH))
                {
                    if (regKey != null)
                    {
                        return regKey.GetValue(key) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to check registry value {key}: {ex.Message}");
            }
            return false;
        }

        public static void DeleteAllValues()
        {
            try
            {
                BaseKey.DeleteSubKeyTree(REGISTRY_PATH);
                Logger.Info("Deleted all registry values");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to delete all registry values: {ex.Message}");
            }
        }

        // Window Management
        public static void SaveWindowPosition(string formName, int x, int y)
        {
            SetValue($"{formName}_X", x);
            SetValue($"{formName}_Y", y);
        }

        public static Point GetWindowPosition(string formName, Point defaultPosition)
        {
            int x = GetValue($"{formName}_X", defaultPosition.X);
            int y = GetValue($"{formName}_Y", defaultPosition.Y);
            return new Point(x, y);
        }

        public static void SaveWindowSize(string formName, int width, int height)
        {
            SetValue($"{formName}_Width", width);
            SetValue($"{formName}_Height", height);
        }

        public static Size GetWindowSize(string formName, Size defaultSize)
        {
            int width = GetValue($"{formName}_Width", defaultSize.Width);
            int height = GetValue($"{formName}_Height", defaultSize.Height);
            return new Size(width, height);
        }

        // Theme Management
        public static void SaveTheme(string themeName)
        {
            SetValue("Theme", themeName);
        }

        public static string GetTheme(string defaultTheme = "default")
        {
            return GetValue("Theme", defaultTheme);
        }

        // Path Management
        public static void SaveLastUsedPath(string path)
        {
            SetValue("LastUsedPath", path);
        }

        public static string GetLastUsedPath(string defaultPath = "")
        {
            return GetValue("LastUsedPath", defaultPath);
        }

        // User Preferences
        public static void SaveUserPreference(string preferenceName, object value)
        {
            SetValue($"UserPref_{preferenceName}", value);
        }

        public static T GetUserPreference<T>(string preferenceName, T defaultValue = default)
        {
            return GetValue($"UserPref_{preferenceName}", defaultValue);
        }

        // Application Settings
        public static void SaveAppSetting(string settingName, object value)
        {
            SetValue($"AppSetting_{settingName}", value);
        }

        public static T GetAppSetting<T>(string settingName, T defaultValue = default)
        {
            return GetValue($"AppSetting_{settingName}", defaultValue);
        }
    }
} 