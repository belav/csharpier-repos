using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Used to capture path info so redirects can be computed properly within an app.Map().
/// </summary>
public class AuthenticationFeature : IAuthenticationFeature
{
    /// <summary>
    /// The original path base.
    /// </summary>
    public PathString OriginalPathBase { get; set; }

    /// <summary>
    /// The original path.
    /// </summary>
    public PathString OriginalPath { get; set; }
}
