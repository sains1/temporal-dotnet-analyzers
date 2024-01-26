using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// An analyzer that reports any usage of .NET Timers in workflows
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class WorkflowTimerAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TMPRL0001";
    private const string Category = "Non-Determinism";

    private static readonly LocalizableString Title = "Workflow contains a timer";
    private static readonly LocalizableString MessageFormat = "Workflow contains a timer: '{0}'";
    private static readonly LocalizableString Description = "Workflows should not contain timers.";

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

        // find any usages of Task.Delay in the run methods
        var finder = new TimeDelayUsageFinder(context.SemanticModel);
        foreach (var method in runMethods)
        {
            var usages = finder.FindUsages(method);
            foreach (var usage in usages)
            {
                var diagnostic = Diagnostic.Create(Rule, usage.GetLocation(), usage.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private class TimeDelayUsageFinder : CSharpSyntaxWalker
    {
        public TimeDelayUsageFinder(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        private readonly List<InvocationExpressionSyntax> _delayUsages = new();
        private readonly SemanticModel _semanticModel;

        public IReadOnlyList<InvocationExpressionSyntax> FindUsages(MethodDeclarationSyntax methodDeclaration)
        {
            _delayUsages.Clear();
            Visit(methodDeclaration);
            return _delayUsages;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Check if the invocation is Task.Delay
            if (node.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Delay" } delayMa &&
                _semanticModel.GetSymbolInfo(delayMa).Symbol?.ContainingType?.Name == "Task")
            {
                _delayUsages.Add(node);
            }

            // Check if the invocation is Thread.Sleep
            if (node.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Sleep" } sleepMa &&
                _semanticModel.GetSymbolInfo(sleepMa).Symbol?.ContainingType?.Name == "Thread")
            {
                _delayUsages.Add(node);
            }

            base.VisitInvocationExpression(node);
        }
    }

}