using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers.SyntaxWalkers;

internal class InvocationExpressionUsageFinder(Dictionary<string, string> memberIdentifiers)
    : CSharpSyntaxWalker
{
    private List<InvocationExpressionSyntax> Usages { get; } = new();

    public IEnumerable<InvocationExpressionSyntax> FindUsages(MethodDeclarationSyntax methodDeclaration)
    {
        Visit(methodDeclaration);
        return Usages;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.Expression is MemberAccessExpressionSyntax ma
            && memberIdentifiers.ContainsKey(ma.Name.Identifier.Text)
            && ma.Expression is IdentifierNameSyntax ins
            && ins.Identifier.Text == memberIdentifiers[ma.Name.Identifier.Text])
        {
            Usages.Add(node);
        }

        base.VisitInvocationExpression(node);
    }
}