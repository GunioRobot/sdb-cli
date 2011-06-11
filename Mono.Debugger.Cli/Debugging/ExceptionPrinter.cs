using System.Collections.Generic;
using Mono.Debugger.Cli.Logging;
using Mono.Debugger.Soft;

namespace Mono.Debugger.Cli.Debugging
{
    internal static class ExceptionPrinter
    {
        public static void Print(ThreadMirror thread, ObjectMirror ex)
        {
            // We disregard thread safety here, since an extra lookup really doesn't matter...
            var exception = _exception ?? (_exception = thread.Domain.Corlib.GetType("System.Exception"));
            var getMessage = _exGetMsg ?? (_exGetMsg = exception.GetProperty("Message").GetGetMethod());
            var getInner = _exGetInner ?? (_exGetInner = exception.GetProperty("InnerException").GetGetMethod());

            PrintException(string.Empty, thread, getMessage, ex);

            var innerEx = ex.InvokeMethod(thread, getInner, new List<Value>());
            while (!(innerEx is PrimitiveValue)) // If it's a PrimitiveValue, it's null.
            {
                var exMirror = (ObjectMirror)innerEx;
                PrintException("--> ", thread, getMessage, exMirror);

                innerEx = exMirror.InvokeMethod(thread, getInner, _noValues);
            }
        }

        private static void PrintException(string p, ThreadMirror thread, MethodMirror getMsg, ObjectMirror x)
        {
            var msg = (StringMirror)x.InvokeMethod(thread, getMsg, new List<Value>());
            Logger.WriteErrorLine("{0}{1}: {2}", p, x.Type.FullName, msg.Value);
        }

        private static readonly List<Value> _noValues = new List<Value>();

        private static TypeMirror _exception;

        private static MethodMirror _exGetMsg;

        private static MethodMirror _exGetInner;
    }
}
