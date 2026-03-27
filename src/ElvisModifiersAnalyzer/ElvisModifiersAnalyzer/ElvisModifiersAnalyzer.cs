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
                    analyzePropertyAccess(setPropertyRule, context, access, true);
                    return;
                }
                break;

            case PrefixUnaryExpressionSyntax:
            case PostfixUnaryExpressionSyntax:
                analyzePropertyAccess(setPropertyRule, context, access, true);
                return;
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
            //!!! maybe better if user use [Exclude] attribute for constant?
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
        in ISymbol? memberSymbol, // some member (of potential attractor) correspond to the 'memberNode'
        in SyntaxNode memberNode,
        in bool isSet = false)
    {
        static bool analyzeOperation(
            in SyntaxNodeAnalysisContext context,
            in DiagnosticDescriptor rule,
            in OuterInfo outerInfo,
            in ISymbol memberSymbol,
            in SyntaxNode memberNode,
            in IEnumerable<AttributeData> oAttrs,
            //in IEnumerable<AttributeData> oSetAttrsOnClass,
            in bool isMethod,
            bool isSet,
            params object?[]? messageArgs)
        {
            (MemberDeclarationSyntax outerMember,
             ClassDeclarationSyntax outerClass,
             INamedTypeSymbol outerClassSymbol) = outerInfo;
            
            var oSetAttrs = oAttrs
                .Where(static a => a.AttributeClass?.Name is E.OnlyYouSetAttribute
                                                          or E.OnlyAliasSetAttribute)
                .ToList();
            //oSetAttrs.AddRange(oSetAttrsOnClass);
            if ((isMethod || !isSet) && oSetAttrs.Count() == oAttrs.Count())
            {
                // All can read [Only*Set] property
                return false;
            }

            // flag: isSet and there are (non zero) both [Only*Set] and [OnlyYou/Alias] attributes
            bool isMixSet = isSet && oSetAttrs.Count() > 0 && oAttrs.Count() > oSetAttrs.Count();
            
            IEnumerable<AttrInfo> oAttrsInfos = 
                (isMixSet ? oSetAttrs : oAttrs) // when isMixSet then only oSetAttrs intresting for us
                .Where(a => AttrInfo.IsFit(a, outerClassSymbol, isSet))
                .Select(static a => new AttrInfo(a, a.AttributeClass!.IsGenericType, a.AttributeClass!))
                .ToList();
            if (oAttrsInfos.Count() == 0) // outerClass is not "Only" type
            //if (oAttrsInfos.Count() < oAttrs.Count()) // outerClass is not "Only" type
            {
                reportDiagnostic(rule, context, memberNode, messageArgs);
                return true;
            }

            oAttrsInfos = oAttrsInfos
                .Where(static a => a.IsGeneric
                            ? a.AttrData.ConstructorArguments[0].Values.Count() > 0
                            : a.AttrData.ConstructorArguments[1].Values.Count() > 0)
                .ToList();
            if (oAttrsInfos.Count() == 0)
            {
                // => all members of the outerClass is friends (i.e. allowed)
                return false;
            }

            bool isAllowByOU = true; // is allow (by ouAttrsInfos) to invoke our member (symbol)?
            string outerMemberName = outerMember.GetName();
            // select [OnlyYou(Set)] attributes (OU)
            var ouAttrsInfos = isMethod //|| !isSet
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
                var oaAttrInfos = isMethod //|| !isSet
                    ? oAttrsInfos.Where(static a => a.AttrSymbol?.Name == E.OnlyAliasAttribute)
                    : oAttrsInfos.Where(static a => a.AttrSymbol?.Name is E.OnlyAliasAttribute or E.OnlyAliasSetAttribute);
                if (oaAttrInfos.Count() == 0)
                {
                    isAllowByOA = false;
                    break;
                }

                ISymbol? outerMemberSymbol = context.SemanticModel.GetDeclaredSymbol(outerMember);
                ISymbol? outerInterfaceMethod = outerMemberSymbol?.FindInterfaceMember();
                if (outerMember.AttributeLists.Count == 0 && outerInterfaceMethod is null)
                {
                    isAllowByOA = false;
                    break;
                }

                // [Alias(methodAlias)]
                // get [Alias] attribute for outerMember
                var aliasAttr = outerMemberSymbol?
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

                // [OnlyAlias(Set)(type, aliases)]
                var aliases = oaAttrInfos
                    .SelectMany(static a => a.IsGeneric
                        ? a.AttrData.ConstructorArguments[0].Values
                        : a.AttrData.ConstructorArguments[1].Values)
                    .Select(a => a.Value?.ToString())
                    .Where(a => a is not null);

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
                reportDiagnostic(rule, context, memberNode, messageArgs);
                return true;
            }

            return false;
        }

        static IEnumerable<AttributeData> getAttributes(in ISymbol symbol, bool isMethod, bool isSet)
            => isMethod //|| !isSet
            ? symbol.GetAttributes()
                    .Where(static a => a.AttributeClass?.Name is E.OnlyYouAttribute
                                                              or E.OnlyAliasAttribute)
            : symbol.GetAttributes()
                    .Where(static a => a.AttributeClass?.Name is E.OnlyYouAttribute
                                                              or E.OnlyAliasAttribute
                                                              or E.OnlyYouSetAttribute
                                                              or E.OnlyAliasSetAttribute);

        /*-----------------------------------------------------------*/

        //Beep();

        if (memberSymbol is null)
        {
            return false;
        }

        // get 'outer' method and class/interface in which the member is located
        var outerInfo = OuterInfo.Get(context, memberSymbol, memberNode);
        if (outerInfo is null)
        {
            return false;
        }

        // get interface (if exists in parents) in which the member is defined
        ISymbol? interfaceMember = memberSymbol.FindInterfaceMember();

        #region `[Exclude*]` attributes analysis

        // get `[Exclude*]` attributes on the member itself and its interface-member
        var allExcludeAttrsOnMember = memberSymbol.GetAttributes()
            .Where(static a => a.AttributeClass?.Name == E.ExcludeAttribute)
            .ToList();
        if (interfaceMember is not null)
        {
            // get and add `[Exclude*]` attributes on interface-member
            IEnumerable<AttributeData>? oAttrsOnInterfaceMember = interfaceMember
                .GetAttributes()
                .Where(static a => a.AttributeClass?.Name is E.ExcludeAttribute);
            allExcludeAttrsOnMember.AddRange(oAttrsOnInterfaceMember);
        }

        // get `TypeInfo` for types of `[Exclude+]` (attributes with type arguments)
        IEnumerable<TypeInfo> excludeTypesOnMember = allExcludeAttrsOnMember
            .Where(static a => a.AttributeClass!.IsGenericType || a.ConstructorArguments.Count() > 0)
            .Select(static a => TypeInfo.Create(a))
            .ToList();
        if (allExcludeAttrsOnMember.Count() > excludeTypesOnMember.Count())
        {
            // => there is "strong" `[Exclude]` (without any arguments)
            return false;
        }

        #endregion `[Exclude*]` attributes analysis

        bool isMethod = memberSymbol is IMethodSymbol;

        // get o-attributes on member's class (potential attractor) itself and its interface
        var oAttrsOnClass = getAttributes(memberSymbol.ContainingType, isMethod, isSet).ToList();
        if (interfaceMember is not null)
        {
            // get and add o-attributes on interface of member's class
            oAttrsOnClass.AddRange(getAttributes(interfaceMember.ContainingType, isMethod, isSet));
        }

        //!!! ??? the following code doesn't work properly: IntelliSense doesn't highlight the error
        //if (oAttrsOnClass.Count() == 0 && allExcludeAttrsOnMember.Count() > 0)
        //{
        //    var syntaxReferences = symbol.DeclaringSyntaxReferences;
        //    SyntaxNode syntaxNode = syntaxReferences.First().GetSyntax();
        //    var span = syntaxNode.Span;
        //    span = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(span.Start, span.Start + 10);
        //    var diagnostic = Diagnostic.Create(
        //        typeRule,
        //        Location.Create(syntaxNode.SyntaxTree, span),
        //        ["qq1", "qq2"]
        //    );
        //    context.ReportDiagnostic(diagnostic);
        //    return true;
        //}

        // exclude o-attributes which should be excluded according to `[Exclude+]` (with type arguments)
        if (excludeTypesOnMember.Count() > 0)
        {
            oAttrsOnClass = oAttrsOnClass
                .Where(a => !excludeTypesOnMember.Any(t => t.IsFit(a)))
                .ToList();
        }

        if (oAttrsOnClass.Count() > 0) // => realy attractor
        {
            /* /// Example
            [OnlyYou<OuterClass>(nameof(OuterClass.OuterMethod1))] // oAttrsOnClass
            class Attractor {                
                public void Member1();
                public int  Member2;
            }

            class OuterClass {
                void OuterMethod1(in Me me) => me.Member1();   // memberNode // ok
                void OuterMethod2(in Me me) => me.Member2 = 0; // memberNode // err
            }
             */

            bool hasOnlySetAttr = isSet && oAttrsOnClass.Any(static a =>
                a.AttributeClass?.Name is E.OnlyYouSetAttribute or E.OnlyAliasSetAttribute);
            if (analyzeOperation(
                context,
                hasOnlySetAttr ? setTypeRule : typeRule,
                outerInfo,
                memberSymbol,
                memberNode,
                oAttrsOnClass,
                //[],
                isMethod,
                isSet,
                outerInfo.OuterMember.GetName(), memberSymbol.ContainingType.Name))
            {
                return true;
            }
        }

        ///////////// Members analysis: /////////////

        /* /// Example
        class Attractor {
            [OnlyYou<OuterClass>(nameof(OuterClass.OuterMethod))] // oAttrsOnMember
            public void Member();
        }
         
        class OuterClass {
            void OuterMethod(in Me me) => me.Member(); // memberNode // ok
        }
         */

        //IEnumerable<AttributeData> oSetAttrsOnClass = isSet
        //    ? oAttrsOnClass
        //      .Where(static a => a.AttributeClass?.Name is E.OnlyYouSetAttribute
        //                                                or E.OnlyAliasSetAttribute)
        //      .ToList()
        //    : [];

        // get o-attributes on member (member of attractor) itself and its interface-member
        var oAttrsOnMember = getAttributes(memberSymbol, isMethod, isSet).ToList();
        if (interfaceMember is not null)
        {
            oAttrsOnMember.AddRange(getAttributes(interfaceMember, isMethod, isSet));
        }

        if (oAttrsOnMember.Count() == 0) // => member doesn't belong to attractor class
        {
            return false;
        }

        return analyzeOperation(
            context,
            rule,
            outerInfo,
            memberSymbol,
            memberNode,
            oAttrsOnMember,
            //oSetAttrsOnClass,
            isMethod,
            isSet,
            memberSymbol.Name, outerInfo.OuterMember.GetName());
        //return false;
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
            in ISymbol operationSymbol,
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
        if (operationSymbol.ContainingType.GenericCaseEqualTo(outerClassSymbol))
        {
            return null;
        }

