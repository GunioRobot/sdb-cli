using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class InitializeCommand : ICommand
    {
        public string Name
        {
            get { return "Init"; }
        }

        public string Description
        {
            get { return "Initializes a new session."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
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
