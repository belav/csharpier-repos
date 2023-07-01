using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserAuthorizationTests : AuthorizationTests<Startup, IdentityDbContext>
{
    public IdentityUserAuthorizationTests(ServerFactory<Startup, IdentityDbContext> serverFactory)
        : base(serverFactory) { }
}
