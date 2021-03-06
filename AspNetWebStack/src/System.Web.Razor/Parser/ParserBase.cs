// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Resources;
using System.Web.Razor.Text;

namespace System.Web.Razor.Parser
{
    public abstract class ParserBase
    {
        private ParserContext _context;

        public virtual ParserContext Context
        {
            get { return _context; }
            set
            {
                Debug.Assert(_context == null, "Context has already been set for this parser!");
                _context = value;
                _context.AssertOnOwnerTask();
            }
        }

        public virtual bool IsMarkupParser
        {
            get { return false; }
        }

        protected abstract ParserBase OtherParser { get; }

        public abstract void BuildSpan(SpanBuilder span, SourceLocation start, string content);

        public abstract void ParseBlock();

        // Markup Parsers need the ParseDocument and ParseSection methods since the markup parser is the first parser to hit the document 
        // and the logic may be different than the ParseBlock method.
        public virtual void ParseDocument()
        {
            Debug.Assert(IsMarkupParser);
            throw new NotSupportedException(RazorResources.ParserIsNotAMarkupParser);
        }

        public virtual void ParseSection(Tuple<string, string> nestingSequences, bool caseSensitive)
        {
            Debug.Assert(IsMarkupParser);
            throw new NotSupportedException(RazorResources.ParserIsNotAMarkupParser);
        }
    }
}
