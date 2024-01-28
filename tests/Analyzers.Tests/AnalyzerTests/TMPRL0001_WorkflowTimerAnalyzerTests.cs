using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers;
using Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class WorkflowTimerAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0001_TaskDelayWorkflow.cs", "Task.Delay(1000)", 12, 15)]
    [InlineData("TMPRL0001_ThreadSleepWorkflow.cs", "Thread.Sleep(1000)", 13, 9)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, string arguments, int line, int column)
    {
        var diagnostic = base.Diagnostic(WorkflowTimerAnalyzer.Descriptor)
            .WithLocation(line, column)
            .WithArguments(arguments);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}