using System;
using System.Configuration;
using System.Reflection;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli
{
    internal static class Configuration
    {
        private static readonly System.Configuration.Configuration _cfg;

        static Configuration()
        {
            try
            {
                _cfg = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

                RuntimePathPrefixes = _cfg.AppSettings.Settings["runtimePathPrefixes"].Value.Split(';');
                AddinAssemblyPaths = _cfg.AppSettings.Settings["addinAssemblyPaths"].Value.Split(';');
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Failed to load configuration: {0}", ex.Message);
            }
        }

        public static string[] RuntimePathPrefixes { get; set; }

        public static string[] AddinAssemblyPaths { get; set; }
    }
}
