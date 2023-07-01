using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.AspNetCore.Analyzers;

internal sealed class TestCompilation
{
    public static Compilation Create(string source)
    {
        return CSharpCompilation.Create(
            "Test",
            new[] { CSharpSyntaxTree.ParseText(source) },
            TestReferences.MetadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }
}
