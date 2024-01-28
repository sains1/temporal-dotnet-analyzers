using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports usage of Guid generation in workflows
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GuidAnalyzer : BaseAnalyzer
{
    #region diagnostic constants
    public const string DiagnosticId = "TMPRL0003";
    private const string Category = "Non-Determinism";

    private static readonly LocalizableString Title = "Workflow contains use of Guid generator";
    private static readonly LocalizableString MessageFormat = "Workflow contains use of Guid generator: '{0}'";
    private static readonly LocalizableString Description = "Workflows should not contain usages of Guid generator.";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);
    #endregion

    private static readonly InvocationExpressionUsageFinder Finder = new(new Dictionary<string, string>
    {
        [nameof(Guid.NewGuid)] = nameof(Guid)
    });

    protected override void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        Finder.Visit(method);

        foreach (var usage in Finder.FindUsages(method))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, usage.GetLocation(), usage.ToString()));
        }
    }
}