# CloudLauncher Plugin Development Guide

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [Plugin Architecture](#plugin-architecture)
4. [Basic Plugin Development](#basic-plugin-development)
5. [Event System](#event-system)
6. [Configuration Management](#configuration-management)
7. [UI Integration](#ui-integration)
8. [Minecraft Integration](#minecraft-integration)
9. [Inter-Plugin Communication](#inter-plugin-communication)
10. [Security Considerations](#security-considerations)
11. [Testing and Debugging](#testing-and-debugging)
12. [Distribution and Installation](#distribution-and-installation)
13. [Advanced Features](#advanced-features)
14. [API Reference](#api-reference)
15. [Examples](#examples)
16. [Best Practices](#best-practices)
17. [Troubleshooting](#troubleshooting)

## Overview

CloudLauncher's plugin system allows developers to extend the launcher's functionality through a powerful and flexible API. Plugins can:

- React to launcher events (login, logout, game launch, etc.)
- Modify game launch parameters
- Add custom UI elements
- Store persistent configuration
- Interact with other plugins
- Access Minecraft installation information
- Provide custom functionality

## Getting Started

### Prerequisites

- .NET 8.0 or later
- Visual Studio 2022 or VS Code
- Basic knowledge of C# and WinForms
- CloudLauncher source code or SDK

### Development Environment Setup

1. **Create a new Class Library project:**
   ```bash
   dotnet new classlib -n MyPlugin
   cd MyPlugin
   ```

2. **Add references to CloudLauncher:**
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <TargetFramework>net8.0-windows</TargetFramework>
       <UseWindowsForms>true</UseWindowsForms>
     </PropertyGroup>
     
     <ItemGroup>
       <Reference Include="CloudLauncher">
         <HintPath>path\to\CloudLauncher.exe</HintPath>
       </Reference>
     </ItemGroup>
   </Project>
   ```

3. **Install required NuGet packages:**
   ```bash
   dotnet add package System.Windows.Forms
   ```

## Plugin Architecture

### Core Components

```
CloudLauncher Plugin System
├── IPlugin (Interface)
├── BasePlugin (Abstract Base Class)
├── IPluginContext (API Access)
├── IEventManager (Event System)
├── PluginManager (Plugin Lifecycle)
└── Plugin APIs
    ├── IPluginConfig
    ├── IPluginLogger
    ├── IPluginUI
    └── IMinecraftAPI
```

### Plugin Lifecycle

1. **Discovery**: PluginManager scans for DLL files
2. **Loading**: Assembly is loaded and plugin instances created
3. **Initialization**: `OnLoad()` called with context
4. **Enabling**: `OnEnable()` called if plugin is enabled
5. **Runtime**: Plugin responds to events and provides functionality
6. **Disabling**: `OnDisable()` called when disabled
7. **Unloading**: `OnUnload()` called during shutdown

## Basic Plugin Development

### Creating Your First Plugin

```csharp
using CloudLauncher.plugins;
using CloudLauncher.plugins.Events;

public class MyFirstPlugin : BasePlugin
{
    public override string PluginId => "mycompany.myfirstplugin";
    public override string Name => "My First Plugin";
    public override string Description => "A simple example plugin";
    public override string Version => "1.0.0";
    public override string Author => "Your Name";

    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        LogInfo("Plugin loaded successfully!");
        
        // Subscribe to events
        Context.Events.Subscribe<UserLoginEvent>(OnUserLogin);
    }

    private void OnUserLogin(UserLoginEvent loginEvent)
    {
        LogInfo($"User logged in: {loginEvent.Username}");
        ShowMessage($"Welcome, {loginEvent.Username}!");
    }
}
```

### Plugin Properties

Every plugin must provide these essential properties:

- **PluginId**: Unique identifier (format: "company.pluginname")
- **Name**: Human-readable name
- **Description**: Brief description of functionality
- **Version**: Plugin version (semantic versioning recommended)
- **Author**: Plugin author/company name

### Optional Properties

- **RequiredLauncherVersion**: Minimum launcher version required
- **Dependencies**: List of required plugin IDs
- **HasConfiguration**: Whether plugin has configuration UI

## Event System

### Available Events

#### Core Events
- `ApplicationStartEvent`: Launcher starts
- `ApplicationExitEvent`: Launcher exits (cancellable)
- `UserLoginEvent`: User logs in
- `UserLogoutEvent`: User logs out

#### Game Events
- `GameLaunchEvent`: Before game launch (cancellable)
- `GameLaunchedEvent`: After game launch attempt
- `GameExitEvent`: Game process exits
- `VersionSelectedEvent`: Version selection changed
- `VersionInstallStartEvent`: Version installation begins
- `VersionInstallCompleteEvent`: Version installation complete

#### System Events
- `SettingsChangedEvent`: Settings modified
- `ThemeChangedEvent`: UI theme changed
- `NotificationEvent`: Notification shown
- `ErrorEvent`: Error occurred
- `WarningEvent`: Warning issued

#### Plugin Events
- `PluginLoadedEvent`: Plugin loaded
- `PluginEnabledEvent`: Plugin enabled
- `PluginDisabledEvent`: Plugin disabled
- `PluginUnloadedEvent`: Plugin unloaded

### Event Subscription

```csharp
// Basic subscription
Context.Events.Subscribe<UserLoginEvent>(OnUserLogin);

// Subscription with priority (higher numbers = higher priority)
Context.Events.Subscribe<GameLaunchEvent>(OnGameLaunch, 10);

// Unsubscribe
Context.Events.Unsubscribe<UserLoginEvent>(OnUserLogin);
```

### Event Handling

```csharp
private void OnGameLaunch(GameLaunchEvent launchEvent)
{
    // Read event data
    LogInfo($"Launching {launchEvent.Version} with {launchEvent.RamMb}MB RAM");
    
    // Modify launch parameters
    launchEvent.JvmArguments.Add("-XX:+UseG1GC");
    
    // Cancel launch if needed
    if (ShouldCancelLaunch(launchEvent))
    {
        launchEvent.IsCancelled = true;
        launchEvent.CancellationReason = "Launch cancelled by plugin";
    }
    
    // Mark as handled
    launchEvent.IsHandled = true;
}
```

### Custom Events

```csharp
public class MyCustomEvent : BaseEvent
{
    public string CustomData { get; set; }
    public int CustomValue { get; set; }
}

// Publishing custom events
Context.Events.Publish(new MyCustomEvent
{
    CustomData = "Hello World",
    CustomValue = 42
});
```

## Configuration Management

### Basic Configuration

```csharp
// Save configuration
SetConfigValue("EnableNotifications", true);
SetConfigValue("NotificationDuration", 3000);
SetConfigValue("ServerList", new List<string> { "server1", "server2" });

// Load configuration
bool notifications = GetConfigValue("EnableNotifications", true);
int duration = GetConfigValue("NotificationDuration", 3000);
var servers = GetConfigValue("ServerList", new List<string>());

// Check if value exists
if (Context.Config.HasValue("CustomSetting"))
{
    // Handle existing setting
}

// Delete configuration
Context.Config.DeleteValue("ObsoleteSetting");
```

### Configuration UI

```csharp
public override bool HasConfiguration => true;

public override void ShowConfiguration(Form parentForm)
{
    var configForm = new MyPluginConfigForm(this);
    configForm.ShowDialog(parentForm);
}
```

### Configuration Validation

```csharp
public override bool ValidateConfiguration()
{
    // Check required settings
    if (!Context.Config.HasValue("RequiredSetting"))
    {
        LogError("Required setting is missing");
        return false;
    }
    
    // Validate setting values
    int timeout = GetConfigValue("Timeout", 5000);
    if (timeout < 1000 || timeout > 60000)
    {
        LogError("Timeout must be between 1000 and 60000ms");
        return false;
    }
    
    return base.ValidateConfiguration();
}
```

## UI Integration

### Showing Messages

```csharp
// Simple message
ShowMessage("Hello World!");

// Message with title and type
ShowMessage("Operation completed successfully", "Success", MessageType.Success);
Context.UI.ShowMessage("Warning message", "Warning", MessageType.Warning);
Context.UI.ShowMessage("Error occurred", "Error", MessageType.Error);
```

### Custom Forms

```csharp
public class MyPluginForm : Form
{
    public MyPluginForm()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        this.Text = "My Plugin";
        this.Size = new Size(400, 300);
        // Add controls...
    }
}

// Show form
var form = new MyPluginForm();
Context.UI.ShowForm(form, modal: true);
```

### Adding UI Elements

```csharp
// Add menu item (placeholder - implementation depends on UI framework)
Context.UI.AddMenuItem("My Plugin Action", OnMenuItemClick);

// Add button to settings
Context.UI.AddButton("Plugin Settings", OnSettingsClick, UILocation.Settings);
```

## Minecraft Integration

### Version Information

```csharp
// Get installed versions
var versions = Context.Minecraft.GetInstalledVersions();
LogInfo($"Found {versions.Count} installed versions");

// Get current version
string currentVersion = Context.Minecraft.GetCurrentVersion();
LogInfo($"Current version: {currentVersion}");

// Get Minecraft path
string mcPath = Context.Minecraft.GetMinecraftPath();
LogInfo($"Minecraft path: {mcPath}");
```

### Launch Settings

```csharp
// Get current launch settings
var settings = Context.Minecraft.GetLaunchSettings();
int ram = (int)settings["RamMb"];
bool fullscreen = (bool)settings["FullScreen"];

// Modify launch arguments
public string ModifyLaunchArguments(string arguments)
{
    // Add custom arguments
    return arguments + " -Dmy.custom.property=value";
}
```

## Inter-Plugin Communication

### Finding Other Plugins

```csharp
// Get specific plugin
var otherPlugin = Context.GetPlugin("company.otherplugin");
if (otherPlugin != null && otherPlugin.IsEnabled)
{
    // Interact with other plugin
}

// Get all plugins
var allPlugins = Context.GetAllPlugins();
foreach (var plugin in allPlugins)
{
    LogInfo($"Found plugin: {plugin.Name} v{plugin.Version}");
}
```

### Plugin Messaging

```csharp
// Register message handler
Context.RegisterMessageHandler("MyMessage", OnMessageReceived);

// Send message to another plugin
Context.SendPluginMessage("target.plugin", "HelloMessage", "Hello from my plugin!");

private void OnMessageReceived(string message, object data)
{
    LogInfo($"Received message: {message}, Data: {data}");
}
```

### Dependencies

```csharp
public override List<string> Dependencies => new List<string>
{
    "required.plugin1",
    "required.plugin2"
};
```

## Security Considerations

### Safe Coding Practices

```csharp
// Always use SafeExecute for risky operations
SafeExecute(() =>
{
    // Potentially risky code
    FileHelper.DeleteFile(filePath);
}, "Failed to delete file");

// Validate inputs
private bool ValidateInput(string input)
{
    if (string.IsNullOrEmpty(input))
        return false;
    
    // Add validation logic
    return true;
}
```

### File System Access

```csharp
// Use plugin data directory for file operations
string dataDir = Context.PluginDataDirectory;
string configFile = Path.Combine(dataDir, "config.json");

// Always check file paths
if (!filePath.StartsWith(dataDir))
{
    LogError("Invalid file path - outside plugin directory");
    return;
}
```

## Testing and Debugging

### Logging

```csharp
// Use plugin logger
LogInfo("Information message");
LogWarning("Warning message");
LogError("Error message");
LogDebug("Debug message");

// Logger automatically includes plugin ID
// Output: [Plugin:mycompany.myplugin] Information message
```

### Status Reporting

```csharp
public override PluginStatus GetStatus()
{
    var status = new PluginStatus
    {
        IsHealthy = true,
        StatusMessage = "All systems operational",
        LastUpdate = DateTime.Now
    };
    
    // Add warnings or errors
    if (HasIssues())
    {
        status.IsHealthy = false;
        status.Errors.Add("Configuration error");
        status.Warnings.Add("Performance degraded");
    }
    
    return status;
}
```

### Testing Framework

```csharp
// Create test plugin context
public class TestPluginContext : IPluginContext
{
    // Implement test versions of all interfaces
    // Use for unit testing
}

[Test]
public void TestPluginFunctionality()
{
    var plugin = new MyPlugin();
    var context = new TestPluginContext();
    
    plugin.OnLoad(context);
    
    // Test plugin behavior
    Assert.IsTrue(plugin.IsEnabled);
}
```

## Distribution and Installation

### Building Plugin

```bash
# Build plugin
dotnet build -c Release

# Copy to plugins directory
copy bin\Release\net8.0-windows\MyPlugin.dll "C:\Users\Username\AppData\Roaming\.cloudlauncher\plugins\"
```

### Plugin Manifest (Optional)

```json
{
  "id": "mycompany.myplugin",
  "name": "My Plugin",
  "version": "1.0.0",
  "author": "My Company",
  "description": "Plugin description",
  "requiredLauncherVersion": "1.0.0",
  "dependencies": ["required.plugin"],
  "assembly": "MyPlugin.dll",
  "website": "https://example.com",
  "supportUrl": "https://example.com/support"
}
```

### Installation Methods

1. **Manual**: Copy DLL to plugins directory
2. **Drag & Drop**: Drop DLL onto launcher
3. **Plugin Manager**: Use built-in plugin manager UI
4. **Package Manager**: Use package manager for distribution

## Advanced Features

### Custom Event Types

```csharp
public interface IMyCustomEvent : IEvent
{
    string CustomProperty { get; set; }
}

public class MyCustomEvent : BaseEvent, IMyCustomEvent
{
    public string CustomProperty { get; set; }
}
```

### Plugin Hooks

```csharp
// Hook into launcher methods
public class GameLaunchHook
{
    public static void PreLaunch(GameLaunchEvent e)
    {
        // Modify launch parameters
    }
    
    public static void PostLaunch(GameLaunchedEvent e)
    {
        // React to launch result
    }
}
```

### Resource Management

```csharp
public class ResourceManager : IDisposable
{
    private bool _disposed = false;
    
    public void Dispose()
    {
        if (!_disposed)
        {
            // Cleanup resources
            _disposed = true;
        }
    }
}
```

## API Reference

### IPlugin Interface

```csharp
public interface IPlugin
{
    string PluginId { get; }
    string Name { get; }
    string Description { get; }
    string Version { get; }
    string Author { get; }
    string RequiredLauncherVersion { get; }
    List<string> Dependencies { get; }
    bool IsEnabled { get; set; }
    bool HasConfiguration { get; }
    
    void OnLoad(IPluginContext context);
    void OnEnable();
    void OnDisable();
    void OnUnload();
    void ShowConfiguration(Form parentForm);
    bool ValidateConfiguration();
    PluginStatus GetStatus();
}
```

### IPluginContext Interface

```csharp
public interface IPluginContext
{
    string LauncherVersion { get; }
    MSession CurrentSession { get; }
    string WorkingDirectory { get; }
    string PluginDataDirectory { get; }
    IEventManager Events { get; }
    IPluginConfig Config { get; }
    IPluginLogger Logger { get; }
    IPluginUI UI { get; }
    IMinecraftAPI Minecraft { get; }
    
    IPlugin GetPlugin(string pluginId);
    List<IPlugin> GetAllPlugins();
    bool IsPluginEnabled(string pluginId);
    void SendPluginMessage(string targetPluginId, string message, object data = null);
    void RegisterMessageHandler(string message, Action<string, object> handler);
}
```

## Examples

### Simple Event Logger

```csharp
public class EventLoggerPlugin : BasePlugin
{
    public override string PluginId => "example.eventlogger";
    public override string Name => "Event Logger";
    public override string Description => "Logs all launcher events";
    public override string Version => "1.0.0";
    public override string Author => "Example Corp";

    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        
        // Subscribe to all events
        Context.Events.Subscribe<UserLoginEvent>(e => LogInfo($"Login: {e.Username}"));
        Context.Events.Subscribe<GameLaunchEvent>(e => LogInfo($"Launch: {e.Version}"));
        Context.Events.Subscribe<GameLaunchedEvent>(e => LogInfo($"Launched: {e.LaunchSuccess}"));
    }
}
```

### Performance Monitor

```csharp
public class PerformanceMonitorPlugin : BasePlugin
{
    private System.Timers.Timer _timer;
    private PerformanceCounter _cpuCounter;
    private PerformanceCounter _ramCounter;

    public override string PluginId => "example.perfmon";
    public override string Name => "Performance Monitor";
    public override string Description => "Monitors system performance";
    public override string Version => "1.0.0";
    public override string Author => "Example Corp";

    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        InitializeCounters();
        StartMonitoring();
    }

    private void InitializeCounters()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    }

    private void StartMonitoring()
    {
        _timer = new System.Timers.Timer(5000); // 5 seconds
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        float cpu = _cpuCounter.NextValue();
        float ram = _ramCounter.NextValue();
        
        LogInfo($"CPU: {cpu:F1}%, Available RAM: {ram:F0}MB");
        
        // Warn if performance is poor
        if (cpu > 90)
        {
            ShowMessage("High CPU usage detected!", "Performance Warning", MessageType.Warning);
        }
    }

    public override void OnUnload()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _cpuCounter?.Dispose();
        _ramCounter?.Dispose();
        base.OnUnload();
    }
}
```

### Auto-Updater

```csharp
public class AutoUpdaterPlugin : BasePlugin
{
    public override string PluginId => "example.autoupdate";
    public override string Name => "Auto Updater";
    public override string Description => "Automatically updates Minecraft versions";
    public override string Version => "1.0.0";
    public override string Author => "Example Corp";

    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        Context.Events.Subscribe<ApplicationStartEvent>(OnApplicationStart);
    }

    private async void OnApplicationStart(ApplicationStartEvent e)
    {
        if (GetConfigValue("AutoUpdateEnabled", false))
        {
            await CheckForUpdates();
        }
    }

    private async Task CheckForUpdates()
    {
        try
        {
            var installedVersions = Context.Minecraft.GetInstalledVersions();
            var currentVersion = Context.Minecraft.GetCurrentVersion();
            
            // Check for updates logic
            LogInfo("Checking for updates...");
            
            // Implement update checking
        }
        catch (Exception ex)
        {
            LogError($"Update check failed: {ex.Message}");
        }
    }
}
```

## Best Practices

### 1. Error Handling

```csharp
// Always use try-catch for external operations
try
{
    // Risky operation
    var result = ExternalAPI.GetData();
    ProcessData(result);
}
catch (Exception ex)
{
    LogError($"Operation failed: {ex.Message}");
    // Don't crash the launcher
}

// Use SafeExecute for convenience
SafeExecute(() => RiskyOperation(), "Risky operation failed");
```

### 2. Resource Management

```csharp
// Implement IDisposable for resource cleanup
public class MyPlugin : BasePlugin, IDisposable
{
    private HttpClient _httpClient;
    private Timer _timer;
    
    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        _httpClient = new HttpClient();
        _timer = new Timer();
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
        _timer?.Dispose();
    }
    
    public override void OnUnload()
    {
        Dispose();
        base.OnUnload();
    }
}
```

### 3. Performance

```csharp
// Use async methods for I/O operations
private async Task<string> DownloadDataAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}

// Cache expensive operations
private readonly Dictionary<string, object> _cache = new();

private T GetCachedValue<T>(string key, Func<T> factory)
{
    if (!_cache.ContainsKey(key))
    {
        _cache[key] = factory();
    }
    return (T)_cache[key];
}
```

### 4. Configuration

```csharp
// Provide sensible defaults
private const int DEFAULT_TIMEOUT = 5000;
private const bool DEFAULT_ENABLED = true;

// Validate configuration
private bool ValidateTimeout(int timeout)
{
    return timeout > 0 && timeout <= 60000;
}

// Use configuration classes
public class PluginConfig
{
    public bool Enabled { get; set; } = true;
    public int Timeout { get; set; } = 5000;
    public List<string> Servers { get; set; } = new();
}
```

### 5. Logging

```csharp
// Use appropriate log levels
LogDebug("Detailed debugging information");
LogInfo("General information");
LogWarning("Something unexpected happened");
LogError("An error occurred");

// Include context in log messages
LogInfo($"Processing user {username} with version {version}");
```

### 6. Event Handling

```csharp
// Check event data before processing
private void OnGameLaunch(GameLaunchEvent e)
{
    if (e == null || string.IsNullOrEmpty(e.Version))
    {
        LogWarning("Invalid game launch event");
        return;
    }
    
    // Process event
}

// Don't block event processing
private async void OnGameLaunch(GameLaunchEvent e)
{
    // Use async for time-consuming operations
    await ProcessLaunchAsync(e);
}
```

## Troubleshooting

### Common Issues

#### 1. Plugin Not Loading

**Symptoms**: Plugin doesn't appear in plugin list

**Solutions**:
- Check DLL is in correct directory
- Verify plugin implements IPlugin interface
- Check for compilation errors
- Ensure .NET version compatibility

#### 2. Events Not Firing

**Symptoms**: Event handlers not called

**Solutions**:
- Verify event subscription in OnLoad
- Check event handler method signature
- Ensure plugin is enabled
- Check for exceptions in event handlers

#### 3. Configuration Not Persisting

**Symptoms**: Settings reset after restart

**Solutions**:
- Use Context.Config methods
- Check registry permissions
- Verify key names are consistent
- Handle serialization issues

#### 4. UI Issues

**Symptoms**: Forms not displaying correctly

**Solutions**:
- Check thread safety (use Invoke for UI updates)
- Verify form initialization
- Check parent form references
- Test on different DPI settings

### Debugging Tips

1. **Enable Debug Logging**:
   ```csharp
   LogDebug("Debug message");
   ```

2. **Check Plugin Status**:
   ```csharp
   public override PluginStatus GetStatus()
   {
       var status = base.GetStatus();
       status.StatusMessage = "Custom status info";
       return status;
   }
   ```

3. **Use Debugger**:
   - Attach debugger to CloudLauncher.exe
   - Set breakpoints in plugin code
   - Step through event handlers

4. **Monitor Registry**:
   - Use Registry Editor to check saved settings
   - Look for plugin-specific keys

### Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| "Plugin validation failed" | Missing required properties | Implement all required properties |
| "Dependency not found" | Missing dependency | Install required plugins |
| "Configuration invalid" | Invalid settings | Fix configuration validation |
| "Event handler exception" | Exception in event handler | Add try-catch blocks |

## Conclusion

The CloudLauncher plugin system provides a powerful and flexible way to extend the launcher's functionality. By following this guide and the best practices outlined, you can create robust, efficient, and user-friendly plugins that enhance the CloudLauncher experience.

For additional support and examples, visit the CloudLauncher GitHub repository or join the community forums.

---

*This documentation covers CloudLauncher Plugin System v1.0.0. For the latest updates and changes, please refer to the official repository.* 