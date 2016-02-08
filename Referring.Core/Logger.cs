using System;
using System.Text;
using System.IO;

namespace Referring.Core
{
    enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        private static object sync = new object();
        private static string filename = string.Empty;

        static Logger()
        {
            filename = "Referring.log";
            string startString = "\r\n*** START APPLICATION ***\r\n";
            File.AppendAllText(filename, startString, Encoding.GetEncoding("Windows-1251"));
        }

        private static void Log(LogLevel logLevel, string message)
        {
            string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}: {2}\r\n", DateTime.Now, logLevel.ToString(), message);

            lock (sync)
            {
                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
            }
        }
        public static void LogInfo(string message)
        {
            Log(LogLevel.Info, message);
        }
        public static void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }
        public static void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }
    }
}