#if !STRICT_OU
        // allow a call inside the dirived classes
        if (outerClassSymbol?.InheritsFrom(operationSymbol.ContainingType) ?? true)
        {
            return null;
        }
#endif
        return new OuterInfo(outerMember, outerClass, outerClassSymbol!);
    }
}

file class AttrInfo
{
    public AttrInfo(
        AttributeData attrData,
        bool isGeneric,
        INamedTypeSymbol attrSymbol)
    {
        AttrData = attrData;
        IsGeneric = isGeneric;
        AttrSymbol = attrSymbol;
    }

    //public static IEnumerable<ITypeSymbol> Get

    public static bool IsFit(
        in AttributeData attrData,
        in INamedTypeSymbol outerTypeSymbol,
        in bool isSet
        //,in IEnumerable<AttributeData> oAttrs,
        //in IEnumerable<AttributeData> oSetAttrs
        )
    {
        var outerTypeInterfaces = outerTypeSymbol.AllInterfaces.ToList();
        var outerType = outerTypeSymbol.IsGenericType
            ? outerTypeSymbol.OriginalDefinition
            : outerTypeSymbol;

        var attributeClass = attrData.AttributeClass!;
        if (attributeClass.IsGenericType)
        {
            if (SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), outerTypeSymbol)
                || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), i)))
            {
                return true;
            }

            //if (!isSet && attributeClass.Name is E.OnlyYouSetAttribute or E.OnlyAliasSetAttribute)
            //{
            //    return true;
            //}

            return false;
        }

        var friendType = attrData.ConstructorArguments[0].Value as INamedTypeSymbol;
        if (friendType is null)
        {
            string? friendTypeName = attrData.ConstructorArguments[0].Value as string;
            if (friendTypeName is not null)
            {
                string? outerTypeName = outerTypeSymbol.ToDisplayString(Const.SymbolDisplayFormat);
                if (friendTypeName.StartsWith(Const.RegExPrefix))
                {
                    friendTypeName = friendTypeName.Substring(Const.RegExPrefixLen).Trim();
                    var friendTypeRegEx = new Regex(friendTypeName, RegexOptions.Compiled);
                    if (friendTypeRegEx.IsMatch(outerTypeName)
                        || outerTypeInterfaces.Any(i => friendTypeRegEx.IsMatch(i.ToDisplayString(Const.SymbolDisplayFormat))))
                    {
                        return true;
                    }
                }

                if (friendTypeName == outerTypeName
                    || outerTypeInterfaces.Any(i => i.ToDisplayString(Const.SymbolDisplayFormat) == friendTypeName))
                {
                    return true;
                }
            }

            return false;
        }

        friendType = friendType.IsGenericType ? friendType.OriginalDefinition : friendType;
        if (SymbolEqualityComparer.Default.Equals(friendType, outerType)
            || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(friendType, i)))
        {
            return true;
        }

        return false;
    }

    public static bool IsFit0(
        in AttributeData attrData,
        in INamedTypeSymbol outerTypeSymbol,
        in bool isSet)
    {
        var outerTypeInterfaces = outerTypeSymbol.AllInterfaces.ToList();
        var outerType = outerTypeSymbol.IsGenericType
            ? outerTypeSymbol.OriginalDefinition
            : outerTypeSymbol;

        var attributeClass = attrData.AttributeClass!;
        if (attributeClass.IsGenericType)
        {
            if (SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), outerTypeSymbol)
                || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(attributeClass.TypeArguments.First(), i)))
            {
                return true;
            }

            //if (!isSet && attributeClass.Name is E.OnlyYouSetAttribute or E.OnlyAliasSetAttribute)
            //{
            //    return true;
            //}

            return false;
        }

        var friendType = attrData.ConstructorArguments[0].Value as INamedTypeSymbol;
        if (friendType is null)
        {
            string? friendTypeName = attrData.ConstructorArguments[0].Value as string;
            if (friendTypeName is not null)
            {
                string? outerTypeName = outerTypeSymbol.ToDisplayString(Const.SymbolDisplayFormat);
                if (friendTypeName.StartsWith(Const.RegExPrefix))
                {
                    friendTypeName = friendTypeName.Substring(Const.RegExPrefixLen).Trim();
                    var friendTypeRegEx = new Regex(friendTypeName, RegexOptions.Compiled);
                    if (friendTypeRegEx.IsMatch(outerTypeName)
                        || outerTypeInterfaces.Any(i => friendTypeRegEx.IsMatch(i.ToDisplayString(Const.SymbolDisplayFormat))))
                    {
                        return true;
                    }
                }

                if (friendTypeName == outerTypeName
                    || outerTypeInterfaces.Any(i => i.ToDisplayString(Const.SymbolDisplayFormat) == friendTypeName))
                {
                    return true;
                }
            }

            return false;
        }

        friendType = friendType.IsGenericType ? friendType.OriginalDefinition : friendType;
        if (SymbolEqualityComparer.Default.Equals(friendType, outerType)
            || outerTypeInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(friendType, i)))
        {
            return true;
        }

        return false;
    }

    public AttributeData AttrData { get; }
    public bool IsGeneric { get; }
    public INamedTypeSymbol AttrSymbol { get; }
}

