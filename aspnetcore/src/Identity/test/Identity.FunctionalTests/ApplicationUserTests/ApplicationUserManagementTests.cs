using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.IdentityUserTests;

public class ApplicationUserManagementTests
    : ManagementTests<ApplicationUserStartup, ApplicationDbContext>
{
    public ApplicationUserManagementTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory) { }
}
