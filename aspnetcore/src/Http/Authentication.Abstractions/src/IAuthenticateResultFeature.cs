using Microsoft.AspNetCore.Http.Features.Authentication;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Used to capture the <see cref="AuthenticateResult"/> from the authorization middleware.
/// </summary>
public interface IAuthenticateResultFeature
{
    /// <summary>
    /// The <see cref="AuthenticateResult"/> from the authorization middleware.
    /// Set to null if the <see cref="IHttpAuthenticationFeature.User"/> property is set after the authorization middleware.
    /// </summary>
    AuthenticateResult? AuthenticateResult { get; set; }
}
