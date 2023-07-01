using System.Net;
using System.Net.Http;

namespace Microsoft.AspNetCore.Diagnostics.FunctionalTests;

public class MiddlewareAnalysisSampleTest
    : IClassFixture<TestFixture<MiddlewareAnaysisSample.Startup>>
{
    public MiddlewareAnalysisSampleTest(TestFixture<MiddlewareAnaysisSample.Startup> fixture)
    {
        Client = fixture.Client;
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task MiddlewareAnalysisPage_ShowsAnalysis()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
