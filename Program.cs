using System;
using System.Diagnostics;
using System.Windows.Forms;
using CloudLauncher.utils;
using CloudLauncher.Components;
using CloudLauncher.forms.ui;
using CloudLauncher.forms.auth;
using System.Linq;
using CloudLauncher.forms.dashboard;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;

namespace CloudLauncher
{
    internal static class Program
    {
        public static string appWorkDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MythicalSystems",
            "CloudLauncher"
        );
        public static string appVersion = "1.0.0";
        public static string appName = "CloudLauncher";
        public static string appAuthor = "CloudLauncher";
        public static InstanceConfig selectedInstance;

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
                bool shouldRestart = false;

                do
                {
                    shouldRestart = false;

                    // Check if we have a remembered instance first
                    bool rememberInstance = RegistryConfig.GetUserPreference("RememberLastInstance", false);
                    string lastInstanceDir = RegistryConfig.GetUserPreference<string>("LastInstanceDirectory");
                    
                    InstanceConfig selectedInstance = null;
                    
                    if (rememberInstance && !string.IsNullOrEmpty(lastInstanceDir))
                    {
                        // Try to load the remembered instance
                        var instancesManager = new InstancesManager();
                        var instances = instancesManager.GetInstances();
                        selectedInstance = instances.FirstOrDefault(i => 
                            Path.GetDirectoryName(i.ConfigPath).Equals(lastInstanceDir, StringComparison.OrdinalIgnoreCase));
                    }

                    if (selectedInstance == null)
                    {
                        // Show instance selector if no remembered instance
                        using (var instanceSelector = new FrmInstanceSelector())
                        {
                            if (instanceSelector.ShowDialog() != DialogResult.OK)
                            {
                                Application.Exit();
                                return;
                            }
                            selectedInstance = instanceSelector.SelectedInstance;
                        }
                    }

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
                                    continue;
                                }
                                else
                                {                
                                    var loginHandler = JELoginHandlerBuilder.BuildDefault();
                                    var accounts = loginHandler.AccountManager.GetAccounts();
                                    
                                    bool autoLoginSuccessful = false;
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
                                                autoLoginSuccessful = true;
                                                break;
                                            }
                                        }
                                        catch
                                        {
                                            // Try next account
                                            continue;
                                        }
                                    }

                                    if (autoLoginSuccessful)
                                    {
                                        continue;
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
                    var loginForm = new FrmLogin();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        shouldRestart = loginForm.IsLoggingOut;
                    }
                    else
                    {
                        Application.Exit();
                        return;
                    }

                } while (shouldRestart);
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