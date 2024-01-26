using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers;

internal static class DeclarationSyntaxFilters
{
    internal static bool HasAttribute(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                semanticModel.GetTypeInfo(attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }

    internal static bool HasAttribute(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = methodDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                semanticModel.GetTypeInfo(attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }
}