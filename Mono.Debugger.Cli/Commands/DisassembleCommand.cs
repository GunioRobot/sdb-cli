using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class DisassembleCommand : ICommand
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
            get { yield return Argument.Optional("LowerLines", "UpperLines"); }
        }

        public void Execute(CommandArguments args)
        {
            var hasArgs = args.HasArguments;
            var lowerLines = 0;
            var upperLines = 0;

            if (hasArgs)
            {
                lowerLines = args.NextInt32();
                upperLines = args.NextInt32();

                if (upperLines < 0)
                {
                    Logger.WriteErrorLine("Invalid line count.");
                    return;
                }
            }

            var backtrace = SoftDebugger.Backtrace;

            if (backtrace == null)
            {
                Logger.WriteErrorLine("No backtrace available.");
                return;
            }

            var frame = backtrace.CurrentStackFrame;

            if (frame == null)
            {
                Logger.WriteErrorLine("No stack frame available.");
                return;
            }

            var disasm = frame.Disassemble(hasArgs ? lowerLines : 0, hasArgs ? upperLines : 20);

            foreach (var line in disasm)
            {
                if (line.IsOutOfRange)
                    continue;

                var str = string.Format("{0}:\t{1}", line.Address.ToString(Environment.Is64BitProcess ? "X8" : "X4"), line.Code);
                if (line.Address == frame.Address)
                    Logger.WriteEmphasisLine("{0}", str);
                else
                    Logger.WriteInfoLine("{0}", str);
            }
        }
    }
}
