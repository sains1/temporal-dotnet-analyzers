using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers;
using Analyzers.DiagnosticAnalyzers.WorkflowRunAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class GuidAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0003_GuidNewGuidWorkflow.cs", "Guid.NewGuid()", 13, 13)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, string arguments, int line, int column)
    {
        var diagnostic = base.Diagnostic(GuidAnalyzer.Descriptor)
            .WithLocation(line, column)
            .WithArguments(arguments);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}