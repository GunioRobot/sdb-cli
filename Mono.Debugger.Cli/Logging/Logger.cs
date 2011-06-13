using System;
using System.Diagnostics;
using System.Reflection;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli.Logging
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static readonly string _programName = Assembly.GetExecutingAssembly().GetName().Name;

        static Logger()
        {
            LoggingService.CustomLogger = new LoggerProxy();
        }

        private static void Write(ConsoleColor color, string format, params object[] args)
        {
            lock (_lock)
            {
                if (Configuration.UseColors)
                    Console.ForegroundColor = color;

                Console.Write(string.Format("[{0}] {1}", _programName, string.Format(format, args)));

                if (Configuration.UseColors)
                    Console.ResetColor();
            }
        }

        public static void WriteInfo(string format, params object[] args)
        {
            Write(Configuration.InfoColor, format, args);
        }

        public static void WriteInfoLine(string format, params object[] args)
        {
            WriteInfo(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteEmphasis(string format, params object[] args)
        {
            Write(Configuration.EmphasisColor, format, args);
        }

        public static void WriteEmphasisLine(string format, params object[] args)
        {
            WriteEmphasis(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteWarning(string format, params object[] args)
        {
            Write(Configuration.WarningColor, format, args);
        }

        public static void WriteWarningLine(string format, params object[] args)
        {
            WriteWarning(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteError(string format, params object[] args)
        {
            Write(Configuration.ErrorColor, format, args);
        }

        public static void WriteErrorLine(string format, params object[] args)
        {
            WriteError(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebug(string format, params object[] args)
        {
            Write(Configuration.DebugColor, format, args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebugLine(string format, params object[] args)
        {
            WriteDebug(string.Format("{0}{1}", format, Environment.NewLine), args);
        }
    }
}
