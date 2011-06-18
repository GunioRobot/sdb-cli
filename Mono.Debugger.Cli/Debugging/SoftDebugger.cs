using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mono.Debugger.Cli.Logging;
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

        public static DebuggerState State
        {
            get
            {
                if (Session == null)
                    return DebuggerState.Null;

                if (_isPaused)
                    return DebuggerState.Paused;

                if (Session.IsRunning)
                    return DebuggerState.Running;

                return DebuggerState.Initialized;
            }
        }

        public static FileInfo CurrentExecutable { get; private set; }

        public static BacktraceInfo Backtrace { get; private set; }

        public static string WorkingDirectory { get; set; }

        public static bool CatchFirstChanceExceptions { get; set; }

        private static bool _isPaused;

        private static bool _isExcepted;

        private static bool _isDoomed;

        private static string _runtimePath;

        static SoftDebugger()
        {
            InitializeSession();

            WorkingDirectory = Environment.CurrentDirectory;
            CatchFirstChanceExceptions = true;

            Console.CancelKeyPress += (sender, e) =>
            {
                if (State == DebuggerState.Running)
                {
                    Pause();
                    CommandLine.ResumeEvent.Set();
                }

                e.Cancel = true;
            };

            FindRuntime();
        }

        private static void FindRuntime()
        {
            var rtPaths = Configuration.RuntimePathPrefixes;
            string fullPath = null;

            if (rtPaths != null)
            {
                foreach (var prefix in rtPaths)
                {
                    if (string.IsNullOrWhiteSpace(prefix))
                        continue;

                    fullPath = Path.Combine(prefix, "bin");

                    if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                        fullPath = Path.Combine(fullPath, "mono");
                    else
                        fullPath = Path.Combine(fullPath, "mono.exe");

                    if (File.Exists(fullPath))
                    {
                        _runtimePath = prefix;
                        break;
                    }
                }

                if (_runtimePath != null)
                    Logger.WriteInfoLine("Using runtime: {0}", fullPath);
                else
                    Logger.WriteErrorLine("No valid runtime found.");
            }
            else
            {
                Logger.WriteErrorLine("Failed to load configuration.");
                return;
            }
        }

        public static void InitializeSession()
        {
            Session = new SoftDebuggerCliSession
            {
                Breakpoints = new BreakpointStore(),
            };

            Session.ExceptionHandler = ex =>
            {
                Logger.WriteErrorLine("Internal Error: {0}: {1}", ex.GetType(), ex.Message);
                Logger.WriteErrorLine("{0}", ex.StackTrace);

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.WriteErrorLine("--> {0}: {1}", innerEx.GetType(), innerEx.Message);
                    Logger.WriteErrorLine("{0}", innerEx.StackTrace);

                    innerEx = innerEx.InnerException;
                }

                return true;
            };

            Session.LogWriter = (isStdErr, text) =>
            {
                var str = string.Format("[Mono] {0}", text);

                // The strings we get already have a line feed.
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

            Session.TargetHitBreakpoint += (sender, e) =>
            {
                _isPaused = true;
                SetBacktrace(e.Backtrace);

                var bp = (Breakpoint)e.BreakEvent;
                Logger.WriteEmphasisLine("Breakpoint hit: {0}:{1}", bp.FileName, bp.Line);

                if (CommandLine.Suspended)
                    CommandLine.ResumeEvent.Set();
            };

            Session.TargetInterrupted += (sender, e) =>
            {
                _isPaused = true;
                SetBacktrace(e.Backtrace);

                Logger.WriteEmphasisLine("Process interrupted.");

                if (CommandLine.Suspended)
                    CommandLine.ResumeEvent.Set();
            };

            Session.TargetExited += (sender, e) =>
            {
                Stop();

                Logger.WriteEmphasisLine("Process exited.");

                if (CommandLine.Suspended)
                    CommandLine.ResumeEvent.Set();
            };

            Session.TargetEvent += (sender, e) =>
            {
                Logger.WriteDebugLine("Event: {0}", e.Type);
            };
        }

        private static void ExceptionHandler(object sender, TargetEventArgs e, bool firstChance)
        {
            SetBacktrace(e.Backtrace);

            _isPaused = true;
            _isExcepted = !firstChance;

            ExceptionPrinter.Print(Backtrace.CurrentStackFrame.GetException());

            if (CommandLine.Suspended)
                CommandLine.ResumeEvent.Set();
        }

        public static void SetBacktrace(Backtrace bt)
        {
            var list = new List<StackFrame>();

            for (var i = 0; i < bt.FrameCount; i++)
                list.Add(bt.GetFrame(i - 1));

            Backtrace = new BacktraceInfo(list);
            Backtrace.SetActiveFrame(0);
        }

        public static bool Start(string path, string args)
        {
            if (Session == null)
                InitializeSession();

            if (_runtimePath == null)
            {
                Logger.WriteErrorLine("No valid runtime found.");
                return false;
            }

            Session.Run(new SoftDebuggerStartInfo(_runtimePath, new Dictionary<string, string>())
            {
                Arguments = args,
                Command = path,
                WorkingDirectory = WorkingDirectory,
            }, new DebuggerSessionOptions
            {
                EvaluationOptions = EvaluationOptions.DefaultOptions,
            });
            CurrentExecutable = new FileInfo(path);

            return true;
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
            if (!_isDoomed && State.IsStarted())
                Session.Exit();

            //Session.Dispose();

            Backtrace = null;
            _isPaused = false;
            _isExcepted = false;
            _isDoomed = false;

            Session = null;
            CurrentExecutable = null;
        }
    }
}
