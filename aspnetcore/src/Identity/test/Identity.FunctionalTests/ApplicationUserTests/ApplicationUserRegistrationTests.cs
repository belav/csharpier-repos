using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class ApplicationUserRegistrationTests
    : RegistrationTests<ApplicationUserStartup, ApplicationDbContext>
{
    public ApplicationUserRegistrationTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory) { }
}
