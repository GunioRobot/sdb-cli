using System;
using System.Configuration;
using System.Reflection;

namespace Mono.Debugger.Cli
{
    public static class Configuration
    {
        private static readonly System.Configuration.Configuration _cfg;

        static Configuration()
        {
            _cfg = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            RuntimePaths = _cfg.AppSettings.Settings["runtimePaths"].Value.Split(';');
        }

        public static string[] RuntimePaths { get; set; }
    }
}
