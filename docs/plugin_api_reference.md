# CloudLauncher Plugin API Reference

## Quick Reference

### Core Interfaces

#### IPlugin
```csharp
public interface IPlugin
{
    // Required Properties
    string PluginId { get; }                    // Unique identifier
    string Name { get; }                        // Display name
    string Description { get; }                 // Brief description
    string Version { get; }                     // Plugin version
    string Author { get; }                      // Author name
    
    // Optional Properties
    string RequiredLauncherVersion { get; }     // Min launcher version
    List<string> Dependencies { get; }          // Required plugins
    bool HasConfiguration { get; }              // Has config UI
    bool IsEnabled { get; set; }               // Enable/disable state
    
    // Lifecycle Methods
    void OnLoad(IPluginContext context);       // Plugin loaded
    void OnEnable();                           // Plugin enabled
    void OnDisable();                          // Plugin disabled
    void OnUnload();                           // Plugin unloaded
    
    // Configuration
    void ShowConfiguration(Form parentForm);   // Show config UI
    bool ValidateConfiguration();              // Validate settings
    
    // Status
    PluginStatus GetStatus();                  // Get plugin status
}
```

#### IPluginContext
```csharp
public interface IPluginContext
{
    // Properties
    string LauncherVersion { get; }            // Launcher version
    MSession CurrentSession { get; }           // Current user session
    string WorkingDirectory { get; }           // Launcher working dir
    string PluginDataDirectory { get; }        // Plugin data dir
    
    // Services
    IEventManager Events { get; }              // Event system
    IPluginConfig Config { get; }              // Configuration
    IPluginLogger Logger { get; }              // Logging
    IPluginUI UI { get; }                      // UI integration
    IMinecraftAPI Minecraft { get; }           // Minecraft API
    
    // Plugin Management
    IPlugin GetPlugin(string pluginId);       // Get plugin by ID
    List<IPlugin> GetAllPlugins();            // Get all plugins
    bool IsPluginEnabled(string pluginId);    // Check if enabled
    
    // Messaging
    void SendPluginMessage(string targetPluginId, string message, object data = null);
    void RegisterMessageHandler(string message, Action<string, object> handler);
}
```

### Service Interfaces

#### IPluginConfig
```csharp
public interface IPluginConfig
{
    T GetValue<T>(string key, T defaultValue = default);     // Get config value
    void SetValue<T>(string key, T value);                   // Set config value
    void DeleteValue(string key);                            // Delete config value
    bool HasValue(string key);                               // Check if exists
    List<string> GetKeys();                                  // Get all keys
}
```

#### IPluginLogger
```csharp
public interface IPluginLogger
{
    void Info(string message);                               // Log info
    void Warning(string message);                            // Log warning
    void Error(string message);                              // Log error
    void Debug(string message);                              // Log debug
}
```

#### IPluginUI
```csharp
public interface IPluginUI
{
    void ShowMessage(string message, string title = "Plugin Message", MessageType type = MessageType.Info);
    void AddMenuItem(string text, Action onClick, string parentMenu = null);
    void AddButton(string text, Action onClick, UILocation location = UILocation.Settings);
    void ShowForm(Form form, bool modal = false);
}
```

#### IMinecraftAPI
```csharp
public interface IMinecraftAPI
{
    List<string> GetInstalledVersions();                     // Get installed versions
    string GetCurrentVersion();                              // Get current version
    string GetMinecraftPath();                               // Get MC path
    Dictionary<string, object> GetLaunchSettings();          // Get launch settings
    string ModifyLaunchArguments(string arguments);          // Modify launch args
}
```

#### IEventManager
```csharp
public interface IEventManager
{
    void Subscribe<T>(Action<T> handler) where T : IEvent;   // Subscribe to event
    void Subscribe<T>(Action<T> handler, int priority) where T : IEvent;  // Subscribe with priority
    void Unsubscribe<T>(Action<T> handler) where T : IEvent; // Unsubscribe
    void Publish<T>(T eventData) where T : IEvent;          // Publish event
    void Clear();                                            // Clear all subscriptions
}
```

## Event Types

### Core Events
- `ApplicationStartEvent` - Launcher starts
- `ApplicationExitEvent` - Launcher exits (cancellable)
- `UserLoginEvent` - User logs in
- `UserLogoutEvent` - User logs out

### Game Events
- `GameLaunchEvent` - Before game launch (cancellable)
- `GameLaunchedEvent` - After game launch attempt
- `GameExitEvent` - Game process exits
- `VersionSelectedEvent` - Version selection changed
- `VersionInstallStartEvent` - Version installation begins
- `VersionInstallCompleteEvent` - Version installation complete

### System Events
- `SettingsChangedEvent` - Settings modified
- `ThemeChangedEvent` - UI theme changed
- `NotificationEvent` - Notification shown
- `ErrorEvent` - Error occurred
- `WarningEvent` - Warning issued

### Plugin Events
- `PluginLoadedEvent` - Plugin loaded
- `PluginEnabledEvent` - Plugin enabled
- `PluginDisabledEvent` - Plugin disabled
- `PluginUnloadedEvent` - Plugin unloaded

## Base Classes

