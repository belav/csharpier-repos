using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Diagnostics;

/// <summary>Default implementation for <see cref="IStatusCodeReExecuteFeature" />.</summary>
public class StatusCodeReExecuteFeature : IStatusCodeReExecuteFeature
{
    /// <inheritdoc/>
    public string OriginalPath { get; set; } = default!;

    /// <inheritdoc/>
    public string OriginalPathBase { get; set; } = default!;

    /// <inheritdoc/>
    public string? OriginalQueryString { get; set; }

    /// <inheritdoc/>
    public Endpoint? Endpoint { get; set; }

    /// <inheritdoc/>
    public RouteValueDictionary? RouteValues { get; set; }
}
