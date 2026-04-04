// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ElvisModifiersAnalyzer;

public static class CodeAnalysisExtensions
{
    public static SyntaxNode? SearchInParent(
        this SyntaxNode? node,
        in Predicate<SyntaxNode> predicate)
    {
        while (node != null)
        {
            if (predicate(node))
            {
                return node;
            }

            node = node.Parent;
        }

        return null;
    }

    public static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(this INamedTypeSymbol type)
    {
        INamedTypeSymbol? current = type.BaseType;
        while (current != null && current.SpecialType != SpecialType.System_Object)
        {
            yield return current;
            current = current.BaseType;
        }

        // get all implemented interfaces:
        //var allInterfaces = type.AllInterfaces;
    }

    public static ISymbol? FindInterfaceMember(this ISymbol symbol)
    {
        static ISymbol? findInterfaceMember<T>(INamedTypeSymbol typeSymbol, T memberSymbol)
            where T : ISymbol
        {
            foreach (var interfaceType in typeSymbol.AllInterfaces)
            {
                foreach (var member in interfaceType.GetMembers())
                {
                    if (member is T interfaceIProperty)
                    {
                        var implementingMember = memberSymbol.ContainingType
                            .FindImplementationForInterfaceMember(interfaceIProperty);

                        if (SymbolEqualityComparer.Default.Equals(memberSymbol, implementingMember))
                        {
                            return interfaceIProperty;
                        }
                    }
                }
            }

            return null;
        }

        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                // Check explicit implementations first
                if (methodSymbol.ExplicitInterfaceImplementations.Any())
                {
                    return methodSymbol.ExplicitInterfaceImplementations.First();
                }

                // Check implicit implementations
                return findInterfaceMember(methodSymbol.ContainingType, methodSymbol);

            case IPropertySymbol propertySymbol:
                if (propertySymbol.ExplicitInterfaceImplementations.Any())
                {
                    return propertySymbol.ExplicitInterfaceImplementations.First();
                }

                return findInterfaceMember(propertySymbol.ContainingType, propertySymbol);

            case IEventSymbol eventSymbol:
                if (eventSymbol.ExplicitInterfaceImplementations.Any())
                {
                    return eventSymbol.ExplicitInterfaceImplementations.First();
                }

                return findInterfaceMember(eventSymbol.ContainingType, eventSymbol);
        }

        return null;
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    public static ISymbol? FindBaseClassesMember(this ISymbol symbol)
    {
        if (symbol is IMethodSymbol methodSymbol)
        {
            // Check implicit implementations
            foreach (var baseType in methodSymbol.ContainingType.GetAllBaseTypes())
            {
                foreach (var member in baseType.GetMembers())
                {
                    if (member is IMethodSymbol baseMethod)
                    {
                    }
                }
            }
        }
        else if (symbol is IPropertySymbol propertySymbol)
        {

            // Check implicit implementations
            foreach (var baseType in propertySymbol.ContainingType.GetAllBaseTypes())
            {
                foreach (var member in baseType.GetMembers())
                {
                    if (member is IPropertySymbol baseProperty)
                    {
                    }
                }
            }
        }

        return null;
    }

    public static bool InheritsFrom(this ITypeSymbol type, ITypeSymbol baseType) // InheritsFromOrEquals(..)
    {
        //var current = type;
        var current = type.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;

            }

            current = current.BaseType;
        }

        return false;
    }

    public static bool GenericCaseEqualTo(this INamedTypeSymbol type, INamedTypeSymbol? otherType)
    {
        type = type.IsGenericType ? type.OriginalDefinition : type;
        otherType = otherType is not null && otherType.IsGenericType ? otherType.OriginalDefinition : otherType;
        return SymbolEqualityComparer.Default.Equals(type, otherType);
    }

    public static string GetName(this MemberDeclarationSyntax member)
    {
        static IEnumerable<string> getFieldNames(in FieldDeclarationSyntax fieldDeclaration)
        {
            // The Declaration provides access to the type and the list of variables.
            // A single declaration can have multiple variable declarators (e.g., int x, y;).
            return fieldDeclaration.Declaration.Variables.Select(v => v.Identifier.ValueText);
        }

        return member switch
        {
            MethodDeclarationSyntax method => method.Identifier.Text,
            ConstructorDeclarationSyntax ctor => ctor.Identifier.Text,
            PropertyDeclarationSyntax prop => prop.Identifier.Text,
            FieldDeclarationSyntax field => string.Join(", ", getFieldNames(field)),
            IndexerDeclarationSyntax indexer => "this[..]",
            _ => "__undefined__"
        };
    }

    public static string GetNamespace(this ITypeSymbol classSymbol)
    {
        return classSymbol.ContainingNamespace.ToDisplayString();
    }
}
