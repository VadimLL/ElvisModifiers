using Microsoft.CodeAnalysis;

namespace ElvisModifiersGenerator;

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
}
