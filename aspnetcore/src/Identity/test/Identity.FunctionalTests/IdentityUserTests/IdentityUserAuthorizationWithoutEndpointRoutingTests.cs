using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class IdentityUserAuthorizationWithoutEndpointRoutingTests
    : AuthorizationTests<StartupWithoutEndpointRouting, IdentityDbContext>
{
    public IdentityUserAuthorizationWithoutEndpointRoutingTests(
        ServerFactory<StartupWithoutEndpointRouting, IdentityDbContext> serverFactory
    )
        : base(serverFactory) { }
}
