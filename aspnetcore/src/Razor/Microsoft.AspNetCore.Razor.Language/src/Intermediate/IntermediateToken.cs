﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate
{
    public class IntermediateToken : IntermediateNode
    {
        public override IntermediateNodeCollection Children => IntermediateNodeCollection.ReadOnly;

        public virtual string Content { get; set; }

        public bool IsCSharp => Kind == TokenKind.CSharp;

        public bool IsHtml => Kind == TokenKind.Html;

        public TokenKind Kind { get; set; } = TokenKind.Unknown;

        public override void Accept(IntermediateNodeVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitToken(this);
        }

        public override void FormatNode(IntermediateNodeFormatter formatter)
        {
            formatter.WriteContent(Content);

            formatter.WriteProperty(nameof(Content), Content);
        }
    }
}