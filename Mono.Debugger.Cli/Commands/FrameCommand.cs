using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class FrameCommand : ICommand
    {
        public string Name
        {
            get { return "Frame"; }
        }

        public string Description
        {
            get { return "Switches the current stack frame."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Required("FrameId"); }
        }

        public void Execute(CommandArguments args)
        {
            if (SoftDebugger.CurrentBacktrace == null)
            {
                Logger.WriteErrorLine("No backtrace is available.");
                return;
            }

            var bt = SoftDebugger.CurrentBacktrace;
            var frame = args.NextInt32();

            if (frame < 0 || frame > bt.FrameCount - 1)
            {
                Logger.WriteErrorLine("Invalid stack frame.");
                return;
            }

            SoftDebugger.CurrentStackFrame = bt.GetFrame(frame);
            Logger.WriteInfoLine("Switched to frame: {0}", frame);
        }
    }
}
