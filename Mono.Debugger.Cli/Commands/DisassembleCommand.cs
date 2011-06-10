using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class DisassembleCommand : ICommand
    {
        public string Name
        {
            get { return "Disasm"; }
        }

        public string Description
        {
            get { return "Disassembles the current stack frame."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            var frame = SoftDebugger.CurrentStackFrame;

            if (frame == null)
            {
                Logger.WriteErrorLine("No stack frame available.");
                return;
            }

            var disasm = frame.Disassemble(0, frame.SourceLocation.Line);

            foreach (var line in disasm)
                if (!line.IsOutOfRange)
                    Logger.WriteInfoLine("{0}:\t{1}", line.Address.ToString(Environment.Is64BitProcess ? "X8" : "X4"), line.Code);
        }
    }
}
