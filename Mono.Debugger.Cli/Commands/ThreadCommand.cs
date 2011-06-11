using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class ThreadCommand : ICommand
    {
        public string Name
        {
            get { return "Thread"; }
        }

        public string Description
        {
            get { return "Lists/switches between active threads."; }
        }

        public string Arguments
        {
            get { return "[Switch <ThreadId>]"; }
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

            var threads = session.ActiveProcess.GetThreads();

            if (args.HasArguments)
            {
                var op = args.NextString();

                switch (op.ToLower())
                {
                    case "switch":
                        var reqId = args.NextInt32();

                        if (SoftDebugger.State == DebuggerState.Running)
                        {
                            Logger.WriteErrorLine("Process is running.");
                            return;
                        }

                        var thread = threads.Single(x => x.Id == reqId);

                        if (thread == null)
                        {
                            Logger.WriteErrorLine("Could not find thread: {0}", reqId);
                            return;
                        }

                        var bt = thread.Backtrace;
                        var list = new List<StackFrame>();

                        for (var i = 0; i < bt.FrameCount; i++)
                            list.Add(bt.GetFrame(i - 1));

                        SoftDebugger.Backtrace = new BacktraceInfo(list);
                        SoftDebugger.Backtrace.SetActiveFrame(0);

                        Logger.WriteInfoLine("Switched context to thread: {0}", reqId);
                        return;
                    default:
                        Logger.WriteErrorLine("Unknown thread operation: {0}", op);
                        return;
                }
            }

            Logger.WriteInfoLine("Threads:");

            foreach (var thread in threads)
            {
                var str = string.Format("[{0}] {1}: {2}", thread.Id, thread.Name, thread.Location);

                if (thread.Id == session.ActiveThread.Id)
                    Logger.WriteEmphasisLine("{0}", str);
                else
                    Logger.WriteInfoLine("{0}", str);
            }
        }
    }
}
