using System;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Debugging
{
    public sealed class BacktraceState
    {
        public BacktraceState(Backtrace bt)
        {
            CurrentBacktrace = bt;
        }

        public Backtrace CurrentBacktrace { get; private set; }

        public StackFrame CurrentStackFrame { get; set; }

        public int CurrentStackFrameId { get; set; }
    }
}
