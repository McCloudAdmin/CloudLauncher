using System;
using System.Collections.Generic;
using CloudLauncher.plugins.Events;
using CloudLauncher.utils;
using CmlLib.Core.Auth;

namespace CloudLauncher.plugins
{
    /// <summary>
    /// Plugin context providing access to launcher APIs and services.
    /// This is the main interface plugins use to interact with CloudLauncher.
    /// </summary>
    public interface IPluginContext
    {
        /// <summary>
        /// Launcher version information
        /// </summary>
        string LauncherVersion { get; }

        /// <summary>
        /// Current logged in user session (null if not logged in)
        /// </summary>
        MSession CurrentSession { get; }

        /// <summary>
        /// Launcher working directory
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Plugin-specific data directory for storing plugin files
        /// </summary>
        string PluginDataDirectory { get; }

        /// <summary>
        /// Event manager for subscribing to launcher events
        /// </summary>
        IEventManager Events { get; }

        /// <summary>
        /// Configuration manager for plugin settings
        /// </summary>
        IPluginConfig Config { get; }

        /// <summary>
        /// Logger instance for the plugin
        /// </summary>
        IPluginLogger Logger { get; }

        /// <summary>
        /// UI manager for plugin UI integration
        /// </summary>
        IPluginUI UI { get; }

        /// <summary>
        /// Minecraft integration APIs
        /// </summary>
        IMinecraftAPI Minecraft { get; }

        /// <summary>
        /// Get another plugin instance (for inter-plugin communication)
        /// </summary>
        /// <param name="pluginId">Plugin ID to get</param>
        /// <returns>Plugin instance or null if not found/loaded</returns>
        IPlugin GetPlugin(string pluginId);

        /// <summary>
        /// Get list of all loaded plugins
        /// </summary>
        /// <returns>List of loaded plugins</returns>
        List<IPlugin> GetAllPlugins();

        /// <summary>
        /// Check if a plugin is loaded and enabled
        /// </summary>
        /// <param name="pluginId">Plugin ID to check</param>
        /// <returns>True if plugin is loaded and enabled</returns>
        bool IsPluginEnabled(string pluginId);

        /// <summary>
        /// Send a message to another plugin
        /// </summary>
        /// <param name="targetPluginId">Target plugin ID</param>
        /// <param name="message">Message to send</param>
        /// <param name="data">Additional data</param>
        void SendPluginMessage(string targetPluginId, string message, object data = null);

        /// <summary>
        /// Register a plugin message handler
        /// </summary>
        /// <param name="message">Message name to handle</param>
        /// <param name="handler">Handler function</param>
        void RegisterMessageHandler(string message, Action<string, object> handler);
    }

    /// <summary>
    /// Plugin configuration manager
    /// </summary>
    public interface IPluginConfig
    {
        /// <summary>
        /// Get a configuration value with default fallback
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value</returns>
        T GetValue<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Set a configuration value
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Value to set</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Delete a configuration value
        /// </summary>
        /// <param name="key">Configuration key</param>
        void DeleteValue(string key);

        /// <summary>
        /// Check if a configuration key exists
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>True if key exists</returns>
        bool HasValue(string key);

        /// <summary>
        /// Get all configuration keys
        /// </summary>
        /// <returns>List of configuration keys</returns>
        List<string> GetKeys();
    }

    /// <summary>
    /// Plugin logger interface
    /// </summary>
    public interface IPluginLogger
    {
        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warning(string message);

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);
    }

    /// <summary>
    /// Plugin UI manager interface
    /// </summary>
    public interface IPluginUI
    {
        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="title">Title of message box</param>
        /// <param name="type">Type of message (info, warning, error)</param>
        void ShowMessage(string message, string title = "Plugin Message", MessageType type = MessageType.Info);

        /// <summary>
        /// Add a menu item to the main menu
        /// </summary>
        /// <param name="text">Menu item text</param>
        /// <param name="onClick">Click handler</param>
        /// <param name="parentMenu">Parent menu (null for top level)</param>
        void AddMenuItem(string text, Action onClick, string parentMenu = null);

        /// <summary>
        /// Add a button to the main UI
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="onClick">Click handler</param>
        /// <param name="location">Location to add button</param>
        void AddButton(string text, Action onClick, UILocation location = UILocation.Settings);

        /// <summary>
        /// Show a custom form
        /// </summary>
        /// <param name="form">Form to show</param>
        /// <param name="modal">Whether to show as modal dialog</param>
        void ShowForm(Form form, bool modal = false);
    }

    /// <summary>
    /// Minecraft integration API
    /// </summary>
    public interface IMinecraftAPI
    {
        /// <summary>
        /// Get list of installed Minecraft versions
        /// </summary>
        /// <returns>List of installed versions</returns>
        List<string> GetInstalledVersions();

        /// <summary>
        /// Get current selected version
        /// </summary>
        /// <returns>Currently selected version or null</returns>
        string GetCurrentVersion();

        /// <summary>
        /// Get Minecraft installation path
        /// </summary>
        /// <returns>Minecraft installation path</returns>
        string GetMinecraftPath();

        /// <summary>
        /// Get current launch settings
        /// </summary>
        /// <returns>Launch settings</returns>
        Dictionary<string, object> GetLaunchSettings();

        /// <summary>
        /// Modify launch arguments (called before game launch)
        /// </summary>
        /// <param name="arguments">Current arguments</param>
        /// <returns>Modified arguments</returns>
        string ModifyLaunchArguments(string arguments);
    }

    /// <summary>
    /// Message types for UI messages
    /// </summary>
    public enum MessageType
    {
        Info,
        Warning,
        Error,
        Success
    }

    /// <summary>
    /// UI locations for adding plugin elements
    /// </summary>
    public enum UILocation
    {
        Settings,
        MainMenu,
        GameLaunch,
        UserProfile
    }
} 