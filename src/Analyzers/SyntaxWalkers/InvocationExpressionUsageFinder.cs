using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

/// <summary>
/// Walks the csharp syntax tree and finds usages of the specified member & type identifiers. Does not use symbol
/// analysis so it is possible to get false positives
/// </summary>
/// <param name="memberIdentifiers"></param>
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

/// <summary>
/// Walks the csharp syntax tree and finds usages of the specified member identifiers. Does not check the type
/// identifier but this can be checked manually in the callback e.g. using semantic model for more expensive type lookups
/// </summary>
/// <param name="memberIdentifiers"></param>
internal class InvocationExpressionWithoutTypeUsageFinder(ISet<string> memberIdentifiers)
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
            && memberIdentifiers.Contains(ma.Name.Identifier.Text))
        {
            _onUsageFound?.Invoke(node);
        }

        base.VisitInvocationExpression(node);
    }
}