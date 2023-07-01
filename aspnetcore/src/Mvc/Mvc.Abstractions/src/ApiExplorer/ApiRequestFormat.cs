using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer;

/// <summary>
/// A possible format for the body of a request.
/// </summary>
public class ApiRequestFormat
{
    /// <summary>
    /// The formatter used to read this request.
    /// </summary>
    public IInputFormatter Formatter { get; set; } = default!;

    /// <summary>
    /// The media type of the request.
    /// </summary>
    public string MediaType { get; set; } = default!;
}
