using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using CloudLauncher.utils;
using CloudLauncher.Components;
using CloudLauncher.forms.auth;
using CloudLauncher.forms.dashboard;
using CloudLauncher.plugins;
using CloudLauncher.plugins.Events;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;

namespace CloudLauncher
{
    internal static class Program
    {
        public static string appWorkDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".cloudlauncher"
        );
        public static string appVersion = "1.0.0";
        public static string appName = "CloudLauncher";
        public static string appAuthor = "CloudLauncher";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Logger.Info("Application starting...");

            try
            {
                // Check for multiple instances
                if (!CheckMultipleInstances())
                {
                    Logger.Info("Another instance is already running and multiple instances are disabled.");
                    return;
                }

                // Initialize version management (install/update if needed)
                VersionManager.InitializeVersionManagement();
                
                // If we're not running from the installed location, restart from there
                if (!VersionManager.IsRunningFromInstalledLocation())
                {
                    Logger.Info("Restarting from installed location...");
                    VersionManager.RestartFromInstalledLocation();
                    return;
                }

                // Extract embedded resources (assets and docs) to working directory
                ResourceExtractor.ExtractEmbeddedResources();
                
                // Cleanup old versions (keep 3 latest)
                VersionManager.CleanupOldVersions(3);

                // Initialize plugin manager
                PluginManager.Instance.Initialize();
                
                // Load plugins asynchronously
                _ = Task.Run(async () => await PluginManager.Instance.LoadAllPluginsAsync());
            }
            catch (Exception ex)
            {
                Logger.Warning($"Initialization warning: {ex.Message}");
            }

