using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// An <see cref="StatusCodeResult"/> that when executed will produce an empty
/// <see cref="StatusCodes.Status200OK"/> response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class OkResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status200OK;

    /// <summary>
    /// Initializes a new instance of the <see cref="OkResult"/> class.
    /// </summary>
    public OkResult()
        : base(DefaultStatusCode) { }
}
