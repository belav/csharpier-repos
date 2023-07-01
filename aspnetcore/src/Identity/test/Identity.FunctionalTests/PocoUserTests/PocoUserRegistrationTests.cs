using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class PocoUserRegistrationTests : RegistrationTests<PocoUserStartup, IdentityDbContext>
{
    public PocoUserRegistrationTests(
        ServerFactory<PocoUserStartup, IdentityDbContext> serverFactory
    )
        : base(serverFactory) { }
}
