using System;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports usage of SystemClock in workflows
/// </summary>
internal class SystemClockAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants
    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0002";
        public const string Title = "Workflow contains use of System Clock";
        public const string MessageFormat = "Workflow contains use of System Clock: '{0}'";
        public const string Description = "Workflows should not contain usages of System Clock.";
        public const string Category = "TemporalWorkflow";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        public const bool IsEnabledByDefault = true;
    }

    private static readonly DiagnosticDescriptor Descriptor = new(RuleConstants.DiagnosticId, RuleConstants.Title,
        RuleConstants.MessageFormat, RuleConstants.Category, RuleConstants.Severity, RuleConstants.IsEnabledByDefault,
        RuleConstants.Description);

    public DiagnosticDescriptor DiagnosticDescriptor => Descriptor;

    #endregion

    private static readonly MemberAccessUsageFinder ClockUsageFinder = new([
        (nameof(DateTime), nameof(DateTime.Now)),
        (nameof(DateTime), nameof(DateTime.UtcNow)),
        (nameof(DateTimeOffset), nameof(DateTimeOffset.Now)),
        (nameof(DateTimeOffset), nameof(DateTimeOffset.UtcNow))
    ]);

    public void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        ClockUsageFinder.FindUsages(method,
            usage => context.ReportDiagnostic(Diagnostic.Create(Descriptor, usage.GetLocation(), usage.ToString())));
    }
}