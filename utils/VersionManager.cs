using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CloudLauncher.utils
{
    public static class VersionManager
    {
        private static readonly string VersionsDir = Path.Combine(Program.appWorkDir, "versions");
        private static readonly string CurrentVersionFile = Path.Combine(Program.appWorkDir, "current_version.txt");
        private static readonly string StartMenuPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            "Programs",
            "CloudLauncher.lnk"
        );

        public static string CurrentVersion => Program.appVersion;
        public static string InstalledExecutablePath => Path.Combine(VersionsDir, CurrentVersion, "CloudLauncher.exe");

        public static void InitializeVersionManagement()
        {
            try
            {
                // Skip version management in debug mode
                if (IsDebugMode())
                {
                    Logger.Info("Debug mode detected - skipping version management");
                    return;
                }
                
                Logger.Info("Initializing version management...");
                
                // Create versions directory
                Directory.CreateDirectory(VersionsDir);
                
                // Check if we need to install/update
                if (ShouldInstallOrUpdate())
                {
                    InstallCurrentVersion();
                    CreateStartMenuShortcut();
                    UpdateCurrentVersionFile();
                    Logger.Info($"Application installed/updated to version {CurrentVersion}");
                }
                else
                {
                    Logger.Info($"Application version {CurrentVersion} already installed");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize version management: {ex.Message}");
            }
        }

        private static bool ShouldInstallOrUpdate()
        {
            // Check if current version is already installed
            if (!System.IO.File.Exists(InstalledExecutablePath))
            {
                Logger.Info("Executable not found in versions folder - installation needed");
                return true;
            }

            // Check if we're running from a different location (update scenario)
            string currentExePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            if (!string.IsNullOrEmpty(currentExePath) && 
                !currentExePath.Equals(InstalledExecutablePath, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Info("Running from different location - checking for version update");
                
                // Compare versions
                if (IsNewerVersion(CurrentVersion, GetInstalledVersion()))
                {
                    Logger.Info($"Newer version detected: {CurrentVersion} > {GetInstalledVersion()}");
                    return true;
                }
            }

            return false;
        }

        private static void InstallCurrentVersion()
        {
            try
            {
                Logger.Info($"Installing version {CurrentVersion}...");
                
                // Create version directory
                string versionDir = Path.Combine(VersionsDir, CurrentVersion);
                Directory.CreateDirectory(versionDir);
                
                // Copy current executable
                string currentExePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                if (!string.IsNullOrEmpty(currentExePath))
                {
                    string destinationPath = Path.Combine(versionDir, "CloudLauncher.exe");
                    
                    // If destination exists, delete it first
                    if (System.IO.File.Exists(destinationPath))
                    {
                        System.IO.File.Delete(destinationPath);
                    }
                    
                    System.IO.File.Copy(currentExePath, destinationPath, true);
                    Logger.Info($"Executable copied to: {destinationPath}");
                }
                else
                {
                    Logger.Warning("Could not determine current executable path");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to install version {CurrentVersion}: {ex.Message}");
                throw;
            }
        }

        private static void CreateStartMenuShortcut()
        {
            try
            {
                Logger.Info("Creating start menu shortcut...");
                
                // Ensure start menu directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(StartMenuPath) ?? string.Empty);
                
                // Use PowerShell to create the shortcut (works without COM references)
                string powershellScript = $@"
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut('{StartMenuPath}')
$Shortcut.TargetPath = '{InstalledExecutablePath}'
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(InstalledExecutablePath)}'
$Shortcut.Description = 'CloudLauncher - The best Minecraft launcher'
$Shortcut.IconLocation = '{InstalledExecutablePath},0'
$Shortcut.Save()
";

                var processInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{powershellScript.Replace("\"", "\\\"")}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(processInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            Logger.Info($"Start menu shortcut created: {StartMenuPath}");
                        }
                        else
                        {
                            Logger.Warning($"PowerShell shortcut creation failed with exit code: {process.ExitCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to create start menu shortcut: {ex.Message}");
                // Fallback: Create a simple batch file
                CreateFallbackShortcut();
            }
        }

        private static void CreateFallbackShortcut()
        {
            try
            {
                string batchPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    "CloudLauncher.bat"
                );

                string batchContent = $@"@echo off
cd /d ""{Path.GetDirectoryName(InstalledExecutablePath)}""
start """" ""{InstalledExecutablePath}""
";

                System.IO.File.WriteAllText(batchPath, batchContent);
                Logger.Info($"Fallback batch shortcut created: {batchPath}");
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to create fallback shortcut: {ex.Message}");
            }
        }

        private static void UpdateCurrentVersionFile()
        {
            try
            {
                System.IO.File.WriteAllText(CurrentVersionFile, CurrentVersion);
                Logger.Info($"Current version file updated: {CurrentVersion}");
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to update current version file: {ex.Message}");
            }
        }

        private static string GetInstalledVersion()
        {
            try
            {
                if (System.IO.File.Exists(CurrentVersionFile))
                {
                    return System.IO.File.ReadAllText(CurrentVersionFile).Trim();
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to read current version file: {ex.Message}");
            }
            return "0.0.0";
        }

        private static bool IsNewerVersion(string version1, string version2)
        {
            try
            {
                Version v1 = new Version(version1);
                Version v2 = new Version(version2);
                return v1 > v2;
            }
            catch
            {
                return false;
            }
        }

        public static void CleanupOldVersions(int keepCount = 3)
        {
            try
            {
                Logger.Info($"Cleaning up old versions (keeping {keepCount} latest)...");
                
                var versionDirs = Directory.GetDirectories(VersionsDir)
                    .Select(dir => new DirectoryInfo(dir))
                    .Where(dir => Version.TryParse(dir.Name, out _))
                    .OrderByDescending(dir => new Version(dir.Name))
                    .Skip(keepCount)
                    .ToList();

                foreach (var dir in versionDirs)
                {
                    try
                    {
                        dir.Delete(true);
                        Logger.Info($"Removed old version: {dir.Name}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning($"Failed to remove old version {dir.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to cleanup old versions: {ex.Message}");
            }
        }

        public static void RestartFromInstalledLocation()
        {
            try
            {
                if (System.IO.File.Exists(InstalledExecutablePath))
                {
                    Logger.Info("Restarting from installed location...");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = InstalledExecutablePath,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(InstalledExecutablePath)
                    });
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to restart from installed location: {ex.Message}");
            }
        }

        public static bool IsRunningFromInstalledLocation()
        {
            try
            {
                // In debug mode, we're never running from the installed location
                if (IsDebugMode())
                {
                    return true; // Return true to avoid restart attempts
                }
                
                string currentExePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                return currentExePath.Equals(InstalledExecutablePath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        
        private static bool IsDebugMode()
        {
            try
            {
                #if DEBUG
                Logger.Info("Debug mode detected: DEBUG preprocessor directive");
                return true;
                #else
                
                // Get current process info
                string currentExePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                Logger.Info($"Current executable path: {currentExePath}");
                
                // Check if we're running a DLL instead of EXE (debug mode)
                if (currentExePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info("Debug mode detected: Running as DLL");
                    return true;
                }
                
                // Check if path contains debug indicators
                if (currentExePath.Contains("bin\\Debug", StringComparison.OrdinalIgnoreCase) ||
                    currentExePath.Contains("bin/Debug", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info("Debug mode detected: Path contains Debug folder");
                    return true;
                }
                
                // Check if we're in a development environment (dotnet.exe)
                if (currentExePath.Contains("dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info("Debug mode detected: Running via dotnet.exe");
                    return true;
                }
                
                // Check assembly location
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                Logger.Info($"Assembly location: {assemblyLocation}");
                
                if (assemblyLocation.Contains("Debug", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info("Debug mode detected: Assembly in Debug folder");
                    return true;
                }
                
                // Check if file size is suspiciously small (like the 415KB DLL)
                if (System.IO.File.Exists(currentExePath))
                {
                    var fileInfo = new FileInfo(currentExePath);
                    if (fileInfo.Length < 1024 * 1024) // Less than 1MB
                    {
                        Logger.Info($"Debug mode detected: File size too small ({fileInfo.Length} bytes)");
                        return true;
                    }
                }
                
                // Check if we're attached to a debugger
                if (Debugger.IsAttached)
                {
                    Logger.Info("Debug mode detected: Debugger attached");
                    return true;
                }
                
                Logger.Info("Release mode detected");
                return false;
                
                #endif
            }
            catch (Exception ex)
            {
                Logger.Warning($"Error detecting debug mode: {ex.Message}");
                return false;
            }
        }
    }
} 