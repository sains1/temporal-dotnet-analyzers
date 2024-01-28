using System.Collections.Generic;
using System.Collections.Immutable;
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
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class WorkflowTimerAnalyzer : BaseAnalyzer
{
    #region diagnostic constants
    public const string DiagnosticId = "TMPRL0001";
    private const string Category = "Non-Determinism";

    private static readonly LocalizableString Title = "Workflow contains a timer";
    private static readonly LocalizableString MessageFormat = "Workflow contains a timer: '{0}'";
    private static readonly LocalizableString Description = "Workflows should not contain timers.";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);
    # endregion

    private static readonly InvocationExpressionUsageFinder Finder = new(new Dictionary<string, string>
    {
        [nameof(Task.Delay)] = nameof(Task),
        [nameof(Thread.Sleep)] = nameof(Thread)
    });

    protected override void AnalyzeWorkflowRunMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        var usages = Finder.FindUsages(method);
        foreach (var usage in usages)
        {
            var diagnostic = Diagnostic.Create(Rule, usage.GetLocation(), usage.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}