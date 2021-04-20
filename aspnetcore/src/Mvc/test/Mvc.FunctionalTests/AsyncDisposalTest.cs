// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BasicWebSite.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class AsyncDisposalTest : IClassFixture<MvcTestFixture<BasicWebSite.Startup>>
    {
        public AsyncDisposalTest(MvcTestFixture<BasicWebSite.Startup> fixture)
        {
            Factory = fixture.Factories.FirstOrDefault() ?? fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
            Client = Factory.CreateDefaultClient();
        }

        private static void ConfigureWebHostBuilder(IWebHostBuilder builder) =>
            builder.UseStartup<BasicWebSite.Startup>()
            .ConfigureServices(s => s.AddSingleton<ControllerTestDisposeAsync>());

        public WebApplicationFactory<BasicWebSite.Startup> Factory { get; }

        public HttpClient Client { get; }

        [Fact]
        public async Task CanDisposeAsyncController()
        {
            // Arrange & Act
            var sink = Factory.Services.GetRequiredService<ControllerTestDisposeAsync>();
            var response = await Client.GetAsync("http://localhost/Disposal/DisposeMode/Async(false)/Throws(false)");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(sink.DisposeAsyncInvoked);
        }

        [Fact]
        public async Task HandlesAsyncExceptionsDuringAsyncDisposal()
        {
            // Arrange & Act
            var sink = Factory.Services.GetRequiredService<ControllerTestDisposeAsync>();
            var response = await Client.GetAsync("http://localhost/Disposal/DisposeMode/Async(true)/Throws(true)");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.True(sink.DisposeAsyncInvoked);
        }

        [Fact]
        public async Task HandlesSyncExceptionsDuringAsyncDisposal()
        {
            // Arrange & Act
            var sink = Factory.Services.GetRequiredService<ControllerTestDisposeAsync>();
            var response = await Client.GetAsync("http://localhost/Disposal/DisposeMode/Async(false)/Throws(true)");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.True(sink.DisposeAsyncInvoked);
        }
    }
}
