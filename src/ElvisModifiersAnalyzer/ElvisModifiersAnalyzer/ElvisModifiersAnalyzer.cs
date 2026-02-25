// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

//#define LAUNCH_DEBUGGER
//#define INJECT_ATTR

//extern alias EML;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

//using static ElvisAttributes;
using static DebugHelper;


namespace ElvisModifiersAnalyzer;

#if INJECT_ATTR    
    [Generator]
#endif
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ElvisModifiersAnalyzer : DiagnosticAnalyzer
#if INJECT_ATTR    
    , IIncrementalGenerator
#endif
{
    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    //private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    //private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    //private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    //private const string Category = "Naming";

#if INJECT_ATTR    
    #region IIncrementalGenerator implementation

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //InjectAttrCodeHelper.AutoDetectCsVersionAndInject(context);
        InjectAttrCodeHelper.UseParamForInject(context);
        InjectAttrCodeHelper.JustInject(context, LanguageVersion.CSharp11);
    }

    #endregion IIncrementalGenerator implementation
#endif


    #region DiagnosticAnalyzer implementation

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create([methodRule, propertyRule, setPropertyRule, typeRule, setTypeRule]);

    public override void Initialize(AnalysisContext context)
    {
#if LAUNCH_DEBUGGER
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

        //beep(500, 200);

        context.RegisterSyntaxNodeAction(analyzeInvocation, SyntaxKind.InvocationExpression);

        context.RegisterSyntaxNodeAction(analyzeSimpleMemberAccess, SyntaxKind.SimpleMemberAccessExpression);

        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.SimpleAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.AddAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.SubtractAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.MultiplyAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.DivideAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.ModuloAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.AndAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.ExclusiveOrAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.OrAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.LeftShiftAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.RightShiftAssignmentExpression);
        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.CoalesceAssignmentExpression);

        //context.RegisterSyntaxNodeAction(analyzePrefixUnary, SyntaxKind.PreIncrementExpression);
        //context.RegisterSyntaxNodeAction(analyzePrefixUnary, SyntaxKind.PreDecrementExpression);

        //context.RegisterSyntaxNodeAction(analyzePostfixUnary, SyntaxKind.PostIncrementExpression);
        //context.RegisterSyntaxNodeAction(analyzePostfixUnary, SyntaxKind.PostDecrementExpression);


        //context.RegisterSyntaxNodeAction(analyzePropertyAssignment, SyntaxKind.FieldExpression);
        //context.RegisterSyntaxNodeAction(analyzeInvocation, SyntaxKind.IndexExpression);
    }

    #endregion DiagnosticAnalyzer implementation

    public const string EA = nameof(EA);

    #region methodRule

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1032 // Define diagnostic message correctly
    public const string EA_METH = $"{EA}_METH";
    static readonly DiagnosticDescriptor methodRule = new DiagnosticDescriptor(
        id: $"{EA_METH}_001",
        title: "Method should be a friend",
        messageFormat: "The '{0}' is not a friend for the '{1}' member.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The method should be a friend.");
#pragma warning restore RS2008 // Enable analyzer release tracking
#pragma warning restore RS1032 // Define diagnostic message correctly

    static void analyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.ToString().StartsWith("base."))
        {
            return;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol
            ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();       
        analyzeOperation(context, methodRule, methodSymbol, invocation);
    }

    #endregion methodRule

    #region propertyRule

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1032 // Define diagnostic message correctly
    public const string EA_PROP = $"{EA}_PROP";
    static readonly DiagnosticDescriptor propertyRule = new DiagnosticDescriptor(
        id: $"{EA_PROP}_001",
        title: "Property should be a friend",
        messageFormat: "The '{0}' is not a friend for the '{1}' member.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The property should be a friend.");

    static readonly DiagnosticDescriptor setPropertyRule = new DiagnosticDescriptor(
        id: $"{EA_PROP}_002",
        title: "Property should be a friend or set-friend",
        messageFormat: "The '{0}' is not a friend or set-friend for the '{1}' member.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The property should be a friend or set-friend.");
