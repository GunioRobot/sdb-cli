using System;

namespace Mono.Debugger.Cli.Debugging
{
    public enum DebuggerState : byte
    {
        Null = 0,
        Initialized = 1,
        Running = 2,
        Paused = 3,
    }
}
