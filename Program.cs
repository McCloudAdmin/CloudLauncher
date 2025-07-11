using System;
using System.Diagnostics;
using System.Windows.Forms;
using CloudLauncher.utils;
using CloudLauncher.Components;
using CloudLauncher.forms.auth;
using CloudLauncher.forms.dashboard;
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
            }
            catch (Exception ex)
            {
                Logger.Warning($"Initialization warning: {ex.Message}");
            }

            try
            {
                // Try auto-login first
                bool keepLoggedIn = RegistryConfig.GetUserPreference("KeepLoggedIn", false);
                if (keepLoggedIn)
                {
                    string lastUsername = RegistryConfig.GetUserPreference<string>("LastUsername");
                    string lastUUID = RegistryConfig.GetUserPreference<string>("LastUUID");
                    bool wasOffline = RegistryConfig.GetUserPreference("WasOffline", false);

                    if (!string.IsNullOrEmpty(lastUsername) && !string.IsNullOrEmpty(lastUUID))
                    {
                        try
                        {
                            MSession session = null;
                            if (wasOffline)
                            {
                                session = MSession.CreateOfflineSession(lastUsername);
                                var gameLaunch = new GameLaunch(session);
                                Application.Run(gameLaunch);
                                return;
                            }
                            else
                            {                
                                var loginHandler = JELoginHandlerBuilder.BuildDefault();
                                var accounts = loginHandler.AccountManager.GetAccounts();
                                
                                foreach (var account in accounts)
                                {
                                    try
                                    {
                                        // Use synchronous call to avoid handle issues
                                        session = loginHandler.AuthenticateSilently(account).GetAwaiter().GetResult();
                                        if (session != null && session.Username == lastUsername)
                                        {
                                            var gameLaunch = new GameLaunch(session);
                                            Application.Run(gameLaunch);
                                            return;
                                        }
                                    }
                                    catch
                                    {
                                        // Try next account
                                        continue;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Auto-login failed: {ex.Message}");
                        }
                    }
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
                        
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(uuid))
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
            Application.Exit();
        }

        public static void restart()
        {
            Logger.Info("Application restarting...");
            Application.Restart();
        }
    }
}