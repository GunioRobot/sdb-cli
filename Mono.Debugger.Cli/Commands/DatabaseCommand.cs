using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class DatabaseCommand : ICommand
    {
        public string Name
        {
            get { return "DB"; }
        }

        public string Description
        {
            get { return "Saves/loads the debugger database for the active process."; }
        }

        public string Arguments
        {
            get { return "Save|Load"; }
        }

        public void Execute(CommandArguments args)
        {
            var op = args.NextString();

            if (SoftDebugger.State == DebuggerState.Null)
            {
                Logger.WriteErrorLine("No session active.");
                return;
            }

            if (SoftDebugger.State == DebuggerState.Initialized)
            {
                Logger.WriteErrorLine("No process active.");
                return;
            }

            var breakpoints = SoftDebugger.Session.Breakpoints;
            var fileName = SoftDebugger.CurrentExecutable.FullName + ".sdb";

            switch (op.ToLower())
            {
                case "save":
                    using (var writer = XmlWriter.Create(fileName))
                    {
                        breakpoints.Save().WriteTo(writer);
                        writer.Flush();
                    }
                    return;
                case "load":
                    var doc = new XmlDocument();
                    doc.Load(fileName);
                    breakpoints.Load(doc.DocumentElement);
                    return;
            }

            Logger.WriteErrorLine("Unknown database operation: {0}", op);
        }
    }
}
