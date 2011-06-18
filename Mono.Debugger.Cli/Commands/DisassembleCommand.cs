using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class DisassembleCommand : ICommand
    {
        public string Description
        {
            get { return "Disassembles the current stack frame."; }
        }

        public string Arguments
        {
            get { return "[<LowerOffset> <UpperOffset>]"; }
        }

        public void Execute(CommandArguments args)
        {
            var hasArgs = args.HasArguments;
            var lowerOffset = 0;
            var upperOffset = 0;

            if (hasArgs)
            {
                lowerOffset = args.NextInt32();
                upperOffset = args.NextInt32();

                if (upperOffset < 0)
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

            if (hasArgs)
                upperOffset += System.Math.Abs(lowerOffset);

            var disasm = frame.Disassemble(hasArgs ? lowerOffset : -10, hasArgs ? upperOffset + 1 : 20);

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
