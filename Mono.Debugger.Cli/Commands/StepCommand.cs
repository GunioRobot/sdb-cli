using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class StepCommand : ICommand
    {
        public string Name
        {
            get { return "Step"; }
        }

        public string Description
        {
            get { return "Steps into/over an instruction/line or out of a method."; }
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                yield return Argument.Required("Into|Over|Out");
                yield return Argument.Optional("Line/Instr");
            }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString();
            var session = SoftDebugger.Session;

            if (session == null)
            {
                Logger.WriteErrorLine("No process active.");
                return;
            }

            if (session.IsRunning)
            {
                Logger.WriteErrorLine("Process is running.");
                return;
            }

            switch (op.ToLower())
            {
                case "into":
                    var intoArgs = args.NextString();

                    switch (intoArgs)
                    {
                        case "line":
                            session.NextLine();
                            return;
                        case "instr":
                            session.NextInstruction();
                            return;
                    }

                    Logger.WriteErrorLine("Unknown step into operation: {0}", intoArgs);
                    return;
                case "over":
                    var overArgs = args.NextString();

                    switch (overArgs)
                    {
                        case "line":
                            session.StepLine();
                            return;
                        case "instr":
                            session.StepInstruction();
                            return;
                    }

                    Logger.WriteErrorLine("Unknown step over operation: {0}", overArgs);
                    return;
                case "out":
                    session.Finish();
                    return;
            }

            Logger.WriteErrorLine("Unknown step operation: {0}", op);
        }
    }
}
