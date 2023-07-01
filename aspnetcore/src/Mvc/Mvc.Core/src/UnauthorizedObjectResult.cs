using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// An <see cref="ObjectResult"/> that when executed will produce a Unauthorized (401) response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class UnauthorizedObjectResult : ObjectResult
{
    private const int DefaultStatusCode = StatusCodes.Status401Unauthorized;

    /// <summary>
    /// Creates a new <see cref="UnauthorizedObjectResult"/> instance.
    /// </summary>
    public UnauthorizedObjectResult([ActionResultObjectValue] object? value)
        : base(value)
    {
        StatusCode = DefaultStatusCode;
    }
}
