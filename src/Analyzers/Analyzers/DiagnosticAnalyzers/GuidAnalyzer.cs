using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports usage of Random generators in workflows
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GuidAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TMPRL0003";
    private const string Category = "Non-Determinism";

    private static readonly LocalizableString Title = "Workflow contains use of Guid generator";
    private static readonly LocalizableString MessageFormat = "Workflow contains use of Guid generator: '{0}'";
    private static readonly LocalizableString Description = "Workflows should not contain usages of Guid generator.";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
    }

    private static readonly List<(string memberIdentifier, string containingType)> ClockIdentifiers =
    [
        (nameof(Guid), nameof(Guid.NewGuid)),
    ];

    private static readonly MemberAccessUsageFinder MemberAccessUsageFinder = new(ClockIdentifiers);

    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationNode)
            return;

        // filter out classes not decorated with [Workflow]
        if (!DeclarationSyntaxFilters.HasAttribute(classDeclarationNode, context.SemanticModel,
                TemporalConstants.WorkflowAttribute))
            return;

        // find all of the methods on the class with [WorkflowRun]
        var runMethods = classDeclarationNode
            .DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(x => DeclarationSyntaxFilters.HasAttribute(x, context.SemanticModel,
                TemporalConstants.WorkflowRunAttribute));

        foreach (var method in runMethods)
        {
            // find all usages of system clocks within the method
            var usages = MemberAccessUsageFinder.FindUsages(method);
            foreach (var usage in usages)
            {
                var diagnostic = Diagnostic.Create(Rule, usage.GetLocation(), usage.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}