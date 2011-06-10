using System;
using System.Collections.Generic;
using System.IO;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    public sealed class BacktraceCommand : ICommand
    {
        public string Name
        {
            get { return "BT"; }
        }

        public string Description
        {
            get { return "Displays a backtrace when in a paused state."; }
        }

        public IEnumerable<string> Arguments
        {
            get { return Argument.None(); }
        }

        public void Execute(CommandArguments args)
        {
            if (!SoftDebugger.IsPaused)
            {
                Logger.WriteErrorLine("Process is not paused.");
                return;
            }

            var bt = SoftDebugger.CurrentBacktrace;
            for (var i = 0; i < bt.FrameCount; i++)
            {
                var frame = bt.GetFrame(i);
                var loc = frame.SourceLocation;
                var fileName = loc.FileName;
                var location = "<unknown>";

                // We can't really rely on frame.HasDebugInfo.
                var hasSource = fileName != string.Empty && loc.Line != -1;
                if (hasSource)
                    location = string.Format("{0}:{1}{2}", fileName, loc.Line, loc.Column == -1 ? string.Empty : "," + loc.Column);

                // TODO: Build the method name.
                var method = loc.MethodName;

                Logger.WriteInfoLine("[{0}] {1}: {2}", i, location, method);

                if (hasSource)
                {
                    // Locate the source code.
                    StreamReader reader;
                    if (!_fileReaderCache.TryGetValue(fileName, out reader))
                    {
                        try
                        {
                            reader = File.OpenText(fileName);
                        }
                        catch (Exception)
                        {
                        }

                        if (reader != null)
                            _fileReaderCache.Add(fileName, reader);
                    }
                    else
                    {
                        reader.BaseStream.Position = 0;
                        reader.DiscardBufferedData();
                    }

                    if (reader != null)
                    {
                        for (var j = 0; j < loc.Line - 1; j++)
                            reader.ReadLine();

                        var source = reader.ReadLine();
                        Logger.WriteInfoLine("{0}", source);
                    }
                }
            }
        }

        private readonly Dictionary<string, StreamReader> _fileReaderCache = new Dictionary<string, StreamReader>();
    }
}
