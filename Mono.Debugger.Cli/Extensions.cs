using Mono.Debugging.Client;

namespace Mono.Debugger.Cli
{
    public static class Extensions
    {
        public static bool HasSource(this SourceLocation loc)
        {
            return loc.FileName != string.Empty && loc.Line != -1;
        }
    }
}