### BasePlugin
```csharp
public abstract class BasePlugin : IPlugin
{
    // Properties (must implement)
    public abstract string PluginId { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Version { get; }
    public abstract string Author { get; }
    
    // Properties (virtual)
    public virtual string RequiredLauncherVersion => "1.0.0";
    public virtual List<string> Dependencies => new List<string>();
    public virtual bool HasConfiguration => false;
    public bool IsEnabled { get; set; } = true;
    
    // Context
    protected IPluginContext Context { get; private set; }
    
    // Lifecycle (virtual)
    public virtual void OnLoad(IPluginContext context);
    public virtual void OnEnable();
    public virtual void OnDisable();
    public virtual void OnUnload();
    
    // Configuration (virtual)
    public virtual void ShowConfiguration(Form parentForm);
    public virtual bool ValidateConfiguration();
    public virtual PluginStatus GetStatus();
    
    // Utility Methods
    protected void SafeExecute(Action action, string errorMessage = "An error occurred");
    protected T SafeExecute<T>(Func<T> function, T defaultValue = default, string errorMessage = "An error occurred");
    protected T GetConfigValue<T>(string key, T defaultValue = default);
    protected void SetConfigValue<T>(string key, T value);
    protected void LogInfo(string message);
    protected void LogWarning(string message);
    protected void LogError(string message);
    protected void LogDebug(string message);
    protected void ShowMessage(string message, string title = null, MessageType type = MessageType.Info);
}
```

### BaseEvent
```csharp
public abstract class BaseEvent : IEvent
{
    public DateTime Timestamp { get; private set; }
    public bool IsHandled { get; set; }
    public Dictionary<string, object> Data { get; private set; }
    
    protected BaseEvent()
    {
        Timestamp = DateTime.Now;
        IsHandled = false;
        Data = new Dictionary<string, object>();
    }
}
```

### BaseCancellableEvent
```csharp
public abstract class BaseCancellableEvent : BaseEvent, ICancellableEvent
{
    public bool IsCancelled { get; set; }
    public string CancellationReason { get; set; }
    
    protected BaseCancellableEvent()
    {
        IsCancelled = false;
        CancellationReason = null;
    }
}
```

## Enumerations

### MessageType
```csharp
public enum MessageType
{
    Info,
    Warning,
    Error,
    Success
}
```

### UILocation
```csharp
public enum UILocation
{
    Settings,
    MainMenu,
    GameLaunch,
    UserProfile
}
```

## Data Classes

### PluginStatus
```csharp
public class PluginStatus
{
    public bool IsHealthy { get; set; } = true;
    public string StatusMessage { get; set; } = "OK";
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}
```

## Common Patterns

### Basic Plugin Structure
```csharp
public class MyPlugin : BasePlugin
{
    public override string PluginId => "company.myplugin";
    public override string Name => "My Plugin";
    public override string Description => "Plugin description";
    public override string Version => "1.0.0";
    public override string Author => "My Company";
    
    public override void OnLoad(IPluginContext context)
    {
        base.OnLoad(context);
        // Initialize plugin
        Context.Events.Subscribe<UserLoginEvent>(OnUserLogin);
    }
    
    private void OnUserLogin(UserLoginEvent e)
    {
        LogInfo($"User logged in: {e.Username}");
    }
}
```

### Event Handling
```csharp
// Subscribe to event
Context.Events.Subscribe<GameLaunchEvent>(OnGameLaunch);

// Handle event
private void OnGameLaunch(GameLaunchEvent e)
{
    // Read event data
    LogInfo($"Launching {e.Version}");
    
    // Modify event
    e.JvmArguments.Add("-XX:+UseG1GC");
    
    // Cancel event if needed
    if (shouldCancel)
    {
        e.IsCancelled = true;
        e.CancellationReason = "Cancelled by plugin";
    }
}
```

### Configuration Management
```csharp
// Save configuration
SetConfigValue("EnableFeature", true);
SetConfigValue("Timeout", 5000);

// Load configuration
bool enabled = GetConfigValue("EnableFeature", false);
int timeout = GetConfigValue("Timeout", 3000);
```

### UI Integration
```csharp
// Show message
ShowMessage("Hello World!", "My Plugin", MessageType.Info);

// Show custom form
var form = new MyConfigForm();
Context.UI.ShowForm(form, modal: true);
```

### Error Handling
```csharp
// Safe execution
SafeExecute(() => 
{
    // Risky operation
    DoSomethingRisky();
}, "Operation failed");

// Try-catch pattern
try
{
    // Code that might fail
}
catch (Exception ex)
{
    LogError($"Error: {ex.Message}");
}
```

## File Structure

```
MyPlugin/
├── MyPlugin.cs              # Main plugin class
├── Events/                  # Custom event classes
├── Forms/                   # UI forms
├── Services/                # Plugin services
├── Config/                  # Configuration classes
├── MyPlugin.csproj          # Project file
└── README.md               # Plugin documentation
```

## Build Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <OutputType>Library</OutputType>
  </PropertyGroup>
  
  <ItemGroup>
    <Reference Include="CloudLauncher">
      <HintPath>path\to\CloudLauncher.exe</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

## Registry Keys

Plugin configuration is stored in the Windows Registry under:
```
HKEY_CURRENT_USER\Software\MythicalSystems\CloudLauncher\Plugin_{PluginId}_{Key}
```

Example:
```
HKEY_CURRENT_USER\Software\MythicalSystems\CloudLauncher\Plugin_mycompany.myplugin_EnableNotifications
```

## File Locations

- **Plugins Directory**: `%APPDATA%\.cloudlauncher\plugins\`
- **Plugin Data Directory**: `%APPDATA%\.cloudlauncher\plugins\data\{PluginId}\`
- **Launcher Working Directory**: `%APPDATA%\.cloudlauncher\`

## Common Issues

1. **Plugin not loading**: Check DLL is in plugins directory and implements IPlugin
2. **Events not firing**: Verify subscription in OnLoad() method
3. **Configuration not saving**: Use Context.Config methods, not direct registry access
4. **UI thread issues**: Use Form.Invoke() for UI updates from background threads

## Version Compatibility

| Plugin API | Launcher Version | .NET Version |
|------------|------------------|--------------|
| 1.0.0      | 1.0.0+          | .NET 8.0+    |

---

*This reference covers CloudLauncher Plugin API v1.0.0* 