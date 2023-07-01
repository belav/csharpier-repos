using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Default claims transformation is a no-op.
/// </summary>
public class NoopClaimsTransformation : IClaimsTransformation
{
    /// <summary>
    /// Returns the principal unchanged.
    /// </summary>
    /// <param name="principal">The user.</param>
    /// <returns>The principal unchanged.</returns>
    public virtual Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        return Task.FromResult(principal);
    }
}
