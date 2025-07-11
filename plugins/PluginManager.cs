using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CloudLauncher.plugins.Events;
using CloudLauncher.utils;
using CmlLib.Core.Auth;

namespace CloudLauncher.plugins
{
    /// <summary>
    /// Plugin manager for loading, managing, and coordinating plugins
    /// </summary>
    public class PluginManager
    {
        private static PluginManager _instance;
        private static readonly object _lock = new object();

        private readonly Dictionary<string, IPlugin> _loadedPlugins = new Dictionary<string, IPlugin>();
        private readonly Dictionary<string, PluginContext> _pluginContexts = new Dictionary<string, PluginContext>();
        private readonly Dictionary<string, Assembly> _pluginAssemblies = new Dictionary<string, Assembly>();
        private readonly EventManager _eventManager = new EventManager();
        private readonly Dictionary<string, Dictionary<string, Action<string, object>>> _messageHandlers = new Dictionary<string, Dictionary<string, Action<string, object>>>();
        
        private string _pluginsDirectory;
        private MSession _currentSession;
        private bool _isInitialized = false;

        public static PluginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PluginManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private PluginManager()
        {
            _pluginsDirectory = Path.Combine(Program.appWorkDir, "plugins");
            Directory.CreateDirectory(_pluginsDirectory);
        }

        /// <summary>
        /// Initialize the plugin manager
        /// </summary>
        public void Initialize(MSession session = null)
        {
            if (_isInitialized)
                return;

            _currentSession = session;
            _isInitialized = true;

            Logger.Info("Plugin manager initialized");

            // Publish application start event
            _eventManager.Publish(new ApplicationStartEvent
            {
                Version = Program.appVersion,
                WorkingDirectory = Program.appWorkDir,
                Arguments = Environment.GetCommandLineArgs().ToList()
            });
        }

        /// <summary>
        /// Load all plugins from the plugins directory
        /// </summary>
        public void LoadAllPlugins()
        {
            Logger.Info("Loading plugins from directory: " + _pluginsDirectory);

            if (!Directory.Exists(_pluginsDirectory))
            {
                Logger.Warning("Plugins directory does not exist: " + _pluginsDirectory);
                return;
            }

            var dllFiles = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.AllDirectories);
            var loadedCount = 0;

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    if (LoadPlugin(dllFile))
                    {
                        loadedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load plugin from {dllFile}: {ex.Message}");
                }
            }

            Logger.Info($"Loaded {loadedCount} plugins successfully");

