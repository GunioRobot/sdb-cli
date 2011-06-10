using System.Collections.Generic;
using System.Linq;

namespace Mono.Debugger.Cli
{
    internal static class Argument
    {
        public static string Required(string name)
        {
            return "<" + name + ">";
        }

        public static string Optional(string name)
        {
            return "[" + name + "]";
        }

        public static string Optional(string name1, string name2)
        {
            return "[" + name1 + " " + name2 + "]";
        }

        public static IEnumerable<string> None()
        {
            return Enumerable.Empty<string>();
        }
    }
}
