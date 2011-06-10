using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class StopCommand : ICommand
    {
        public string Name
        {
            get { return "Stop"; }
        }

        public string Description
        {
            get { return "Stops the currently running process and detaches."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.Session == null)
            {
                Logger.WriteErrorLine("No process is running.");
                return;
            }

            SoftDebugger.Stop();
            Logger.WriteInfoLine("Process stopped.");
        }
    }
}
