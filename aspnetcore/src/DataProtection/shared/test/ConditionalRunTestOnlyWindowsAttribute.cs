using System;
using Microsoft.AspNetCore.Cryptography.Cng;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.DataProtection.Test.Shared;

public class ConditionalRunTestOnlyOnWindowsAttribute : Attribute, ITestCondition
{
    public bool IsMet => OSVersionUtil.IsWindows();

    public string SkipReason { get; } =
        "Test requires Windows 7 / Windows Server 2008 R2 or higher.";
}
