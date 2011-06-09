using System;
using System.Runtime.Serialization;

namespace Mono.Debugger.Cli
{
    [Serializable]
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException()
        {
        }

        public CommandArgumentException(string message)
            : base(message)
        {
        }

        public CommandArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected CommandArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
