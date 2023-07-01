using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserLoginTests : LoginTests<Startup, IdentityDbContext>
{
    public IdentityUserLoginTests(ServerFactory<Startup, IdentityDbContext> serverFactory)
        : base(serverFactory) { }
}
