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

### Network Usage
- **Version Metadata**: Reduced from daily to every 6 hours
- **User Avatars**: Cached for a week, significant bandwidth savings
- **Java Discovery**: Cached for 24 hours, faster game launches

### Memory Usage
- **Memory Cache**: Frequently accessed items kept in memory
- **File Cache**: Persistent storage for application restarts
- **Efficient Storage**: JSON serialization with compression

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

### Error Handling
- **Network Failures**: Fallback to cached data when possible
- **File Corruption**: Automatic recovery and re-caching
- **Permission Issues**: Graceful degradation without caching 