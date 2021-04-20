// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.FunctionalTests.Http2
{
    [OSSkipCondition(OperatingSystems.MacOSX, SkipReason = "Missing SslStream ALPN support: https://github.com/dotnet/runtime/issues/27727")]
    [MinimumOSVersion(OperatingSystems.Windows, WindowsVersions.Win10)]
    public class ShutdownTests : TestApplicationErrorLoggerLoggedTest
    {
        private static X509Certificate2 _x509Certificate2 = TestResources.GetTestCertificate();

        private HttpClient Client { get; set; }
        private List<Http2Frame> ReceivedFrames { get; } = new List<Http2Frame>();

        public ShutdownTests()
        {
            Client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            {
                DefaultRequestVersion = new Version(2, 0),
            };
        }

        [CollectDump]
        [ConditionalFact]
        public async Task GracefulShutdownWaitsForRequestsToFinish()
        {
            var requestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestUnblocked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestStopping = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var mockKestrelTrace = new Mock<KestrelTrace>(Logger)
            {
                CallBase = true
            };
            mockKestrelTrace
                .Setup(m => m.Http2ConnectionClosing(It.IsAny<string>()))
                .Callback(() => requestStopping.SetResult());

            var testContext = new TestServiceContext(LoggerFactory, mockKestrelTrace.Object);

            testContext.InitializeHeartbeat();

            await using (var server = new TestServer(async context =>
            {
                requestStarted.SetResult();
                await requestUnblocked.Task.DefaultTimeout();
                await context.Response.WriteAsync("hello world " + context.Request.Protocol);
            },
            testContext,
            kestrelOptions =>
            {
                kestrelOptions.Listen(IPAddress.Loopback, 0, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                    listenOptions.UseHttps(_x509Certificate2);
                });
            }))
            {
                var requestTask = Client.GetStringAsync($"https://localhost:{server.Port}/");
                Assert.False(requestTask.IsCompleted);

                await requestStarted.Task.DefaultTimeout();

                var stopTask = server.StopAsync();

                await requestStopping.Task.DefaultTimeout();

                // Unblock the request
                requestUnblocked.SetResult();

                Assert.Equal("hello world HTTP/2", await requestTask);
                await stopTask.DefaultTimeout();
            }

            Assert.Contains(LogMessages, m => m.Message.Contains("Request finished "));
            Assert.Contains(LogMessages, m => m.Message.Contains("is closing."));
            Assert.Contains(LogMessages, m => m.Message.Contains("is closed. The last processed stream ID was 1."));
        }

        [ConditionalFact]
        public async Task GracefulTurnsAbortiveIfRequestsDoNotFinish()
        {
            var requestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestUnblocked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var memoryPoolFactory = new DiagnosticMemoryPoolFactory(allowLateReturn: true);

            var testContext = new TestServiceContext(LoggerFactory)
            {
                MemoryPoolFactory = memoryPoolFactory.Create
            };

            ThrowOnUngracefulShutdown = false;

            // Abortive shutdown leaves one request hanging
            await using (var server = new TestServer(async context =>
            {
                requestStarted.SetResult();
                await requestUnblocked.Task.DefaultTimeout();
                await context.Response.WriteAsync("hello world " + context.Request.Protocol);
            },
            testContext,
            kestrelOptions =>
            {
                kestrelOptions.Listen(IPAddress.Loopback, 0, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                    listenOptions.UseHttps(_x509Certificate2);
                });
            },
            _ => { }))
            {
                var requestTask = Client.GetStringAsync($"https://localhost:{server.Port}/");
                Assert.False(requestTask.IsCompleted);
                await requestStarted.Task.DefaultTimeout();

                // Wait for the graceful shutdown log before canceling the token passed to StopAsync and triggering an ungraceful shutdown.
                // Otherwise, graceful shutdown might be skipped causing there to be no corresponding log. https://github.com/dotnet/aspnetcore/issues/6556
                var closingMessageTask = WaitForLogMessage(m => m.Message.Contains("is closing.")).DefaultTimeout();

                var cts = new CancellationTokenSource();
                var stopServerTask = server.StopAsync(cts.Token).DefaultTimeout();

                await closingMessageTask;

                var closedMessageTask = WaitForLogMessage(m => m.Message.Contains("is closed. The last processed stream ID was 1.")).DefaultTimeout();
                cts.Cancel();

                // Wait for "is closed" message as this is logged from a different thread and aborting
                // can timeout and return from server.StopAsync before this is logged.
                await closedMessageTask;
                requestUnblocked.SetResult();
                await stopServerTask;
            }

            Assert.Contains(LogMessages, m => m.Message.Contains("is closing."));
            Assert.Contains(LogMessages, m => m.Message.Contains("is closed. The last processed stream ID was 1."));
            Assert.Contains(LogMessages, m => m.Message.Contains("Some connections failed to close gracefully during server shutdown."));
            Assert.DoesNotContain(LogMessages, m => m.Message.Contains("Request finished in"));

            await memoryPoolFactory.WhenAllBlocksReturned(TestConstants.DefaultTimeout);
        }
    }
}
