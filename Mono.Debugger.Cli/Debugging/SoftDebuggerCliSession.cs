using System;
using System.Collections.Generic;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli.Debugging
{
    public sealed class SoftDebuggerCliSession : SoftDebuggerSession
    {
        private long _lastWatchId;

        public SortedDictionary<long, string> Watches { get; private set; }

        public long GenerateWatchId()
        {
            return _lastWatchId++;
        }

        public SoftDebuggerCliSession()
        {
            Watches = new SortedDictionary<long, string>();
        }
    }
}
