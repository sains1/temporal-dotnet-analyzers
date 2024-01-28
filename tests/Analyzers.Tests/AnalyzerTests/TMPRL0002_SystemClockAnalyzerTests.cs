using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class SystemClockAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0002_DateTimeNowWorkflow.cs", "DateTime.Now", 13, 13)]
    [InlineData("TMPRL0002_DateTimeOffsetNowWorkflow.cs", "DateTimeOffset.Now", 13, 13)]
    [InlineData("TMPRL0002_DateTimeUtcNowWorkflow.cs", "DateTime.UtcNow", 13, 13)]
    [InlineData("TMPRL0002_DateTimeOffsetUtcNowWorkflow.cs", "DateTimeOffset.UtcNow", 13, 13)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, string arguments, int line, int column)
    {
        var diagnostic = base.Diagnostic(SystemClockAnalyzer.Descriptor)
            .WithLocation(line, column)
            .WithArguments(arguments);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}