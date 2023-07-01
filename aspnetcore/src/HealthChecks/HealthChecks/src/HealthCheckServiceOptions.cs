using System.Collections.Generic;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Options for the default implementation of <see cref="HealthCheckService"/>
/// </summary>
public sealed class HealthCheckServiceOptions
{
    /// <summary>
    /// Gets the health check registrations.
    /// </summary>
    public ICollection<HealthCheckRegistration> Registrations { get; } =
        new List<HealthCheckRegistration>();
}
