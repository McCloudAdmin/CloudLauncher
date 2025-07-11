using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CloudLauncher.plugins.Events;
using CloudLauncher.utils;
using CmlLib.Core.Auth;
using CmlLib.Core;

namespace CloudLauncher.plugins
{
    /// <summary>
    /// Plugin context implementation providing access to launcher APIs
    /// </summary>
    internal class PluginContext : IPluginContext
    {
        private readonly string _pluginId;
        private readonly PluginManager _pluginManager;
        private readonly IEventManager _eventManager;
        private readonly IPluginConfig _config;
        private readonly IPluginLogger _logger;
        private readonly IPluginUI _ui;
        private readonly IMinecraftAPI _minecraft;
        private readonly string _pluginDataDirectory;
        private MSession _currentSession;

        public PluginContext(string pluginId, PluginManager pluginManager, IEventManager eventManager)
        {
            _pluginId = pluginId;
            _pluginManager = pluginManager;
            _eventManager = eventManager;
            
            // Create plugin data directory
            _pluginDataDirectory = Path.Combine(Program.appWorkDir, "plugins", "data", pluginId);
            Directory.CreateDirectory(_pluginDataDirectory);

            // Initialize services
            _config = new PluginConfig(pluginId);
            _logger = new PluginLogger(pluginId);
            _ui = new PluginUI(pluginId);
            _minecraft = new MinecraftAPI();
        }

        public string LauncherVersion => Program.appVersion;
        public MSession CurrentSession => _currentSession;
        public string WorkingDirectory => Program.appWorkDir;
        public string PluginDataDirectory => _pluginDataDirectory;
        public IEventManager Events => _eventManager;
        public IPluginConfig Config => _config;
        public IPluginLogger Logger => _logger;
        public IPluginUI UI => _ui;
        public IMinecraftAPI Minecraft => _minecraft;

        public IPlugin GetPlugin(string pluginId)
        {
            return _pluginManager.GetPlugin(pluginId);
        }

        public List<IPlugin> GetAllPlugins()
        {
            return _pluginManager.GetAllPlugins();
        }

        public bool IsPluginEnabled(string pluginId)
        {
            return _pluginManager.IsPluginEnabled(pluginId);
        }

        public void SendPluginMessage(string targetPluginId, string message, object data = null)
        {
            _pluginManager.SendPluginMessage(targetPluginId, message, data);
        }

        public void RegisterMessageHandler(string message, Action<string, object> handler)
        {
            _pluginManager.RegisterMessageHandler(_pluginId, message, handler);
        }

        public void UpdateCurrentSession(MSession session)
        {
            _currentSession = session;
        }
    }

    /// <summary>
    /// Plugin configuration implementation
    /// </summary>
    internal class PluginConfig : IPluginConfig
    {
        private readonly string _pluginId;
        private readonly string _keyPrefix;

        public PluginConfig(string pluginId)
        {
            _pluginId = pluginId;
            _keyPrefix = $"Plugin_{pluginId}_";
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            return RegistryConfig.GetUserPreference(_keyPrefix + key, defaultValue);
        }

        public void SetValue<T>(string key, T value)
        {
            RegistryConfig.SaveUserPreference(_keyPrefix + key, value);
        }

        public void DeleteValue(string key)
        {
            RegistryConfig.DeleteValue(_keyPrefix + key);
        }

        public bool HasValue(string key)
        {
            return RegistryConfig.ValueExists(_keyPrefix + key);
        }

        public List<string> GetKeys()
        {
            // This would need to be implemented in RegistryConfig to get all keys with prefix
            // For now, return empty list
            return new List<string>();
        }
    }

    /// <summary>
    /// Plugin logger implementation
    /// </summary>
    internal class PluginLogger : IPluginLogger
    {
        private readonly string _pluginId;

        public PluginLogger(string pluginId)
        {
            _pluginId = pluginId;
        }

        public void Info(string message)
        {
            Logger.Info($"[Plugin:{_pluginId}] {message}");
        }

        public void Warning(string message)
        {
            Logger.Warning($"[Plugin:{_pluginId}] {message}");
        }

        public void Error(string message)
        {
            Logger.Error($"[Plugin:{_pluginId}] {message}");
        }

        public void Debug(string message)
        {
            Logger.Debug($"[Plugin:{_pluginId}] {message}");
        }
    }

    /// <summary>
    /// Plugin UI implementation
    /// </summary>
    internal class PluginUI : IPluginUI
    {
        private readonly string _pluginId;

        public PluginUI(string pluginId)
        {
            _pluginId = pluginId;
        }

        public void ShowMessage(string message, string title = "Plugin Message", MessageType type = MessageType.Info)
        {
            switch (type)
            {
                case MessageType.Info:
                    Alert.Info(message, title);
                    break;
                case MessageType.Warning:
                    Alert.Warning(message, title);
                    break;
                case MessageType.Error:
                    Alert.Error(message, title);
                    break;
                case MessageType.Success:
                    Alert.Success(message, title);
                    break;
            }
        }

        public void AddMenuItem(string text, Action onClick, string parentMenu = null)
        {
            // This would need to be implemented in the main form
            // For now, just log the request
            Logger.Info($"[Plugin:{_pluginId}] Requested to add menu item: {text}");
        }

        public void AddButton(string text, Action onClick, UILocation location = UILocation.Settings)
        {
            // This would need to be implemented in the main form
            // For now, just log the request
            Logger.Info($"[Plugin:{_pluginId}] Requested to add button: {text} at location: {location}");
        }

        public void ShowForm(Form form, bool modal = false)
        {
            try
            {
                if (modal)
                {
                    form.ShowDialog();
                }
                else
                {
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[Plugin:{_pluginId}] Error showing form: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Minecraft API implementation
    /// </summary>
    internal class MinecraftAPI : IMinecraftAPI
    {
        private static MinecraftPath _minecraftPath;
        private static MinecraftLauncher _launcher;

        static MinecraftAPI()
        {
            _minecraftPath = new MinecraftPath();
            _launcher = new MinecraftLauncher(_minecraftPath);
        }

        public List<string> GetInstalledVersions()
        {
            try
            {
                var versions = _launcher.GetAllVersionsAsync().GetAwaiter().GetResult();
                return versions.Select(v => v.Name).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting installed versions: {ex.Message}");
                return new List<string>();
            }
        }

        public string GetCurrentVersion()
        {
            // This would need to be implemented to get current version from UI
            return RegistryConfig.GetUserPreference<string>("LastVersion", null);
        }

        public string GetMinecraftPath()
        {
            return _minecraftPath.BasePath;
        }

        public Dictionary<string, object> GetLaunchSettings()
        {
            var settings = new Dictionary<string, object>();
            
            try
            {
                settings["RamMb"] = RegistryConfig.GetUserPreference("RamMb", 4096);
                settings["ScreenWidth"] = RegistryConfig.GetUserPreference("ScreenWidth", 854);
                settings["ScreenHeight"] = RegistryConfig.GetUserPreference("ScreenHeight", 480);
                settings["FullScreen"] = RegistryConfig.GetUserPreference("FullScreen", false);
                settings["CustomArgs"] = RegistryConfig.GetUserPreference("CustomArgs", "");
                settings["JoinServerIP"] = RegistryConfig.GetUserPreference("JoinServerIP", "");
                settings["JoinServerPort"] = RegistryConfig.GetUserPreference("JoinServerPort", 25565);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting launch settings: {ex.Message}");
            }

            return settings;
        }

        public string ModifyLaunchArguments(string arguments)
        {
            // This is a placeholder - plugins can override this to modify launch arguments
            return arguments;
        }
    }
} 