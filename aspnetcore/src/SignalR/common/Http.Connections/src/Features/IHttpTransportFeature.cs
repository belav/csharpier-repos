using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.Http.Connections.Features;

/// <summary>
/// Feature set on the <see cref="ConnectionContext"/> that exposes the <see cref="HttpTransportType"/>
/// the connection is using.
/// </summary>
public interface IHttpTransportFeature
{
    /// <summary>
    /// The <see cref="HttpTransportType"/> the connection is using.
    /// </summary>
    HttpTransportType TransportType { get; }
}
