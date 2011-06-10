using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Commands;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli
{
    public static class CommandLine
    {
        public static bool Stop { get; set; }

        public static List<ICommand> Commands { get; private set; }

        static CommandLine()
        {
            Commands = new List<ICommand>
            {
                new HelpCommand(),
                new ExitCommand(),
                new CurrentDirectoryCommand(),
                new StartCommand(),
                new PauseCommand(),
                new ContinueCommand(),
                new StopCommand(),
                new BreakpointCommand(),
                new FirstChanceCommand(),
                new BacktraceCommand(),
                new FrameCommand(),
                new DisassembleCommand(),
                new SourceCommand(),
                new LocalsCommand(),
                new EvaluationCommand(),
                new WatchCommand(),
            };
        }

        public static void CommandLoop()
        {
            Logger.WriteInfoLine("Welcome to the Mono Soft Debugger CLI!");
            Logger.WriteInfoLine("Using {0} and {1} with features: {2}", typeof(VirtualMachine).Assembly.GetName().Name,
                typeof(SoftDebuggerSession).Assembly.GetName().Name, SoftDebugger.Features);
            Logger.WriteInfoLine("Type \"Help\" for a list of commands.");

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var fullCmd = line.Split(' ');
                var cmd = fullCmd[0];

                var command = Commands.SingleOrDefault(x => x.Name.Equals(cmd, StringComparison.OrdinalIgnoreCase));
                if (command == null)
                {
                    Logger.WriteErrorLine("No such command.");
                    continue;
                }

                try
                {
                    command.Execute(new CommandArguments(fullCmd.Skip(1)));
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorLine("Error executing command {0}:", cmd);
                    Logger.WriteErrorLine(ex.Message);

                    if (!(ex is CommandArgumentException))
                        Logger.WriteErrorLine(ex.StackTrace);
                }

                if (Stop)
                    break;
            }

            if (SoftDebugger.Session != null)
                SoftDebugger.Stop();
        }
    }
}
