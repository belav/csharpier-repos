using System.Net;
using System.Net.Http;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class MvcSandboxTest : IClassFixture<MvcTestFixture<MvcSandbox.Startup>>
{
    public MvcSandboxTest(MvcTestFixture<MvcSandbox.Startup> fixture)
    {
        Client = fixture.CreateDefaultClient();
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task Home_Pages_ReturnSuccess()
    {
        // Arrange & Act
        var response = await Client.GetAsync("http://localhost");

        // Assert
        await response.AssertStatusCodeAsync(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RazorPages_ReturnSuccess()
    {
        // Arrange & Act
        var response = await Client.GetAsync("http://localhost/PagesHome");

        // Assert
        await response.AssertStatusCodeAsync(HttpStatusCode.OK);
    }
}
