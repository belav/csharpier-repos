using System.Security.Claims;

namespace Microsoft.AspNetCore.Components.Authorization;

/// <summary>
/// Provides information about the currently authenticated user, if any.
/// </summary>
public class AuthenticationState
{
    /// <summary>
    /// Constructs an instance of <see cref="AuthenticationState"/>.
    /// </summary>
    /// <param name="user">A <see cref="ClaimsPrincipal"/> representing the user.</param>
    public AuthenticationState(ClaimsPrincipal user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    /// <summary>
    /// Gets a <see cref="ClaimsPrincipal"/> that describes the current user.
    /// </summary>
    public ClaimsPrincipal User { get; }
}
