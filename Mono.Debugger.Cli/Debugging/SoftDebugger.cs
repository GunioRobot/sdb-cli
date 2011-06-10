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

        public static BacktraceState Backtrace { get; private set; }

        public static string WorkingDirectory { get; set; }

        public static bool CatchFirstChanceExceptions { get; set; }

        private static bool _isPaused;

        private static bool _isExcepted;

        private static bool _isDoomed;

        static SoftDebugger()
        {
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

            Session.TargetUnhandledException += (sender, e) => ExceptionHandler(sender, e, false);

            Session.TargetExceptionThrown += (sender, e) =>
            {
                if (CatchFirstChanceExceptions)
                    ExceptionHandler(sender, e, true);
            };

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

        private static void ExceptionHandler(object sender, TargetEventArgs e, bool firstChance)
        {
            var bt = e.Backtrace;

            Backtrace = new BacktraceState(bt)
            {
                CurrentStackFrame = bt.GetFrame(0),
                CurrentStackFrameId = 0,
            };

            _isPaused = true;
            _isExcepted = !firstChance;

            var session = (SoftDebuggerSession)sender;
            var thread = session.VirtualMachine.GetThreads().Single(x => x.Id == e.Thread.Id);
            var ex = session.GetExceptionObject(thread);

            ExceptionPrinter.Print(thread, ex);
        }

        public static void Start(string path, string args)
        {
            if (Session == null)
                InitializeSession();

            string runtimePath = null;
            string fullPath = null;

            foreach (var prefix in Configuration.RuntimePaths)
            {
                fullPath = Path.Combine(prefix, "bin");

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

            Logger.WriteInfoLine("Using runtime: {0}", fullPath);

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

        public static void Pause()
        {
            _isPaused = true;

            Session.Stop();
        }

        public static void Continue()
        {
            Backtrace = null;
            _isPaused = false;

            Session.Continue();

            _isDoomed = _isExcepted;
        }

        public static void Stop()
        {
            if (!_isDoomed && (Session.IsRunning || _isPaused))
                Session.Exit();

            //Session.Dispose();

            Backtrace = null;
            _isPaused = false;
            _isExcepted = false;
            _isDoomed = false;

            Session = null;
        }
    }
}
