// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace Microsoft.AspNetCore.Rewrite.PatternSegments;

internal sealed class RemotePortSegment : PatternSegment
{
    public override string? Evaluate(RewriteContext context, BackReferenceCollection? ruleBackReferences, BackReferenceCollection? conditionBackReferences)
    {
        return context.HttpContext.Connection.RemotePort.ToString(CultureInfo.InvariantCulture);
    }
}
