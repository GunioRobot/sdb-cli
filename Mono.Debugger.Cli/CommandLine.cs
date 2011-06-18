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

        internal static CommandDialect Dialect { get; private set; }

        internal static AutoResetEvent ResumeEvent { get; private set; }

        static CommandLine()
        {
            ResumeEvent = new AutoResetEvent(false);

            switch (Configuration.CommandDialect)
            {
                case CommandDialect.Sdb:
                    InitializeSdbDialect();
                    break;
                case CommandDialect.Gdb:
                    InitializeGdbDialect();
                    break;
                default:
                    Logger.WriteErrorLine("Unknown command dialect: {0}", Configuration.CommandDialect);
                    InitializeSdbDialect();
                    break;
            }
        }

        private static void InitializeSdbDialect()
        {
            Dialect = new CommandDialect(CommandDialect.Sdb, new Dictionary<string, ICommand>
            {
                { "Help", new HelpCommand() },
                { "Exit", new ExitCommand() },
                { "CD", new CurrentDirectoryCommand() },
                { "Init", new InitializeCommand() },
                { "Start", new StartCommand() },
                { "Pause", new PauseCommand() },
                { "Continue", new ContinueCommand() },
                { "Step", new StepCommand() },
                { "Stop", new StopCommand() },
                { "BP", new BreakpointCommand() },
                { "CP", new CatchpointCommand() },
                { "DB", new DatabaseCommand() },
                { "FC", new FirstChanceCommand() },
                { "BT", new BacktraceCommand() },
                { "Frame", new FrameCommand() },
                { "Disasm", new DisassembleCommand() },
                { "Source", new SourceCommand() },
                { "Decompile", new DecompileCommand() },
                { "Locals", new LocalsCommand() },
                { "Eval", new EvaluationCommand() },
                { "Watch", new WatchCommand() },
                { "Thread", new ThreadCommand() },
            });
        }

        private static void InitializeGdbDialect()
        {
            Dialect = new CommandDialect(CommandDialect.Sdb, new Dictionary<string, ICommand>
            {
                { "Help", new HelpCommand() },
                { "Quit", new ExitCommand() },
                { "CD", new CurrentDirectoryCommand() },
                { "Init", new InitializeCommand() },
                { "Run", new StartCommand() },
                { "Pause", new PauseCommand() },
                { "Continue", new ContinueCommand() },
                { "Step", new StepCommand() },
                { "Stop", new StopCommand() },
                { "Break", new BreakpointCommand() },
                { "Catch", new CatchpointCommand() },
                { "DB", new DatabaseCommand() },
                { "FC", new FirstChanceCommand() },
                { "BT", new BacktraceCommand() },
                { "Frame", new FrameCommand() },
                { "Disassemble", new DisassembleCommand() },
                { "Source", new SourceCommand() },
                { "Decompile", new DecompileCommand() },
                { "Locals", new LocalsCommand() },
                { "Print", new EvaluationCommand() },
                { "Watch", new WatchCommand() },
                { "Thread", new ThreadCommand() },
            });
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
                var command = Dialect.Commands.SingleOrDefault(x => x.Key.Equals(cmd, StringComparison.OrdinalIgnoreCase)).Value;

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
