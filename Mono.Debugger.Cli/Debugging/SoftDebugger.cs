using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

        public static SoftDebuggerCliSession Session { get; set; }

        public static Backtrace CurrentBacktrace { get; private set; }

        public static Mono.Debugging.Client.StackFrame CurrentStackFrame { get; set; }

        public static string WorkingDirectory { get; set; }

        public static bool IsPaused { get; private set; }

        public static bool CatchFirstChanceExceptions { get; set; }

        static SoftDebugger()
        {
            InitializeSession();

            WorkingDirectory = Environment.CurrentDirectory;
        }

        private static void InitializeSession()
        {
            Session = new SoftDebuggerCliSession
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

            Session.TargetExceptionThrown += UnhandledExceptionHandler;

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
            CurrentBacktrace = e.Backtrace;
            CurrentStackFrame = CurrentBacktrace.GetFrame(e.Backtrace.FrameCount - 1);

            ExceptionPrinter.Print(thread, ex);
        }

        private static void UnhandledExceptionHandler(object sender, TargetEventArgs e)
        {
            if (CatchFirstChanceExceptions)
                ExceptionHandler(sender, e);
        }

        public static void Start(string path, string args)
        {
            if (Session == null)
                InitializeSession();

            string runtimePath = null;
            foreach (var prefix in Configuration.RuntimePaths)
            {
                var fullPath = Path.Combine(prefix, "bin");

                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                    fullPath = Path.Combine(fullPath, "mono");
                else
                    fullPath = Path.Combine(fullPath, "mono.exe");

                if (File.Exists(fullPath))
                {
                    runtimePath = prefix;
                    break;
                }
            }

            if (runtimePath == null)
            {
                Logger.WriteErrorLine("No valid runtime found.");
                return;
            }

            Logger.WriteInfoLine("Using runtime: {0}", runtimePath);

            Session.Run(new SoftDebuggerStartInfo(runtimePath, new Dictionary<string, string>())
            {
                Arguments = args,
                Command = path,
                WorkingDirectory = WorkingDirectory,
            }, new DebuggerSessionOptions
            {
                EvaluationOptions = EvaluationOptions.DefaultOptions,
            });
        }

        public static void Continue()
        {
            CurrentBacktrace = null;
            CurrentStackFrame = null;

            Session.Continue();
        }

        public static void Stop()
        {
            CurrentBacktrace = null;
            CurrentStackFrame = null;

            if (Session.IsRunning || IsPaused)
                Session.Exit();

            //Session.Dispose();
            Session = null;
        }
    }
}
