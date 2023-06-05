// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace Microsoft.AspNetCore.Razor.Language.Syntax;

partial internal class MarkupTextLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}

partial internal class MarkupEphemeralTextLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}

partial internal class CSharpStatementLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}

partial internal class CSharpExpressionLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}

partial internal class CSharpEphemeralTextLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}

partial internal class UnclassifiedTextLiteralSyntax
{
    protected override string GetDebuggerDisplay()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} [{1}]",
            base.GetDebuggerDisplay(),
            this.GetContent()
        );
    }
}
