using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class PocoUserLoginTests : LoginTests<PocoUserStartup, IdentityDbContext>
{
    public PocoUserLoginTests(ServerFactory<PocoUserStartup, IdentityDbContext> serverFactory)
        : base(serverFactory) { }
}
