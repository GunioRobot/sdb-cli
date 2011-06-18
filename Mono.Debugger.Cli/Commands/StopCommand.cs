using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class StopCommand : ICommand
    {
        public string Description
        {
            get { return "Stops the currently running process and detaches."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.State == DebuggerState.Null)
            {
                Logger.WriteErrorLine("No session is active.");
                return;
            }

            SoftDebugger.Stop();
            Logger.WriteInfoLine("Process stopped.");
        }
    }
}