            // Resolve dependencies and enable plugins
            ResolveDependenciesAndEnablePlugins();
        }

        /// <summary>
        /// Load a specific plugin from a DLL file
        /// </summary>
        /// <param name="dllPath">Path to the plugin DLL</param>
        /// <returns>True if plugin was loaded successfully</returns>
        public bool LoadPlugin(string dllPath)
        {
            try
            {
                if (!File.Exists(dllPath))
                {
                    Logger.Error($"Plugin file not found: {dllPath}");
                    return false;
                }

                Logger.Info($"Loading plugin from: {dllPath}");

                // Load the assembly
                var assembly = Assembly.LoadFrom(dllPath);

                // Find all types that implement IPlugin
                var pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && typeof(IPlugin).IsAssignableFrom(type))
                    .ToList();

                if (pluginTypes.Count == 0)
                {
                    Logger.Warning($"No plugin classes found in {dllPath}");
                    return false;
                }

                foreach (var pluginType in pluginTypes)
                {
                    try
                    {
                        // Create plugin instance
                        var plugin = (IPlugin)Activator.CreateInstance(pluginType);

                        // Check if plugin is already loaded
                        if (_loadedPlugins.ContainsKey(plugin.PluginId))
                        {
                            Logger.Warning($"Plugin {plugin.PluginId} is already loaded");
                            continue;
                        }

                        // Validate plugin
                        if (!ValidatePlugin(plugin))
                        {
                            Logger.Error($"Plugin validation failed for {plugin.PluginId}");
                            continue;
                        }

                        // Create plugin context
                        var context = new PluginContext(plugin.PluginId, this, _eventManager);

                        // Store plugin and context
                        _loadedPlugins[plugin.PluginId] = plugin;
                        _pluginContexts[plugin.PluginId] = context;
                        _pluginAssemblies[plugin.PluginId] = assembly;

                        // Load the plugin
                        plugin.OnLoad(context);

                        Logger.Info($"Plugin {plugin.PluginId} loaded successfully");

                        // Publish plugin loaded event
                        _eventManager.Publish(new PluginLoadedEvent
                        {
                            PluginId = plugin.PluginId,
                            PluginName = plugin.Name,
                            PluginVersion = plugin.Version,
                            PluginAuthor = plugin.Author
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to load plugin class {pluginType.Name}: {ex.Message}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load plugin assembly {dllPath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unload a specific plugin
        /// </summary>
        /// <param name="pluginId">Plugin ID to unload</param>
        /// <param name="reason">Reason for unloading</param>
        public void UnloadPlugin(string pluginId, string reason = "Manual unload")
        {
            if (!_loadedPlugins.ContainsKey(pluginId))
            {
                Logger.Warning($"Plugin {pluginId} is not loaded");
                return;
            }

            try
            {
                var plugin = _loadedPlugins[pluginId];
                
                // Disable the plugin first
                DisablePlugin(pluginId, reason);

                // Unload the plugin
                plugin.OnUnload();

                // Remove from collections
                _loadedPlugins.Remove(pluginId);
                _pluginContexts.Remove(pluginId);
                _pluginAssemblies.Remove(pluginId);
                _messageHandlers.Remove(pluginId);

                Logger.Info($"Plugin {pluginId} unloaded successfully");

                // Publish plugin unloaded event
                _eventManager.Publish(new PluginUnloadedEvent
                {
                    PluginId = pluginId,
                    PluginName = plugin.Name,
                    Reason = reason
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to unload plugin {pluginId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Enable a plugin
        /// </summary>
        /// <param name="pluginId">Plugin ID to enable</param>
        public void EnablePlugin(string pluginId)
        {
            if (!_loadedPlugins.ContainsKey(pluginId))
            {
                Logger.Warning($"Plugin {pluginId} is not loaded");
                return;
            }

            try
            {
                var plugin = _loadedPlugins[pluginId];
                
                if (plugin.IsEnabled)
                {
                    Logger.Debug($"Plugin {pluginId} is already enabled");
                    return;
                }

                // Validate configuration
                if (!plugin.ValidateConfiguration())
                {
                    Logger.Error($"Plugin {pluginId} configuration validation failed");
                    return;
                }

                // Enable the plugin
                plugin.IsEnabled = true;
                plugin.OnEnable();

                // Save plugin state
                RegistryConfig.SaveUserPreference($"Plugin_{pluginId}_Enabled", true);

                Logger.Info($"Plugin {pluginId} enabled successfully");

                // Publish plugin enabled event
                _eventManager.Publish(new PluginEnabledEvent
                {
                    PluginId = pluginId,
                    PluginName = plugin.Name
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to enable plugin {pluginId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Disable a plugin
        /// </summary>
        /// <param name="pluginId">Plugin ID to disable</param>
        /// <param name="reason">Reason for disabling</param>
        public void DisablePlugin(string pluginId, string reason = "Manual disable")
        {
            if (!_loadedPlugins.ContainsKey(pluginId))
            {
                Logger.Warning($"Plugin {pluginId} is not loaded");
                return;
            }

            try
            {
                var plugin = _loadedPlugins[pluginId];
                
                if (!plugin.IsEnabled)
                {
                    Logger.Debug($"Plugin {pluginId} is already disabled");
                    return;
                }

                // Disable the plugin
                plugin.IsEnabled = false;
                plugin.OnDisable();

                // Save plugin state
                RegistryConfig.SaveUserPreference($"Plugin_{pluginId}_Enabled", false);

                Logger.Info($"Plugin {pluginId} disabled successfully");

                // Publish plugin disabled event
                _eventManager.Publish(new PluginDisabledEvent
                {
                    PluginId = pluginId,
                    PluginName = plugin.Name,
                    Reason = reason
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to disable plugin {pluginId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a plugin by ID
        /// </summary>
        /// <param name="pluginId">Plugin ID</param>
        /// <returns>Plugin instance or null if not found</returns>
        public IPlugin GetPlugin(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId) ? _loadedPlugins[pluginId] : null;
        }

        /// <summary>
        /// Get all loaded plugins
        /// </summary>
        /// <returns>List of loaded plugins</returns>
        public List<IPlugin> GetAllPlugins()
        {
            return new List<IPlugin>(_loadedPlugins.Values);
        }

        /// <summary>
        /// Check if a plugin is loaded and enabled
        /// </summary>
        /// <param name="pluginId">Plugin ID to check</param>
        /// <returns>True if plugin is loaded and enabled</returns>
        public bool IsPluginEnabled(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId) && _loadedPlugins[pluginId].IsEnabled;
        }

        /// <summary>
        /// Update current session for all plugins
        /// </summary>
        /// <param name="session">New session</param>
        public void UpdateCurrentSession(MSession session)
        {
            _currentSession = session;
            
            // Update all plugin contexts
            foreach (var context in _pluginContexts.Values)
            {
                context.UpdateCurrentSession(session);
            }
        }

        /// <summary>
        /// Get the event manager
        /// </summary>
        /// <returns>Event manager instance</returns>
        public IEventManager GetEventManager()
        {
            return _eventManager;
        }

        /// <summary>
        /// Send a message to a plugin
        /// </summary>
        /// <param name="targetPluginId">Target plugin ID</param>
        /// <param name="message">Message to send</param>
        /// <param name="data">Additional data</param>
        public void SendPluginMessage(string targetPluginId, string message, object data = null)
        {
            if (_messageHandlers.ContainsKey(targetPluginId) && _messageHandlers[targetPluginId].ContainsKey(message))
            {
                try
                {
                    _messageHandlers[targetPluginId][message](message, data);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error sending message '{message}' to plugin {targetPluginId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Register a message handler for a plugin
        /// </summary>
        /// <param name="pluginId">Plugin ID</param>
        /// <param name="message">Message name</param>
        /// <param name="handler">Handler function</param>
        public void RegisterMessageHandler(string pluginId, string message, Action<string, object> handler)
        {
            if (!_messageHandlers.ContainsKey(pluginId))
            {
                _messageHandlers[pluginId] = new Dictionary<string, Action<string, object>>();
            }

            _messageHandlers[pluginId][message] = handler;
        }

        /// <summary>
        /// Unload all plugins
        /// </summary>
        public void UnloadAllPlugins()
        {
            Logger.Info("Unloading all plugins");

            var pluginIds = new List<string>(_loadedPlugins.Keys);
            foreach (var pluginId in pluginIds)
            {
                UnloadPlugin(pluginId, "Application shutdown");
            }

            _eventManager.Clear();
            Logger.Info("All plugins unloaded");
        }

        #region Private Methods

        private bool ValidatePlugin(IPlugin plugin)
        {
            if (string.IsNullOrEmpty(plugin.PluginId))
            {
                Logger.Error("Plugin ID cannot be null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(plugin.Name))
            {
                Logger.Error($"Plugin {plugin.PluginId} name cannot be null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(plugin.Version))
            {
                Logger.Error($"Plugin {plugin.PluginId} version cannot be null or empty");
                return false;
            }

            // Check version compatibility
            if (!IsVersionCompatible(plugin.RequiredLauncherVersion))
            {
                Logger.Error($"Plugin {plugin.PluginId} requires launcher version {plugin.RequiredLauncherVersion}, but current version is {Program.appVersion}");
                return false;
            }

            return true;
        }

        private bool IsVersionCompatible(string requiredVersion)
        {
            try
            {
                var required = new Version(requiredVersion);
                var current = new Version(Program.appVersion);
                return current >= required;
            }
            catch
            {
                return false;
            }
        }

        private void ResolveDependenciesAndEnablePlugins()
        {
            Logger.Info("Resolving plugin dependencies and enabling plugins");

            var resolvedPlugins = new HashSet<string>();
            var pluginQueue = new Queue<string>(_loadedPlugins.Keys);

            while (pluginQueue.Count > 0)
            {
                var pluginId = pluginQueue.Dequeue();
                var plugin = _loadedPlugins[pluginId];

                // Check if all dependencies are resolved
                bool canEnable = true;
                if (plugin.Dependencies != null && plugin.Dependencies.Count > 0)
                {
                    foreach (var dependency in plugin.Dependencies)
                    {
                        if (!resolvedPlugins.Contains(dependency))
                        {
                            canEnable = false;
                            break;
                        }
                    }
                }

                if (canEnable)
                {
                    // Check if plugin was previously enabled
                    bool wasEnabled = RegistryConfig.GetUserPreference($"Plugin_{pluginId}_Enabled", true);
                    if (wasEnabled)
                    {
                        EnablePlugin(pluginId);
                    }
                    resolvedPlugins.Add(pluginId);
                }
                else
                {
                    // Re-queue for later processing
                    pluginQueue.Enqueue(pluginId);
                }
            }

            // Disable plugins with unresolved dependencies
            foreach (var pluginId in _loadedPlugins.Keys)
            {
                if (!resolvedPlugins.Contains(pluginId))
                {
                    DisablePlugin(pluginId, "Unresolved dependencies");
                }
            }
        }

        #endregion
    }
} 