file class TypeInfo
{
    public static TypeInfo Create(AttributeData attrData)
    {
        if (attrData.AttributeClass!.IsGenericType)
        {
            return new TypeInfo(attrData.AttributeClass.TypeArguments.First() as INamedTypeSymbol, null);
        }

        return attrData.ConstructorArguments[0].Value switch
        {
            string str => new TypeInfo(null, str),
            INamedTypeSymbol symbol => new TypeInfo(symbol, null),
            _ => new TypeInfo(null, null)
            //_ => throw new ArgumentException()
        };
    }

    private TypeInfo(INamedTypeSymbol? symbol, string? name)
    {
        Symbol = symbol;
        Name = name;
    }

    public INamedTypeSymbol? Symbol { get; }
    public string? Name { get; }
    public bool IsFit(AttributeData attrData)
    {
        ITypeSymbol? tSymbol = null;
        if (attrData.AttributeClass!.IsGenericType)
        {
            tSymbol = attrData.AttributeClass.TypeArguments.First();
        }

        string? tName = null;
        if (tSymbol is null)
        {
            switch(attrData.ConstructorArguments[0].Value)
            {
                case INamedTypeSymbol s:
                    tSymbol = s;
                    break;
                case string n:
                    tName = n;
                    break;
            }
        }

        if (tSymbol is not null)
        {
            if (Symbol is not null)
            {
                return SymbolEqualityComparer.Default.Equals(tSymbol, Symbol);
            }
            if (Name is not null)
            {
                return tSymbol.ToDisplayString(Const.SymbolDisplayFormat) == Name;
            }

            return false;
        }

        if (tName is not null)
        {
            if(Name is not null && tName.StartsWith(Const.RegExPrefix) && !Name.StartsWith(Const.RegExPrefix))
            {
                return Regex.IsMatch(Name, tName.Substring(Const.RegExPrefixLen).Trim());
            }

            return tName == Name;
        }

        return false;
    }
}

file static class Const
{
    public const string RegExPrefix = "R:";
    public static readonly int RegExPrefixLen = RegExPrefix.Length;

    public static readonly SymbolDisplayFormat SymbolDisplayFormat = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
}
