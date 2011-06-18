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
                    var attrs = type.GetCustomAttributes(typeof(CommandAddinAttribute), false) as CommandAddinAttribute[];

                    if (attrs.Length == 0)
                        continue;

                    var attr = attrs[0];
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

                    string name = null;

                    switch (CommandLine.Dialect.Name)
                    {
                        case CommandDialect.Sdb:
                            name = attr.SdbName;
                            break;
                        case CommandDialect.Gdb:
                            name = attr.GdbName;
                            break;
                    }

                    var commands = CommandLine.Dialect.Commands;

                    if (commands.Keys.Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        Logger.WriteErrorLine("Could not load {0}: Duplicate command name.", type.Name);
                        continue;
                    }

                    commands.Add(name, cmd);
                }
            }
        }
    }
}
