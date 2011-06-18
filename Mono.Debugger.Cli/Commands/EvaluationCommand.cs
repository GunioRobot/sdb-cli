using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class EvaluationCommand : ICommand
    {
        public string Description
        {
            get { return "Evaluates an expression in the current stack frame's context."; }
        }

        public string Arguments
        {
            get { return "<Expr>"; }
        }

        public void Execute(CommandArguments args)
        {
            var expr = args.NextString();

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

            try
            {
                var result = frame.GetExpressionValue(expr, true);
                Logger.WriteInfoLine("[{0}] {1}", result.TypeName, result.DisplayValue);
            }
            catch (Exception)
            {
                Logger.WriteErrorLine("Error in expression.");
            }
        }
    }
}
