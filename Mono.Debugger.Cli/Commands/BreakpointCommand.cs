using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Commands
{
    internal class BreakpointCommand : ICommand
    {
        public string Name
        {
            get { return "BP"; }
        }

        public string Description
        {
            get { return "Creates/removes/lists breakpoints."; }
        }

        public string Arguments
        {
            get { return "[Add <FileName> <Line>|Set <FrameId> <Line>|Del <FileName> <Line>|Clear]"; }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString(string.Empty);

            if (SoftDebugger.State == DebuggerState.Null)
            {
                Logger.WriteErrorLine("No session active.");
                return;
            }

            var session = SoftDebugger.Session;

            switch (op.ToLower())
            {
                case "add":
                    var file = args.NextString();
                    var line = args.NextInt32();

                    if (line < 1)
                    {
                        Logger.WriteErrorLine("Invalid line number.");
                        return;
                    }

                    session.Breakpoints.Add(file, line, true);

                    Logger.WriteInfoLine("Added breakpoint: {0}:{1}", file, line);
                    return;
                case "set":
                    var frame = args.NextInt32();
                    var setLine = args.NextInt32();

                    var bt = SoftDebugger.Backtrace;

                    if (bt == null)
                    {
                        Logger.WriteErrorLine("No backtrace available.");
                        return;
                    }

                    if (frame < 0 || frame > bt.CurrentBacktrace.Count - 1)
                    {
                        Logger.WriteErrorLine("Invalid stack frame.");
                        return;
                    }

                    if (setLine < 1)
                    {
                        Logger.WriteErrorLine("Invalid line number.");
                        return;
                    }

                    var fileName = bt.CurrentBacktrace[frame].SourceLocation.FileName;
                    session.Breakpoints.Add(fileName, setLine);

                    Logger.WriteInfoLine("Set breakpoint: {0}:{1}");
                    return;
                case "del":
                    var delFile = args.NextString();
                    var delLine = args.NextInt32();

                    if (delLine < 1)
                    {
                        Logger.WriteErrorLine("Invalid line number.");
                        return;
                    }

                    session.Breakpoints.Remove(delFile, delLine);

                    Logger.WriteInfoLine("Deleted breakpoint: {0}:{1}", delFile, delLine);
                    return;
                case "clear":
                    session.Breakpoints.ClearBreakpoints();

                    Logger.WriteInfoLine("Cleared all breakpoints.");
                    return;
                case "":
                    Logger.WriteInfoLine("Breakpoints:");

                    foreach (var bp in session.Breakpoints.GetBreakpoints())
                        Logger.WriteInfoLine("{0}:{1}", bp.FileName, bp.Line);
                    return;
            }

            Logger.WriteErrorLine("Unknown breakpoint operation: {0}", op);
        }
    }
}
