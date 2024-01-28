using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.DiagnosticAnalyzers;

public interface ITemporalRunAnalyzer
{
    DiagnosticDescriptor DiagnosticDescriptor { get; }
    void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method);
}