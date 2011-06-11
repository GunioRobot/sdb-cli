using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class ExitCommand : ICommand
    {
        public string Name
        {
            get { return "Exit"; }
        }

        public string Description
        {
            get { return "Exits the debugger, killing any running process."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.State != DebuggerState.Null)
                SoftDebugger.Stop();

            CommandLine.Stopped = true;
            Logger.WriteInfoLine("Bye!");
        }
    }
}
