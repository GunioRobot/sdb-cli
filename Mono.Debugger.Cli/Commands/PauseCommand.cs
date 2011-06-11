using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class PauseCommand : ICommand
    {
        public string Name
        {
            get { return "Pause"; }
        }

        public string Description
        {
            get { return "Pauses the debuggee process."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.State == DebuggerState.Null)
            {
                Logger.WriteErrorLine("No session active.");
                return;
            }

            if (SoftDebugger.State == DebuggerState.Initialized)
            {
                Logger.WriteErrorLine("No process active.");
                return;
            }

            if (SoftDebugger.State == DebuggerState.Paused)
            {
                Logger.WriteErrorLine("Process is already paused.");
                return;
            }

            SoftDebugger.Pause();
        }
    }
}
