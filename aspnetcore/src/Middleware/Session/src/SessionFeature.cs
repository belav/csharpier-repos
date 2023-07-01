using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.Session;

/// <inheritdoc />
public class SessionFeature : ISessionFeature
{
    /// <inheritdoc />
    public ISession Session { get; set; } = default!;
}
