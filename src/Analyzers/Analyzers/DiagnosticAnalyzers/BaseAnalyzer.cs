using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

public abstract class BaseAnalyzer : DiagnosticAnalyzer
{
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
        if (!HasAttribute(classDeclarationNode, context.SemanticModel,
                TemporalConstants.WorkflowAttribute))
            return;

        // find all of the methods on the class with [WorkflowRun]
        var runMethods = classDeclarationNode
            .DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(x => HasAttribute(x, context.SemanticModel, TemporalConstants.WorkflowRunAttribute));

        foreach (var method in runMethods)
        {
            AnalyzeWorkflowRunMethod(context, method);
        }
    }

    protected abstract void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method);

    private static bool HasAttribute(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                semanticModel.GetTypeInfo(attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }

    private static bool HasAttribute(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, string attributeDisplayString)
    {
        var attributeSyntax = methodDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attribute =>
                semanticModel.GetTypeInfo(attribute).Type?.ToDisplayString() == attributeDisplayString);

        return attributeSyntax != null;
    }
}