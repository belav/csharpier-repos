using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IntegrationTesting;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly | AttributeTargets.Class)]
public sealed partial class SkipIfIISExpressSchemaMissingInProcessAttribute
    : Attribute,
        ITestCondition
{
    public bool IsMet => IISExpressAncmSchema.SupportsInProcessHosting;

    public string SkipReason => IISExpressAncmSchema.SkipReason;
}
