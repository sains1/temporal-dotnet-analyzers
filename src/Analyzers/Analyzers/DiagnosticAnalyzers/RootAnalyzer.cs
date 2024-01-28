using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers.WorkflowQueryAnalyzers;
using Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

/// <summary>
/// Root analyzer is the entrypoint for the other analyzers. As our analyzers all work on the same WorkflowRun methods
/// we can prevent the need to do the expensive symbol analysis multiple times
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RootAnalyzer : DiagnosticAnalyzer
{
    private static readonly GuidAnalyzer GuidAnalyzer = new();
    private static readonly SystemClockAnalyzer SystemClockAnalyzer = new();
    private static readonly WorkflowTimerAnalyzer WorkflowTimerAnalyzer = new();
    private static readonly WorkflowQueryReturnTypeAnalyzer WorkflowQueryReturnTypeAnalyzer = new();

    private readonly List<ITemporalRunAnalyzer> _workflowRunAnalyzers =
        [GuidAnalyzer, SystemClockAnalyzer, WorkflowTimerAnalyzer];
    
    private readonly List<ITemporalRunAnalyzer> _workflowQueryAnalyzers =
        [WorkflowQueryReturnTypeAnalyzer];

    private ImmutableArray<DiagnosticDescriptor> Diagnostics =>
        _workflowRunAnalyzers.Select(x => x.DiagnosticDescriptor)
            .Concat(_workflowQueryAnalyzers.Select(x => x.DiagnosticDescriptor)).ToImmutableArray();

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Diagnostics;

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

        // filter out classes not decorated with WorkflowAttribute
        if (!HasAttribute(classDeclarationNode, context.SemanticModel,
                TemporalConstants.WorkflowAttribute))
            return;

        // find all of the methods on the class with WorkflowRunAttribute
        var runMethods = classDeclarationNode
            .DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(x => HasAttribute(x, context.SemanticModel, TemporalConstants.WorkflowRunAttribute));
        
        // find all of the methods on the class with WorkflowQueryAttribute
        var queryMethods = classDeclarationNode
            .DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(x => HasAttribute(x, context.SemanticModel, TemporalConstants.WorkflowQueryAttribute));

        foreach (var method in runMethods)
        {
            Parallel.ForEach(_workflowRunAnalyzers, analyzer =>
            {
                analyzer.AnalyzeWorkflowRunMethod(context, method);
            });
        }
        
        foreach (var method in queryMethods)
        {
            Parallel.ForEach(_workflowQueryAnalyzers, analyzer =>
            {
                analyzer.AnalyzeWorkflowRunMethod(context, method);
            });
        }
    }

    private static bool HasAttribute(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                ModelExtensions.GetTypeInfo(semanticModel, attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }

    private static bool HasAttribute(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = methodDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                ModelExtensions.GetTypeInfo(semanticModel, attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }
}