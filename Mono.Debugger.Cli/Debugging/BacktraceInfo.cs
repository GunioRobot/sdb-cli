using System;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Debugging
{
    public sealed class BacktraceInfo
    {
        public BacktraceInfo(Backtrace bt)
        {
            CurrentBacktrace = bt;
        }

        public Backtrace CurrentBacktrace { get; private set; }

        public StackFrame CurrentStackFrame { get; set; }

        public int CurrentStackFrameId { get; set; }

        public void SetActiveFrame(int frameId)
        {
            CurrentStackFrame = CurrentBacktrace.GetFrame(frameId);
            CurrentStackFrameId = frameId;
        }
    }
}