#pragma warning restore RS2008 // Enable analyzer release tracking
#pragma warning restore RS1032 // Define diagnostic message correctly

    //SimpleMemberAccess
    static void analyzeSimpleMemberAccess(SyntaxNodeAnalysisContext context)
    {
        var access = (MemberAccessExpressionSyntax) context.Node;
        var parent = access?.Parent;
        SyntaxNode? topParenthesNode = access;
        while (parent is ParenthesizedExpressionSyntax)
        {
            topParenthesNode = parent;
            parent = parent.Parent;
        }

        switch (parent)
        {
            case AssignmentExpressionSyntax assignment:
                if (assignment.Left == topParenthesNode)
                {
                    if (analyzePropertyAccess(setPropertyRule, context, access, true))
                    {
                        return;
                    }
                }
                break;

            case PrefixUnaryExpressionSyntax:
            case PostfixUnaryExpressionSyntax:
                if (analyzePropertyAccess(setPropertyRule, context, access, true))
                {
                    return;
                }
                break;
        }

        analyzePropertyAccess(propertyRule, context, access);
    }

    //static void analyzePropertyAssignment(SyntaxNodeAnalysisContext context)
    //{
    //    var assignment = context.Node as AssignmentExpressionSyntax;
    //    analyzePropertyAccess(propertyRule, context, assignment?.Left as MemberAccessExpressionSyntax);
    //}

    //static void analyzePrefixUnary(SyntaxNodeAnalysisContext context)
    //{
    //    var prefixUnary = (PrefixUnaryExpressionSyntax)context.Node;
    //    analyzePropertyAccess(propertyRule, context, prefixUnary.Operand as MemberAccessExpressionSyntax);
    //}

    //static void analyzePostfixUnary(SyntaxNodeAnalysisContext context)
    //{
    //    var postfixUnary = (PostfixUnaryExpressionSyntax)context.Node;
    //    analyzePropertyAccess(propertyRule, context, postfixUnary.Operand as MemberAccessExpressionSyntax);
    //}

    static bool analyzePropertyAccess(
        in DiagnosticDescriptor rule,
        in SyntaxNodeAnalysisContext context,
        in MemberAccessExpressionSyntax? memberAccess,
        in bool isSet = false)
    {
        if (memberAccess is null)
        {
            return false;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess);
        var propertySymbol = symbolInfo.Symbol as IPropertySymbol
            ?? symbolInfo.CandidateSymbols.OfType<IPropertySymbol>().FirstOrDefault();
        return analyzeOperation(context, rule, propertySymbol, memberAccess, isSet);
    }

    #endregion propertyRule

    #region typeRule

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1032 // Define diagnostic message correctly
    public const string EA_TYPE = $"{EA}_TYPE";
    static readonly DiagnosticDescriptor typeRule = new DiagnosticDescriptor(
        id: $"{EA_TYPE}_001",
        title: "Type should be a friend",
        messageFormat: "The '{0}' is not a friend for the '{1}' type.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Type should be a friend.");

    static readonly DiagnosticDescriptor setTypeRule = new DiagnosticDescriptor(
        id: $"{EA_TYPE}_002",
        title: "Type should be a friend or set-friend",
        messageFormat: "The '{0}' is not a friend or set-friend for the '{1}' type.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Type should be a friend or set-friend.");

#pragma warning restore RS2008 // Enable analyzer release tracking
#pragma warning restore RS1032 // Define diagnostic message correctly

    #endregion typeRule

    #region attributeRule

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1032 // Define diagnostic message correctly
    static readonly DiagnosticDescriptor attributeRule = new DiagnosticDescriptor(
        id: "EA_ATTR_001",
        title: "Attributes conflict",
        messageFormat: $"Not allowed to apply '{ElvisAttributes.OnlyYouAttribute}' and '{ElvisAttributes.OnlyAliasAttribute}' attributes at the same time.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Not allowed to apply '{ElvisAttributes.OnlyYouAttribute}' and '{ElvisAttributes.OnlyAliasAttribute}' attributes at the same time.");
