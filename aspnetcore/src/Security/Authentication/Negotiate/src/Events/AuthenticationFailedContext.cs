using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Negotiate;

/// <summary>
/// State for the AuthenticationFailed event.
/// </summary>
public class AuthenticationFailedContext : RemoteAuthenticationContext<NegotiateOptions>
{
    /// <summary>
    /// Creates a <see cref="AuthenticationFailedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public AuthenticationFailedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        NegotiateOptions options
    )
        : base(context, scheme, options, properties: null) { }

    /// <summary>
    /// The exception that occurred while processing the authentication.
    /// </summary>
    public Exception Exception { get; set; } = default!;
}
