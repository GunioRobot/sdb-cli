using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Debugging.Client;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Decompilation
{
    public sealed class DecompilerFormatter : IOutputFormatter
    {
        private bool _needsIndent = true;

        private bool _firstBrace = true;

        private int _indentation;

        private Action<string, string[]> _writer;

        private readonly StackFrame _frame;

        public string IndentationString { get; set; }

        private void Write(string value)
        {
            _writer("{0}", new[] { value });
        }

        public DecompilerFormatter(StackFrame frame)
        {
            _frame = frame;
            _writer = Logger.WriteInfoString;
            IndentationString = "    ";
        }

        public void WriteIdentifier(string ident)
        {
            WriteIndentation();

            Write(ident);
        }

        public void WriteKeyword(string keyword)
        {
            WriteIndentation();

            Write(keyword);
        }

        public void WriteToken(string token)
        {
            WriteIndentation();

            Write(token);
        }

        public void Space()
        {
            WriteIndentation();

            Write(" ");
        }

        public void OpenBrace(BraceStyle style)
        {
            if (!_firstBrace)
                NewLine();
            else
                _firstBrace = false;

            WriteIndentation();

            Write("{");

            Indent();
            NewLine();
        }

        public void CloseBrace(BraceStyle style)
        {
            Unindent();
            WriteIndentation();

            Write("}");
        }

        private void WriteIndentation()
        {
            if (_needsIndent)
            {
                Logger.WriteInfo(string.Empty);

                _needsIndent = false;

                for (var i = 0; i < _indentation; i++)
                    Write(IndentationString);
            }
        }

        public void NewLine()
        {
            Logger.WriteInfoStringLine(string.Empty);

            _needsIndent = true;
        }

        public void Indent()
        {
            _indentation++;
        }

        public void Unindent()
        {
            _indentation--;
        }

        public void WriteComment(CommentType commentType, string content)
        {
            throw new NotSupportedException();
        }

        public void StartNode(AstNode node)
        {
            // This entire method is a crazy heuristic that likely won't work
            // universally. We really need a better way to correlate IL offsets
            // with decompiled code.

            if (node is AstType)
                return;

            var isCurrentExpression = false;

            Func<ILRange, bool> f = range =>
            {
                var ofs = _frame.Address;
                return isCurrentExpression = ofs >= range.From && ofs <= range.To;
            };

            foreach (var range in node.Annotations.OfType<List<ILRange>>().SelectMany(x => x))
                if (f(range))
                    break;

            if (!isCurrentExpression)
                if (node.Parent != null)
                    foreach (var range in node.Parent.Annotations.OfType<List<ILRange>>().SelectMany(x => x))
                        if (f(range))
                            break;

            if (!isCurrentExpression)
                foreach (var child in node.Children)
                    foreach (var range in child.Annotations.OfType<List<ILRange>>().SelectMany(x => x))
                        if (f(range))
                            break;

            if (isCurrentExpression)
                _writer = Logger.WriteEmphasisString;
            else
                _writer = Logger.WriteInfoString;
        }

        public void EndNode(AstNode node)
        {
        }
    }
}
