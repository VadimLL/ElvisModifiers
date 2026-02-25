//#define LAUNCH_DEBUGGER

using System.Text;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static DebugHelper;

namespace ElvisModifiersGenerator;

[Generator]
public class ElvisModifiersGenerator : IIncrementalGenerator
{
    public const string EG = nameof(EG);

#pragma warning disable RS2008, RS1032
    public const string EG_METH = $"{EG}_METH";
    static readonly DiagnosticDescriptor methodRule = new DiagnosticDescriptor(
        id: $"{EG_METH}_001",
        title: "Method should be a friend",
        messageFormat: "The '{0}' is not a friend for the '{1}' method.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The method should be a friend.");
#pragma warning restore RS2008, RS1032

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if LAUNCH_DEBUGGER
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif
        // Add the attribute to the compilation
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "ElvisModifiersGenerator_Attribute.g.cs",
            SourceText.From(ElvisAttributes.Code10, Encoding.UTF8)));

        var invocationProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is InvocationExpressionSyntax,
                transform: analyzeInvocation)
            .Where(static x => x is not null)!
            .Select(static (x, _) => x!);

        context.RegisterSourceOutput(invocationProvider, static (context, invocationInfo) =>
        {
            var diagnostic = Diagnostic.Create(
                methodRule,
                Location.Create(
                    invocationInfo.Node.SyntaxTree,
                    invocationInfo.Node.Span),
                invocationInfo.Symbol.Name,
                invocationInfo.OuterMemberName
            );
            context.ReportDiagnostic(diagnostic);
        });

        static OperationInfo? analyzeInvocation(
            GeneratorSyntaxContext context,
            CancellationToken ct)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation.ToString().StartsWith("base."))
            {
                return null;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol
                ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
            return analyzeOperation(context, methodRule, methodSymbol, invocation);
        }

        static OperationInfo? analyzeOperation(
            in GeneratorSyntaxContext context,
            in DiagnosticDescriptor rule,
            in ISymbol? symbol,
            in SyntaxNode operationNode)
        {
            //Beep(500, 100);

            if (symbol is null)
            {
                return null;
            }

            var attrs = symbol
                .GetAttributes()
                .Where(static a => a.AttributeClass?.Name == ElvisAttributes.OnlyYouAttribute);
            if (attrs.Count() == 0)
            {
                return null;
            }

            if (operationNode.Parent.SearchInParent(static n => n is MethodDeclarationSyntax)
                is not MethodDeclarationSyntax outerMethod)
            {
                return null;
            }

            if (outerMethod.Parent.SearchInParent(static n => n is ClassDeclarationSyntax)
                is not ClassDeclarationSyntax outerClass)
            {
                return null;
            }

            // scan base types: (instead of StartsWith("base.") and etc.)
            //foreach (INamedTypeSymbol t in context.SemanticModel.GetDeclaredSymbol(outerClass).GetAllBaseTypes())
            //{
            //}

            string outerClassName = outerClass.Identifier.Text;
            attrs = attrs
                .Where(a => (a.ConstructorArguments[0].Value as INamedTypeSymbol)?.Name == outerClassName);

            string outerMethodName = outerMethod.Identifier.Text;
            if (attrs.Count() == 0)
            {
                return new OperationInfo(operationNode, symbol, outerMethodName);
            }

            var friendMembersNames = attrs
                .SelectMany(static attr => attr.ConstructorArguments[1].Values
                    .Select(static v => v.Value?.ToString()));
            if (!friendMembersNames.Contains(outerMethodName))
            {
                return new OperationInfo(operationNode, symbol, outerMethodName);
            }

            return null;
        }
    }
}

file class OperationInfo
{
    public OperationInfo(
        SyntaxNode node,
        ISymbol symbol,
        string outerMemberName)
    {
        Node = node;
        Symbol = symbol;
        OuterMemberName = outerMemberName;
    }

    public SyntaxNode Node { get; }
    public ISymbol Symbol { get; }
    public string OuterMemberName { get; }
}
