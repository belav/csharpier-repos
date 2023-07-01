using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// A <see cref="StatusCodeResult"/> that when executed will produce a Conflict (409) response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class ConflictResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status409Conflict;

    /// <summary>
    /// Creates a new <see cref="ConflictResult"/> instance.
    /// </summary>
    public ConflictResult()
        : base(DefaultStatusCode) { }
}
