using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Bootstrap4Tests;

public class Bootstrap4RegistrationTests
    : RegistrationTests<ApplicationUserStartup, ApplicationDbContext>
{
    public Bootstrap4RegistrationTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory)
    {
        serverFactory.BootstrapFrameworkVersion = "V4";
    }
}
