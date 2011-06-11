using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class WatchCommand : ICommand
    {
        public string Name
        {
            get { return "Watch"; }
        }

        public string Description
        {
            get { return "Creates/removes/views expression watches."; }
        }

        public IEnumerable<string> Arguments
        {
            get
            {
                yield return Argument.Optional("Add|Delete|Clear");
                yield return Argument.Optional("Expr|WatchId");
            }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString(string.Empty);
            var session = SoftDebugger.Session;

            if (session == null)
            {
                Logger.WriteErrorLine("No program loaded.");
                return;
            }

            switch (op.ToLower())
            {
                case "add":
                    var expr = args.NextString();
                    var id = session.GenerateWatchId();
                    session.Watches.Add(id, expr);

                    Logger.WriteInfoLine("Added watch {0}: {1}", id, expr);
                    return;
                case "delete":
                    var delId = args.NextInt64();
                    session.Watches.Remove(delId);

                    Logger.WriteInfoLine("Deleted watch: {0}", delId);
                    return;
                case "clear":
                    session.Watches.Clear();

                    Logger.WriteInfoLine("Cleared all watches.");
                    return;
                case "":
                    var backtrace = SoftDebugger.Backtrace;

                    if (backtrace == null)
                    {
                        Logger.WriteErrorLine("No backtrace available.");
                        return;
                    }

                    var frame = backtrace.CurrentStackFrame;

                    if (frame == null)
                    {
                        Logger.WriteInfoLine("No stack frame available.");
                        return;
                    }

                    Logger.WriteInfoLine("Watches:");

                    foreach (var watch in session.Watches)
                    {
                        var evalExpr = watch.Value;
                        var prefix = string.Format("{0}: ({1}) =", watch.Key, evalExpr);

                        try
                        {
                            var result = frame.GetExpressionValue(evalExpr, true);
                            Logger.WriteInfoLine("{0} [{1}] {2}", prefix, result.TypeName, result.DisplayValue);
                        }
                        catch (Exception)
                        {
                            Logger.WriteErrorLine("{0} Error", prefix);
                        }
                    }

                    return;
            }

            Logger.WriteErrorLine("Unknown watch operation: {0}", op);
        }
    }
}
