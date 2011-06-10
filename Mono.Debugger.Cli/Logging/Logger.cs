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
                Console.ForegroundColor = color;
                Console.Write(string.Format("[{0}] {1}", _programName, string.Format(format, args)));
                Console.ResetColor();
            }
        }

        public static void WriteInfo(string format, params object[] args)
        {
            Write(ConsoleColor.White, format, args);
        }

        public static void WriteInfoLine(string format, params object[] args)
        {
            WriteInfo(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteEmphasis(string format, params object[] args)
        {
            Write(ConsoleColor.Cyan, format, args);
        }

        public static void WriteEmphasisLine(string format, params object[] args)
        {
            WriteEmphasis(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteError(string format, params object[] args)
        {
            Write(ConsoleColor.Red, format, args);
        }

        public static void WriteErrorLine(string format, params object[] args)
        {
            WriteError(string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        [Conditional("DEBUG")]
        public static void WriteDebug(string format, params object[] args)
        {
            Write(ConsoleColor.Yellow, format, args);
        }

        [Conditional("DEBUG")]
        public static void WriteDebugLine(string format, params object[] args)
        {
            WriteDebug(string.Format("{0}{1}", format, Environment.NewLine), args);
        }
    }
}
