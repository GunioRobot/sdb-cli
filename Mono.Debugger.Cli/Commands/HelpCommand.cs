using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal class HelpCommand : ICommand
    {
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
            foreach (var command in CommandLine.Dialect.Commands)
                Logger.WriteInfoLine("{0} {1}: {2}", command.Key, command.Value.Arguments, command.Value.Description);
        }
    }
}
