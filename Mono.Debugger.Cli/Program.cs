using Mono.Debugger.Cli.Addins;

namespace Mono.Debugger.Cli
{
    internal static class Program
    {
        private static void Main()
        {
            AddinManager.Scan();
            CommandLine.CommandLoop();
        }
    }
}
