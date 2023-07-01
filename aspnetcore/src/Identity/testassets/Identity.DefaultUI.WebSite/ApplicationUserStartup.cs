using Identity.DefaultUI.WebSite.Data;

namespace Identity.DefaultUI.WebSite;

public class ApplicationUserStartup : StartupBase<ApplicationUser, ApplicationDbContext>
{
    public ApplicationUserStartup(IConfiguration configuration)
        : base(configuration) { }
}
