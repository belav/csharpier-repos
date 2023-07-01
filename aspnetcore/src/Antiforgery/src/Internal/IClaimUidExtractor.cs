using System.Security.Claims;

namespace Microsoft.AspNetCore.Antiforgery;

/// <summary>
/// This interface can extract unique identifers for a <see cref="ClaimsPrincipal"/>.
/// </summary>
internal interface IClaimUidExtractor
{
    /// <summary>
    /// Extracts claims identifier.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/>.</param>
    /// <returns>The claims identifier.</returns>
    string? ExtractClaimUid(ClaimsPrincipal claimsPrincipal);
}
