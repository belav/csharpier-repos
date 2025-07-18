// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.AspNetCore.Mvc.Api.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public class ApiActionsDoNotRequireExplicitModelValidationCheckCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            ApiDiagnosticDescriptors.API1003_ApiActionsDoNotRequireExplicitModelValidationCheck.Id
        );

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (context.Diagnostics.Length != 1)
        {
            return Task.CompletedTask;
        }

        var diagnostic = context.Diagnostics[0];
        context.RegisterCodeFix(new MyCodeAction(context.Document, context.Span), diagnostic);
        return Task.CompletedTask;
    }

    private sealed class MyCodeAction : CodeAction
    {
        private readonly Document _document;
        private readonly TextSpan _ifBlockSpan;

        public MyCodeAction(Document document, TextSpan ifBlockSpan)
        {
            _document = document;
            _ifBlockSpan = ifBlockSpan;
        }

        public override string EquivalenceKey => Title;

        public override string Title => "Remove ModelState.IsValid check";

        protected override async Task<Document> GetChangedDocumentAsync(
            CancellationToken cancellationToken
        )
        {
            var rootNode = await _document
                .GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);
            var editor = await DocumentEditor
                .CreateAsync(_document, cancellationToken)
                .ConfigureAwait(false);

            var ifBlockSyntax = rootNode!.FindNode(_ifBlockSpan);
            editor.RemoveNode(ifBlockSyntax);

            return editor.GetChangedDocument();
        }
    }
}
