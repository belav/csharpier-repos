// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Razor.Text;

namespace System.Web.Razor.Tokenizer.Symbols
{
    public interface ISymbol
    {
        SourceLocation Start { get; }
        string Content { get; }

        void OffsetStart(SourceLocation documentStart);
        void ChangeStart(SourceLocation newStart);
    }
}
