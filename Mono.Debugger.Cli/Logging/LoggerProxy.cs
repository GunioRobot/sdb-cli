using System;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli.Logging
{
    internal sealed class LoggerProxy : ICustomLogger
    {
        public void LogError(string message, Exception ex)
        {
            if (Configuration.SdbDebugLog)
                Logger.WriteErrorLine("[SDB] {0}: {1}", ex, message);
        }

        public void LogAndShowException(string message, Exception ex)
        {
            LogError(message, ex);
        }

        public void LogMessage(string messageFormat, params object[] args)
        {
            if (Configuration.SdbDebugLog)
                Logger.WriteInfoLine("[SDB] {0}", string.Format(messageFormat, args));
        }
    }
}
