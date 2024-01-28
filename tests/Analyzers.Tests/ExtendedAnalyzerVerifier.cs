using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Analyzers.Tests;

public abstract class ExtendedAnalyzerVerifier<TAnalyzer> : ExtendedAnalyzerVerifier<TAnalyzer,
    CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new();

public class ExtendedAnalyzerVerifier<TAnalyzer, TTest, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TTest : AnalyzerTest<TVerifier>, new()
    where TVerifier : IVerifier, new()
{
    public static DiagnosticResult Diagnostic()
    {
        var analyzer = new TAnalyzer();
        try
        {
            return Diagnostic(analyzer.SupportedDiagnostics.Single());
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"'{nameof(Diagnostic)}()' can only be used when the analyzer has a single supported diagnostic. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
                ex);
        }
    }

    public static DiagnosticResult Diagnostic(string diagnosticId)
    {
        var analyzer = new TAnalyzer();
        try
        {
            return Diagnostic(analyzer.SupportedDiagnostics.Single(i => i.Id == diagnosticId));
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"'{nameof(Diagnostic)}(string)' can only be used when the analyzer has a single supported diagnostic with the specified ID. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
                ex);
        }
    }

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new DiagnosticResult(descriptor);

    public static Task VerifyAnalyzerAsync(string source, Action<TTest>? configure = default, params DiagnosticResult[] expected)
    {
        var test = new TTest
        {
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        configure?.Invoke(test);

        return test.RunAsync(CancellationToken.None);
    }
}