using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers.WorkflowSignalAnalyzers;

using Xunit;

namespace Analyzers.Tests.AnalyzerTests;

public class WorkflowSignalReturnTypeAnalyzerTests : AnalyzerTestBase
{
    [Theory]
    [InlineData("TMPRL0005_VoidReturnSignal.cs", "void", 8, 12)]
    [InlineData("TMPRL0005_StringReturnSignal.cs", "string", 8, 12)]
    [InlineData("TMPRL0005_TaskTReturnSignal.cs", "Task<string>", 10, 12)]
    public async Task ShouldProduceExpectedDiagnosticResult(string file, string arguments, int line, int column)
    {
        var diagnostic = base.Diagnostic(WorkflowSignalReturnTypeAnalyzer.Descriptor)
            .WithLocation(line, column)
            .WithArguments(arguments);

        await base.VerifyAnalyzerAsync(file, diagnostic);
    }
}