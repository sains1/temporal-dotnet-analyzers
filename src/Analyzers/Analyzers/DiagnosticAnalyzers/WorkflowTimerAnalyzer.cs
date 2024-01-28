using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports any usage of .NET Timers in workflows
/// </summary>
public class WorkflowTimerAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants
    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0001";
        public const string Title = "Workflow contains a timer";
        public const string MessageFormat = "Workflow contains a timer: '{0}'";
        public const string Description = "Workflows should not contain timers.";
        public const string Category = "TemporalWorkflow";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        public const bool IsEnabledByDefault = true;
    }

    public static readonly DiagnosticDescriptor Descriptor = new(RuleConstants.DiagnosticId, RuleConstants.Title,
        RuleConstants.MessageFormat, RuleConstants.Category, RuleConstants.Severity, RuleConstants.IsEnabledByDefault,
        RuleConstants.Description);

    public DiagnosticDescriptor DiagnosticDescriptor => Descriptor;

    #endregion

    private static readonly InvocationExpressionUsageFinder Finder = new(new Dictionary<string, string>
    {
        [nameof(Task.Delay)] = nameof(Task),
        [nameof(Thread.Sleep)] = nameof(Thread)
    });

    public void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        Finder.FindUsages(method,
            usage => context.ReportDiagnostic(Diagnostic.Create(Descriptor, usage.GetLocation(), usage.ToString())));
    }
}