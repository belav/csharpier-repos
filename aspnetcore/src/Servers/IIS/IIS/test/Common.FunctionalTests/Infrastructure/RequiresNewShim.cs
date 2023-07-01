using System;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiresNewShimAttribute : Attribute, ITestCondition
{
    public bool IsMet => DeployerSelector.HasNewShim;

    public string SkipReason =>
        "Test verifies new behavior in the aspnetcorev2.dll that isn't supported in earlier versions.";
}
