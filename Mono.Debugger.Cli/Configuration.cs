using System;
using System.Configuration;
using System.Reflection;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli
{
    internal static class Configuration
    {
        private static readonly System.Configuration.Configuration _cfg;

        private static string GetValue(string name)
        {
            return _cfg.AppSettings.Settings[name].Value;
        }

        static Configuration()
        {
            try
            {
                _cfg = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

                RuntimePathPrefixes = GetValue("runtimePathPrefixes").Split(';');
                AddinAssemblyPaths = GetValue("addinAssemblyPaths").Split(';');
                DebugLog = bool.Parse(GetValue("debugLog"));
                UseColors = bool.Parse(GetValue("useColors"));
                InfoColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), GetValue("infoColor"));
                EmphasisColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), GetValue("emphasisColor"));
                WarningColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), GetValue("warningColor"));
                ErrorColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), GetValue("errorColor"));
                DebugColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), GetValue("debugColor"));
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Failed to load configuration: {0}", ex.Message);
            }
        }

        public static string[] RuntimePathPrefixes { get; private set; }

        public static string[] AddinAssemblyPaths { get; private set; }

        public static bool DebugLog { get; private set; }

        public static bool UseColors { get; private set; }

        public static ConsoleColor InfoColor { get; private set; }

        public static ConsoleColor EmphasisColor { get; private set; }

        public static ConsoleColor WarningColor { get; private set; }

        public static ConsoleColor ErrorColor { get; private set; }

        public static ConsoleColor DebugColor { get; private set; }
    }
}
