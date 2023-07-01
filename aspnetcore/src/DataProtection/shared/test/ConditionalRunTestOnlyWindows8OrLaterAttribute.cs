using System;
using Microsoft.AspNetCore.Cryptography.Cng;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.DataProtection.Test.Shared;

public class ConditionalRunTestOnlyOnWindows8OrLaterAttribute : Attribute, ITestCondition
{
    public bool IsMet => OSVersionUtil.IsWindows8OrLater();

    public string SkipReason { get; } = "Test requires Windows 8 / Windows Server 2012 or higher.";
}
