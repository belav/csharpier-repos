// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Testing;
using Templates.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test;

public class GrpcTemplateTest : LoggedTest
{
    public GrpcTemplateTest(ProjectFactoryFixture projectFactory)
    {
        ProjectFactory = projectFactory;
    }

    public ProjectFactoryFixture ProjectFactory { get; }
    private ITestOutputHelper _output;
    public ITestOutputHelper Output
    {
        get
        {
            if (_output == null)
            {
                _output = new TestOutputLogger(Logger);
            }
            return _output;
        }
    }

    [ConditionalTheory]
    [SkipOnHelix("Not supported queues", Queues = "All.OSX;" + HelixConstants.Windows10Arm64 + HelixConstants.DebianArm64)]
    [SkipOnAlpine("https://github.com/grpc/grpc/issues/18338")]
    [InlineData(true)]
    [InlineData(false)]
    [QuarantinedTest("https://github.com/dotnet/aspnetcore/issues/41716")]
    public async Task GrpcTemplate(bool useProgramMain)
    {
        var project = await ProjectFactory.CreateProject(Output);

        var args = useProgramMain ? new[] { ArgConstants.UseProgramMain } : null;
        await project.RunDotNetNewAsync("grpc", args: args);

        var expectedLaunchProfileNames = new[] { "http", "https" };
        await project.VerifyLaunchSettings(expectedLaunchProfileNames);

        await project.RunDotNetPublishAsync();

        await project.RunDotNetBuildAsync();

        var isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        var isWindowsOld = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version < new Version(6, 2);
        var unsupported = isOsx || isWindowsOld;

        using (var serverProcess = project.StartBuiltProjectAsync(hasListeningUri: !unsupported, logger: Logger))
        {
            // These templates are HTTPS + HTTP/2 only which is not supported on some platforms.
            if (isWindowsOld)
            {
                serverProcess.Process.WaitForExit(assertSuccess: false);
                Assert.True(serverProcess.Process.HasExited, "built");
                Assert.Contains("System.NotSupportedException: HTTP/2 over TLS is not supported on Windows 7 due to missing ALPN support.",
                    ErrorMessages.GetFailedProcessMessageOrEmpty("Run built service", project, serverProcess.Process));
            }
            else
            {
                Assert.False(
                    serverProcess.Process.HasExited,
                    ErrorMessages.GetFailedProcessMessageOrEmpty("Run built service", project, serverProcess.Process));
            }
        }

        using (var aspNetProcess = project.StartPublishedProjectAsync(hasListeningUri: !unsupported))
        {
            // These templates are HTTPS + HTTP/2 only which is not supported on some platforms.
            if (isWindowsOld)
            {
                aspNetProcess.Process.WaitForExit(assertSuccess: false);
                Assert.True(aspNetProcess.Process.HasExited, "published");
                Assert.Contains("System.NotSupportedException: HTTP/2 over TLS is not supported on Windows 7 due to missing ALPN support.",
                    ErrorMessages.GetFailedProcessMessageOrEmpty("Run published service", project, aspNetProcess.Process));
            }
            else
            {
                Assert.False(
                    aspNetProcess.Process.HasExited,
                    ErrorMessages.GetFailedProcessMessageOrEmpty("Run published service", project, aspNetProcess.Process));
            }
        }
    }
}
