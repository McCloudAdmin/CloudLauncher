using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CloudLauncher.utils
{
    /// <summary>
    /// UI Rendering Cache Helper - Provides high-level methods for caching UI elements
    /// Similar to browser cache for faster form rendering
    /// </summary>
    public static class UIRenderCache
    {
        private static readonly CacheManager _cache = CacheManager.Instance;

        #region Background Rendering Cache

        /// <summary>
        /// Cache a rendered background with gradients, effects, etc.
        /// </summary>
        public static Image GetOrCreateBackground(string backgroundKey, Size size, Func<Size, Image> backgroundGenerator)
        {
            try
            {
                // Check for cached background
                var cached = _cache.GetCachedRenderedBackground(backgroundKey);
                if (cached != null && cached.Size == size)
                {
                    return cached;
                }

                // Generate new background
                var background = backgroundGenerator(size);
                if (background != null)
                {
                    _cache.CacheRenderedBackground(backgroundKey, background, TimeSpan.FromDays(1));
                    Logger.Debug($"Generated and cached background: {backgroundKey}");
                }

                return background;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get or create background {backgroundKey}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a gradient background with caching
        /// </summary>
        public static Image GetOrCreateGradientBackground(string key, Size size, Color color1, Color color2, LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            var cacheKey = $"gradient_{key}_{size.Width}x{size.Height}_{color1.ToArgb()}_{color2.ToArgb()}_{mode}";
            
            return GetOrCreateBackground(cacheKey, size, (s) =>
            {
                var bitmap = new Bitmap(s.Width, s.Height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, s.Width, s.Height), color1, color2, mode))
                    {
                        graphics.FillRectangle(brush, 0, 0, s.Width, s.Height);
                    }
                }
                return bitmap;
            });
        }

        #endregion

        #region Component Rendering Cache

        /// <summary>
        /// Cache a pre-rendered component for faster redraws
        /// </summary>
        public static void CacheComponent(Control component, string componentKey = null)
        {
            try
            {
                if (component.Width <= 0 || component.Height <= 0) return;

                var key = componentKey ?? $"{component.GetType().Name}_{component.Name}_{component.GetHashCode()}";
                _cache.CachePreRenderedComponent(key, component, TimeSpan.FromHours(8));
                Logger.Debug($"Cached component: {key}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache component {component.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a cached component rendering
        /// </summary>
        public static Image GetCachedComponent(string componentKey)
        {
            return _cache.GetCachedPreRenderedComponent(componentKey);
        }

        /// <summary>
        /// Cache multiple components at once (useful for form initialization)
        /// </summary>
        public static void CacheFormComponents(Form form)
        {
            try
            {
                var components = GetAllControls(form).Where(c => c.Width > 0 && c.Height > 0).ToList();
                
                foreach (var component in components)
                {
                    var key = $"{form.Name}_{component.Name}_{component.GetType().Name}";
                    CacheComponent(component, key);
                }

                Logger.Info($"Cached {components.Count} components for form {form.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache form components for {form.Name}: {ex.Message}");
            }
        }

        #endregion

        #region Style and Layout Cache

        /// <summary>
        /// Cache computed styles for faster application
        /// </summary>
        public static void CacheFormStyles(Form form)
        {
            try
            {
                var formKey = $"styles_{form.Name}";
                var styles = new Dictionary<string, object>();

                // Cache form-level styles
                styles["BackColor"] = form.BackColor;
                styles["ForeColor"] = form.ForeColor;
                styles["Font"] = form.Font?.Name;
                styles["Size"] = form.Size;

                // Cache control styles
                foreach (Control control in GetAllControls(form))
                {
                    var controlKey = $"{control.Name}";
                    styles[$"{controlKey}_BackColor"] = control.BackColor;
                    styles[$"{controlKey}_ForeColor"] = control.ForeColor;
                    styles[$"{controlKey}_Font"] = control.Font?.Name;
                    styles[$"{controlKey}_Size"] = control.Size;
                    styles[$"{controlKey}_Location"] = control.Location;
                }

                _cache.CacheComputedStyle(formKey, styles, TimeSpan.FromHours(24));
                Logger.Debug($"Cached styles for form: {form.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache form styles for {form.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply cached styles to a form (for consistent theming)
        /// </summary>
        public static bool ApplyCachedStyles(Form form)
        {
            try
            {
                var formKey = $"styles_{form.Name}";
                var styles = _cache.GetCachedComputedStyle(formKey);

                if (styles == null) return false;

                // Apply form-level styles
                if (styles.ContainsKey("BackColor"))
                    form.BackColor = (Color)styles["BackColor"];
                if (styles.ContainsKey("ForeColor"))
                    form.ForeColor = (Color)styles["ForeColor"];

                // Apply control styles
                foreach (Control control in GetAllControls(form))
                {
                    var controlKey = $"{control.Name}";
                    
                    if (styles.ContainsKey($"{controlKey}_BackColor"))
                        control.BackColor = (Color)styles[$"{controlKey}_BackColor"];
                    if (styles.ContainsKey($"{controlKey}_ForeColor"))
                        control.ForeColor = (Color)styles[$"{controlKey}_ForeColor"];
                    if (styles.ContainsKey($"{controlKey}_Location"))
                        control.Location = (Point)styles[$"{controlKey}_Location"];
                }

                Logger.Debug($"Applied cached styles to form: {form.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to apply cached styles to {form.Name}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Theme Resource Cache

        /// <summary>
        /// Cache theme resources (colors, fonts, images)
        /// </summary>
        public static void CacheTheme(string themeName, Dictionary<string, Color> colors, Dictionary<string, string> fonts, Dictionary<string, byte[]> images)
        {
            try
            {
                var themeData = new ThemeResourceData
                {
                    Colors = colors ?? new Dictionary<string, Color>(),
                    Fonts = fonts ?? new Dictionary<string, string>(),
                    Images = images ?? new Dictionary<string, byte[]>()
                };

                _cache.CacheThemeResources(themeName, themeData, TimeSpan.FromDays(30));
                Logger.Info($"Cached theme resources: {themeName}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to cache theme {themeName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get cached theme resources
        /// </summary>
        public static ThemeResourceData GetCachedTheme(string themeName)
        {
            return _cache.GetCachedThemeResources(themeName);
        }

        #endregion

        #region Asset Processing Cache

        /// <summary>
        /// Get or process an image asset with caching
        /// </summary>
        public static Image GetOrProcessAsset(string assetPath, Func<Image> processor)
        {
            try
            {
                return _cache.GetOrCacheProcessedImage(assetPath, processor, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get or process asset {assetPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Process and cache a resized image
        /// </summary>
        public static Image GetOrCreateResizedImage(string imagePath, Size targetSize)
        {
            var cacheKey = $"resized_{imagePath}_{targetSize.Width}x{targetSize.Height}";
            
            return GetOrProcessAsset(cacheKey, () =>
            {
                if (!File.Exists(imagePath)) return null;

                using (var original = Image.FromFile(imagePath))
                {
                    var resized = new Bitmap(targetSize.Width, targetSize.Height);
                    using (var graphics = Graphics.FromImage(resized))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(original, 0, 0, targetSize.Width, targetSize.Height);
                    }
                    return resized;
                }
            });
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get all controls recursively
        /// </summary>
        private static IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;
                foreach (Control child in GetAllControls(control))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Clear all UI rendering cache
        /// </summary>
        public static void ClearUICache()
        {
            try
            {
                // This would clear specific UI cache entries
                // For now, we'll trigger a general cache cleanup
                _cache.ClearExpiredCache();
                Logger.Info("UI rendering cache cleared");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to clear UI cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Pre-warm cache for a form (cache common elements before showing)
        /// </summary>
        public static void PreWarmFormCache(Form form)
        {
            try
            {
                // Cache form layout
                var layoutData = new FormLayoutData
                {
                    FormSize = form.Size,
                    FormLocation = form.Location
                };

                foreach (Control control in GetAllControls(form))
                {
                    layoutData.ControlBounds[control.Name] = control.Bounds;
                    layoutData.ControlColors[control.Name] = control.BackColor;
                    if (control.Font != null)
                    {
                        layoutData.ControlFonts[control.Name] = control.Font;
                    }
                }

                _cache.CacheFormLayout(form.Name, layoutData, TimeSpan.FromDays(7));

                // Cache styles
                CacheFormStyles(form);

                Logger.Info($"Pre-warmed cache for form: {form.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to pre-warm cache for {form.Name}: {ex.Message}");
            }
        }

        #endregion
    }
} 