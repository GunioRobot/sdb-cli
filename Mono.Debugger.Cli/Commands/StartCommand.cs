using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class StartCommand : ICommand
    {
        public string Name
        {
            get { return "Start"; }
        }

        public string Description
        {
            get { return "Starts a process in the debugger and attaches to it."; }
        }

        public string Arguments
        {
            get { return "<Path> [<Args>]"; }
        }

        public void Execute(CommandArguments args)
        {
            var path = args.NextString();

            if (SoftDebugger.State.IsStarted())
            {
                Logger.WriteErrorLine("A process is already active.");
                return;
            }

            string progArg;
            var progArgs = string.Empty;
            while ((progArg = args.NextString(string.Empty)) != string.Empty)
                progArgs += progArg + " ";

            if (SoftDebugger.Start(path, progArgs))
                CommandLine.Suspended = true;
        }
    }
}
