using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class StartCommand : ICommand
    {
        public string Name
        {
            get { return "Start"; }
        }

        public string Description
        {
            get { return "Starts a process in the debugger and attaches to it."; }
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                yield return Argument.Required("Path");
                yield return Argument.Optional("Args");
            }
        }

        public void Execute(CommandArguments args)
        {
            var path = args.NextString();
            var session = SoftDebugger.Session;

            if (session != null && session.IsRunning)
            {
                // TODO: Display the actual process name (not Mono) somehow...
                var proc = SoftDebugger.Session.VirtualMachine.Process;
                Logger.WriteErrorLine("A process ({0}: {1}) is already running!", proc.Id, proc.ProcessName);
                return;
            }

            string progArg;
            var progArgs = string.Empty;
            while ((progArg = args.NextString(string.Empty)) != string.Empty)
                progArgs += progArg + " ";

            SoftDebugger.Start(path, progArgs);
        }
    }
}
