using System.Collections.Generic;
using System.Threading.Tasks;

using Analyzers.SyntaxWalkers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

/// <summary>
/// Analyzer that reports any usage of ConfigureAwait(false) in workflows
/// </summary>
public class ConfigureAwaitFalseAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants
    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0006";
        public const string Title = "Workflow contains use of ConfigureAwait(false)";
        public const string MessageFormat = "Do not use ConfigureAwait(false) in workflows";
        public const string Description = "Workflows should not contain usages of ConfigureAwait(false).";
        public const string Category = "TemporalWorkflow";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        public const bool IsEnabledByDefault = true;
    }

    public static readonly DiagnosticDescriptor Descriptor = new(RuleConstants.DiagnosticId, RuleConstants.Title,
        RuleConstants.MessageFormat, RuleConstants.Category, RuleConstants.Severity, RuleConstants.IsEnabledByDefault,
        RuleConstants.Description);

    public DiagnosticDescriptor DiagnosticDescriptor => Descriptor;

    #endregion

    private static readonly InvocationExpressionWithoutTypeUsageFinder Finder = new(new HashSet<string>
    {
        nameof(Task.ConfigureAwait)
    });

    public void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        Finder.FindUsages(method, usage =>
        {
            // check if the parameter list contains a single 'false' literal before doing more expensive symbol lookups
            if (usage.ArgumentList.Arguments.Count != 1 ||
                usage.ArgumentList.Arguments.First() is not
                { Expression: LiteralExpressionSyntax { Token.ValueText: "false" } })
            {
                return;
            }

            // get type information to ensure that the expression is a Task
            var symbol = context.SemanticModel.GetSymbolInfo(usage.Expression);
            if (symbol.Symbol?.ContainingType is { } nts &&
                nts.ToDisplayString().StartsWith("System.Threading.Tasks.Task"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, usage.ArgumentList.GetLocation()));
            }
        });
    }
}