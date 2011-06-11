using System;
using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class FirstChanceCommand : ICommand
    {
        public string Name
        {
            get { return "FC"; }
        }

        public string Description
        {
            get { return "Sets/displays whether we're catching first-chance exceptions."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Optional("Enabled"); }
        }

        public void Execute(CommandArguments args)
        {
            if (!args.HasArguments)
            {
                Logger.WriteInfoLine("Currently {0} first-chance exceptions.", SoftDebugger.CatchFirstChanceExceptions ?
                    "catching" : "passing on");
                return;
            }

            var fc = args.NextBoolean();
            SoftDebugger.CatchFirstChanceExceptions = fc;
            Logger.WriteInfoLine("Now {0} first-chance exceptions.", fc ? "catching" : "passing on");
        }
    }
}
