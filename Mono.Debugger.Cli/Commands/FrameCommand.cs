using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class FrameCommand : ICommand
    {
        public string Description
        {
            get { return "Switches/shows the current stack frame."; }
        }

        public string Arguments
        {
            get { return "[<FrameId>]"; }
        }

        public void Execute(CommandArguments args)
        {
            var hasArgs = args.HasArguments;
            var frame = 0;

            if (hasArgs)
                frame = args.NextInt32();

            var backtrace = SoftDebugger.Backtrace;

            if (backtrace == null)
            {
                Logger.WriteErrorLine("No backtrace is available.");
                return;
            }

            if (frame < 0 || frame > backtrace.CurrentBacktrace.Count - 1)
            {
                Logger.WriteErrorLine("Invalid stack frame.");
                return;
            }

            if (hasArgs)
            {
                backtrace.SetActiveFrame(frame);
                Logger.WriteInfoLine("Switched to frame: {0}", frame);
            }
            else
                Logger.WriteInfoLine("Current frame: {0}", backtrace.CurrentStackFrameId);
        }
    }
}
