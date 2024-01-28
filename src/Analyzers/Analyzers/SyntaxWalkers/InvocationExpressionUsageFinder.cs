using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

internal class InvocationExpressionUsageFinder(Dictionary<string, string> memberIdentifiers)
    : CSharpSyntaxWalker
{
    private readonly object _lockObject = new();
    private Action<InvocationExpressionSyntax>? _onUsageFound;

    public void FindUsages(MethodDeclarationSyntax methodDeclaration, Action<InvocationExpressionSyntax> onUsageFound)
    {
        lock (_lockObject)
        {
            _onUsageFound = onUsageFound;
            Visit(methodDeclaration);
            _onUsageFound = null;
        }
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.Expression is MemberAccessExpressionSyntax ma
            && memberIdentifiers.ContainsKey(ma.Name.Identifier.Text)
            && ma.Expression is IdentifierNameSyntax ins
            && ins.Identifier.Text == memberIdentifiers[ma.Name.Identifier.Text])
        {
            _onUsageFound?.Invoke(node);
        }

        base.VisitInvocationExpression(node);
    }
}