using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class GetDirectoryCommand : ICommand
    {
        public string Name
        {
            get { return "GetCWD"; }
        }

        public string Description
        {
            get { return "Print current working directory."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            Logger.WriteInfoLine("Current working directory: {0}", SoftDebugger.WorkingDirectory);
        }
    }
}
