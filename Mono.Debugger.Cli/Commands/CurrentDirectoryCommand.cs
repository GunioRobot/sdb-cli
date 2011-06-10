using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class CurrentDirectoryCommand : ICommand
    {
        public string Name
        {
            get { return "CD"; }
        }

        public string Description
        {
            get { return "Changes/prints current working directory."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Optional("Dir"); }
        }

        public void Execute(CommandArguments args)
        {
            if (args.HasArguments)
            {
                var dir = args.NextString();
                SoftDebugger.WorkingDirectory = dir;
                Logger.WriteInfoLine("Changed working directory: {0}", dir);
            }
            else
                Logger.WriteInfoLine("Current working directory: {0}", SoftDebugger.WorkingDirectory);
        }
    }
}
