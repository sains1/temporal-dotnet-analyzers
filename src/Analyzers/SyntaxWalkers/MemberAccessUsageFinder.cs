using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

internal class MemberAccessUsageFinder(List<(string containingType, string memberIdentifier)> memberIdentifiers)
    : CSharpSyntaxWalker
{
    private readonly object _lockObject = new();
    private Action<MemberAccessExpressionSyntax>? _onUsageFound;

    public void FindUsages(MethodDeclarationSyntax methodDeclaration, Action<MemberAccessExpressionSyntax> onUsageFound)
    {
        lock (_lockObject)
        {
            _onUsageFound = onUsageFound;
            Visit(methodDeclaration);
            _onUsageFound = null;
        }
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        foreach (var identifier in memberIdentifiers)
        {
            if (node.Name.Identifier.Text == identifier.memberIdentifier &&
                node.Expression is IdentifierNameSyntax ins &&
                ins.Identifier.Text == identifier.containingType)
            {
                _onUsageFound?.Invoke(node);
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}