using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserManagementTests : ManagementTests<Startup, IdentityDbContext>
{
    public IdentityUserManagementTests(ServerFactory<Startup, IdentityDbContext> serverFactory)
        : base(serverFactory) { }
}
