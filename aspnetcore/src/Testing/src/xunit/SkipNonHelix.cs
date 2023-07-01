using System;

namespace Microsoft.AspNetCore.Testing;

/// <summary>
/// Skip test if running on CI
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SkipNonHelixAttribute : Attribute, ITestCondition
{
    public SkipNonHelixAttribute(string issueUrl = "")
    {
        IssueUrl = issueUrl;
    }

    public string IssueUrl { get; }

    public bool IsMet
    {
        get { return OnHelix(); }
    }

    public string SkipReason
    {
        get { return "This test is skipped if not on Helix"; }
    }

    public static bool OnHelix() => HelixHelper.OnHelix();

    public static string GetTargetHelixQueue() => HelixHelper.GetTargetHelixQueue();
}