#pragma warning restore RS2008 // Enable analyzer release tracking
#pragma warning restore RS1032 // Define diagnostic message correctly

    #endregion attributeRule

    #region helpers

    static bool analyzeOperation(
        SyntaxNodeAnalysisContext context,
        in DiagnosticDescriptor rule,
        in ISymbol? symbol, // 'symbol' (some member of potential attractor) correspond to the 'operationNode'
        in SyntaxNode operationNode,
        in bool isSet = false)
    {
        static OuterInfo? getOuterInfo(
                in SyntaxNodeAnalysisContext context,
                in ISymbol symbol,
                in SyntaxNode operationNode)
        {
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

            INamedTypeSymbol? outerClassSymbol = context.SemanticModel.GetDeclaredSymbol(outerClass);
            // allow a call inside the same class:
            if (ReferenceEquals(symbol.ContainingType, outerClassSymbol))
            {
                return null;
            }

            return new OuterInfo(outerMethod, outerClass, outerClassSymbol!);
        }

        static bool analyzeOperation(
            in SyntaxNodeAnalysisContext context,
            in DiagnosticDescriptor rule,
            in OuterInfo outerInfo,
            in ISymbol symbol,
            in SyntaxNode operationNode,
            IEnumerable<AttributeData> oAttrs,
            in bool isSet,
            params object?[]? messageArgs)
        {
            (MethodDeclarationSyntax outerMethod,
             ClassDeclarationSyntax outerClass,
             INamedTypeSymbol outerClassSymbol) = outerInfo;

            bool isMethod = symbol is IMethodSymbol;
            bool hasOnlySetAttr = oAttrs.Any(static a =>
                a.AttributeClass?.Name is ElvisAttributes.OnlyYouSetAttribute or ElvisAttributes.OnlyAliasSetAttribute);
            if (!isMethod && !isSet && hasOnlySetAttr)
            {
                return false;
            }

            IEnumerable<(AttributeData AttrData, bool IsGeneric, INamedTypeSymbol AttrSymbol)>
            oAttrsInfos = oAttrs
                .Where(a => {
                    // select only that O<T> attributes where T == outerClass
                    var attributeClass = a.AttributeClass!;
                    return attributeClass.IsGenericType
                        ? SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), outerClassSymbol)
                        : ReferenceEquals(a.ConstructorArguments[0].Value, outerClassSymbol);
                })
                .Select(a => (a, a.AttributeClass!.IsGenericType, a.AttributeClass!));

            if (oAttrsInfos.Count() == 0) // outerClass is not "Only" type
            {
                reportDiagnostic(rule, context, operationNode, messageArgs);
                return true;
            }

            oAttrsInfos = oAttrsInfos.Where(static a => a.IsGeneric
                            ? a.AttrData.ConstructorArguments[0].Values.Count() > 0
                            : a.AttrData.ConstructorArguments[1].Values.Count() > 0);
            if (oAttrsInfos.Count() == 0)
            {
                // => all members of the outerClass is friends (i.e. allowed)
                return false;
            }

            bool isAllowByOU = true; // is allow (by ouAttrsInfos) to invoke our member (symbol)?
            string outerMethodName = outerMethod.Identifier.Text;

            // select [OnlyYou(Set)] attributes (OU)
            var ouAttrsInfos = isMethod || !isSet
                ? oAttrsInfos.Where(static a => a.AttrSymbol?.Name == ElvisAttributes.OnlyYouAttribute)
                : oAttrsInfos.Where(static a => a.AttrSymbol?.Name is ElvisAttributes.OnlyYouAttribute or ElvisAttributes.OnlyYouSetAttribute);
            if (ouAttrsInfos.SelectMany(static a => a.IsGeneric
                        ? a.AttrData.ConstructorArguments[0].Values
                        : a.AttrData.ConstructorArguments[1].Values)
                    .All(v => v.Value?.ToString() != outerMethodName))
            {
                isAllowByOU = false;
            }

            // [OnlyAlias(Set)] case (OA)
            bool isAllowByOA = true; // is allow to invoke our member (symbol) in alias case?
            do
            {
                // select [OnlyAlias(Set)] attributes (OA)
                var oaAttrInfos = isMethod || !isSet
                    ? oAttrsInfos.Where(static a => a.AttrSymbol?.Name == ElvisAttributes.OnlyAliasAttribute)
                    : oAttrsInfos.Where(static a => a.AttrSymbol?.Name is ElvisAttributes.OnlyAliasAttribute or ElvisAttributes.OnlyAliasSetAttribute);
                if (oaAttrInfos.Count() == 0)
                {
                    isAllowByOA = false;
                    break;
                }

                // [OnlyAlias(Set)(type, aliases)]
                var aliases = oaAttrInfos
                    .SelectMany(a => a.IsGeneric
                        ? a.AttrData.ConstructorArguments[0].Values
                        : a.AttrData.ConstructorArguments[1].Values)
                    .Select(a => a.Value?.ToString())
                    .Where(a => a is not null);
                if (outerMethod.AttributeLists.Count == 0)
                {
                    isAllowByOA = false;
                    break;
                }

                // [Alias(methodAlias)]
                // get [Alias] attribute for outerMethod
                var aliasAttr = context.SemanticModel
                    .GetDeclaredSymbol(outerMethod)?
                    .GetAttributes()
                    .Where(static a => a.AttributeClass?.Name is ElvisAttributes.AliasAttribute)
                    .FirstOrDefault();
                if (aliasAttr is null)
                {
                    isAllowByOA = false;
                    break;
                }

                string methodAlias = aliasAttr.ConstructorArguments.First().Value!.ToString();
                if (!aliases.Contains(methodAlias))
                {
                    isAllowByOA = false;
                    break;
                }
            } while (false);

            if (!isAllowByOU && !isAllowByOA)
            {
                reportDiagnostic(rule, context, operationNode, messageArgs);
                return true;
            }

            return false;
        }

        /*-----------------------------------------------------------*/

        //beep();

        if (symbol is null)
        {
            return false;
        }

        var outerInfo = getOuterInfo(context, symbol, operationNode);
        if (outerInfo is null)
        {
            return false;
        }

        // scan base types: (instead of StartsWith("base.") and etc.)
        //foreach (INamedTypeSymbol t in context.SemanticModel.GetDeclaredSymbol(outerClass).GetAllBaseTypes())
        //{
        //}

        // get o-attributes on class (potential attractor)
        var oAttrsOnClass = symbol.ContainingType.GetAttributes()
            .Where(static a => a.AttributeClass?.Name is ElvisAttributes.OnlyYouAttribute
                                                      or ElvisAttributes.OnlyAliasAttribute
                                                      or ElvisAttributes.OnlyYouSetAttribute
                                                      or ElvisAttributes.OnlyAliasSetAttribute)
            .ToList();
        if (oAttrsOnClass.Count() > 0) // => realy attractor
        {
            /*
            [OnlyYou<OuterClass>(nameof(OuterClass.OuterMethod1))] // oAttrsOnClass
            class Attractor {                
                void Operation1(); // Member
                void Operation2(); // Member
            }

            class OuterClass {
                void OuterMethod1(in Me me) => me.Operation[1/2](); // operationNode // ok
                void OuterMethod2(in Me me) => me.Operation[1/2](); // operationNode // err
            }
             */
            bool isMethod = symbol is IMethodSymbol;
            bool hasOnlySetAttr = oAttrsOnClass.Any(static a =>
                a.AttributeClass?.Name is ElvisAttributes.OnlyYouSetAttribute or ElvisAttributes.OnlyAliasSetAttribute);
            if (analyzeOperation(
                context,
                !isMethod && hasOnlySetAttr ? setTypeRule : typeRule,
                outerInfo,
                symbol,
                operationNode,
                oAttrsOnClass,
                isSet,
                outerInfo.OuterMethod.Identifier.Text, symbol.ContainingType.Name))
            {
                return true;
            }
        }

        /*
        class Attractor {
            [OnlyYou<OuterClass>(nameof(OuterClass.OuterMethod))] // oAttrsOnMember
            void Operation(); // Member
        }
         
        class OuterClass {
            void OuterMethod(in Me me) => me.Operation(); // operationNode // ok
        }
         */
        // get o-attributes on member (member of potential attractor)
        var oAttrsOnMember = symbol.GetAttributes()
            .Where(static a => a.AttributeClass?.Name is ElvisAttributes.OnlyYouAttribute
                                                      or ElvisAttributes.OnlyAliasAttribute
                                                      or ElvisAttributes.OnlyYouSetAttribute
                                                      or ElvisAttributes.OnlyAliasSetAttribute)
            .ToList();

        if (oAttrsOnMember.Count() == 0) // => member doesn't belong to attractor class
        {
            return false;
        }

        return analyzeOperation(
            context,
            rule,
            outerInfo,
            symbol,
            operationNode,
            oAttrsOnMember,
            isSet,
            symbol.Name, outerInfo.OuterMethod.Identifier.Text);
    }

    static void reportDiagnostic(
        in DiagnosticDescriptor rule,
        in SyntaxNodeAnalysisContext context,
        in SyntaxNode node,
        params object?[]? messageArgs)
    {
        var diagnostic = Diagnostic.Create(
            rule,
            Location.Create(node.SyntaxTree, node.Span),
            messageArgs
        );
        context.ReportDiagnostic(diagnostic);
    }

    #endregion helpers
}

/// <summary>
/// Let there is some operation (method invocation, get/set property...).<br/>
/// The OuterInfo contains "outer" information about this operation: <br/>
/// - in which method (outerMethod) occurs this operation <br/>
/// - and in which class (outerClass, outerClassSymbol) contains this outerMethod
/// </summary>
file class OuterInfo
{
    public OuterInfo(
        in MethodDeclarationSyntax outerMethod,
        in ClassDeclarationSyntax outerClass,
        in INamedTypeSymbol outerClassSymbol)
    {
        OuterMethod = outerMethod;
        OuterClass = outerClass;
        OuterClassSymbol = outerClassSymbol;
    }

    public void Deconstruct(
        out MethodDeclarationSyntax outerMethod,
        out ClassDeclarationSyntax outerClass,
        out INamedTypeSymbol outerClassSymbol)
    {
        outerMethod = OuterMethod;
        outerClass = OuterClass;
        outerClassSymbol = OuterClassSymbol;
    }

    public MethodDeclarationSyntax OuterMethod { get; }
    public ClassDeclarationSyntax OuterClass { get; }
    public INamedTypeSymbol OuterClassSymbol { get; }    
}
