using System;
using System.Diagnostics;
// ReSharper disable UnusedMember.Global

namespace CURPG_Engine.Core
{
    public static class Logger
    {
        public static void Error(string message, string module)
        {
            WriteEntry(message, "ERROR", module);
        }

        public static void Error(Exception ex, string module)
        {
            WriteEntry(ex.Message, "ERROR", module);
        }

        public static void Warning(string message, string module)
        {
            WriteEntry(message, "WARN", module);
        }

        public static void Info(string message, string module)
        {
            WriteEntry(message, "INFO", module);
        }

        private static void WriteEntry(string message, string type, string module)
        {
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {module} - {type}: {message}");
        }

    }
}
