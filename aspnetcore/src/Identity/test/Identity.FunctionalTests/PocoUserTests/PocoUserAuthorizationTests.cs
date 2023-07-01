using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class PocoUserAuthorizationTests : AuthorizationTests<PocoUserStartup, IdentityDbContext>
{
    public PocoUserAuthorizationTests(
        ServerFactory<PocoUserStartup, IdentityDbContext> serverFactory
    )
        : base(serverFactory) { }
}
