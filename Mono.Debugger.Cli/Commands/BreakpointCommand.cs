using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class BreakpointCommand : ICommand
    {
        public string Name
        {
            get { return "BP"; }
        }

        public string Description
        {
            get { return "Creates/removes/ breakpoints."; }
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                yield return Argument.Required("Add|Delete|Clear");
                yield return Argument.Optional("FileName", "Line");
            }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString();

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

                    session.Breakpoints.Add(file, line, true);

                    Logger.WriteInfoLine("Added breakpoint: {0}:{1}", file, line);
                    return;
                case "delete":
                    var delFile = args.NextString();
                    var delLine = args.NextInt32();

                    session.Breakpoints.Remove(delFile, delLine);

                    Logger.WriteInfoLine("Deleted breakpoint: {0}:{1}", delFile, delLine);
                    return;
                case "clear":
                    session.Breakpoints.ClearBreakpoints();

                    Logger.WriteInfoLine("Cleared all breakpoints.");
                    return;
            }

            Logger.WriteErrorLine("Unknown breakpoint operation: {0}", op);
        }
    }
}
