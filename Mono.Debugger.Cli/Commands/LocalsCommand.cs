using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class LocalsCommand : ICommand
    {
        public string Name
        {
            get { return "Locals"; }
        }

        public string Description
        {
            get { return "Prints local variables."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
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

            foreach (var local in frame.GetLocalVariables())
                Logger.WriteInfoLine("[{0}] {1}: {2}", local.TypeName, local.Name, local.DisplayValue);
        }
    }
}
