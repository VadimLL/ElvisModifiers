// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

//#define LAUNCH_DEBUGGER
//#define INJECT_ATTR
//#define STRICT_OU // !!!

//extern alias EML;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using E = ElvisAttributes;
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

        context.RegisterSyntaxNodeAction(analyzeIndexer, SyntaxKind.ElementAccessExpression);

        context.RegisterSyntaxNodeAction(analyzeSimpleMemberAccess, SyntaxKind.SimpleMemberAccessExpression);

        context.RegisterSyntaxNodeAction(analyzeObjectCreation,
            SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression);
    }

    #endregion DiagnosticAnalyzer implementation

    public const string EA = nameof(EA);

    #region ctorRule

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1032 // Define diagnostic message correctly
    public const string EA_CTOR = $"{EA}_CTOR";
    static readonly DiagnosticDescriptor ctorRule = new DiagnosticDescriptor(
        id: $"{EA_CTOR}_001",
        title: "Constructor should be a friend",
        messageFormat: "The '{0}' is not a friend for the '{1}' member.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The constructor should be a friend.");
#pragma warning restore RS2008 // Enable analyzer release tracking
#pragma warning restore RS1032 // Define diagnostic message correctly

    static void analyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (BaseObjectCreationExpressionSyntax)context.Node;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(objectCreation);
        var ctorSymbol = symbolInfo.Symbol as IMethodSymbol
            ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        analyzeOperation(context, methodRule, ctorSymbol, objectCreation);
    }

    #endregion ctorRule

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
        //if (invocation.ToString().StartsWith("base."))
        //{
        //    return;
        //}

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol
            ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();       
        analyzeOperation(context, methodRule, methodSymbol, invocation);
    }

    static void analyzeIndexer(SyntaxNodeAnalysisContext context)
    {
        var indexer = (ElementAccessExpressionSyntax)context.Node;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(indexer);
        //var methodSymbol = symbolInfo.Symbol as IMethodSymbol
        //    ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        analyzeOperation(context, methodRule, symbolInfo.Symbol, indexer);
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
        if (symbolInfo.Symbol is IPropertySymbol or IFieldSymbol or IEventSymbol)
        {
            if (symbolInfo.Symbol is IFieldSymbol fieldSymbol && fieldSymbol.IsConst)
            //!!! in the future better use Exclude attribute (not implemented yet) for constant
            {
                return false;
            }

            return analyzeOperation(context, rule, symbolInfo.Symbol, memberAccess, isSet);
        }

        return false;
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
        messageFormat: $"Not allowed to apply '{E.OnlyYouAttribute}' and '{E.OnlyAliasAttribute}' attributes at the same time.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Not allowed to apply '{E.OnlyYouAttribute}' and '{E.OnlyAliasAttribute}' attributes at the same time.");
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
            (MemberDeclarationSyntax outerMember,
             ClassDeclarationSyntax outerClass,
             INamedTypeSymbol outerClassSymbol) = outerInfo;

            bool isMethod = symbol is IMethodSymbol;
            bool hasOnlySetAttr = oAttrs.Any(static a =>
                a.AttributeClass?.Name is E.OnlyYouSetAttribute or E.OnlyAliasSetAttribute);
            if (!isMethod && !isSet && hasOnlySetAttr)
            {
                // All can read [OnlySet*] property
                return false;
            }

            IEnumerable<AttrInfo> oAttrsInfos = oAttrs
                .Select(a => AttrInfo.Create(a, outerClassSymbol))
                .Where(ai => ai != null)
                .Select(ai => ai!)
                .ToList();

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
            string outerMemberName = outerMember.GetName();
            // select [OnlyYou(Set)] attributes (OU)
            var ouAttrsInfos = isMethod || !isSet
                ? oAttrsInfos.Where(static a => a.AttrSymbol?.Name == E.OnlyYouAttribute)
                : oAttrsInfos.Where(static a => a.AttrSymbol?.Name is E.OnlyYouAttribute or E.OnlyYouSetAttribute);
            if (!ouAttrsInfos.SelectMany(static a => a.IsGeneric
                        ? a.AttrData.ConstructorArguments[0].Values
                        : a.AttrData.ConstructorArguments[1].Values)
                    .Any(v => {
                        string memberName = v.Value!.ToString();
                        if (memberName.StartsWith(Const.RegExPrefix))
                        {
                            return Regex.IsMatch(outerMemberName, memberName.Substring(Const.RegExPrefixLen));
                        }

                        return memberName == outerMemberName; 
                    }))
            {
                isAllowByOU = false;
            }

            // [OnlyAlias(Set)] case (OA)
            bool isAllowByOA = true; // is allow to invoke our member (symbol) in alias case?
            do
            {
                // select [OnlyAlias(Set)] attributes (OA)
                var oaAttrInfos = isMethod || !isSet
                    ? oAttrsInfos.Where(static a => a.AttrSymbol?.Name == E.OnlyAliasAttribute)
                    : oAttrsInfos.Where(static a => a.AttrSymbol?.Name is E.OnlyAliasAttribute or E.OnlyAliasSetAttribute);
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

                var outerInterfaceMethod = context.SemanticModel.GetDeclaredSymbol(outerMember)?.FindInterfaceMember();
                if (outerMember.AttributeLists.Count == 0 && outerInterfaceMethod is null)
                {
                    isAllowByOA = false;
                    break;
                }

                // [Alias(methodAlias)]
                // get [Alias] attribute for outerMember
                var aliasAttr = context.SemanticModel
                    .GetDeclaredSymbol(outerMember)?
                    .GetAttributes()
                    .Where(static a => a.AttributeClass?.Name is E.AliasAttribute)
                    .FirstOrDefault();
                if (aliasAttr is null)
                {
                    aliasAttr = outerInterfaceMethod?.GetAttributes()
                        .Where(static a => a.AttributeClass?.Name is E.AliasAttribute)
                        .FirstOrDefault();
                    if (aliasAttr is null)
                    {
                        isAllowByOA = false;
                        break;
                    }
                }

                string methodAlias = aliasAttr.ConstructorArguments.First().Value!.ToString();
                if (!aliases.Any(a => a!.StartsWith(Const.RegExPrefix)
                        ? Regex.IsMatch(methodAlias, a.Substring(Const.RegExPrefixLen).Trim())
                        : a == methodAlias))
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

        var outerInfo = OuterInfo.Get(context, symbol, operationNode);
        if (outerInfo is null)
        {
            return false;
        }

        // get o-attributes on class (potential attractor)
        var oAttrsOnClass = symbol.ContainingType.GetAttributes()
            .Where(static a => a.AttributeClass?.Name is E.OnlyYouAttribute
                                                      or E.OnlyAliasAttribute
                                                      or E.OnlyYouSetAttribute
                                                      or E.OnlyAliasSetAttribute)
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
                a.AttributeClass?.Name is E.OnlyYouSetAttribute or E.OnlyAliasSetAttribute);
            if (analyzeOperation(
                context,
                !isMethod && hasOnlySetAttr ? setTypeRule : typeRule,
                outerInfo,
                symbol,
                operationNode,
                oAttrsOnClass,
                isSet,
                outerInfo.OuterMember.GetName(), symbol.ContainingType.Name))
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
            .Where(static a => a.AttributeClass?.Name is E.OnlyYouAttribute
                                                      or E.OnlyAliasAttribute
                                                      or E.OnlyYouSetAttribute
                                                      or E.OnlyAliasSetAttribute)
            .ToList();

        var interfaceMember = symbol.FindInterfaceMember();
        if (interfaceMember is not null)
        {
            // get o-attributes on interface member (member of potential attractor-interface)
            var oAttrsOnInterfaceMember = interfaceMember.GetAttributes()
                .Where(static a => a.AttributeClass?.Name is E.OnlyYouAttribute
                                                          or E.OnlyAliasAttribute
                                                          or E.OnlyYouSetAttribute
                                                          or E.OnlyAliasSetAttribute);
            oAttrsOnMember.AddRange(oAttrsOnInterfaceMember);
        }

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
            symbol.Name, outerInfo.OuterMember.GetName());
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
/// - in which member (outerMember) occurs this operation <br/>
/// - and in which class (outerClass, outerClassSymbol) contains this outerMember
/// </summary>
file class OuterInfo
{
    public OuterInfo(
        in MemberDeclarationSyntax outerMember,
        in ClassDeclarationSyntax outerClass,
        in INamedTypeSymbol outerClassSymbol)
    {
        OuterMember = outerMember;
        OuterClass = outerClass;
        OuterClassSymbol = outerClassSymbol;
    }

    public void Deconstruct(
        out MemberDeclarationSyntax outerMember,
        out ClassDeclarationSyntax outerClass,
        out INamedTypeSymbol outerClassSymbol)
    {
        outerMember = OuterMember;
        outerClass = OuterClass;
        outerClassSymbol = OuterClassSymbol;
    }

    public MemberDeclarationSyntax OuterMember { get; }
    public ClassDeclarationSyntax OuterClass { get; }
    public INamedTypeSymbol OuterClassSymbol { get; }
    public static OuterInfo? Get(
            in SyntaxNodeAnalysisContext context,
            in ISymbol symbol,
            in SyntaxNode operationNode)
    {
        if (operationNode.Parent.SearchInParent(static n => n is MemberDeclarationSyntax)
            is not MemberDeclarationSyntax outerMember)
        {
            return null;
        }

        if (outerMember.Parent.SearchInParent(static n => n is ClassDeclarationSyntax)
            is not ClassDeclarationSyntax outerClass)
        {
            return null;
        }

        INamedTypeSymbol? outerClassSymbol = context.SemanticModel.GetDeclaredSymbol(outerClass);
        if (symbol.ContainingType.GenericCaseEqualTo(outerClassSymbol))
        {
            return null;
        }

#if !STRICT_OU
        // allow a call inside the dirived classes
        if (outerClassSymbol?.InheritsFrom(symbol.ContainingType) ?? true)
        {
            return null;
        }
#endif
        return new OuterInfo(outerMember, outerClass, outerClassSymbol!);
    }
}

