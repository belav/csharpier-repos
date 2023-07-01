using Microsoft.AspNetCore.Analyzer.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.AspNetCore.Components.Analyzers;

internal class ComponentAnalyzerDiagnosticAnalyzerRunner : DiagnosticAnalyzerRunner
{
    public ComponentAnalyzerDiagnosticAnalyzerRunner(DiagnosticAnalyzer analyzer)
    {
        Analyzer = analyzer;
    }

    public DiagnosticAnalyzer Analyzer { get; }

    public Task<Diagnostic[]> GetDiagnosticsAsync(string source)
    {
        return GetDiagnosticsAsync(sources: new[] { source }, Analyzer, Array.Empty<string>());
    }

    public Task<Diagnostic[]> GetDiagnosticsAsync(Project project)
    {
        return GetDiagnosticsAsync(new[] { project }, Analyzer, Array.Empty<string>());
    }
}
