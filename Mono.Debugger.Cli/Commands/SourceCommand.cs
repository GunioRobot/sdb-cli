using System;
using System.Collections.Generic;
using System.IO;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class SourceCommand : ICommand
    {
        public string Name
        {
            get { return "Source"; }
        }

        public string Description
        {
            get { return "Prints the source code for the current stack frame."; }
        }

        public string Arguments
        {
            get { return "[<LowerOffset> <UpperOffset>]"; }
        }

        public void Execute(CommandArguments args)
        {
            var hasArgs = args.HasArguments;
            var lowerOffset = 0;
            var upperOffset = 0;

            if (hasArgs)
            {
                lowerOffset = args.NextInt32();
                upperOffset = args.NextInt32();

                if (upperOffset < 0)
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

            var loc = frame.SourceLocation;

            if (loc.HasSource())
            {
                var fileName = loc.FileName;
                var line = loc.Line;
                var reader = SoftDebugger.Session.GetSourceReader(fileName);

                if (reader != null)
                {
                    if (File.GetLastWriteTime(fileName) > SoftDebugger.CurrentExecutable.LastWriteTime)
                        Logger.WriteWarningLine("Source file {0} is newer than the debugged executable!", fileName);

                    var start = hasArgs ? line + lowerOffset : line - 5;

                    if (start < 0)
                        start = 0;

                    var end = (hasArgs ? line + upperOffset : start + 10) + 1;

                    for (var i = 0; i < end - 1; i++)
                    {
                        var src = reader.ReadLine();

                        if (i < start - 1)
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
