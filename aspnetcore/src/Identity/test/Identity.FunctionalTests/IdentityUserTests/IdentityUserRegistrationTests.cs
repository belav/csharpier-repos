using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserRegistrationTests : RegistrationTests<Startup, IdentityDbContext>
{
    public IdentityUserRegistrationTests(ServerFactory<Startup, IdentityDbContext> serverFactory)
        : base(serverFactory) { }
}
