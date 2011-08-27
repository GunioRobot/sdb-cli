using System;
using System.IO;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class LogCommand : ICommand
    {
        public string Description
        {
            get { return "Enables/disables file logging."; }
        }

        public string Arguments
        {
            get { return "Start <FileName>|Stop"; }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString(string.Empty);

            switch (op.ToLower())
            {
                case "start":
                    var fileName = args.NextString();

                    try
                    {
                        Logger.LogOutput = new StreamWriter(fileName, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Could not open log file {0}: {1}", fileName, ex.Message);
                    }
                    return;
                case "stop":
                    Logger.LogOutput.Dispose();
                    Logger.LogOutput = null;
                    return;
            }

            Logger.WriteErrorLine("Unknown log operation: {0}", op);
        }
    }
}
