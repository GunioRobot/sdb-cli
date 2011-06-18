using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        internal static bool Stopped { get; set; }

        internal static bool Suspended { get; set; }

        internal static List<ICommand> Commands { get; private set; }

        internal static AutoResetEvent ResumeEvent { get; private set; }

        static CommandLine()
        {
            Commands = new List<ICommand>
            {
                new HelpCommand(),
                new ExitCommand(),
                new CurrentDirectoryCommand(),
                new InitializeCommand(),
                new StartCommand(),
                new PauseCommand(),
                new ContinueCommand(),
                new StepCommand(),
                new StopCommand(),
                new BreakpointCommand(),
                new CatchpointCommand(),
                new DatabaseCommand(),
                new FirstChanceCommand(),
                new BacktraceCommand(),
                new FrameCommand(),
                new DisassembleCommand(),
                new SourceCommand(),
                new DecompileCommand(),
                new LocalsCommand(),
                new EvaluationCommand(),
                new WatchCommand(),
                new ThreadCommand(),
            };

            ResumeEvent = new AutoResetEvent(false);
        }

        internal static void CommandLoop()
        {
            Logger.WriteInfoLine("Welcome to the Mono Soft Debugger CLI!");
            Logger.WriteInfoLine("Using {0} and {1} with features: {2}", typeof(VirtualMachine).Assembly.GetName().Name,
                typeof(SoftDebuggerSession).Assembly.GetName().Name, SoftDebugger.Features);
            Logger.WriteInfoLine("Type \"Help\" for a list of commands or \"Exit\" to quit.");

            string line;

            while (true)
            {
                Logger.WriteInfo(string.Empty);
                line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var fullCmd = line.Split(' ');
                var cmd = fullCmd[0];

                var command = Commands.SingleOrDefault(x => x.Name.Equals(cmd, StringComparison.OrdinalIgnoreCase));
                if (command == null)
                {
                    Logger.WriteErrorLine("No such command: {0}", cmd);
                    continue;
                }

                try
                {
                    command.Execute(new CommandArguments(fullCmd.Skip(1)));
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorLine("Error executing command:", cmd);
                    Logger.WriteErrorLine(ex.Message);

                    if (!(ex is CommandArgumentException))
                        Logger.WriteErrorLine(ex.StackTrace);
                }

                if (Suspended)
                {
                    ResumeEvent.WaitOne();
                    Suspended = false;
                }

                if (Stopped)
                    break;
            }

            if (SoftDebugger.State != DebuggerState.Null)
                SoftDebugger.Stop();
        }
    }
}
