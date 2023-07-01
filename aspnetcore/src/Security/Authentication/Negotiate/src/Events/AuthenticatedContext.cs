using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Negotiate;

/// <summary>
/// State for the Authenticated event.
/// </summary>
public class AuthenticatedContext : ResultContext<NegotiateOptions>
{
    /// <summary>
    /// Creates a new <see cref="AuthenticatedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public AuthenticatedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        NegotiateOptions options
    )
        : base(context, scheme, options) { }
}
