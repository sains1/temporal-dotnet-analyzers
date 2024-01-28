using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers.WorkflowSignalAnalyzers;

/// <summary>
/// An analyzer that reports any usage of disallowed return types in workflow queries
/// </summary>
public class WorkflowSignalReturnTypeAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants

    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0005";
        public const string Title = "Workflow signal contains incorrect return type";
        public const string MessageFormat = "Workflow signals should return 'Task' but got: '{0}'";
        public const string Description = "Workflow signals should return 'Task'.";
        public const string Category = "TemporalWorkflow";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        public const bool IsEnabledByDefault = true;
    }

    public static readonly DiagnosticDescriptor Descriptor = new(RuleConstants.DiagnosticId, RuleConstants.Title,
        RuleConstants.MessageFormat, RuleConstants.Category, RuleConstants.Severity, RuleConstants.IsEnabledByDefault,
        RuleConstants.Description);

    public DiagnosticDescriptor DiagnosticDescriptor => Descriptor;

    #endregion

    public void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (method.ReturnType is IdentifierNameSyntax { Identifier.ValueText: "Task" })
            return;

        context.ReportDiagnostic(Diagnostic.Create(Descriptor, method.ReturnType.GetLocation(),
            method.ReturnType.ToString()));
    }
}