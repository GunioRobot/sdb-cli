using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli.Debugging
{
    public sealed class BacktraceInfo
    {
        public BacktraceInfo(IList<StackFrame> bt)
        {
            CurrentBacktrace = new ReadOnlyCollection<StackFrame>(bt);
        }

        public ReadOnlyCollection<StackFrame> CurrentBacktrace { get; private set; }

        public StackFrame CurrentStackFrame { get; set; }

        public int CurrentStackFrameId { get; set; }

        public void SetActiveFrame(int frameId)
        {
            CurrentStackFrame = CurrentBacktrace[frameId];
            CurrentStackFrameId = frameId;
        }
    }
}
