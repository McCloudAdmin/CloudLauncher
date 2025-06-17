# CloudLauncher Styling System Documentation

## Overview
The CloudLauncher styling system uses YAML files to define the appearance of forms and controls. The styles are applied at runtime and support a hierarchical structure with global and form-specific styles.

## File Location
Styles are defined in `styles.yaml` located in the application's working directory.

## Style Hierarchy
Styles are applied in the following order (highest to lowest priority):
1. Form-specific control styles (e.g., `FrmMain.lblAppName`)
2. Global control styles (e.g., `lblAppName`)
3. Form-specific styles (e.g., `FrmMain`)
4. Global styles (`Global`)

## YAML Structure

### Global Styles
```yaml
Global:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  Font: Segoe UI, 9pt
```

### Form-Specific Styles
```yaml
FrmMain:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  FormBorderStyle: None
  StartPosition: CenterScreen
  Size: 800,600
```

### Form-Specific Control Styles
```yaml
FrmMain.lblAppName:
  Text: CloudLauncher (DEV)
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt, style=Bold
  Location: 10,5
```

### Global Control Styles
```yaml
lblAppName:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
```

## Supported Properties

### Colors
Colors can be specified in two ways:
1. RGB values: `BackColor: 30,30,30`
2. Named colors: `ForeColor: White`

### Fonts
Fonts are specified with name, size, and optional style:
```yaml
Font: Segoe UI, 10pt, style=Bold
```
Available styles:
- Bold
- Italic
- Underline
- Regular

### Size and Location
- Size: `Size: 800,600`
- Location: `Location: 10,5`

### Form Properties
- `FormBorderStyle`: None, FixedSingle, Fixed3D, FixedDialog, Sizable, FixedToolWindow, SizableToolWindow
- `StartPosition`: Manual, CenterScreen, CenterParent, WindowsDefaultLocation, WindowsDefaultBounds

### Cursor
Available cursors:
- Hand
- Arrow
- Wait
- Cross
- IBeam
- Default

### Dock Style
Available dock styles:
- None
- Top
- Bottom
- Left
- Right
- Fill

## Example YAML File
```yaml
# Global styles (applied to all forms)
Global:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  Font: Segoe UI, 9pt

# Form-specific styles
FrmMain:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  FormBorderStyle: None
  StartPosition: CenterScreen
  Size: 800,600

# Component-specific styles
UserNavigationBar:
  BackColor: 40,40,40
  Dock: Top
  Height: 30

# Form-specific control styles
FrmMain.lblAppName:
  Text: CloudLauncher (DEV)
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt, style=Bold
  Location: 10,5

FrmMain.lblClose:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
  Location: 770,5
  Cursor: Hand

# Global control styles
lblAppName:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
```

## Best Practices
1. Use global styles for common properties across all forms
2. Use form-specific styles for form-level customization
3. Use form-specific control styles for unique control appearances
4. Use global control styles for consistent control appearances across forms
5. Keep the YAML file well-organized with comments
6. Use RGB values for precise color control
7. Use named colors for standard colors

## Debugging
The styling system includes debug logging. Enable debug mode to see detailed information about style application:
```csharp
UIStyler.ApplyStyles(this, true);
```

## Error Handling
- Invalid properties are logged as warnings
- Missing styles are ignored
- Invalid values are logged with details
- All errors are caught and logged to prevent application crashes 