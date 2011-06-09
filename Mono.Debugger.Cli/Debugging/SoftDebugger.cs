using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Cli.Logging;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugger.Cli.Debugging
{
    public static class SoftDebugger
    {
        public static DebuggerFeatures Features
        {
            get
            {
                return DebuggerFeatures.ConditionalBreakpoints |
                    DebuggerFeatures.Tracepoints |
                    DebuggerFeatures.Catchpoints |
                    DebuggerFeatures.DebugFile |
                    DebuggerFeatures.Stepping |
                    DebuggerFeatures.Pause |
                    DebuggerFeatures.Breakpoints |
                    DebuggerFeatures.Disassembly;
            }
        }

        public static SoftDebuggerSession Session { get; set; }

        public static SoftDebuggerBacktrace CurrentBacktrace { get; private set; }

        public static string WorkingDirectory { get; set; }

        public static bool IsPaused { get; private set; }

        static SoftDebugger()
        {
            InitializeSession();

            WorkingDirectory = Environment.CurrentDirectory;
        }

        private static void InitializeSession()
        {
            Session = new SoftDebuggerSession
            {
                Breakpoints = new BreakpointStore(),
            };

            Session.ExceptionHandler = ex =>
            {
                Logger.WriteErrorLine("{0}: {1}", ex.GetType(), ex.Message);

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.WriteErrorLine("--> {0}: {1}", innerEx.GetType(), innerEx.Message);

                    innerEx = innerEx.InnerException;
                }

                return true;
            };

            Session.LogWriter = (isStdErr, text) =>
            {
                var str = string.Format("[Mono] {0}", text);

                if (isStdErr)
                    Logger.WriteError(str);
                else
                    Logger.WriteDebug(str);
            };

            Session.OutputWriter = (isStdErr, text) =>
            {
                if (isStdErr)
                    Console.Error.Write(text);
                else
                    Console.Write(text);
            };

            Session.TargetUnhandledException += ExceptionHandler;

            Session.TargetExceptionThrown += ExceptionHandler;

            Session.BreakpointTraceHandler = (be, trace) =>
            {
            	Logger.WriteInfoLine("Breakpoint trace: {0} {0}", be, trace);
            };

            Session.CustomBreakEventHitHandler += (actionId, be) =>
            {
                Logger.WriteInfoLine("Custom break event: {0} {1}", actionId, be);
                return true;
            };
        }

        private static void ExceptionHandler(object sender, TargetEventArgs e)
        {
            var session = (SoftDebuggerSession)sender;
            var thread = session.VirtualMachine.GetThreads().Single(x => x.Id == e.Thread.Id);
            var ex = session.GetExceptionObject(thread);

            IsPaused = true;
            CurrentBacktrace = new SoftDebuggerBacktrace (session, thread);

            ExceptionPrinter.Print(thread, ex);
        }

        public static void Start(string path, string args)
        {
            if (Session == null)
                InitializeSession();

            // TODO: Locate Mono somehow...
            Session.Run(new SoftDebuggerStartInfo("/usr/local", new Dictionary<string, string>())
            {
                Arguments = args,
                Command = path,
                WorkingDirectory = WorkingDirectory,
            }, new DebuggerSessionOptions
            {
                EvaluationOptions = EvaluationOptions.DefaultOptions,
            });
        }

        public static void Stop()
        {
            if (Session.IsRunning || IsPaused)
                Session.Exit();

            //Session.Dispose();
            Session = null;
        }
    }
}
