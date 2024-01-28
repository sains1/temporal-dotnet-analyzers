using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

internal class MemberAccessUsageFinder(List<(string containingType, string memberIdentifier)> memberIdentifiers)
    : CSharpSyntaxWalker
{
    private List<MemberAccessExpressionSyntax> Usages { get; } = new();

    public IEnumerable<MemberAccessExpressionSyntax> FindUsages(MethodDeclarationSyntax methodDeclaration)
    {
        Visit(methodDeclaration);
        return Usages;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        foreach (var identifier in memberIdentifiers)
        {
            if (node.Name.Identifier.Text == identifier.memberIdentifier &&
                node.Expression is IdentifierNameSyntax ins &&
                ins.Identifier.Text == identifier.containingType)
            {
                Usages.Add(node);
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}