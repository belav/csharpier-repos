using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class ApplicationUserLoginTests : LoginTests<ApplicationUserStartup, ApplicationDbContext>
{
    public ApplicationUserLoginTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory) { }
}
