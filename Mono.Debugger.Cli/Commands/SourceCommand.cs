using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class SourceCommand : ICommand
    {
        public string Name
        {
            get { return "Source"; }
        }

        public string Description
        {
            get { return "Prints the source code for the current stack frame."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Optional("LowerLines", "UpperLines"); }
        }

        public void Execute(CommandArguments args)
        {
            var hasArgs = args.HasArguments;
            var lowerLines = 0;
            var upperLines = 0;

            if (hasArgs)
            {
                lowerLines = args.NextInt32();
                upperLines = args.NextInt32();

                if (upperLines < 0)
                {
                    Logger.WriteErrorLine("Invalid line count.");
                    return;
                }
            }

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

            // TODO: Actual logic.
            var loc = frame.SourceLocation;
            var fileName = loc.FileName;
            var line = loc.Line;

            if (loc.HasSource())
            {
                var reader = SoftDebugger.Session.GetSourceReader(fileName);
                if (reader != null)
                {
                    var low = hasArgs ? lowerLines : line - 5;
                    if (low < 0)
                        low = 0;

                    var up = hasArgs ? upperLines : 20;

                    for (var i = 0; i < up - 1; i++)
                    {
                        var src = reader.ReadLine();

                        if (i < low - 1)
                            continue;

                        if (i == line - 1)
                            Logger.WriteEmphasisLine("{0}", src);
                        else
                            Logger.WriteInfoLine("{0}", src);

                        if (reader.EndOfStream)
                            break;
                    }
                }
                else
                    Logger.WriteErrorLine("Could not locate source code file: {0}", fileName);
            }
            else
                Logger.WriteErrorLine("Source code not available.");
        }
    }
}
