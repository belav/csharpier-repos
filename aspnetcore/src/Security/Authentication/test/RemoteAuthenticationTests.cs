// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Authentication
{
    public abstract class RemoteAuthenticationTests<TOptions> : SharedAuthenticationTests<TOptions> where TOptions : RemoteAuthenticationOptions
    {
        protected override string DisplayName => DefaultScheme;

        private Task<IHost> CreateHost(Action<TOptions> configureOptions, Func<HttpContext, Task> testpath = null, bool isDefault = true)
            => CreateHostWithServices(s =>
            {
                var builder = s.AddAuthentication();
                if (isDefault)
                {
                    s.Configure<AuthenticationOptions>(o => o.DefaultScheme = DefaultScheme);
                }
                RegisterAuth(builder, configureOptions);
                s.AddSingleton<ISystemClock>(Clock);
            }, testpath);


        protected virtual async Task<IHost> CreateHostWithServices(Action<IServiceCollection> configureServices, Func<HttpContext, Task> testpath = null)
        {
            var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                    webHostBuilder.UseTestServer()
                        .Configure(app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                if (testpath != null)
                                {
                                    await testpath(context);
                                }
                                await next();
                            });
                        })
                        .ConfigureServices(configureServices))
                .Build();
            await host.StartAsync();
            return host;
        }

        protected abstract void ConfigureDefaults(TOptions o);

        [Fact]
        public async Task VerifySignInSchemeCannotBeSetToSelf()
        {
            using var host = await CreateHost(
                o => 
                {
                    ConfigureDefaults(o);
                    o.SignInScheme = DefaultScheme;
                },
                context => context.ChallengeAsync(DefaultScheme));
            using var server = host.GetTestServer();
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => server.SendAsync("https://example.com/challenge"));
            Assert.Contains("cannot be set to itself", error.Message);
        }

        [Fact]
        public async Task VerifySignInSchemeCannotBeSetToSelfUsingDefaultScheme()
        {

            using var host = await CreateHost(
                o => o.SignInScheme = null,
                context => context.ChallengeAsync(DefaultScheme),
                isDefault: true);
            using var server = host.GetTestServer();
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => server.SendAsync("https://example.com/challenge"));
            Assert.Contains("cannot be set to itself", error.Message);
        }

        [Fact]
        public async Task VerifySignInSchemeCannotBeSetToSelfUsingDefaultSignInScheme()
        {
            using var host = await CreateHostWithServices(
                services =>
                {
                    var builder = services.AddAuthentication(o => o.DefaultSignInScheme = DefaultScheme);
                    RegisterAuth(builder, o => o.SignInScheme = null);
                },
                context => context.ChallengeAsync(DefaultScheme));
            using var server = host.GetTestServer();
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => server.SendAsync("https://example.com/challenge"));
            Assert.Contains("cannot be set to itself", error.Message);
        }
    }
}
