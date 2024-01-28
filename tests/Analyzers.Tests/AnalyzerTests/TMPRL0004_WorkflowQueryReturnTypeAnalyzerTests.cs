using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers;
using Analyzers.DiagnosticAnalyzers.WorkflowQueryAnalyzers;
using Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class WorkflowQueryReturnTypeAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0004_VoidReturnQuery.cs", "void", 8, 12)]
    [InlineData("TMPRL0004_TaskReturnQuery.cs", "Task", 10, 12)]
    [InlineData("TMPRL0004_TaskTReturnQuery.cs", "Task<string>", 10, 12)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, string arguments, int line, int column)
    {
        var diagnostic = base.Diagnostic(WorkflowQueryReturnTypeAnalyzer.Descriptor)
            .WithLocation(line, column)
            .WithArguments(arguments);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}