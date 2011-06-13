using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.Ast.Transforms;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using Mono.Debugger.Cli.Debugging;
using Mono.Debugger.Cli.Decompilation;
using Mono.Debugger.Cli.Logging;

namespace Mono.Debugger.Cli.Commands
{
    internal sealed class DecompileCommand : ICommand
    {
        public string Name
        {
            get { return "Decompile"; }
        }

        public string Description
        {
            get { return "Decompiles the method in the current stack frame."; }
        }

        public string Arguments
        {
            get { return string.Empty; }
        }

        public void Execute(CommandArguments args)
        {
            var backtrace = SoftDebugger.Backtrace;

            if (backtrace == null)
            {
                Logger.WriteErrorLine("No backtrace available.");
                return;
            }

            var frame = backtrace.CurrentStackFrame;

            if (frame == null)
            {
                Logger.WriteErrorLine("No stack frame available.");
                return;
            }

            var moduleName = frame.FullModuleName;
            ModuleDefinition module;

            try
            {
                module = ModuleDefinition.ReadModule(moduleName);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Could not load module {0}: {1}", moduleName, ex.Message);
                return;
            }

            var fullTypeName = frame.FullTypeName;
            var genericIdx = fullTypeName.IndexOf('[');

            if (genericIdx != -1)
                fullTypeName = fullTypeName.Substring(0, genericIdx);

            var type = module.Types.SingleOrDefault(x => x.FullName == fullTypeName);

            if (type == null)
            {
                Logger.WriteErrorLine("Could not locate type: {0}", fullTypeName);
                return;
            }

            var frameMethodName = frame.AddressSpace;
            var method = type.Methods.SingleOrDefault(x =>
            {
                var methodName = new StringBuilder();

                methodName.Append(x.ReturnType.Name);
                methodName.Append(" ");
                methodName.Append(type.Namespace != string.Empty ? type.Namespace + "." : string.Empty);
                methodName.Append(type.Name);
                methodName.Append(":");
                methodName.Append(x.Name);
                methodName.Append(" ");
                methodName.Append("(");

                for (var i = 0; i < x.Parameters.Count; i++)
                {
                    methodName.Append(x.Parameters[i].ParameterType.Name);

                    if (i != x.Parameters.Count - 1)
                        methodName.Append(", ");
                }

                methodName.Append(")");

                return methodName.ToString() == frameMethodName;
            });

            if (method == null)
            {
                Logger.WriteErrorLine("Could not locate method: {0}", frameMethodName);
                return;
            }

            BlockStatement ast = null;

            try
            {
                var context = new DecompilerContext(module)
                {
                    CurrentType = type,
                    CancellationToken = CancellationToken.None,
                };

                ast = AstMethodBodyBuilder.CreateMethodBody(method, context);

                new AddCheckedBlocks().Run(ast);
                new DeclareVariables(context).Run(ast);
                ((IAstTransform)new ReplaceMethodCallsWithOperators()).Run(ast);
                ((IAstTransform)new DelegateConstruction(context)).Run(ast);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Error when decompiling: {0}", ex.Message);
                return;
            }

            Logger.WriteInfoLine(method.FullName);
            new OutputVisitor(new DecompilerFormatter(frame), new CSharpFormattingOptions()).VisitBlockStatement(ast, null);
        }
    }
}
