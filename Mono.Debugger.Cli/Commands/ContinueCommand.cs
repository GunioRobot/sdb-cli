using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class ContinueCommand : ICommand
    {
        public string Name
        {
            get { return "Continue"; }
        }

        public string Description
        {
            get { return "Continues the debuggee process."; }
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

            if (SoftDebugger.State == DebuggerState.Running)
            {
                Logger.WriteErrorLine("Process is already running.");
                return;
            }

            SoftDebugger.Continue();
        }
    }
}
