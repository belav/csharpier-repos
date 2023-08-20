// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Utilities;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.LanguageService;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.Snippets;
using Microsoft.CodeAnalysis.Snippets.SnippetProviders;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Snippets
{
    [ExportSnippetProvider(nameof(ISnippetProvider), LanguageNames.CSharp), Shared]
    internal sealed class CSharpPropSnippetProvider : AbstractPropSnippetProvider
    {
        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public CSharpPropSnippetProvider() { }

        protected override async Task<bool> IsValidSnippetLocationAsync(
            Document document,
            int position,
            CancellationToken cancellationToken
        )
        {
            var syntaxTree = await document
                .GetSyntaxTreeAsync(cancellationToken)
                .ConfigureAwait(false);
            Contract.ThrowIfNull(syntaxTree);

            return syntaxTree.IsMemberDeclarationContext(
                position,
                contextOpt: null,
                SyntaxKindSet.AllMemberModifiers,
                SyntaxKindSet.ClassInterfaceStructRecordTypeDeclarations,
                canBePartial: true,
                cancellationToken
            );
        }

        protected override async Task<SyntaxNode> GenerateSnippetSyntaxAsync(
            Document document,
            int position,
            CancellationToken cancellationToken
        )
        {
            var compilation = await document.Project
                .GetRequiredCompilationAsync(cancellationToken)
                .ConfigureAwait(false);
            var semanticModel = await document
                .GetRequiredSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false);
            var generator = SyntaxGenerator.GetGenerator(document);
            var identifierName = NameGenerator.GenerateUniqueName(
                "MyProperty",
                n => semanticModel.LookupSymbols(position, name: n).IsEmpty
            );
            return generator.PropertyDeclaration(
                name: identifierName,
                type: compilation
                    .GetSpecialType(SpecialType.System_Int32)
                    .GenerateTypeSyntax(allowVar: false),
                accessibility: Accessibility.Public
            );
        }

        protected override int GetTargetCaretPosition(
            ISyntaxFactsService syntaxFacts,
            SyntaxNode caretTarget,
            SourceText sourceText
        )
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)caretTarget;
            return propertyDeclaration.AccessorList!.CloseBraceToken.Span.End;
        }

        protected override ImmutableArray<SnippetPlaceholder> GetPlaceHolderLocationsList(
            SyntaxNode node,
            ISyntaxFacts syntaxFacts,
            CancellationToken cancellationToken
        )
        {
            using var _ = ArrayBuilder<SnippetPlaceholder>.GetInstance(out var arrayBuilder);
            var propertyDeclaration = (PropertyDeclarationSyntax)node;
            var identifier = propertyDeclaration.Identifier;
            var type = propertyDeclaration.Type;

            arrayBuilder.Add(
                new SnippetPlaceholder(
                    identifier: type.ToString(),
                    placeholderPositions: ImmutableArray.Create(type.SpanStart)
                )
            );
            arrayBuilder.Add(
                new SnippetPlaceholder(
                    identifier: identifier.ValueText,
                    placeholderPositions: ImmutableArray.Create(identifier.SpanStart)
                )
            );
            return arrayBuilder.ToImmutableArray();
        }
    }
}
