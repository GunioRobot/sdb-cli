using System;
using System.Configuration;
using System.Reflection;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli
{
    public static class Configuration
    {
        private static readonly System.Configuration.Configuration _cfg;

        static Configuration()
        {
            try
            {
                _cfg = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

                RuntimePaths = _cfg.AppSettings.Settings["runtimePaths"].Value.Split(';');
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Failed to load configuration: {0}", ex.Message);
            }
        }

        public static string[] RuntimePaths { get; set; }
    }
}
