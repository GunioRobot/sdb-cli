using System.Collections.Generic;

namespace Mono.Debugger.Cli
{
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        IEnumerable<string> Arguments { get; }

        void Execute(CommandArguments args);
    }
}
