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
            get { return "Breakpoint"; }
        }

        public string Description
        {
            get { return "Performs various operations surrounding breakpoints."; }
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                yield return Argument.Required("Add|Delete|Clear");
                yield return Argument.Optional("FileName");
                yield return Argument.Optional("Line");
            }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString();

            var session = SoftDebugger.Session;
            if (session == null)
            {
                Logger.WriteErrorLine("No program loaded.");
                return;
            }

            string file;
            int line;

            switch (op.ToLower())
            {
                case "add":
                    file = args.NextString();
                    line = args.NextInt32();

                    session.Breakpoints.Add(file, line, true);

                    Logger.WriteInfoLine("Added breakpoint: {0}:{1}", file, line);
                    return;
                case "delete":
                    file = args.NextString();
                    line = args.NextInt32();

                    session.Breakpoints.Remove(file, line);

                    Logger.WriteInfoLine("Deleted breakpoint: {0}:{1}", file, line);
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
