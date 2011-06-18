using System.Collections.Generic;

namespace Mono.Debugger.Cli
{
    public interface ICommand
    {
        string Description { get; }

        string Arguments { get; }

        void Execute(CommandArguments args);
    }
}
