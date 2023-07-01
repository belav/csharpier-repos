using Microsoft.AspNetCore.Authentication;

namespace Identity.DefaultUI.WebSite;

public static class ContosoAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddContosoAuthentication(
        this AuthenticationBuilder builder,
        Action<ContosoAuthenticationOptions> configure
    ) =>
        builder.AddScheme<ContosoAuthenticationOptions, ContosoAuthenticationHandler>(
            ContosoAuthenticationConstants.Scheme,
            ContosoAuthenticationConstants.DisplayName,
            configure
        );
}