file class AttrInfo
{
    static readonly SymbolDisplayFormat symbolDisplayFormat = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    private AttrInfo(
        AttributeData attrData,
        bool isGeneric,
        INamedTypeSymbol attrSymbol)
    {
        AttrData = attrData;
        IsGeneric = isGeneric;
        AttrSymbol = attrSymbol;
    }

    public static AttrInfo? Create(AttributeData attrData, INamedTypeSymbol outerTypeSymbol)
    {
        var outerTypeInterfaces = outerTypeSymbol.AllInterfaces.ToList();
        var outerType = outerTypeSymbol.IsGenericType
            ? outerTypeSymbol.OriginalDefinition
            : outerTypeSymbol;

        var attributeClass = attrData.AttributeClass!;
        if (attributeClass.IsGenericType)
        {
            if(SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), outerTypeSymbol)
                || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), i)))
            {
                return new AttrInfo(attrData, attrData.AttributeClass!.IsGenericType, attrData.AttributeClass!);
            }

            return null;
        }

        var friendType = attrData.ConstructorArguments[0].Value as INamedTypeSymbol;
        if (friendType is null)
        {
            string? friendTypeName = attrData.ConstructorArguments[0].Value as string;
            if (friendTypeName is not null)
            {
                string? outerTypeName = outerTypeSymbol.ToDisplayString(symbolDisplayFormat);
                if (friendTypeName.StartsWith(Const.RegExPrefix))
                {
                    friendTypeName = friendTypeName.Substring(Const.RegExPrefixLen).Trim();
                    var friendTypeRegEx = new Regex(friendTypeName, RegexOptions.Compiled);
                    if (friendTypeRegEx.IsMatch(outerTypeName)
                        || outerTypeInterfaces.Any(i => friendTypeRegEx.IsMatch(i.ToDisplayString(symbolDisplayFormat))))
                    {
                        return new AttrInfo(attrData, false, attrData.AttributeClass!);
                    }
                }

                if (friendTypeName == outerTypeName
                    || outerTypeInterfaces.Any(i => i.ToDisplayString(symbolDisplayFormat) == friendTypeName))
                {
                    return new AttrInfo(attrData, false, attrData.AttributeClass!);
                }
            }

            return null;
        }

        friendType = friendType.IsGenericType ? friendType.OriginalDefinition : friendType;
        if(SymbolEqualityComparer.Default.Equals(friendType, outerType)
            || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(friendType, i)))
        {
            return new AttrInfo(attrData, false, attrData.AttributeClass!);
        }

        return null;
    }

    public AttributeData AttrData { get; }
    public bool IsGeneric { get; }
    public INamedTypeSymbol AttrSymbol { get; }
}

file static class Const
{
    public const string RegExPrefix = "R:";
    public static readonly int RegExPrefixLen = RegExPrefix.Length;
}
