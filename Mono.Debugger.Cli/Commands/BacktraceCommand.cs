using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class BacktraceCommand : ICommand
    {
        public string Name
        {
            get { return "BT"; }
        }

        public string Description
        {
            get { return "Displays a backtrace when in a paused state."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (!SoftDebugger.IsPaused)
            {
                Logger.WriteErrorLine("Process is not paused.");
                return;
            }

            var bt = SoftDebugger.CurrentBacktrace;
            var count = bt.FrameCount - 1;
            foreach (var frame in bt.GetStackFrames(0, bt.FrameCount))
            {
                var loc = frame.SourceLocation;
                var location = "<unknown>";

                // We can't really rely on frame.HasDebugInfo.
                if (loc.FileName != string.Empty && loc.Line != -1)
                    location = string.Format("{0}:{1}{2}", loc.FileName, loc.Line, loc.Column == -1 ? string.Empty : "," + loc.Column);

                // TODO: Build the method name.
                var method = loc.MethodName;

                Logger.WriteInfoLine("[{0}] {1}: {2}", count, location, method);
                count--;
            }
        }
    }
}
