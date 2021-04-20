// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IIS.FunctionalTests.Utilities;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Server.IntegrationTesting.IIS;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests
{
    [Collection(PublishedSitesCollection.Name)]
    public class LoggingTests : IISFunctionalTestBase
    {
        public LoggingTests(PublishedSitesFixture fixture) : base(fixture)
        {
        }

        public static TestMatrix TestVariants
            => TestMatrix.ForServers(DeployerSelector.ServerType)
                .WithTfms(Tfm.Default)
                .WithApplicationTypes(ApplicationType.Portable)
                .WithAllHostingModels();

        public static TestMatrix InprocessTestVariants
            => TestMatrix.ForServers(DeployerSelector.ServerType)
                .WithTfms(Tfm.Default)
                .WithApplicationTypes(ApplicationType.Portable)
                .WithHostingModels(HostingModel.InProcess);

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(TestVariants))]
        public async Task CheckStdoutLoggingToFile(TestVariant variant)
        {
            await CheckStdoutToFile(variant, "ConsoleWrite");
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(TestVariants))]
        public async Task CheckStdoutErrLoggingToFile(TestVariant variant)
        {
            await CheckStdoutToFile(variant, "ConsoleErrorWrite");
        }

        private async Task CheckStdoutToFile(TestVariant variant, string path)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
            deploymentParameters.EnableLogging(LogFolderPath);

            var deploymentResult = await DeployAsync(deploymentParameters);

            await Helpers.AssertStarts(deploymentResult, path);

            StopServer();

            var contents = Helpers.ReadAllTextFromFile(Helpers.GetExpectedLogName(deploymentResult, LogFolderPath), Logger);

            Assert.Contains("TEST MESSAGE", contents);
            Assert.DoesNotContain("\r\n\r\n", contents);
            Assert.Contains("\r\n", contents);
        }

        // Move to separate file
        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(TestVariants))]
        public async Task InvalidFilePathForLogs_ServerStillRuns(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);

            deploymentParameters.WebConfigActionList.Add(
                WebConfigHelpers.AddOrModifyAspNetCoreSection("stdoutLogEnabled", "true"));
            deploymentParameters.WebConfigActionList.Add(
                WebConfigHelpers.AddOrModifyAspNetCoreSection("stdoutLogFile", Path.Combine("Q:", "std")));

            var deploymentResult = await DeployAsync(deploymentParameters);

            await Helpers.AssertStarts(deploymentResult, "HelloWorld");

            StopServer();
            if (variant.HostingModel == HostingModel.InProcess)
            {
                // Error is getting logged twice, from shim and handler
                EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.CouldNotStartStdoutFileRedirection("Q:\\std", deploymentResult), Logger, allowMultiple: true);
            }
        }

        [ConditionalTheory]
        [MemberData(nameof(InprocessTestVariants))]
        [RequiresNewShim]
        public async Task StartupMessagesAreLoggedIntoDebugLogFile(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
            deploymentParameters.HandlerSettings["debugLevel"] = "file";
            deploymentParameters.HandlerSettings["debugFile"] = "subdirectory\\debug.txt";

            var deploymentResult = await DeployAsync(deploymentParameters);

            await deploymentResult.HttpClient.GetAsync("/");

            AssertLogs(Path.Combine(deploymentResult.ContentRoot, "subdirectory", "debug.txt"));
        }

        [ConditionalTheory]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task StartupMessagesAreLoggedIntoDefaultDebugLogFile(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
            deploymentParameters.HandlerSettings["debugLevel"] = "file";

            var deploymentResult = await DeployAsync(deploymentParameters);

            await deploymentResult.HttpClient.GetAsync("/");

            AssertLogs(Path.Combine(deploymentResult.ContentRoot, "aspnetcore-debug.log"));
        }

        [ConditionalTheory]
        [RequiresIIS(IISCapability.PoolEnvironmentVariables)]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task StartupMessagesAreLoggedIntoDefaultDebugLogFileWhenEnabledWithEnvVar(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
            deploymentParameters.EnvironmentVariables["ASPNETCORE_MODULE_DEBUG"] = "file";
            // Add empty debugFile handler setting to prevent IIS deployer from overriding debug settings
            deploymentParameters.HandlerSettings["debugFile"] = "";
            var deploymentResult = await DeployAsync(deploymentParameters);

            await deploymentResult.HttpClient.GetAsync("/");

            AssertLogs(Path.Combine(deploymentResult.ContentRoot, "aspnetcore-debug.log"));
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [RequiresIIS(IISCapability.PoolEnvironmentVariables)]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task StartupMessagesLogFileSwitchedWhenLogFilePresentInWebConfig(TestVariant variant)
        {
            var firstTempFile = Path.GetTempFileName();
            var secondTempFile = Path.GetTempFileName();

            try
            {
                var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
                deploymentParameters.EnvironmentVariables["ASPNETCORE_MODULE_DEBUG_FILE"] = firstTempFile;
                deploymentParameters.AddDebugLogToWebConfig(secondTempFile);

                var deploymentResult = await DeployAsync(deploymentParameters);

                var response = await deploymentResult.HttpClient.GetAsync("/");

                StopServer();
                var logContents = Helpers.ReadAllTextFromFile(firstTempFile, Logger);

                Assert.Contains("Switching debug log files to", logContents);

                AssertLogs(secondTempFile);
            }
            finally
            {
                File.Delete(firstTempFile);
                File.Delete(secondTempFile);
            }
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task DebugLogsAreWrittenToEventLog(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);
            deploymentParameters.HandlerSettings["debugLevel"] = "file,eventlog";
            var deploymentResult = await StartAsync(deploymentParameters);
            StopServer();
            EventLogHelpers.VerifyEventLogEvent(deploymentResult, @"\[aspnetcorev2.dll\] Initializing logs for .*?Description: IIS ASP.NET Core Module V2", Logger);
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task CheckUTF8File(TestVariant variant)
        {
            var path = "CheckConsoleFunctions";

            var deploymentParameters = Fixture.GetBaseDeploymentParameters(Fixture.InProcessTestSite, variant.HostingModel);
            deploymentParameters.TransformArguments((a, _) => $"{a} {path}"); // For standalone this will need to remove space

            var logFolderPath = LogFolderPath + "\\彡⾔";
            deploymentParameters.EnableLogging(logFolderPath);

            var deploymentResult = await DeployAsync(deploymentParameters);

            var response = await deploymentResult.HttpClient.GetAsync(path);

            Assert.False(response.IsSuccessStatusCode);

            StopServer();

            var contents = Helpers.ReadAllTextFromFile(Helpers.GetExpectedLogName(deploymentResult, logFolderPath), Logger);
            Assert.Contains("彡⾔", contents);
            EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.InProcessThreadExitStdOut(deploymentResult, "12", "(.*)彡⾔(.*)"), Logger);
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [MemberData(nameof(InprocessTestVariants))]
        public async Task OnlyOneFileCreatedWithProcessStartTime(TestVariant variant)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(variant);

            deploymentParameters.EnableLogging(LogFolderPath);

            var deploymentResult = await DeployAsync(deploymentParameters);
            await Helpers.AssertStarts(deploymentResult, "ConsoleWrite");

            StopServer();

            Assert.Single(Directory.GetFiles(LogFolderPath), Helpers.GetExpectedLogName(deploymentResult, LogFolderPath));
        }

        [ConditionalFact]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        public async Task CaptureLogsForOutOfProcessWhenProcessFailsToStart()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.OutOfProcess);
            deploymentParameters.TransformArguments((a, _) => $"{a} ConsoleWriteSingle");
            var deploymentResult = await DeployAsync(deploymentParameters);

            var response = await deploymentResult.HttpClient.GetAsync("Test");

            StopServer();

            EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.OutOfProcessFailedToStart(deploymentResult, "Wow!"), Logger);
        }

        [ConditionalFact]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [RequiresNewShim]
        public async Task DisableRedirectionNoLogs()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.OutOfProcess);
            deploymentParameters.HandlerSettings["enableOutOfProcessConsoleRedirection"] = "false";
            deploymentParameters.TransformArguments((a, _) => $"{a} ConsoleWriteSingle");
            var deploymentResult = await DeployAsync(deploymentParameters);

            var response = await deploymentResult.HttpClient.GetAsync("Test");

            StopServer();

            EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.OutOfProcessFailedToStart(deploymentResult, ""), Logger);
        }

        [ConditionalFact]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        public async Task CaptureLogsForOutOfProcessWhenProcessFailsToStart30KbMax()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.OutOfProcess);
            deploymentParameters.TransformArguments((a, _) => $"{a} ConsoleWrite30Kb");
            var deploymentResult = await DeployAsync(deploymentParameters);

            var response = await deploymentResult.HttpClient.GetAsync("Test");

            StopServer();

            EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.OutOfProcessFailedToStart(deploymentResult, new string('a', 30000)), Logger);
        }

        [ConditionalTheory]
        [MaximumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10_20H2, SkipReason = "Shutdown hangs https://github.com/dotnet/aspnetcore/issues/25107")]
        [InlineData("ConsoleErrorWriteStartServer")]
        [InlineData("ConsoleWriteStartServer")]
        public async Task CheckStdoutLoggingToPipeWithFirstWrite(string path)
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters();

            var firstWriteString = "TEST MESSAGE";

            deploymentParameters.TransformArguments((a, _) => $"{a} {path}");

            var deploymentResult = await DeployAsync(deploymentParameters);

            await Helpers.AssertStarts(deploymentResult);

            StopServer();

            if (deploymentParameters.ServerType == ServerType.IISExpress)
            {
                // We can't read stdout logs from IIS as they aren't redirected.
                Assert.Contains(TestSink.Writes, context => context.Message.Contains(firstWriteString));
            }
        }

        private static string ReadLogs(string logPath)
        {
            using (var stream = File.Open(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static void AssertLogs(string logPath)
        {
            var logContents = ReadLogs(logPath);
            Assert.Contains("[aspnetcorev2.dll]", logContents);
            Assert.True(logContents.Contains("[aspnetcorev2_inprocess.dll]") || logContents.Contains("[aspnetcorev2_outofprocess.dll]"));
            Assert.Contains("Description: IIS ASP.NET Core Module V2. Commit:", logContents);
            Assert.Contains("Description: IIS ASP.NET Core Module V2 Request Handler. Commit:", logContents);
        }
    }
}
