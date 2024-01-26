using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

internal class MemberAccessUsageFinder(List<(string containingType, string memberIdentifier)> memberIdentifiers)
    : CSharpSyntaxWalker
{
    private event Action<MemberAccessExpressionSyntax>? OnUsageFound;

    public IEnumerable<MemberAccessExpressionSyntax> FindUsages(MethodDeclarationSyntax methodDeclaration)
    {
        var usages = new List<MemberAccessExpressionSyntax>();
        var usageFound = (MemberAccessExpressionSyntax usage) => usages.Add(usage);

        OnUsageFound += usageFound;
        Visit(methodDeclaration);
        OnUsageFound -= usageFound;

        return usages;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        foreach (var identifier in memberIdentifiers)
        {
            if (node.Name.Identifier.Text == identifier.memberIdentifier &&
                node.Expression is IdentifierNameSyntax ins &&
                ins.Identifier.Text == identifier.containingType)
            {
                OnUsageFound?.Invoke(node);
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}