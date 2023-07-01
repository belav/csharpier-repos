using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class ApplicationUserAuthorizationTests
    : AuthorizationTests<ApplicationUserStartup, ApplicationDbContext>
{
    public ApplicationUserAuthorizationTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory) { }
}
