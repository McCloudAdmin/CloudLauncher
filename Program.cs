using System;
using System.Diagnostics;
using System.Windows.Forms;
using CloudLauncher.utils;
using CloudLauncher.Components;

namespace CloudLauncher
{
    internal static class Program
    {
        public static string appWorkDir = Application.StartupPath;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Logger.Info("Application starting...");

                // Check if another instance of the application is already running
                Logger.Info("Checking if another instance of the application is already running...");
                Mutex mutex = new Mutex(true, "CloudLauncher", out bool createdNew);
                if (!createdNew)
                {
                    Logger.Info("Another instance of the application is already running. Exiting...");
                    MessageBox.Show("Another instance of the application is already running. Exiting...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Logger.Info("No other instance of the application is running. Continuing...");

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Logger.Info("Application configuration initialized");

                Application.Run(new FrmLoadingScreen());
            }
            catch (Exception ex)
            {
                Logger.Error($"Fatal error: {ex.Message}");
                Logger.Error($"Stack trace: {ex.StackTrace}");
                MessageBox.Show("A fatal error occurred. Please check the logs for details.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c timeout /t 1 && taskkill /F /F /IM \"{Path.GetFileName(Application.ExecutablePath)}\" && start \"\" \"{Application.ExecutablePath}\"",
                UseShellExecute = true,
                CreateNoWindow = false
            });
            Application.Exit();
        }
    }
}