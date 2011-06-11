using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class ThreadsCommand : ICommand
    {
        public string Name
        {
            get { return "Threads"; }
        }

        public string Description
        {
            get { return "Lists all active threads."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            var session = SoftDebugger.Session;

            if (SoftDebugger.State == DebuggerState.Null)
            {
                Logger.WriteErrorLine("No session active.");
                return;
            }

            if (SoftDebugger.State == DebuggerState.Initialized)
            {
                Logger.WriteErrorLine("No process active.");
                return;
            }

            var threads = session.VirtualMachine.GetThreads();

            foreach (var thread in threads)
                Logger.WriteInfoLine("[{0}] {1}: {2}", thread.Id, thread.Name, thread.ThreadState);
        }
    }
}
