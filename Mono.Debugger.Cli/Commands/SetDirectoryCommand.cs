using System.Collections.Generic;
using Mono.Debugger.Cli.Debugging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class SetDirectoryCommand : ICommand
    {
        public string Name
        {
            get { return "CWD"; }
        }

        public string Description
        {
            get { return "Change current working directory."; }
        }

        public IEnumerable<string> Arguments
        {
            get { yield return Argument.Required("Dir"); }
        }

        public void Execute(CommandArguments args)
        {
            SoftDebugger.WorkingDirectory = args.NextString();
        }
    }
}
