using System;
using System.Collections.Generic;
using CmlLib.Core.Auth;

namespace CloudLauncher.plugins.Events
{
    /// <summary>
    /// Event manager interface for plugin event handling
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        void Subscribe<T>(Action<T> handler) where T : IEvent;

        /// <summary>
        /// Subscribe to an event with priority
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        /// <param name="priority">Event priority (higher numbers run first)</param>
        void Subscribe<T>(Action<T> handler, int priority) where T : IEvent;

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        void Unsubscribe<T>(Action<T> handler) where T : IEvent;

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="eventData">Event data</param>
        void Publish<T>(T eventData) where T : IEvent;

        /// <summary>
        /// Clear all event subscriptions
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Base interface for all events
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Event timestamp
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Whether the event has been handled
        /// </summary>
        bool IsHandled { get; set; }

        /// <summary>
        /// Additional event data
        /// </summary>
        Dictionary<string, object> Data { get; }
    }

    /// <summary>
    /// Base event implementation
    /// </summary>
    public abstract class BaseEvent : IEvent
    {
        public DateTime Timestamp { get; private set; }
        public bool IsHandled { get; set; }
        public Dictionary<string, object> Data { get; private set; }

        protected BaseEvent()
        {
            Timestamp = DateTime.Now;
            IsHandled = false;
            Data = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Cancellable event interface
    /// </summary>
    public interface ICancellableEvent : IEvent
    {
        /// <summary>
        /// Whether the event is cancelled
        /// </summary>
        bool IsCancelled { get; set; }

        /// <summary>
        /// Cancellation reason
        /// </summary>
        string CancellationReason { get; set; }
    }

    /// <summary>
    /// Base cancellable event implementation
    /// </summary>
    public abstract class BaseCancellableEvent : BaseEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public string CancellationReason { get; set; }

        protected BaseCancellableEvent()
        {
            IsCancelled = false;
            CancellationReason = null;
        }
    }

    #region Launcher Events

    /// <summary>
    /// Fired when a user logs in successfully
    /// </summary>
    public class UserLoginEvent : BaseEvent
    {
        public MSession Session { get; set; }
        public string Username { get; set; }
        public bool IsOffline { get; set; }
    }

    /// <summary>
    /// Fired when a user logs out
    /// </summary>
    public class UserLogoutEvent : BaseEvent
    {
        public string Username { get; set; }
        public bool WasOffline { get; set; }
    }

    /// <summary>
    /// Fired before game launch (cancellable)
    /// </summary>
    public class GameLaunchEvent : BaseCancellableEvent
    {
        public string Version { get; set; }
        public string Username { get; set; }
        public int RamMb { get; set; }
        public Dictionary<string, object> LaunchOptions { get; set; }
        public List<string> JvmArguments { get; set; }
    }

    /// <summary>
    /// Fired after game launch
    /// </summary>
    public class GameLaunchedEvent : BaseEvent
    {
        public string Version { get; set; }
        public string Username { get; set; }
        public int ProcessId { get; set; }
        public bool LaunchSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Fired when game process exits
    /// </summary>
    public class GameExitEvent : BaseEvent
    {
        public string Version { get; set; }
        public string Username { get; set; }
        public int ProcessId { get; set; }
        public int ExitCode { get; set; }
        public TimeSpan PlayTime { get; set; }
    }

    /// <summary>
    /// Fired when application starts
    /// </summary>
    public class ApplicationStartEvent : BaseEvent
    {
        public string Version { get; set; }
        public string WorkingDirectory { get; set; }
        public List<string> Arguments { get; set; }
    }

    /// <summary>
    /// Fired when application is about to exit
    /// </summary>
    public class ApplicationExitEvent : BaseCancellableEvent
    {
        public string Reason { get; set; }
    }

    /// <summary>
    /// Fired when settings are changed
    /// </summary>
    public class SettingsChangedEvent : BaseEvent
    {
        public string SettingName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    /// <summary>
    /// Fired when a version is selected
    /// </summary>
    public class VersionSelectedEvent : BaseEvent
    {
        public string Version { get; set; }
        public string VersionType { get; set; }
        public string PreviousVersion { get; set; }
    }

    /// <summary>
    /// Fired when version installation starts
    /// </summary>
    public class VersionInstallStartEvent : BaseEvent
    {
        public string Version { get; set; }
        public string VersionType { get; set; }
    }

    /// <summary>
    /// Fired when version installation completes
    /// </summary>
    public class VersionInstallCompleteEvent : BaseEvent
    {
        public string Version { get; set; }
        public string VersionType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Fired when download progress updates
    /// </summary>
    public class DownloadProgressEvent : BaseEvent
    {
        public string FileName { get; set; }
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public double ProgressPercentage { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Fired when plugin is loaded
    /// </summary>
    public class PluginLoadedEvent : BaseEvent
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
        public string PluginVersion { get; set; }
        public string PluginAuthor { get; set; }
    }

    /// <summary>
    /// Fired when plugin is unloaded
    /// </summary>
    public class PluginUnloadedEvent : BaseEvent
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Fired when plugin is enabled
    /// </summary>
    public class PluginEnabledEvent : BaseEvent
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
    }

    /// <summary>
    /// Fired when plugin is disabled
    /// </summary>
    public class PluginDisabledEvent : BaseEvent
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Fired when an error occurs
    /// </summary>
    public class ErrorEvent : BaseEvent
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public string Source { get; set; }
        public string Severity { get; set; }
    }

    /// <summary>
    /// Fired when a warning occurs
    /// </summary>
    public class WarningEvent : BaseEvent
    {
        public string WarningMessage { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// Fired when a notification is shown
    /// </summary>
    public class NotificationEvent : BaseEvent
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public int Duration { get; set; }
    }

    /// <summary>
    /// Fired when UI theme changes
    /// </summary>
    public class ThemeChangedEvent : BaseEvent
    {
        public string OldTheme { get; set; }
        public string NewTheme { get; set; }
    }

    /// <summary>
    /// Fired when launcher UI is shown/hidden
    /// </summary>
    public class UIVisibilityChangedEvent : BaseEvent
    {
        public bool IsVisible { get; set; }
        public string FormName { get; set; }
        public string Reason { get; set; }
    }

    #endregion
} 