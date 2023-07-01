using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserLoginWithoutEndpointRoutingTests
    : LoginTests<StartupWithoutEndpointRouting, IdentityDbContext>
{
    public IdentityUserLoginWithoutEndpointRoutingTests(
        ServerFactory<StartupWithoutEndpointRouting, IdentityDbContext> serverFactory
    )
        : base(serverFactory) { }
}
