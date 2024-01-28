using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Analyzers.DiagnosticAnalyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Temporalio.Api.Command.V1;

using Verifier = Analyzers.Tests.ExtendedAnalyzerVerifier<Analyzers.DiagnosticAnalyzers.RootAnalyzer>;

namespace Analyzers.Tests;

public abstract class AnalyzerTestBase
{
    protected DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
    {
        return Verifier.Diagnostic(descriptor);
    }

    protected Task VerifyAnalyzerAsync(string fileName, params DiagnosticResult[] expected)
    {
        var text = File.ReadAllText(Path.Join("Sources", fileName));
        return Verifier.VerifyAnalyzerAsync(text, Configure, expected);
    }

    private void Configure(CSharpAnalyzerTest<RootAnalyzer, XUnitVerifier> configuration)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
        configuration.TestState.AdditionalReferences.AddRange(
            new[]
            {
                MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
            });
    }
}