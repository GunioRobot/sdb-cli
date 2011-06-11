using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal class HelpCommand : ICommand
    {
        public string Name
        {
            get { return "Help"; }
        }

        public string Description
        {
            get { return "Displays a list of available commands."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
        }

        public void Execute(CommandArguments args)
        {
            foreach (var command in CommandLine.Commands)
                Logger.WriteInfoLine("{0} {1}: {2}", command.Name, command.Arguments, command.Description);
        }
    }
}
