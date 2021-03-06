// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Resources;
using System.Web.Razor.Test.Framework;
using System.Web.Razor.Text;
using Microsoft.TestCommon;

namespace System.Web.Razor.Test.Parser.CSharp
{
    public class CSharpExplicitExpressionTest : CsHtmlCodeParserTestBase
    {
        [Fact]
        public void ParseBlockShouldOutputZeroLengthCodeSpanIfExplicitExpressionIsEmpty()
        {
            ParseBlockTest("@()",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.EmptyCSharp().AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldOutputZeroLengthCodeSpanIfEOFOccursAfterStartOfExplicitExpression()
        {
            ParseBlockTest("@(",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.EmptyCSharp().AsExpression()
                               ),
                           new RazorError(
                               String.Format(RazorResources.ParseError_Expected_EndOfBlock_Before_EOF,
                                             RazorResources.BlockName_ExplicitExpression,
                                             ")", "("),
                               new SourceLocation(1, 0, 1)));
        }

        [Fact]
        public void ParseBlockShouldAcceptEscapedQuoteInNonVerbatimStrings()
        {
            ParseBlockTest("@(\"\\\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("\"\\\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptEscapedQuoteInVerbatimStrings()
        {
            ParseBlockTest("@(@\"\"\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("@\"\"\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptMultipleRepeatedEscapedQuoteInVerbatimStrings()
        {
            ParseBlockTest("@(@\"\"\"\"\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("@\"\"\"\"\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptMultiLineVerbatimStrings()
        {
            ParseBlockTest(@"@(@""" + Environment.NewLine
                         + @"Foo" + Environment.NewLine
                         + @"Bar" + Environment.NewLine
                         + @"Baz" + Environment.NewLine
                         + @""")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("@\"\r\nFoo\r\nBar\r\nBaz\r\n\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptMultipleEscapedQuotesInNonVerbatimStrings()
        {
            ParseBlockTest("@(\"\\\"hello, world\\\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("\"\\\"hello, world\\\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptMultipleEscapedQuotesInVerbatimStrings()
        {
            ParseBlockTest("@(@\"\"\"hello, world\"\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("@\"\"\"hello, world\"\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptConsecutiveEscapedQuotesInNonVerbatimStrings()
        {
            ParseBlockTest("@(\"\\\"\\\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("\"\\\"\\\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }

        [Fact]
        public void ParseBlockShouldAcceptConsecutiveEscapedQuotesInVerbatimStrings()
        {
            ParseBlockTest("@(@\"\"\"\"\"\")",
                           new ExpressionBlock(
                               Factory.CodeTransition(),
                               Factory.MetaCode("(").Accepts(AcceptedCharacters.None),
                               Factory.Code("@\"\"\"\"\"\"").AsExpression(),
                               Factory.MetaCode(")").Accepts(AcceptedCharacters.None)
                               ));
        }
    }
}