            try
            {
                // Try auto-login first
                bool keepLoggedIn = RegistryConfig.GetUserPreference("KeepLoggedIn", false);
                Logger.Info($"Auto-login enabled: {keepLoggedIn}");
                if (keepLoggedIn)
                {
                    string lastUsername = RegistryConfig.GetUserPreference<string>("LastUsername");
                    string lastUUID = RegistryConfig.GetUserPreference<string>("LastUUID");
                    bool wasOffline = RegistryConfig.GetUserPreference("WasOffline", false);

                    Logger.Info($"Auto-login data - Username: {lastUsername}, UUID: {(string.IsNullOrEmpty(lastUUID) ? "empty" : "present")}, WasOffline: {wasOffline}");

                    if (!string.IsNullOrEmpty(lastUsername) && (wasOffline || !string.IsNullOrEmpty(lastUUID)))
                    {
                        try
                        {
                            MSession session = null;
                            if (wasOffline)
                            {
                                Logger.Info("Attempting offline auto-login");
                                session = MSession.CreateOfflineSession(lastUsername);
                                
                                // Update plugin manager with session and publish login event
                                PluginManager.Instance.UpdateCurrentSession(session);
                                PluginManager.Instance.GetEventManager().Publish(new UserLoginEvent
                                {
                                    Session = session,
                                    Username = lastUsername,
                                    IsOffline = true
                                });
                                
                                Logger.Info("Offline auto-login successful, launching game");
                                var gameLaunch = new GameLaunch(session);
                                Application.Run(gameLaunch);
                                return;
                            }
                            else
                            {                
                                Logger.Info("Attempting online auto-login");
                                var loginHandler = JELoginHandlerBuilder.BuildDefault();
                                var accounts = loginHandler.AccountManager.GetAccounts();
                                Logger.Info($"Found {accounts.Count} saved accounts for auto-login");
                                
                                foreach (var account in accounts)
                                {
                                    try
                                    {
                                        Logger.Info($"Trying silent auth for account: {account.Identifier}");
                                        // Use synchronous call to avoid handle issues
                                        session = loginHandler.AuthenticateSilently(account).GetAwaiter().GetResult();
                                        if (session != null && session.Username == lastUsername)
                                        {
                                            Logger.Info("Online auto-login successful, launching game");
                                            // Update plugin manager with session and publish login event
                                            PluginManager.Instance.UpdateCurrentSession(session);
                                            PluginManager.Instance.GetEventManager().Publish(new UserLoginEvent
                                            {
                                                Session = session,
                                                Username = lastUsername,
                                                IsOffline = false
                                            });
                                            
                                            var gameLaunch = new GameLaunch(session);
                                            Application.Run(gameLaunch);
                                            return;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Warning($"Silent auth failed for account {account.Identifier}: {ex.Message}");
                                        // Try next account
                                        continue;
                                    }
                                }
                                Logger.Warning("No matching account found for auto-login");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Auto-login failed: {ex.Message}");
                        }
                    }
                    else
                    {
                        Logger.Info("Auto-login conditions not met - either username is empty or (UUID is empty and not offline mode)");
                    }
                }
                else
                {
                    Logger.Info("Auto-login is disabled");
                }

                // If auto-login failed or wasn't enabled, show login form
                while (true)
                {
                    var loginForm = new FrmLogin();
                    var result = loginForm.ShowDialog();
                    
                    if (result == DialogResult.OK)
                    {
                        // Get the session info from registry (saved by HandleSuccessfulLogin)
                        string username = RegistryConfig.GetUserPreference<string>("LastUsername");
                        string uuid = RegistryConfig.GetUserPreference<string>("LastUUID");
                        bool wasOffline = RegistryConfig.GetUserPreference("WasOffline", false);
                        
                        if (!string.IsNullOrEmpty(username) && (wasOffline || !string.IsNullOrEmpty(uuid)))
                        {
                            MSession session;
                            if (wasOffline)
                            {
                                session = MSession.CreateOfflineSession(username);
                            }
                            else
                            {
                                session = new MSession(username, uuid, "access_token"); // Create session with saved data
                            }
                            
                            // Update plugin manager with session and publish login event
                            PluginManager.Instance.UpdateCurrentSession(session);
                            PluginManager.Instance.GetEventManager().Publish(new UserLoginEvent
                            {
                                Session = session,
                                Username = username,
                                IsOffline = wasOffline
                            });
                            
                            var gameLaunch = new GameLaunch(session);
                            Application.Run(gameLaunch);
                            
                            // After GameLaunch closes, check if we should restart (logout) or exit
                            bool shouldRestart = RegistryConfig.GetUserPreference("RestartForLogout", false);
                            if (shouldRestart)
                            {
                                RegistryConfig.SaveUserPreference("RestartForLogout", false); // Reset flag
                                continue; // Show login form again
                            }
                            else
                            {
                                break; // Exit application
                            }
                        }
                    }
                    else
                    {
                        break; // User cancelled login, exit application
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Application error: {ex.Message}");
                Alert.Error("An error occurred. Please check the logs for details.");
            }
        }

        public static void stop()
        {
            Logger.Info("Application stopping...");
            
            // Publish application exit event
            PluginManager.Instance.GetEventManager().Publish(new ApplicationExitEvent
            {
                Reason = "User requested exit"
            });
            
            // Unload all plugins
            PluginManager.Instance.UnloadAllPlugins();
            
            Application.Exit();
        }

        public static void restart()
        {
            Logger.Info("Application restarting...");
            Application.Restart();
        }

        private static bool CheckMultipleInstances()
        {
            try
            {
                // Check if multiple instances are allowed
                bool allowMultiple = RegistryConfig.GetUserPreference("AllowMultipleInstances", false);
                
                if (allowMultiple)
                {
                    Logger.Info("Multiple instances allowed");
                    return true;
                }

                // Check if another instance is already running
                string processName = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length > 1)
                {
                    Logger.Warning("Another instance is already running");
                    
                    // Try to bring the existing instance to the front
                    foreach (var process in processes)
                    {
                        if (process.Id != Process.GetCurrentProcess().Id)
                        {
                            try
                            {
                                // Find the main window and bring it to front
                                if (process.MainWindowHandle != IntPtr.Zero)
                                {
                                    ShowWindow(process.MainWindowHandle, SW_RESTORE);
                                    SetForegroundWindow(process.MainWindowHandle);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Warning($"Failed to bring existing instance to front: {ex.Message}");
                            }
                            break;
                        }
                    }
                    return false;
                }

                Logger.Info("No other instances found");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking multiple instances: {ex.Message}");
                return true; // Allow startup if check fails
            }
        }

        // Windows API functions for bringing window to front
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_RESTORE = 9;
    }
}