using System;

namespace Mono.Debugger.Cli.Addins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CommandAddinAttribute : Attribute
    {
        public CommandAddinAttribute(string sdbName, string gdbName)
        {
            SdbName = sdbName;
            GdbName = gdbName;
        }

        public string SdbName { get; private set; }

        public string GdbName { get; private set; }
    }
}
