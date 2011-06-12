using System;
using System.Linq;
using System.Reflection;
using Mono.Debugger.Cli;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Addins
{
    internal static class AddinManager
    {
        public static void Scan()
        {
            var addinPaths = Configuration.AddinAssemblyPaths;

            if (addinPaths != null)
            {
                foreach (var path in addinPaths)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    try
                    {
                        var asm = Assembly.LoadFile(path);

                        LoadCommands(asm);

                        Logger.WriteInfoLine("Loaded addin: {0}", asm.GetName().Name);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Failed to load addin: {0}", ex.Message);
                    }
                }
            }
            else
            {
                Logger.WriteErrorLine("Failed to load configuration.");
                return;
            }
        }

        private static void LoadCommands(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(ICommand)))
                {
                    var ctor = type.GetConstructor(Type.EmptyTypes);

                    if (ctor == null)
                    {
                        Logger.WriteErrorLine("Could not load {0}: No parameterless constructor.", type.Name);
                        continue;
                    }

                    ICommand cmd = null;

                    try
                    {
                        cmd = (ICommand)ctor.Invoke(null);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Could not load {0}: {1}", ex.Message);
                        Logger.WriteErrorLine("{0}", ex.StackTrace);
                        continue;
                    }

                    if (CommandLine.Commands.Any(x => x.Name == cmd.Name))
                    {
                        Logger.WriteErrorLine("Could not load {0}: Duplicate command name.", type.Name);
                        continue;
                    }

                    CommandLine.Commands.Add(cmd);
                }
            }
        }
    }
}
