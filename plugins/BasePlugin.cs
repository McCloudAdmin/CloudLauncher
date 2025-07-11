using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CloudLauncher.plugins
{
    /// <summary>
    /// Base plugin class providing default implementations and utility methods.
    /// Plugin developers can inherit from this class to simplify plugin development.
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        protected IPluginContext Context { get; private set; }
        protected bool IsLoaded { get; private set; }

        #region Required Plugin Properties (must be implemented by derived classes)

        /// <summary>
        /// Unique identifier for the plugin. Must be implemented by derived classes.
        /// Recommended format: "author.pluginname" (e.g., "mythical.exampleplugin")
        /// </summary>
        public abstract string PluginId { get; }

        /// <summary>
        /// Display name of the plugin. Must be implemented by derived classes.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Brief description of what the plugin does. Must be implemented by derived classes.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Plugin version. Must be implemented by derived classes.
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Plugin author name. Must be implemented by derived classes.
        /// </summary>
        public abstract string Author { get; }

        #endregion

        #region Virtual Plugin Properties (can be overridden)

        /// <summary>
        /// Minimum required CloudLauncher version for this plugin.
        /// Default is "1.0.0". Override to specify different version.
        /// </summary>
        public virtual string RequiredLauncherVersion => "1.0.0";

        /// <summary>
        /// List of plugin dependencies. Default is empty list.
        /// Override to specify required plugins.
        /// </summary>
        public virtual List<string> Dependencies => new List<string>();

        /// <summary>
        /// Whether the plugin has a configuration UI. Default is false.
        /// Override to return true if plugin has configuration.
        /// </summary>
        public virtual bool HasConfiguration => false;

        #endregion

        #region Plugin State

        /// <summary>
        /// Whether the plugin is currently enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        #endregion

        #region Plugin Lifecycle Methods

        /// <summary>
        /// Called when the plugin is loaded for the first time.
        /// Base implementation stores the context. Override to add custom initialization.
        /// </summary>
        /// <param name="context">Plugin context providing access to launcher APIs</param>
        public virtual void OnLoad(IPluginContext context)
        {
            Context = context;
            IsLoaded = true;
            Context.Logger.Info($"Plugin {PluginId} loaded successfully");
        }

        /// <summary>
        /// Called when the plugin is enabled.
        /// Override to add custom enable logic.
        /// </summary>
        public virtual void OnEnable()
        {
            Context?.Logger?.Info($"Plugin {PluginId} enabled");
        }

        /// <summary>
        /// Called when the plugin is disabled.
        /// Override to add custom disable logic.
        /// </summary>
        public virtual void OnDisable()
        {
            Context?.Logger?.Info($"Plugin {PluginId} disabled");
        }

        /// <summary>
        /// Called when the plugin is unloaded (application shutdown).
        /// Override to add custom cleanup logic.
        /// </summary>
        public virtual void OnUnload()
        {
            Context?.Logger?.Info($"Plugin {PluginId} unloaded");
            IsLoaded = false;
        }

        /// <summary>
        /// Called to show the plugin's configuration UI.
        /// Override to implement custom configuration UI.
        /// </summary>
        /// <param name="parentForm">Parent form to show the configuration dialog</param>
        public virtual void ShowConfiguration(Form parentForm)
        {
            Context?.UI?.ShowMessage(
                "This plugin does not have a configuration interface.",
                "No Configuration",
                MessageType.Info
            );
        }

        /// <summary>
        /// Called to validate plugin configuration and dependencies.
        /// Override to implement custom validation logic.
        /// </summary>
        /// <returns>True if plugin is properly configured and ready to use</returns>
        public virtual bool ValidateConfiguration()
        {
            // Check if all dependencies are available
            if (Dependencies != null && Dependencies.Count > 0)
            {
                foreach (string dependency in Dependencies)
                {
                    if (!Context.IsPluginEnabled(dependency))
                    {
                        Context?.Logger?.Error($"Plugin {PluginId} dependency {dependency} is not available");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Called to get plugin status information for display in UI.
        /// Override to provide custom status information.
        /// </summary>
        /// <returns>Status information about the plugin</returns>
        public virtual PluginStatus GetStatus()
        {
            var status = new PluginStatus
            {
                IsHealthy = IsEnabled && IsLoaded,
                StatusMessage = IsEnabled ? "Running" : "Disabled",
                LastUpdate = DateTime.Now
            };

            if (!IsLoaded)
            {
                status.Errors.Add("Plugin not loaded");
                status.IsHealthy = false;
            }

            if (!ValidateConfiguration())
            {
                status.Errors.Add("Configuration validation failed");
                status.IsHealthy = false;
            }

            return status;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Safely execute an action with error handling and logging
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="errorMessage">Error message to log if action fails</param>
        protected void SafeExecute(Action action, string errorMessage = "An error occurred")
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Context?.Logger?.Error($"{errorMessage}: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely execute a function with error handling and logging
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="function">Function to execute</param>
        /// <param name="defaultValue">Default value to return if function fails</param>
        /// <param name="errorMessage">Error message to log if function fails</param>
        /// <returns>Function result or default value</returns>
        protected T SafeExecute<T>(Func<T> function, T defaultValue = default(T), string errorMessage = "An error occurred")
        {
            try
            {
                return function != null ? function() : defaultValue;
            }
            catch (Exception ex)
            {
                Context?.Logger?.Error($"{errorMessage}: {ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Get a configuration value with type safety
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value</returns>
        protected T GetConfigValue<T>(string key, T defaultValue = default(T))
        {
            if (Context?.Config == null)
                return defaultValue;
                
            return Context.Config.GetValue(key, defaultValue);
        }

        /// <summary>
        /// Set a configuration value with type safety
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Value to set</param>
        protected void SetConfigValue<T>(string key, T value)
        {
            Context?.Config?.SetValue(key, value);
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogInfo(string message)
        {
            Context?.Logger?.Info($"[{PluginId}] {message}");
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogWarning(string message)
        {
            Context?.Logger?.Warning($"[{PluginId}] {message}");
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogError(string message)
        {
            Context?.Logger?.Error($"[{PluginId}] {message}");
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogDebug(string message)
        {
            Context?.Logger?.Debug($"[{PluginId}] {message}");
        }

        /// <summary>
        /// Show a message to the user
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="title">Title of message box</param>
        /// <param name="type">Type of message</param>
        protected void ShowMessage(string message, string title = null, MessageType type = MessageType.Info)
        {
            Context?.UI?.ShowMessage(message, title ?? Name, type);
        }

        #endregion
    }
} 