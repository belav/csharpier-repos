using System.Security.Claims;

namespace BasicWebSite;

internal static class ConfigureAuthPoliciesExtensions
{
    public static void ConfigureBaseWebSiteAuthPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // This policy cannot succeed since the claim is never added
            options.AddPolicy(
                "Impossible",
                policy =>
                {
                    policy.AuthenticationSchemes.Add("Api");
                    policy.RequireClaim("Never");
                }
            );
            options.AddPolicy(
                "Api",
                policy =>
                {
                    policy.AuthenticationSchemes.Add("Api");
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                }
            );
            options.AddPolicy(
                "Api-Manager",
                policy =>
                {
                    policy.AuthenticationSchemes.Add("Api");
                    policy.Requirements.Add(Operations.Edit);
                }
            );
        });
    }
}
