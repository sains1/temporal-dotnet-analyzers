using System;
using System.Collections.Generic;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports usage of Guid generation in workflows
/// </summary>
internal class GuidAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants
    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0003";
        public const string Title = "Workflow contains use of Guid generator";
        public const string MessageFormat = "Workflow contains use of Guid generator: '{0}'";
        public const string Description = "Workflows should not contain usages of Guid generator.";
        public const string Category = "TemporalWorkflow";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        public const bool IsEnabledByDefault = true;
    }

    private static readonly DiagnosticDescriptor Descriptor = new(RuleConstants.DiagnosticId, RuleConstants.Title,
        RuleConstants.MessageFormat, RuleConstants.Category, RuleConstants.Severity, RuleConstants.IsEnabledByDefault,
        RuleConstants.Description);

    public DiagnosticDescriptor DiagnosticDescriptor => Descriptor;

    #endregion

    private static readonly InvocationExpressionUsageFinder Finder = new(new Dictionary<string, string>
    {
        [nameof(Guid.NewGuid)] = nameof(Guid)
    });

    void ITemporalRunAnalyzer.AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        Finder.FindUsages(method,
            usage => context.ReportDiagnostic(Diagnostic.Create(Descriptor, usage.GetLocation(), usage.ToString())));
    }
}