using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class EvaluationCommand : ICommand
    {
        public string Name
        {
            get { return "Eval"; }
        }

        public string Description
        {
            get { return "Evaluates an expression in the current stack frame's context."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Required("Expr"); }
        }

        public void Execute(CommandArguments args)
        {
            var expr = args.NextString();
            var frame = SoftDebugger.CurrentStackFrame;

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
