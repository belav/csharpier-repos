using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Microsoft.AspNetCore.Mvc.Api.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public class AddResponseTypeAttributeCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            ApiDiagnosticDescriptors.API1000_ActionReturnsUndocumentedStatusCode.Id,
            ApiDiagnosticDescriptors.API1001_ActionReturnsUndocumentedSuccessResult.Id
        );

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var codeFix = new AddResponseTypeAttributeCodeFixAction(context.Document, diagnostic);

        context.RegisterCodeFix(codeFix, diagnostic);
        return Task.CompletedTask;
    }
}
