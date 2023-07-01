using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Provides an interface for implmenting a construct that provides
/// access to authentication-related configuration sections.
/// </summary>
public interface IAuthenticationConfigurationProvider
{
    /// <summary>
    /// Gets the <see cref="ConfigurationSection"/> where authentication
    /// options are stored.
    /// </summary>
    IConfiguration AuthenticationConfiguration { get; }
}
