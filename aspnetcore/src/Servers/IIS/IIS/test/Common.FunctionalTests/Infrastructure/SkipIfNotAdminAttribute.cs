using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipIfNotAdminAttribute : Attribute, ITestCondition
{
    public bool IsMet
    {
        get
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator)
                || SkipInVSTSAttribute.RunningInVSTS;
        }
    }

    public string SkipReason => "The current process is not running as admin.";
}
