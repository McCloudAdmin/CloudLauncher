# Registry Configuration System Documentation

## Overview
The Registry Configuration System provides a way to store and retrieve application settings, user preferences, and other configuration data in the Windows Registry. All settings are stored under `HKEY_CURRENT_USER\Software\MythicalSystems\CloudLauncher`.

## Basic Usage

### Setting Values
```csharp
// Set any type of value
RegistryConfig.SetValue("MySetting", "value");
RegistryConfig.SetValue("NumberSetting", 42);
RegistryConfig.SetValue("BooleanSetting", true);
```

### Getting Values
```csharp
// Get values with type safety
string value = RegistryConfig.GetValue("MySetting", "default");
int number = RegistryConfig.GetValue("NumberSetting", 0);
bool flag = RegistryConfig.GetValue("BooleanSetting", false);
```

### Deleting Values
```csharp
// Delete a specific value
RegistryConfig.DeleteValue("MySetting");

// Delete all values
RegistryConfig.DeleteAllValues();
```

### Checking Value Existence
```csharp
bool exists = RegistryConfig.ValueExists("MySetting");
```

## Specialized Methods

### Window Management
```csharp
// Save window position
RegistryConfig.SaveWindowPosition("FrmMain", x, y);

// Get window position
Point position = RegistryConfig.GetWindowPosition("FrmMain", new Point(100, 100));

// Save window size
RegistryConfig.SaveWindowSize("FrmMain", width, height);

// Get window size
Size size = RegistryConfig.GetWindowSize("FrmMain", new Size(800, 600));
```

### Theme Management
```csharp
// Save theme
RegistryConfig.SaveTheme("dark");

// Get theme
string theme = RegistryConfig.GetTheme("default");
```

### Path Management
```csharp
// Save last used path
RegistryConfig.SaveLastUsedPath(@"C:\MyPath");

// Get last used path
string path = RegistryConfig.GetLastUsedPath(@"C:\Default");
```

### User Preferences
```csharp
// Save user preference
RegistryConfig.SaveUserPreference("Language", "en-US");
RegistryConfig.SaveUserPreference("Notifications", true);

// Get user preference
string language = RegistryConfig.GetUserPreference("Language", "en-US");
bool notifications = RegistryConfig.GetUserPreference("Notifications", true);
```

### Application Settings
```csharp
// Save application setting
RegistryConfig.SaveAppSetting("Version", "1.0.0");
RegistryConfig.SaveAppSetting("DebugMode", false);

// Get application setting
string version = RegistryConfig.GetAppSetting("Version", "0.0.0");
bool debugMode = RegistryConfig.GetAppSetting("DebugMode", false);
```

## Best Practices

1. **Use Default Values**: Always provide default values when getting settings
   ```csharp
   string value = RegistryConfig.GetValue("Setting", "default");
   ```

2. **Group Related Settings**: Use consistent prefixes for related settings
   ```csharp
   RegistryConfig.SetValue("Window_Main_X", 100);
   RegistryConfig.SetValue("Window_Main_Y", 100);
   ```

3. **Error Handling**: The system includes built-in error handling and logging
   ```csharp
   try {
       RegistryConfig.SetValue("CriticalSetting", value);
   } catch (Exception ex) {
       // Handle any unexpected errors
   }
   ```

4. **Type Safety**: Use the generic methods for type-safe access
   ```csharp
   // Good
   int value = RegistryConfig.GetValue<int>("Number", 0);
   
   // Avoid
   object value = RegistryConfig.GetValue("Number", 0);
   ```

## Registry Structure
```
HKEY_CURRENT_USER
└── Software
    └── MythicalSystems
        └── CloudLauncher
            ├── Theme
            ├── LastUsedPath
            ├── FrmMain_X
            ├── FrmMain_Y
            ├── FrmMain_Width
            ├── FrmMain_Height
            ├── UserPref_Language
            ├── UserPref_Notifications
            ├── AppSetting_Version
            └── AppSetting_DebugMode
```

## Security Considerations

1. The registry keys are created under the current user's registry hive
2. No administrative privileges are required
3. Settings are user-specific
4. All operations are logged for debugging purposes
5. Error handling is built-in to prevent application crashes 