using System;
using System.Collections.Immutable;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports usage of SystemClock in workflows
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SystemClockAnalyzer : BaseAnalyzer
{
    #region diagnostic constants
    public const string DiagnosticId = "TMPRL0002";
    private const string Category = "Non-Determinism";

    private static readonly LocalizableString Title = "Workflow contains use of System Clock";
    private static readonly LocalizableString MessageFormat = "Workflow contains use of System Clock: '{0}'";
    private static readonly LocalizableString Description = "Workflows should not contain usages of System Clock.";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);
    #endregion

    private static readonly MemberAccessUsageFinder MemberAccessUsageFinder = new([
        (nameof(DateTime), nameof(DateTime.Now)),
        (nameof(DateTime), nameof(DateTime.UtcNow)),
        (nameof(DateTimeOffset), nameof(DateTimeOffset.Now)),
        (nameof(DateTimeOffset), nameof(DateTimeOffset.UtcNow))
    ]);

    protected override void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        var usages = MemberAccessUsageFinder.FindUsages(method);
        foreach (var usage in usages)
        {
            var diagnostic = Diagnostic.Create(Rule, usage.GetLocation(), usage.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}