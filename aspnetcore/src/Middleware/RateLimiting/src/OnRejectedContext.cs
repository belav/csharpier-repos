using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.RateLimiting;

/// <summary>
/// Holds state needed for the OnRejected callback in the RateLimitingMiddleware.
/// </summary>
public sealed class OnRejectedContext
{
    /// <summary>
    /// Gets or sets the <see cref="HttpContext"/> that the OnRejected callback will have access to
    /// </summary>
    public required HttpContext HttpContext { get; init; }

    /// <summary>
    /// Gets or sets the failed <see cref="RateLimitLease"/> that the OnRejected callback will have access to
    /// </summary>
    public required RateLimitLease Lease { get; init; }
}
