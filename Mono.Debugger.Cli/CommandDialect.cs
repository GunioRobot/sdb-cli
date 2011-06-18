using System;
using System.Collections.Generic;

namespace Mono.Debugger.Cli
{
    public sealed class CommandDialect
    {
        public const string Sdb = "sdb";

        public const string Gdb = "gdb";

        internal CommandDialect(string name, Dictionary<string, ICommand> commands)
        {
            Name = name;
            Commands = commands;
        }

        internal string Name { get; private set; }

        internal Dictionary<string, ICommand> Commands { get; private set; }
    }
}
