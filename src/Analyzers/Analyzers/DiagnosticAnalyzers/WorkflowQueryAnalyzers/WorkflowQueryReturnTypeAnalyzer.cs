using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers.WorkflowQueryAnalyzers;

/// <summary>
/// An analyzer that reports any usage of disallowed return types in workflow queries
/// </summary>
public class WorkflowQueryReturnTypeAnalyzer : ITemporalRunAnalyzer
{
    # region diagnostic constants

    private struct RuleConstants
    {
        public const string DiagnosticId = "TMPRL0004";
        public const string Title = "Workflow query contains incorrect return type";
        public const string MessageFormat = "Workflow query contains disallowed return type: '{0}'";
        public const string Description = "Workflow queries should return void, Task, or Task<T>.";
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
        switch (method.ReturnType)
        {
            case PredefinedTypeSyntax { Keyword.ValueText: "void" }: // identify void return type
            case IdentifierNameSyntax { Identifier.ValueText: "Task" }: // identify Task return type
            case GenericNameSyntax { Identifier.ValueText: "Task" }: // identify Task<T> return type
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, method.ReturnType.GetLocation(),
                    method.ReturnType.ToString()));
                break;
        }
    }
}