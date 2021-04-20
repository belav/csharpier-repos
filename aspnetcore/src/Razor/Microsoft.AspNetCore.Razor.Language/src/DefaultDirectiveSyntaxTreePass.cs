﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Razor.Language.Syntax;

namespace Microsoft.AspNetCore.Razor.Language
{
    internal class DefaultDirectiveSyntaxTreePass : RazorEngineFeatureBase, IRazorSyntaxTreePass
    {
        public int Order => 75;

        public RazorSyntaxTree Execute(RazorCodeDocument codeDocument, RazorSyntaxTree syntaxTree)
        {
            if (codeDocument == null)
            {
                throw new ArgumentNullException(nameof(codeDocument));
            }

            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }
            
            var sectionVerifier = new NestedSectionVerifier(syntaxTree);
            return sectionVerifier.Verify();
        }

        private class NestedSectionVerifier : SyntaxRewriter
        {
            private int _nestedLevel;
            private RazorSyntaxTree _syntaxTree;
            private List<RazorDiagnostic> _diagnostics;

            public NestedSectionVerifier(RazorSyntaxTree syntaxTree)
            {
                _syntaxTree = syntaxTree;
                _diagnostics = new List<RazorDiagnostic>(syntaxTree.Diagnostics);
            }

            public RazorSyntaxTree Verify()
            {
                var root = Visit(_syntaxTree.Root);
                var rewrittenTree = new DefaultRazorSyntaxTree(root, _syntaxTree.Source, _diagnostics, _syntaxTree.Options);
                return rewrittenTree;
            }

            public override SyntaxNode Visit(SyntaxNode node)
            {
                try
                {
                    return base.Visit(node);
                }
                catch (InsufficientExecutionStackException)
                {
                    // We're very close to reaching the stack limit. Let's not go any deeper.
                    // It's okay to not show nested section errors in deeply nested cases instead of crashing.
                    _diagnostics.Add(RazorDiagnosticFactory.CreateRewriter_InsufficientStack(SourceSpan.Undefined));

                    return node;
                }
            }

            public override SyntaxNode VisitRazorDirective(RazorDirectiveSyntax node)
            {
                if (node.DirectiveDescriptor?.Directive != SectionDirective.Directive.Directive)
                {
                    // We only want to track the nesting of section directives.
                    return base.VisitRazorDirective(node);
                }

                _nestedLevel++;
                var result = (RazorDirectiveSyntax)base.VisitRazorDirective(node);

                if (_nestedLevel > 1)
                {
                    var directiveStart = node.Transition.GetSourceLocation(_syntaxTree.Source);
                    var errorLength = /* @ */ 1 + SectionDirective.Directive.Directive.Length;
                    var error = RazorDiagnosticFactory.CreateParsing_SectionsCannotBeNested(new SourceSpan(directiveStart, errorLength));
                    result = result.AppendDiagnostic(error);
                }

                _nestedLevel--;

                return result;
            }
        }
    }
}
