using System.Net.Http;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class TempDataInSessionTest
    : TempDataTestBase,
        IClassFixture<MvcTestFixture<BasicWebSite.StartupWithSessionTempDataProvider>>
{
    public TempDataInSessionTest(
        MvcTestFixture<BasicWebSite.StartupWithSessionTempDataProvider> fixture
    )
    {
        Client = fixture.CreateDefaultClient();
    }

    protected override HttpClient Client { get; }
}
