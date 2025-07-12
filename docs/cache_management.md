# Cache Management

CloudLauncher includes a comprehensive caching system to improve performance by storing frequently accessed data locally. This reduces network requests and speeds up application startup.

## What is Cached

### Minecraft Version Metadata
- **Cache Duration**: 6 hours
- **Description**: List of available Minecraft versions (release, snapshot, alpha, beta)
- **Benefits**: Faster version dropdown loading, offline browsing of versions

### User Avatars
- **Cache Duration**: 7 days
- **Description**: Player avatars from mc-heads.net
- **Benefits**: Instant avatar loading, reduced bandwidth usage

### Installed Versions
- **Cache Duration**: 5 minutes
- **Description**: Local scan of installed Minecraft versions
- **Benefits**: Faster startup, reduced filesystem scanning

### Java Path Discovery
- **Cache Duration**: 24 hours
- **Description**: Automatically discovered Java paths for different Minecraft versions
- **Benefits**: Faster game launching, reduced system scanning

### UI Rendering Elements
- **Cache Duration**: 8-24 hours
- **Description**: Pre-rendered UI components, gradients, backgrounds, and processed images
- **Benefits**: Dramatically faster form rendering, reduced CPU usage, smoother UI

### Theme Resources
- **Cache Duration**: 30 days
- **Description**: Cached color schemes, fonts, and theme assets
- **Benefits**: Instant theme switching, consistent styling

### Form Layouts & Styles
- **Cache Duration**: 7 days
- **Description**: Computed styles, control positions, and layout calculations
- **Benefits**: Faster form initialization, consistent positioning

## Cache Directory Structure

The cache is stored in:
```
%APPDATA%\.cloudlauncher\cache\
```

Each cached item is stored as a JSON file with the following structure:
- `minecraft_versions.cache` - Version metadata
- `installed_versions.cache` - List of installed versions
- `java_path_[version].cache` - Java paths for specific versions
- `image_[hash].cache` - User avatars and other images
- `processed_image_[hash].cache` - Processed/resized images
- `bg_render_[key].cache` - Pre-rendered backgrounds and gradients
- `component_render_[key].cache` - Pre-rendered UI components
- `ui_element_[key].cache` - Cached UI element data
- `computed_style_[key].cache` - Computed styles and layouts
- `form_layout_[form].cache` - Form layout information
- `theme_[name].cache` - Theme resource data

## Cache Management

### Automatic Cleanup
- **Expired Cache**: Cleaned up on application startup
- **Daily Cleanup**: Full cache validation performed once per day
- **Invalid Entries**: Automatically removed when detected

### Manual Management
The application provides the following cache management options:

1. **Clear All Cache**: Removes all cached data
2. **Refresh Versions**: Forces reload of Minecraft version metadata
3. **Cache Size Display**: Shows current cache size in settings

### Cache Settings
Cache behavior is configured through the following options:

- **Auto-cleanup**: Automatically remove expired entries (enabled by default)
- **Cache location**: Fixed to application data directory
- **Maximum age**: Different for each data type (see durations above)

## Performance Benefits

### Startup Time
- **Without Cache**: 3-5 seconds to load all data
- **With Cache**: 0.5-1 second for cached data

### Form Rendering
- **Without UI Cache**: 200-500ms to render complex forms
- **With UI Cache**: 50-100ms using pre-rendered components
- **Background Rendering**: Instant display of cached gradients/backgrounds

### Network Usage
- **Version Metadata**: Reduced from daily to every 6 hours
- **User Avatars**: Cached for a week, significant bandwidth savings
- **Java Discovery**: Cached for 24 hours, faster game launches

### CPU Usage
- **Image Processing**: Avoid repetitive resize/filter operations
- **Gradient Rendering**: Pre-computed backgrounds reduce drawing calls
- **Style Calculations**: Cached computed styles eliminate redundant calculations

### Memory Usage
- **Memory Cache**: Frequently accessed items kept in memory
- **File Cache**: Persistent storage for application restarts
- **Efficient Storage**: JSON serialization with smart compression
- **UI Elements**: Pre-rendered bitmaps reduce real-time drawing

## Troubleshooting

### Cache Issues
If you experience problems with cached data:

1. **Clear Cache**: Use the "Clear Cache" button in settings
2. **Refresh Versions**: Force reload version metadata
3. **Check Logs**: Look for cache-related errors in application logs

### Cache Corruption
If cache files become corrupted:
- The application will automatically detect and remove invalid entries
- Fresh data will be downloaded and cached
- No user intervention required

### Disk Space
- Cache size is typically 1-5 MB
- Automatically cleaned up daily
- Can be manually cleared if needed

## Technical Details

### Cache Implementation
- **Singleton Pattern**: Single CacheManager instance
- **Thread-Safe**: Concurrent access supported
- **Expiration**: Time-based cache invalidation
- **Persistence**: File-based storage with memory layer

### Cache Keys
- Version metadata: `minecraft_versions`
- Installed versions: `installed_versions`
- Java paths: `java_path_[version_name]`
- Images: `image_[url_hash]`
- Processed images: `processed_image_[path_hash]`
- UI elements: `ui_element_[element_key]`
- Rendered backgrounds: `bg_render_[background_key]`
- Pre-rendered components: `component_render_[component_key]`
- Computed styles: `computed_style_[style_key]`
- Form layouts: `form_layout_[form_name]`
- Theme resources: `theme_[theme_name]`
- Gradient backgrounds: `gradient_[key]_[size]_[colors]_[mode]`

### Error Handling
- **Network Failures**: Fallback to cached data when possible
- **File Corruption**: Automatic recovery and re-caching
- **Permission Issues**: Graceful degradation without caching 