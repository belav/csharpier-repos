// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Razor.Resources;
using System.Web.Razor.Text;
using System.Web.Razor.Tokenizer.Symbols;

namespace System.Web.Razor.Tokenizer
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "All generic parameters are required")]
    public class TokenizerView<TTokenizer, TSymbol, TSymbolType>
        where TTokenizer : Tokenizer<TSymbol, TSymbolType>
        where TSymbol : SymbolBase<TSymbolType>
    {
        public TokenizerView(TTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public TTokenizer Tokenizer { get; private set; }
        public bool EndOfFile { get; private set; }
        public TSymbol Current { get; private set; }

        public ITextDocument Source
        {
            get { return Tokenizer.Source; }
        }

        public bool Next()
        {
            Current = Tokenizer.NextSymbol();
            EndOfFile = (Current == null);
            return !EndOfFile;
        }

        public void PutBack(TSymbol symbol)
        {
            Debug.Assert(Source.Position == symbol.Start.AbsoluteIndex + symbol.Content.Length);
            if (Source.Position != symbol.Start.AbsoluteIndex + symbol.Content.Length)
            {
                // We've already passed this symbol
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture,
                                  RazorResources.TokenizerView_CannotPutBack,
                                  symbol.Start.AbsoluteIndex + symbol.Content.Length,
                                  Source.Position));
            }
            Source.Position -= symbol.Content.Length;
            Current = null;
            EndOfFile = Source.Position >= Source.Length;
            Tokenizer.Reset();
        }
    }
}
