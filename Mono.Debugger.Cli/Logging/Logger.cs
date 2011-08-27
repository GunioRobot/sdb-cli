using System;
using System.Diagnostics;
using System.IO;
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

        public static TextWriter LogOutput { get; set; }

        private static void Write(ConsoleColor color, bool programName, string format, params object[] args)
        {
            lock (_lock)
            {
                if (Configuration.UseColors)
                    Console.ForegroundColor = color;

                var prefix = programName ? string.Format("[{0}] ", _programName) : string.Empty;
                var str = string.Format("{0}{1}", prefix, string.Format(format, args));

                Console.Write(str);

                if (LogOutput != null)
                    LogOutput.Write(str);

                if (Configuration.UseColors)
                    Console.ResetColor();
            }
        }

        public static void WriteInfo(string format, params object[] args)
        {
            Write(Configuration.InfoColor, true, format, args);
        }

        public static void WriteInfoLine(string format, params object[] args)
        {
            Write(Configuration.InfoColor, true, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteInfoString(string format, params object[] args)
        {
            Write(Configuration.InfoColor, false, format, args);
        }

        public static void WriteInfoStringLine(string format, params object[] args)
        {
            Write(Configuration.InfoColor, false, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteEmphasis(string format, params object[] args)
        {
            Write(Configuration.EmphasisColor, true, format, args);
        }

        public static void WriteEmphasisLine(string format, params object[] args)
        {
            Write(Configuration.EmphasisColor, true, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteEmphasisString(string format, params object[] args)
        {
            Write(Configuration.EmphasisColor, false, format, args);
        }

        public static void WriteEmphasisStringLine(string format, params object[] args)
        {
            Write(Configuration.EmphasisColor, false, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteWarning(string format, params object[] args)
        {
            Write(Configuration.WarningColor, true, format, args);
        }

        public static void WriteWarningLine(string format, params object[] args)
        {
            Write(Configuration.WarningColor, true, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteWarningString(string format, params object[] args)
        {
            Write(Configuration.WarningColor, false, format, args);
        }

        public static void WriteWarningStringLine(string format, params object[] args)
        {
            Write(Configuration.WarningColor, false, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteError(string format, params object[] args)
        {
            Write(Configuration.ErrorColor, true, format, args);
        }

        public static void WriteErrorLine(string format, params object[] args)
        {
            Write(Configuration.ErrorColor, true, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        public static void WriteErrorString(string format, params object[] args)
        {
            Write(Configuration.ErrorColor, false, format, args);
        }

        public static void WriteErrorStringLine(string format, params object[] args)
        {
            Write(Configuration.ErrorColor, false, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebug(string format, params object[] args)
        {
            if (!Configuration.DebugLog)
                return;

            Write(Configuration.DebugColor, true, format, args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebugLine(string format, params object[] args)
        {
            if (!Configuration.DebugLog)
                return;

            Write(Configuration.DebugColor, true, string.Format("{0}{1}", format, Environment.NewLine), args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebugString(string format, params object[] args)
        {
            if (!Configuration.DebugLog)
                return;

            Write(Configuration.DebugColor, false, format, args);
        }

        [Conditional("DEBUG")]
        internal static void WriteDebugStringLine(string format, params object[] args)
        {
            if (!Configuration.DebugLog)
                return;

            Write(Configuration.DebugColor, false, string.Format("{0}{1}", format, Environment.NewLine), args);
        }
    }
}
