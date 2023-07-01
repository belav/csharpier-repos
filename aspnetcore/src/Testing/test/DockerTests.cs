using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Testing;

public class DockerTests
{
    [ConditionalFact]
    [DockerOnly]
    [Trait("Docker", "true")]
    public void DoesNotRunOnWindows()
    {
        Assert.False(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
    }
}
