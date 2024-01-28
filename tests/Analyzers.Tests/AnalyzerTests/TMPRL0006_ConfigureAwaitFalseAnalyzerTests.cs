using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class ConfigureAwaitFalseAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0006_ConfigureAwaitFalseWorkflow.cs", 12, 48)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, int line, int column)
    {
        var diagnostic = base.Diagnostic(ConfigureAwaitFalseAnalyzer.Descriptor)
            .WithLocation(line, column);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}