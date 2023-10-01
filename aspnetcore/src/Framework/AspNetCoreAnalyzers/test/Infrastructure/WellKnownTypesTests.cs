// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.AspNetCore.Analyzer.Testing;
using Microsoft.AspNetCore.App.Analyzers.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.AspNetCore.Analyzers.Infrastructure;

using WellKnownType = WellKnownTypeData.WellKnownType;

public partial class WellKnownTypesTests
{
    private TestDiagnosticAnalyzerRunner Runner { get; } = new(new TestAnalyzer());

    [Fact]
    public async Task ResolveAllWellKnownTypes()
    {
        // Arrange
        var source = TestSource.Read(@"
class Program
{
    static void Main()
    {
    }
}
");
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Collection(diagnostics, d => Assert.Equal("TEST001", d.Id));
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    private class TestAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor SuccessDescriptor = new(
            "TEST001",
            "Success result",
            "Success result",
            "Usage",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(new[]
        {
            SuccessDescriptor
        });

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSemanticModelAction(AnalyzeSemanticModel);
        }

        public void AnalyzeSemanticModel(SemanticModelAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;

            var wellKnownTypes = WellKnownTypes.GetOrCreate(semanticModel.Compilation);

            var wellKnownTypeKeys = Enum.GetValues<WellKnownType>();
            foreach (var key in wellKnownTypeKeys)
            {
                wellKnownTypes.Get(key);
            }

            context.ReportDiagnostic(Diagnostic.Create(
                SuccessDescriptor,
                location: null));
        }
    }
}
