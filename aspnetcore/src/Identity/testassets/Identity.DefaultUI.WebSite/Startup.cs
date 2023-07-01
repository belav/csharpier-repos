using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.DefaultUI.WebSite;

public class Startup : StartupBase<IdentityUser, IdentityDbContext>
{
    public Startup(IConfiguration configuration)
        : base(configuration) { }
}
