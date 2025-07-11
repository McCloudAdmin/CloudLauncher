using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CloudLauncher.plugins
{
    /// <summary>
    /// Core interface that all CloudLauncher plugins must implement.
    /// Provides essential lifecycle management and metadata for plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Unique identifier for the plugin. Must be unique across all plugins.
        /// Recommended format: "author.pluginname" (e.g., "mythical.exampleplugin")
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// Display name of the plugin shown in the UI
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Brief description of what the plugin does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Plugin version (e.g., "1.0.0")
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Plugin author name
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Minimum required CloudLauncher version for this plugin
        /// </summary>
        string RequiredLauncherVersion { get; }

        /// <summary>
        /// List of plugin dependencies (other plugin IDs this plugin requires)
        /// </summary>
        List<string> Dependencies { get; }

        /// <summary>
        /// Whether the plugin is currently enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Whether the plugin has a configuration UI
        /// </summary>
        bool HasConfiguration { get; }

        /// <summary>
        /// Called when the plugin is loaded for the first time
        /// </summary>
        /// <param name="context">Plugin context providing access to launcher APIs</param>
        void OnLoad(IPluginContext context);

        /// <summary>
        /// Called when the plugin is enabled
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the plugin is disabled
        /// </summary>
        void OnDisable();

        /// <summary>
        /// Called when the plugin is unloaded (application shutdown)
        /// </summary>
        void OnUnload();

        /// <summary>
        /// Called to show the plugin's configuration UI (if HasConfiguration is true)
        /// </summary>
        /// <param name="parentForm">Parent form to show the configuration dialog</param>
        void ShowConfiguration(Form parentForm);

        /// <summary>
        /// Called to validate plugin configuration and dependencies
        /// </summary>
        /// <returns>True if plugin is properly configured and ready to use</returns>
        bool ValidateConfiguration();

        /// <summary>
        /// Called to get plugin status information for display in UI
        /// </summary>
        /// <returns>Status information about the plugin</returns>
        PluginStatus GetStatus();
    }

    /// <summary>
    /// Plugin status information
    /// </summary>
    public class PluginStatus
    {
        public bool IsHealthy { get; set; } = true;
        public string StatusMessage { get; set; } = "OK";
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
} 