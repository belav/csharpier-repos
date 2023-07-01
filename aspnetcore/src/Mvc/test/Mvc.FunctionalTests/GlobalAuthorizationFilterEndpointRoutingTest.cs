using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class GlobalAuthorizationFilterEndpointRoutingTest
    : GlobalAuthorizationFilterTestBase,
        IClassFixture<MvcTestFixture<SecurityWebSite.StartupWithGlobalDenyAnonymousFilter>>
{
    public GlobalAuthorizationFilterEndpointRoutingTest(
        MvcTestFixture<SecurityWebSite.StartupWithGlobalDenyAnonymousFilter> fixture
    )
    {
        Factory =
            fixture.Factories.FirstOrDefault()
            ?? fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
        Client = Factory.CreateDefaultClient();
    }

    private static void ConfigureWebHostBuilder(IWebHostBuilder builder) =>
        builder.UseStartup<SecurityWebSite.StartupWithGlobalDenyAnonymousFilter>();

    public WebApplicationFactory<SecurityWebSite.StartupWithGlobalDenyAnonymousFilter> Factory { get; }
}
