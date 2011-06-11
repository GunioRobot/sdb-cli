using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli.Debugging
{
    public sealed class SoftDebuggerCliSession : SoftDebuggerSession
    {
        private long _lastWatchId;

        public SortedDictionary<long, string> Watches { get; private set; }

        private readonly Dictionary<string, StreamReader> _sourceReaders = new Dictionary<string, StreamReader>();

        public ProcessInfo ActiveProcess
        {
            get { return GetProcesses().Single(); }
        }

        public long GenerateWatchId()
        {
            return _lastWatchId++;
        }

        public StreamReader GetSourceReader(string fileName)
        {
            StreamReader reader;
            if (!_sourceReaders.TryGetValue(fileName, out reader))
            {
                try
                {
                    reader = File.OpenText(fileName);
                }
                catch (Exception)
                {
                }

                if (reader != null)
                    _sourceReaders.Add(fileName, reader);
            }
            else
            {
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData();
            }

            return reader;
        }

        public SoftDebuggerCliSession()
        {
            Watches = new SortedDictionary<long, string>();
        }
    }
}
