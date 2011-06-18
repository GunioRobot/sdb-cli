using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class LocalsCommand : ICommand
    {
        public string Description
        {
            get { return "Prints local variables."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
        }

        public void Execute(CommandArguments args)
        {
            var backtrace = SoftDebugger.Backtrace;

            if (backtrace == null)
            {
                Logger.WriteErrorLine("No backtrace available.");
                return;
            }

            var frame = backtrace.CurrentStackFrame;

            if (frame == null)
            {
                Logger.WriteErrorLine("No stack frame available.");
                return;
            }

            Logger.WriteInfoLine("Locals:");

            foreach (var local in frame.GetLocalVariables())
                if (!local.IsUnknown && !local.IsError && !local.IsNotSupported)
                    Logger.WriteInfoLine("[{0}] {1}: {2}", local.TypeName, local.Name, local.DisplayValue);
        }
    }
}
