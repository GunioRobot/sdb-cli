using System.Collections.Generic;
using Mono.Debugging.Client;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class CatchpointCommand : ICommand
    {
        public string Name
        {
            get { return "CP"; }
        }

        public string Description
        {
            get { return "Creates/removes/lists catchpoints."; }
        }

        public string Arguments
        {
            get { return "[Add <ExcName>|Delete <ExcName>|Clear]"; }
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
                    var excName = args.NextString();
                    session.Breakpoints.AddCatchpoint(excName);

                    Logger.WriteInfoLine("Added catchpoint for exception: {0}", excName);
                    return;
                case "del":
                    var delExcName = args.NextString();
                    session.Breakpoints.RemoveCatchpoint(delExcName);

                    Logger.WriteInfoLine("Deleted catchpoint for exception: {0}", delExcName);
                    return;
                case "clear":
                    session.Breakpoints.ClearCatchpoints();

                    Logger.WriteInfoLine("Cleared all catchpoints.");
                    return;
                case "":
                    Logger.WriteInfoLine("Catchpoints:");

                    foreach (var cp in session.Breakpoints.GetCatchpoints())
                        Logger.WriteInfoLine("{0}", cp.ExceptionName);
                    return;
            }

            Logger.WriteErrorLine("Unknown catchpoint operation: {0}", op);
        }
    }
}
