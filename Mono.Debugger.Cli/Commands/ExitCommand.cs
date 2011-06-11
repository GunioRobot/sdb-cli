using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class ExitCommand : ICommand
    {
        public string Name
        {
            get { return "Exit"; }
        }

        public string Description
        {
            get { return "Exits the debugger, killing any running process."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.State != DebuggerState.Null)
                SoftDebugger.Stop();

            CommandLine.Stop = true;
            Logger.WriteInfoLine("Bye!");
        }
    }
}
