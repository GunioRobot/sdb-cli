using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class InitializeCommand : ICommand
    {
        public string Name
        {
            get { return "Init"; }
        }

        public string Description
        {
            get { return "Initializes a new session."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.State != DebuggerState.Null)
            {
                Logger.WriteErrorLine("A session is already active.");
                return;
            }

            SoftDebugger.InitializeSession();
        }
    }
}
