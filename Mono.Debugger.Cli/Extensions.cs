using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugging.Client;

namespace Mono.Debugger.Cli
{
    public static class Extensions
    {
        public static bool HasSource(this SourceLocation loc)
        {
            return loc.FileName != string.Empty && loc.Line != -1;
        }

        public static bool IsStarted(this DebuggerState state)
        {
            return state == DebuggerState.Running || state == DebuggerState.Paused;
        }
    }
}
