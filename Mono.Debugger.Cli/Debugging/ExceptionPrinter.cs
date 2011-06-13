using System.Collections.Generic;
using Mono.Debugging.Client;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Debugging
{
    internal static class ExceptionPrinter
    {
        public static void Print(ExceptionInfo ex)
        {
            PrintException(string.Empty, ex);

            var innerEx = ex;
            while ((innerEx = innerEx.InnerException) != null)
                PrintException("--> ", innerEx);
        }

        private static void PrintException(string prefix, ExceptionInfo ex)
        {
            Logger.WriteErrorLine("{0}{1}: {2}", prefix, ex.Type, ex.Message);
        }
    }
}
