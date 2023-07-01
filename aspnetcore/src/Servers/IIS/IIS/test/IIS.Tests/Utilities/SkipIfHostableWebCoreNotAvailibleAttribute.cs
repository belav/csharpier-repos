using System;
using System.IO;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipIfHostableWebCoreNotAvailableAttribute : Attribute, ITestCondition
{
    public bool IsMet { get; } = File.Exists(TestServer.HostableWebCoreLocation);

    public string SkipReason { get; } =
        $"Hostable Web Core not available, {TestServer.HostableWebCoreLocation} not found.";
}
