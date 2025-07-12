using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;

namespace CloudLauncher.utils
{
    public class CacheManager
    {
        private static CacheManager _instance;
        private static readonly object _lock = new object();
        private readonly string _cacheDirectory;
        private readonly Dictionary<string, CacheEntry> _memoryCache;
        private readonly HttpClient _httpClient;

        public static CacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new CacheManager();
                    }
                }
                return _instance;
            }
        }

        private CacheManager()
        {
            // Create cache directory in app data
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheDirectory = Path.Combine(Program.appWorkDir, "cache");
            
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }

            _memoryCache = new Dictionary<string, CacheEntry>();
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            Logger.Info($"Cache directory initialized: {_cacheDirectory}");
        }

        #region Cache Entry Management

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var cacheEntry = new CacheEntry<T>
                {
                    Value = value,
                    ExpirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : DateTime.UtcNow.AddDays(1),
                    CreatedTime = DateTime.UtcNow
                };

                // Save to memory cache
                _memoryCache[key] = cacheEntry;

                // Save to file cache
                var filePath = GetCacheFilePath(key);
                var json = JsonSerializer.Serialize(cacheEntry, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                Logger.Debug($"Cached item with key: {key}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache item {key}: {ex.Message}");
            }
        }

        public T Get<T>(string key)
        {
            try
            {
                // Check memory cache first
                if (_memoryCache.TryGetValue(key, out var memoryCacheEntry))
                {
                    if (memoryCacheEntry.ExpirationTime > DateTime.UtcNow)
                    {
                        if (memoryCacheEntry is CacheEntry<T> typedEntry)
                        {
                            Logger.Debug($"Retrieved from memory cache: {key}");
                            return typedEntry.Value;
                        }
                    }
                    else
                    {
                        // Remove expired entry from memory
                        _memoryCache.Remove(key);
                    }
                }

                // Check file cache
                var filePath = GetCacheFilePath(key);
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var cacheEntry = JsonSerializer.Deserialize<CacheEntry<T>>(json);

                    if (cacheEntry != null && cacheEntry.ExpirationTime > DateTime.UtcNow)
                    {
                        // Add back to memory cache
                        _memoryCache[key] = cacheEntry;
                        Logger.Debug($"Retrieved from file cache: {key}");
                        return cacheEntry.Value;
                    }
                    else
                    {
                        // Remove expired file
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to retrieve cached item {key}: {ex.Message}");
            }

            return default(T);
        }

        public bool HasValidCache(string key)
        {
            try
            {
                // Check memory cache first
                if (_memoryCache.TryGetValue(key, out var memoryCacheEntry))
                {
                    return memoryCacheEntry.ExpirationTime > DateTime.UtcNow;
                }

                // Check file cache
                var filePath = GetCacheFilePath(key);
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var cacheEntry = JsonSerializer.Deserialize<CacheEntry<object>>(json);
                    return cacheEntry != null && cacheEntry.ExpirationTime > DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to check cache validity for {key}: {ex.Message}");
            }

            return false;
        }

        public void Remove(string key)
        {
            try
            {
                // Remove from memory cache
                _memoryCache.Remove(key);

                // Remove from file cache
                var filePath = GetCacheFilePath(key);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                Logger.Debug($"Removed cached item: {key}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to remove cached item {key}: {ex.Message}");
            }
        }

        #endregion

        #region Specialized Cache Methods

        public async Task<Image> GetOrDownloadImageAsync(string url, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"image_{url.GetHashCode()}";
                
                // Check if we have cached image
                if (HasValidCache(cacheKey))
                {
                    var cachedBytes = Get<byte[]>(cacheKey);
                    if (cachedBytes != null)
                    {
                        using (var stream = new MemoryStream(cachedBytes))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // Download image
                var imageBytes = await _httpClient.GetByteArrayAsync(url);
                
                // Cache the image bytes
                Set(cacheKey, imageBytes, expiration ?? TimeSpan.FromHours(24));

                // Return the image
                using (var stream = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get or download image from {url}: {ex.Message}");
                return null;
            }
        }

        public Image GetOrCacheProcessedImage(string imagePath, Func<Image> imageProcessor, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"processed_image_{imagePath.GetHashCode()}";
                
                // Check if we have cached processed image
                if (HasValidCache(cacheKey))
                {
                    var cachedBytes = Get<byte[]>(cacheKey);
                    if (cachedBytes != null)
                    {
                        using (var stream = new MemoryStream(cachedBytes))
                        {
                            Logger.Debug($"Retrieved processed image from cache: {imagePath}");
                            return Image.FromStream(stream);
                        }
                    }
                }

                // Process the image
                var processedImage = imageProcessor();
                if (processedImage != null)
                {
                    // Convert to bytes and cache
                    using (var stream = new MemoryStream())
                    {
                        processedImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        var imageBytes = stream.ToArray();
                        Set(cacheKey, imageBytes, expiration ?? TimeSpan.FromHours(12));
                        Logger.Debug($"Cached processed image: {imagePath}");
                    }
                }

                return processedImage;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to process and cache image {imagePath}: {ex.Message}");
                return null;
            }
        }

        public void CacheUIElement(string elementKey, object elementData, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"ui_element_{elementKey}";
                Set(cacheKey, elementData, expiration ?? TimeSpan.FromHours(6));
                Logger.Debug($"Cached UI element: {elementKey}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache UI element {elementKey}: {ex.Message}");
            }
        }

        public T GetCachedUIElement<T>(string elementKey)
        {
            try
            {
                var cacheKey = $"ui_element_{elementKey}";
                var result = Get<T>(cacheKey);
                if (result != null)
                {
                    Logger.Debug($"Retrieved UI element from cache: {elementKey}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached UI element {elementKey}: {ex.Message}");
                return default(T);
            }
        }

        public void CacheRenderedBackground(string backgroundKey, Image background, TimeSpan? expiration = null)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    background.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    var cacheKey = $"bg_render_{backgroundKey}";
                    Set(cacheKey, stream.ToArray(), expiration ?? TimeSpan.FromDays(1));
                    Logger.Debug($"Cached rendered background: {backgroundKey}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache rendered background {backgroundKey}: {ex.Message}");
            }
        }

        public Image GetCachedRenderedBackground(string backgroundKey)
        {
            try
            {
                var cacheKey = $"bg_render_{backgroundKey}";
                var cachedBytes = Get<byte[]>(cacheKey);
                if (cachedBytes != null)
                {
                    using (var stream = new MemoryStream(cachedBytes))
                    {
                        Logger.Debug($"Retrieved rendered background from cache: {backgroundKey}");
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached rendered background {backgroundKey}: {ex.Message}");
            }
            return null;
        }

        public void CacheFormLayout(string formName, FormLayoutData layoutData, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"form_layout_{formName}";
                Set(cacheKey, layoutData, expiration ?? TimeSpan.FromDays(7));
                Logger.Debug($"Cached form layout: {formName}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache form layout {formName}: {ex.Message}");
            }
        }

        public FormLayoutData GetCachedFormLayout(string formName)
        {
            try
            {
                var cacheKey = $"form_layout_{formName}";
                var result = Get<FormLayoutData>(cacheKey);
                if (result != null)
                {
                    Logger.Debug($"Retrieved form layout from cache: {formName}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached form layout {formName}: {ex.Message}");
                return null;
            }
        }

        public void CacheThemeResources(string themeName, ThemeResourceData themeData, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"theme_{themeName}";
                Set(cacheKey, themeData, expiration ?? TimeSpan.FromDays(30));
                Logger.Debug($"Cached theme resources: {themeName}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache theme resources {themeName}: {ex.Message}");
            }
        }

        public ThemeResourceData GetCachedThemeResources(string themeName)
        {
            try
            {
                var cacheKey = $"theme_{themeName}";
                var result = Get<ThemeResourceData>(cacheKey);
                if (result != null)
                {
                    Logger.Debug($"Retrieved theme resources from cache: {themeName}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached theme resources {themeName}: {ex.Message}");
                return null;
            }
        }

        public void CachePreRenderedComponent(string componentKey, Control component, TimeSpan? expiration = null)
        {
            try
            {
                // Render the control to a bitmap
                var bitmap = new Bitmap(component.Width, component.Height);
                component.DrawToBitmap(bitmap, new Rectangle(0, 0, component.Width, component.Height));
                
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    var cacheKey = $"component_render_{componentKey}";
                    
                    var renderData = new UIRenderingData
                    {
                        RenderedImageData = stream.ToArray(),
                        ImageSize = component.Size,
                        RenderingParameters = $"Size:{component.Size},BackColor:{component.BackColor},Font:{component.Font?.Name}"
                    };
                    
                    Set(cacheKey, renderData, expiration ?? TimeSpan.FromHours(8));
                    Logger.Debug($"Cached pre-rendered component: {componentKey}");
                }
                
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache pre-rendered component {componentKey}: {ex.Message}");
            }
        }

        public Image GetCachedPreRenderedComponent(string componentKey)
        {
            try
            {
                var cacheKey = $"component_render_{componentKey}";
                var renderData = Get<UIRenderingData>(cacheKey);
                
                if (renderData?.RenderedImageData != null)
                {
                    using (var stream = new MemoryStream(renderData.RenderedImageData))
                    {
                        Logger.Debug($"Retrieved pre-rendered component from cache: {componentKey}");
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached pre-rendered component {componentKey}: {ex.Message}");
            }
            return null;
        }

        public void CacheComputedStyle(string styleKey, Dictionary<string, object> computedStyle, TimeSpan? expiration = null)
        {
            try
            {
                var cacheKey = $"computed_style_{styleKey}";
                Set(cacheKey, computedStyle, expiration ?? TimeSpan.FromHours(12));
                Logger.Debug($"Cached computed style: {styleKey}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache computed style {styleKey}: {ex.Message}");
            }
        }

        public Dictionary<string, object> GetCachedComputedStyle(string styleKey)
        {
            try
            {
                var cacheKey = $"computed_style_{styleKey}";
                var result = Get<Dictionary<string, object>>(cacheKey);
                if (result != null)
                {
                    Logger.Debug($"Retrieved computed style from cache: {styleKey}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached computed style {styleKey}: {ex.Message}");
                return null;
            }
        }

        public void CacheVersionMetadata(string versionsJson)
        {
            try
            {
                Set("minecraft_versions", versionsJson, TimeSpan.FromHours(6));
                Logger.Info("Minecraft version metadata cached");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache version metadata: {ex.Message}");
            }
        }

        public string GetCachedVersionMetadata()
        {
            try
            {
                return Get<string>("minecraft_versions");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cached version metadata: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Cache Maintenance

        public void ClearExpiredCache()
        {
            try
            {
                var expiredKeys = new List<string>();
                
                // Check memory cache
                foreach (var kvp in _memoryCache)
                {
                    if (kvp.Value.ExpirationTime <= DateTime.UtcNow)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                // Remove expired items from memory
                foreach (var key in expiredKeys)
                {
                    _memoryCache.Remove(key);
                }

                // Clean up file cache
                var cacheFiles = Directory.GetFiles(_cacheDirectory, "*.cache");
                foreach (var file in cacheFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var cacheEntry = JsonSerializer.Deserialize<CacheEntry<object>>(json);
                        
                        if (cacheEntry != null && cacheEntry.ExpirationTime <= DateTime.UtcNow)
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // If we can't read the file, delete it
                        File.Delete(file);
                    }
                }

                Logger.Info($"Cleared {expiredKeys.Count} expired cache entries");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to clear expired cache: {ex.Message}");
            }
        }

        public void ClearAllCache()
        {
            try
            {
                _memoryCache.Clear();
                
                if (Directory.Exists(_cacheDirectory))
                {
                    var cacheFiles = Directory.GetFiles(_cacheDirectory, "*.cache");
                    foreach (var file in cacheFiles)
                    {
                        File.Delete(file);
                    }
                }

                Logger.Info("All cache cleared");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to clear all cache: {ex.Message}");
            }
        }

        public long GetCacheSize()
        {
            try
            {
                long totalSize = 0;
                if (Directory.Exists(_cacheDirectory))
                {
                    var cacheFiles = Directory.GetFiles(_cacheDirectory, "*.cache");
                    foreach (var file in cacheFiles)
                    {
                        totalSize += new FileInfo(file).Length;
                    }
                }
                return totalSize;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get cache size: {ex.Message}");
                return 0;
            }
        }

        #endregion

        #region Private Helper Methods

        private string GetCacheFilePath(string key)
        {
            // Create a safe filename from the key
            var safeKey = key.Replace(Path.GetInvalidFileNameChars(), '_');
            return Path.Combine(_cacheDirectory, $"{safeKey}.cache");
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }

    public class CacheEntry
    {
        public DateTime ExpirationTime { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class CacheEntry<T> : CacheEntry
    {
        public T Value { get; set; }
    }

    public static class StringExtensions
    {
        public static string Replace(this string str, char[] oldChars, char newChar)
        {
            foreach (char c in oldChars)
            {
                str = str.Replace(c, newChar);
            }
            return str;
        }
    }

    // Data structures for caching UI elements
    public class FormLayoutData
    {
        public Dictionary<string, Rectangle> ControlBounds { get; set; } = new Dictionary<string, Rectangle>();
        public Size FormSize { get; set; }
        public Point FormLocation { get; set; }
        public Dictionary<string, Color> ControlColors { get; set; } = new Dictionary<string, Color>();
        public Dictionary<string, Font> ControlFonts { get; set; } = new Dictionary<string, Font>();
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }

    public class UIRenderingData
    {
        public byte[] RenderedImageData { get; set; }
        public Size ImageSize { get; set; }
        public DateTime RenderedAt { get; set; } = DateTime.UtcNow;
        public string RenderingParameters { get; set; } // Store rendering settings as JSON
    }

    public class ThemeResourceData
    {
        public Dictionary<string, Color> Colors { get; set; } = new Dictionary<string, Color>();
        public Dictionary<string, byte[]> Images { get; set; } = new Dictionary<string, byte[]>();
        public Dictionary<string, string> Fonts { get; set; } = new Dictionary<string, string>();
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
} 