using Microsoft.AspNetCore.Identity;

namespace Wasm.Authentication.Server.Models;

public class ApplicationUser : IdentityUser
{
    public UserPreference UserPreference { get; set; }
}
