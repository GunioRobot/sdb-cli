using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class HelpCommand : ICommand
    {
        public string Name
        {
            get { return "Help"; }
        }

        public string Description
        {
            get { return "Displays a list of available commands."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            foreach (var command in CommandLine.Commands)
            {
                var cmdArgs = command.Arguments.Aggregate(string.Empty, (current, arg) => current + arg + " ");
                Logger.WriteInfoLine("{0} {1}: {2}", command.Name, cmdArgs, command.Description);
            }
        }
    }
}
