using CloudLauncher.Components;

namespace CloudLauncher.utils
{
    /// <summary>
    /// Static alert system for easy access throughout the application
    /// </summary>
    public static class Alert
    {
        /// <summary>
        /// Shows a success alert
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void Success(string message)
        {
            ShowAlert(message, AlertComponent.AlertType.Success);
        }

        /// <summary>
        /// Shows a success alert with custom title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The title (currently not used by AlertComponent)</param>
        public static void Success(string message, string title)
        {
            ShowAlert(message, AlertComponent.AlertType.Success);
        }

        /// <summary>
        /// Shows a warning alert
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void Warning(string message)
        {
            ShowAlert(message, AlertComponent.AlertType.Warning);
        }

        /// <summary>
        /// Shows a warning alert with custom title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The title (currently not used by AlertComponent)</param>
        public static void Warning(string message, string title)
        {
            ShowAlert(message, AlertComponent.AlertType.Warning);
        }

        /// <summary>
        /// Shows an error alert
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void Error(string message)
        {
            ShowAlert(message, AlertComponent.AlertType.Error);
        }

        /// <summary>
        /// Shows an error alert with custom title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The title (currently not used by AlertComponent)</param>
        public static void Error(string message, string title)
        {
            ShowAlert(message, AlertComponent.AlertType.Error);
        }

        /// <summary>
        /// Shows an info alert
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void Info(string message)
        {
            ShowAlert(message, AlertComponent.AlertType.Info);
        }

        /// <summary>
        /// Shows an info alert with custom title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The title (currently not used by AlertComponent)</param>
        public static void Info(string message, string title)
        {
            ShowAlert(message, AlertComponent.AlertType.Info);
        }

        /// <summary>
        /// Shows a custom alert with specified type
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="type">The alert type</param>
        public static void Show(string message, AlertComponent.AlertType type)
        {
            ShowAlert(message, type);
        }

        private static void ShowAlert(string message, AlertComponent.AlertType type)
        {
            try
            {
                // Ensure we're on the UI thread
                if (Application.OpenForms.Count > 0)
                {
                    var mainForm = Application.OpenForms[0];
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new Action(() => ShowAlert(message, type)));
                        return;
                    }
                }

                var alert = new AlertComponent();
                alert.ShowAlert(message, type);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to show alert: {ex.Message}");
            }
        }
    }
}