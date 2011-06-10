using System;
using System.Collections.Generic;
using System.IO;
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
            get { yield return Argument.Optional("Source"); }
        }

        public void Execute(CommandArguments args)
        {
            var backtrace = SoftDebugger.Backtrace;

            if (backtrace == null)
            {
                Logger.WriteErrorLine("No backtrace available.");
                return;
            }

            var showSource = false;
            if (args.HasArguments)
            {
                var op = args.NextString();

                switch (op.ToLower())
                {
                    case "source":
                        showSource = true;
                        break;
                    default:
                        Logger.WriteErrorLine("Unknown backtrace operation: {0}", op);
                        return;
                }
            }

            var bt = backtrace.CurrentBacktrace;
            for (var i = 0; i < bt.FrameCount; i++)
            {
                var frame = bt.GetFrame(i);
                var loc = frame.SourceLocation;
                var fileName = loc.FileName;
                var location = "<unknown>";

                // We can't really rely on frame.HasDebugInfo.
                var hasSource = loc.HasSource();
                if (hasSource)
                    location = string.Format("{0}:{1}{2}", fileName, loc.Line, loc.Column == -1 ? string.Empty : "," + loc.Column);

                // TODO: Build the method name.
                var method = loc.MethodName;

                var str = string.Format("[{0}] {1}: {2}", i, location, method);
                if (i == backtrace.CurrentStackFrameId)
                    Logger.WriteEmphasisLine("{0}", str);
                else
                    Logger.WriteInfoLine("{0}", str);

                if (showSource && hasSource)
                {
                    // Locate the source code.
                    var reader = SoftDebugger.Session.GetSourceReader(fileName);
                    if (reader != null)
                    {
                        if (File.GetLastWriteTime(fileName) > SoftDebugger.CurrentExecutable.LastWriteTime)
                            Logger.WriteWarningLine("Source file {0} is newer than the debugged executable!", fileName);

                        for (var j = 0; j < loc.Line - 1; j++)
                            reader.ReadLine();

                        var source = reader.ReadLine();
                        Logger.WriteInfoLine("{0}", source);
                    }
                    else
                        Logger.WriteInfoLine("<no source>");
                }
            }
        }
    }
}
