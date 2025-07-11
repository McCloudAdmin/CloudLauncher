using System.Reflection;

namespace CloudLauncher.utils
{
    public static class ResourceExtractor
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        
        public static void ExtractEmbeddedResources()
        {
            try
            {
                Logger.Info("Extracting embedded resources...");
                
                // Create base directories
                string assetsDir = Path.Combine(Program.appWorkDir, "assets");
                string docsDir = Path.Combine(Program.appWorkDir, "docs");
                
                Directory.CreateDirectory(assetsDir);
                Directory.CreateDirectory(docsDir);
                
                // Get all embedded resource names
                string[] resourceNames = assembly.GetManifestResourceNames();
                
                foreach (string resourceName in resourceNames)
                {
                    // Only extract assets and docs
                    if (resourceName.Contains(".assets.") || resourceName.Contains(".docs."))
                    {
                        ExtractResource(resourceName);
                    }
                }
                
                Logger.Info("Resource extraction completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to extract embedded resources: {ex.Message}");
            }
        }
        
        private static void ExtractResource(string resourceName)
        {
            try
            {
                // Convert resource name to file path
                // Example: "CloudLauncher.assets.bg.png" -> "assets/bg.png"
                string relativePath = ConvertResourceNameToPath(resourceName);
                string fullPath = Path.Combine(Program.appWorkDir, relativePath);
                
                // Skip if file already exists and is not empty
                if (File.Exists(fullPath) && new FileInfo(fullPath).Length > 0)
                {
                    return;
                }
                
                // Ensure directory exists
                string directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Extract resource
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (FileStream fileStream = File.Create(fullPath))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                        Logger.Info($"Extracted: {relativePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to extract resource {resourceName}: {ex.Message}");
            }
        }
        
        private static string ConvertResourceNameToPath(string resourceName)
        {
            // Remove namespace prefix (CloudLauncher.)
            string withoutNamespace = resourceName.Replace("CloudLauncher.", "");
            
            // Replace dots with path separators, except for file extensions
            string[] parts = withoutNamespace.Split('.');
            
            if (parts.Length >= 3)
            {
                // Reconstruct path: folder/subfolder/filename.extension
                string folder = parts[0]; // assets or docs
                string fileName = string.Join(".", parts[1..^1]); // filename parts
                string extension = parts[^1]; // file extension
                
                return Path.Combine(folder, $"{fileName}.{extension}");
            }
            
            return withoutNamespace.Replace('.', Path.DirectorySeparatorChar);
        }
        
        public static string GetExtractedAssetPath(string assetFileName)
        {
            return Path.Combine(Program.appWorkDir, "assets", assetFileName);
        }
        
        public static string GetExtractedDocPath(string docFileName)
        {
            return Path.Combine(Program.appWorkDir, "docs", docFileName);
        }
    }
} 