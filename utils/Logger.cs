using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLauncher.utils
{
    public enum LogLevel
    {
        INFO,
        WARN,
        ERROR,
        DEBUG
    }

    public static class Logger
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string LogFile = Path.Combine(LogDirectory, "log.txt");
        private static readonly object LockObj = new object();

        static Logger()
        {
            // Ensure logs directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public static void Log(LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logMessage = $"[{timestamp}] [{level}] {message}";

            // Console output
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColorForLogLevel(level);
            Console.WriteLine(logMessage);
            Console.ForegroundColor = originalColor;

            // File output
            lock (LockObj)
            {
                // Check if we need to rotate logs
                if (File.Exists(LogFile))
                {
                    FileInfo fileInfo = new FileInfo(LogFile);
                    if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                    {
                        string oldLogName = Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
                        File.Move(LogFile, oldLogName);
                    }
                }

                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
        }

        private static ConsoleColor GetColorForLogLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.INFO => ConsoleColor.White,
                LogLevel.WARN => ConsoleColor.Yellow,
                LogLevel.ERROR => ConsoleColor.Red,
                LogLevel.DEBUG => ConsoleColor.Gray,
                _ => ConsoleColor.White
            };
        }

        // Convenience methods
        public static void Info(string message) => Log(LogLevel.INFO, message);
        public static void Warn(string message) => Log(LogLevel.WARN, message);
        public static void Warning(string message) => Log(LogLevel.WARN, message);
        public static void Error(string message) => Log(LogLevel.ERROR, message);
        public static void Debug(string message) => Log(LogLevel.DEBUG, message);
    }
}
